using SaimDataCopy.DataProviders.Configuration;
using SaimDataCopy.Models.Configuration;
using Microsoft.Data.SqlClient;
using MySqlConnector;
using SaimDataCopy.Helpers;

namespace SaimDataCopy.Services.Configuration
{
    // Service de configuration.
    // Son rôle est de contenir la logique métier liée aux paramètres.
    public class ConfigurationService : IConfigurationService
    {
        // Le Service utilise le DataProvider pour enregistrer ou charger les données.
        private readonly IConfigurationDataProvider _configurationDataProvider;

        // Constructeur simple.
        // Il crée directement le DataProvider de configuration.

        private bool TypeServeurValide(string typeServeur)
        {
            return typeServeur switch
            {
                "SQL Server" => true,
                "MySQL" => true,
                _ => false
            };
        }

        private string NormaliserTypeServeur(string typeServeur)
        {
            return typeServeur switch
            {
                "SQL Server" => "SQL Server",
                "MySQL" => "MySQL",
                _ => "SQL Server"
            };
        }
        public ConfigurationService()
        {
            _configurationDataProvider = new ConfigurationDataProvider();
        }

        // Constructeur utile si plus tard on veut injecter un autre DataProvider.
        public ConfigurationService(IConfigurationDataProvider configurationDataProvider)
        {
            _configurationDataProvider = configurationDataProvider;
        }

        public ConfigurationModel ChargerConfiguration()
        {
            // On demande au DataProvider de charger la dernière configuration.
            ConfigurationModel? configuration = _configurationDataProvider.ChargerConfiguration();

            // Si rien n'est encore sauvegardé, on retourne une configuration vide.
            return configuration ?? new ConfigurationModel();
        }

        public bool ValiderConfiguration(ConfigurationModel configuration, out string message)
        {
            // SOURCE :
            // L'utilisateur peut remplir soit le nom du serveur,
            // soit une chaîne de connexion complète.
            if (!ServeurRenseigne(configuration.ServeurSource))
            {
                message = "Pour le serveur source, renseignez soit le nom du serveur, soit la chaîne de connexion complète.";
                return false;
            }
            if (!TypeServeurValide(configuration.ServeurSource.TypeServeur))
            {
                message = "Le type du serveur source est invalide.";
                return false;
            }

            if (!PortValide(configuration.ServeurSource))
            {
                message = "Le port du serveur source est invalide.";
                return false;
            }

            if (!TesterConnexionServeur(configuration.ServeurSource, "source", out message))
            {
                return false;
            }

            // CIBLE :
            // Même règle : soit nom serveur, soit chaîne complète.
            if (!ServeurRenseigne(configuration.ServeurCible))
            {
                message = "Pour le serveur cible, renseignez soit le nom du serveur, soit la chaîne de connexion complète.";
                return false;
            }

            if (!TypeServeurValide(configuration.ServeurCible.TypeServeur))
            {
                message = "Le type du serveur cible est invalide.";
                return false;
            }

            if (!TypesServeursCompatibles(configuration, out message))
            {
                return false;
            }

            if (!PortValide(configuration.ServeurCible))
            {
                message = "Le port du serveur cible est invalide.";
                return false;
            }

            if (!TesterConnexionServeur(configuration.ServeurCible, "cible", out message))
            {
                return false;
            }

            switch (NormaliserModeCopie(configuration.ModeCopie))
            {
                case "Écraser":
                case "Mise à jour":
                    break;

                default:
                    message = "Le mode de copie est invalide.";
                    return false;
            }

            switch (configuration.ComportementErreur)
            {
                case "Continuer avec les autres":
                    if (configuration.TentativesReprise <= 0)
                    {
                        message = "Le nombre de tentatives de reprise est obligatoire.";
                        return false;
                    }
                    break;

                case "Arrêter tous les traitements":
                    break;

                default:
                    message = "Le comportement en cas d'erreur est invalide.";
                    return false;
            }

            message = "Configuration valide.";
            return true;
        }

        public bool EnregistrerConfiguration(ConfigurationModel configuration, out string message)
        {
            configuration.ServeurSource.TypeServeur = NormaliserTypeServeur(configuration.ServeurSource.TypeServeur);

            configuration.ServeurCible.TypeServeur = NormaliserTypeServeur(configuration.ServeurCible.TypeServeur);

            // On normalise avant validation et sauvegarde.
            configuration.ModeCopie = NormaliserModeCopie(configuration.ModeCopie);

            bool estValide = ValiderConfiguration(configuration, out message);

            if (!estValide)
            {
                return false;
            }

            _configurationDataProvider.EnregistrerConfiguration(configuration);

            message = "Les paramètres de configuration ont été enregistrés.";
            return true;
        }
        private bool ServeurRenseigne(ServeurConfigModel serveur)
        {
            // Si la chaîne de connexion complète est remplie,
            // on n'oblige pas le champ NomServeur.
            if (!string.IsNullOrWhiteSpace(serveur.ChaineConnexion))
            {
                return true;
            }

            // Sinon, le nom du serveur devient obligatoire.
            return !string.IsNullOrWhiteSpace(serveur.NomServeur);
        }

        private bool ChaineConnexionRenseignee(ServeurConfigModel serveur)
        {
            return !string.IsNullOrWhiteSpace(serveur.ChaineConnexion);
        }
        private bool PortValide(ServeurConfigModel serveur)
        {
            // Si une chaîne de connexion complète est utilisée,
            // le port peut être vide car il est déjà dans la chaîne.
            if (ChaineConnexionRenseignee(serveur))
            {
                return true;
            }

            // Un port négatif est toujours invalide.
            if (serveur.Port < 0)
            {
                return false;
            }

            // LocalDB n'utilise pas de port.
            // Donc 0 ou vide est accepté.
            if (ServeurSansPort(serveur.NomServeur))
            {
                return true;
            }

            // Pour un vrai serveur réseau, le port doit être supérieur à 0.
            // Exemple : 192.168.1.50 avec port 1433.
            return serveur.Port > 0;
        }

