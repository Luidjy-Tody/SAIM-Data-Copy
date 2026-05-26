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
        // Exemple : pour les tests ou pour changer la source des données.
        public ConfigurationService(IConfigurationDataProvider configurationDataProvider)
        {
            _configurationDataProvider = configurationDataProvider;
        }

        // Vérifie si la configuration saisie est correcte.
        // Cette méthode contient la logique métier de validation.
        public bool ValiderConfiguration(ConfigurationModel configuration, out string message)
        {
            // Vérification du serveur source.
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

            // Vérification du serveur cible.
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

            // Vérification du mode de copie.
            // On utilise switch parce qu'il y a plusieurs choix possibles.
            switch (configuration.ModeCopie)
            {
                case "Écraser":
                case "Mettre à jour":
                    break;

                default:
                    message = "Le mode de copie est invalide.";
                    return false;
            }

            // Vérification du comportement en cas d'erreur.
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
                    // Si on arrête tous les traitements,
                    // les tentatives de reprise ne sont pas obligatoires.
                    break;

                default:
                    message = "Le comportement en cas d'erreur est invalide.";
                    return false;
            }

            message = "Configuration valide.";
            return true;
        }

        // Valide les données puis demande au DataProvider de les enregistrer.
        public bool EnregistrerConfiguration(ConfigurationModel configuration, out string message)
        {
            bool estValide = ValiderConfiguration(configuration, out message);

            if (!estValide)
            {
                return false;
            }

            // Le Service ne sauvegarde pas directement.
            // Il demande au DataProvider de faire l'enregistrement.
            _configurationDataProvider.EnregistrerConfiguration(configuration);

            message = "Les paramètres de configuration ont été enregistrés.";
            return true;
        }
    }
}