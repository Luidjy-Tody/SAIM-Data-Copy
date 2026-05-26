using SaimDataCopy.Models.Configuration;

namespace SaimDataCopy.Services.Configuration
{
    // Interface du service de configuration.
    // Elle définit les actions disponibles pour la configuration.
    public interface IConfigurationService
    {
        // Vérifie si la configuration est correcte.
        bool ValiderConfiguration(ConfigurationModel configuration, out string message);

        // Valide puis demande l'enregistrement de la configuration.
        bool EnregistrerConfiguration(ConfigurationModel configuration, out string message);
    }
}