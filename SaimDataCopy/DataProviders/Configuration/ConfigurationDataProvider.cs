using SaimDataCopy.Models.Configuration;

namespace SaimDataCopy.DataProviders.Configuration
{
    // DataProvider de configuration.
    // Son rôle est de gérer l'accès aux données.
    // Plus tard, ici on pourra utiliser SQL Server, JSON, ou un fichier de configuration.
    public class ConfigurationDataProvider : IConfigurationDataProvider
    {
        // Pour l'instant, on prépare la méthode.
        // Plus tard, elle sauvegardera la configuration.
        public void EnregistrerConfiguration(ConfigurationModel configuration)
        {
            // TODO : sauvegarder la configuration plus tard.
            // Exemple futur : SQL Server, fichier JSON, etc.
        }

        // Pour l'instant, on retourne null.
        // Plus tard, cette méthode chargera la configuration sauvegardée.
        public ConfigurationModel? ChargerConfiguration()
        {
            // TODO : charger la configuration plus tard.
            return null;
        }
    }
}