using Newtonsoft.Json;
using SaimDataCopy.Models.Configuration;

namespace SaimDataCopy.DataProviders.Configuration
{
    // DataProvider de configuration.
    // Il s'occupe seulement de charger et sauvegarder les données.
    public class ConfigurationDataProvider : IConfigurationDataProvider
    {
        private readonly string _cheminFichier;

        public ConfigurationDataProvider()
        {
            // Le fichier JSON sera stocké dans le dossier Data.
            string dossierData = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "Data"
            );

            if (!Directory.Exists(dossierData))
            {
                Directory.CreateDirectory(dossierData);
            }

            _cheminFichier = Path.Combine(dossierData, "configuration_config.json");
        }

        public void EnregistrerConfiguration(ConfigurationModel configuration)
        {
            string contenuJson = JsonConvert.SerializeObject(
                configuration,
                Formatting.Indented
            );

            File.WriteAllText(_cheminFichier, contenuJson);
        }

        public ConfigurationModel? ChargerConfiguration()
        {
            if (!File.Exists(_cheminFichier))
            {
                return null;
            }

            string contenuJson = File.ReadAllText(_cheminFichier);

            if (string.IsNullOrWhiteSpace(contenuJson))
            {
                return null;
            }

            ConfigurationModel? configuration =
                JsonConvert.DeserializeObject<ConfigurationModel>(contenuJson);

            return configuration;
        }
    }
}