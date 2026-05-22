using SaimDataCopy.Models;

namespace SaimDataCopy.Services
{
    // Service de configuration.
    // Son rôle est de contenir la logique métier liée aux paramètres.
    public class ConfigurationService
    {
        // Vérifie si la configuration saisie est correcte.
        // Cette logique n'est pas dans la View pour respecter MVC.
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
                    break;

                default:
                    message = "Le comportement en cas d'erreur est invalide.";
                    return false;
            }

            message = "Configuration valide.";
            return true;
        }
    }
}