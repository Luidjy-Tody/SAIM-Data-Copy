using SaimDataCopy.DataProviders.Configuration;
using SaimDataCopy.Models.Configuration;

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
            ConfigurationModel? configuration =
                _configurationDataProvider.ChargerConfiguration();

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

            if (!PortValide(configuration.ServeurSource))
            {
                message = "Le port du serveur source est invalide.";
                return false;
            }

            // CIBLE :
            // Même règle : soit nom serveur, soit chaîne complète.
            if (!ServeurRenseigne(configuration.ServeurCible))
            {
                message = "Pour le serveur cible, renseignez soit le nom du serveur, soit la chaîne de connexion complète.";
                return false;
            }

            if (!PortValide(configuration.ServeurCible))
            {
                message = "Le port du serveur cible est invalide.";
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