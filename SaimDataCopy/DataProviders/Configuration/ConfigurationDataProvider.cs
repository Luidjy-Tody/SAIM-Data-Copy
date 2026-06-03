using Newtonsoft.Json;
using SaimDataCopy.Models.Configuration;

namespace SaimDataCopy.DataProviders.Configuration
{
    // DataProvider de configuration.
    // Il sauvegarde et charge les paramètres depuis un fichier JSON.
    public class ConfigurationDataProvider : IConfigurationDataProvider
    {
        private readonly string _cheminFichierConfiguration;

        public ConfigurationDataProvider()
        {
            // Le fichier JSON sera stocké dans le dossier Data de l'application.
            string dossierData = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");

            if (!Directory.Exists(dossierData))
            {
                Directory.CreateDirectory(dossierData);
            }

            _cheminFichierConfiguration = Path.Combine(dossierData, "configuration_execution.json");
        }

        public void EnregistrerConfiguration(ConfigurationModel configuration)
        {
            // Sauvegarde principale dans le fichier JSON.
            string contenuJson = JsonConvert.SerializeObject(configuration, Formatting.Indented);

            File.WriteAllText(_cheminFichierConfiguration, contenuJson);
        }

        public ConfigurationModel? ChargerConfiguration()
        {
            if (!File.Exists(_cheminFichierConfiguration))
            {
                return null;
            }

            string contenuJson = File.ReadAllText(_cheminFichierConfiguration);

            if (string.IsNullOrWhiteSpace(contenuJson))
            {
                return null;
            }

            try
            {
                return JsonConvert.DeserializeObject<ConfigurationModel>(contenuJson);
            }
            catch (JsonException)
            {
                throw new InvalidOperationException(
                    "Le fichier configuration_execution.json est invalide. Vérifiez son contenu JSON.");
            }
        }
    }
}