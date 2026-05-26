using SaimDataCopy.Models.Configuration;

namespace SaimDataCopy.DataProviders.Configuration
{
    // Interface du DataProvider de configuration.
    // Le DataProvider sera responsable de l'accès aux données.
    // Plus tard, il pourra sauvegarder ou charger la configuration depuis SQL Server, JSON, etc.
    public interface IConfigurationDataProvider
    {
        // Sauvegarde les paramètres de configuration.
        void EnregistrerConfiguration(ConfigurationModel configuration);

        // Charge les paramètres de configuration.
        // Pour l'instant, on prépare seulement la méthode.
        ConfigurationModel? ChargerConfiguration();
    }
}