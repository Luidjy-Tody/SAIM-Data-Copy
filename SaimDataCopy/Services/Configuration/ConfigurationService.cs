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
            if (string.IsNullOrWhiteSpace(configuration.ServeurSource.NomServeur))
            {
                message = "Le nom du serveur source est obligatoire.";
                return false;
            }

            if (configuration.ServeurSource.Port <= 0)
            {
                message = "Le port du serveur source est invalide.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(configuration.ServeurCible.NomServeur))
            {
                message = "Le nom du serveur cible est obligatoire.";
                return false;
            }

            if (configuration.ServeurCible.Port <= 0)
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