        private bool ServeurSansPort(string nomServeur)
        {
            if (string.IsNullOrWhiteSpace(nomServeur))
            {
                return false;
            }

            bool estLocalDb = nomServeur.Contains(
                "(localdb)",
                StringComparison.OrdinalIgnoreCase);

            bool estInstanceNommee = nomServeur.Contains('\\');

            // LocalDB : (localdb)\MSSQLLocalDB
            // Instance nommée : PC\SQLEXPRESS
            // Ces deux cas n'ont pas besoin d'un port dans notre interface.
            return estLocalDb || estInstanceNommee;
        }

        private bool TesterConnexionServeur( ServeurConfigModel serveur, string roleServeur, out string message)
        {
            string typeServeur = NormaliserTypeServeur(serveur.TypeServeur);

            return typeServeur switch
            {
                "SQL Server" => TesterConnexionSqlServer(serveur, roleServeur, out message),
                "MySQL" => TesterConnexionMySql(serveur, roleServeur, out message),
                _ => RetournerErreurTypeServeur(roleServeur, out message)
            };
        }

        private bool TesterConnexionSqlServer(
            ServeurConfigModel serveur,
            string roleServeur,
            out string message)
        {
            try
            {
                string chaineConnexion =
                    ChaineConnexionHelper.ConstruireChaineConnexionServeur(serveur);

                using SqlConnection connexion = new SqlConnection(chaineConnexion);
                connexion.Open();

                message = "Connexion SQL Server valide.";
                return true;
            }
            catch (Exception ex)
            {
                message =
                    $"La connexion au serveur {roleServeur} a échoué." + Environment.NewLine +
                    "Vous avez sélectionné SQL Server, mais les paramètres saisis ne permettent pas de se connecter à SQL Server." + Environment.NewLine +
                    "Vérifiez le type de serveur, le nom ou l'adresse IP, le port, l'identifiant et le mot de passe." + Environment.NewLine +
                    "Détail : " + ObtenirMessageErreurConnexion(ex);

                return false;
            }
        }

        private bool TesterConnexionMySql(
            ServeurConfigModel serveur,
            string roleServeur,
            out string message)
        {
            try
            {
                string chaineConnexion =
                    ChaineConnexionHelper.ConstruireChaineConnexionServeur(serveur);

                using MySqlConnection connexion = new MySqlConnection(chaineConnexion);
                connexion.Open();

                message = "Connexion MySQL valide.";
                return true;
            }

            catch (Exception ex)

            {
                message =
                    $"La connexion au serveur {roleServeur} a échoué." + Environment.NewLine +
                    "Vous avez sélectionné MySQL, mais les paramètres saisis ne permettent pas de se connecter à MySQL." + Environment.NewLine +
                    "Vérifiez le type de serveur, le nom ou l'adresse IP, le port, l'identifiant et le mot de passe." + Environment.NewLine +
                    "Détail : " + ObtenirMessageErreurConnexion(ex);

                return false;
            }
        }

        private bool RetournerErreurTypeServeur(string roleServeur, out string message)
        {
            message = $"Le type du serveur {roleServeur} est invalide.";
            return false;
        }

        private string ObtenirMessageErreurConnexion(Exception exception)
        {
            if (exception is MySqlException erreurMySql)
            {
                return erreurMySql.Number switch
                {
                    1045 => "Nom d'utilisateur ou mot de passe MySQL incorrect.",
                    1042 => "Serveur MySQL introuvable ou port incorrect.",
                    1049 => "La base de données MySQL demandée n'existe pas.",
                    2003 => "Impossible de se connecter au serveur MySQL. Vérifiez le nom du serveur et le port.",
                    2005 => "Nom du serveur MySQL invalide.",
                    _ => $"Erreur MySQL ({erreurMySql.Number}) : {erreurMySql.Message}"
                };
            }

            if (exception is SqlException erreurSql)
            {
                return erreurSql.Number switch
                {
                    2 => "Serveur SQL Server introuvable ou inaccessible.",
                    53 => "Serveur SQL Server introuvable ou inaccessible.",
                    18456 => "Identifiant ou mot de passe SQL Server incorrect.",
                    4060 => "Base de données SQL Server inaccessible.",
                    _ => $"Erreur SQL Server ({erreurSql.Number}) : {erreurSql.Message}"
                };
            }

            return exception.Message;
        }

        private bool TypesServeursCompatibles( ConfigurationModel configuration, out string message)
        {
            string typeSource = NormaliserTypeServeur(configuration.ServeurSource.TypeServeur);
            string typeCible = NormaliserTypeServeur(configuration.ServeurCible.TypeServeur);

            if (typeSource == typeCible)
            {
                message = string.Empty;
                return true;
            }

            message =
                "La copie entre deux types de serveurs différents n'est pas encore prise en charge." +
                Environment.NewLine +
                $"Type serveur source : {typeSource}" +
                Environment.NewLine +
                $"Type serveur cible : {typeCible}" +
                Environment.NewLine +
                Environment.NewLine +
                "Types supportés actuellement :" +
                Environment.NewLine +
                "- SQL Server vers SQL Server" +
                Environment.NewLine +
                "- MySQL vers MySQL";

            return false;
        }

        private string NormaliserModeCopie(string modeCopie)
        {
            return modeCopie switch
            {
                "Mettre à jour" => "Mise à jour",
                "Mise à jour" => "Mise à jour",
                "Écraser" => "Écraser",
                _ => string.Empty
            };
        }


    }
}