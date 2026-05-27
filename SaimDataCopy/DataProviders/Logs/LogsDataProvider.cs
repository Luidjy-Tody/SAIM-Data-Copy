using Newtonsoft.Json;
using SaimDataCopy.Models.Logs;

namespace SaimDataCopy.DataProviders.Logs
{
    /// <summary>
    /// DataProvider pour les paramètres de journalisation.
    /// Il s'occupe seulement de charger et sauvegarder les données.
    /// </summary>
    public class LogsDataProvider : ILogsDataProvider
    {
        private readonly string _cheminFichier;

        public LogsDataProvider()
        {
            // Le fichier JSON sera stocké dans le dossier de l'application.
            string dossierData = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
            

            if (!Directory.Exists(dossierData))
            {
                Directory.CreateDirectory(dossierData);
            }

            _cheminFichier = Path.Combine(dossierData, "logs_config.json");
        }

        public LogConfigModel ChargerConfiguration()
        {
            // Si le fichier n'existe pas encore, on retourne les valeurs par défaut.
            if (!File.Exists(_cheminFichier))
            {
                return new LogConfigModel();
            }

            string contenuJson = File.ReadAllText(_cheminFichier);

            // Si le fichier est vide, on retourne aussi les valeurs par défaut.
            if (string.IsNullOrWhiteSpace(contenuJson))
            {
                return new LogConfigModel();
            }

            LogConfigModel? configuration = JsonConvert.DeserializeObject<LogConfigModel>(contenuJson);

            return configuration ?? new LogConfigModel();
        }


        public void EnregistrerConfiguration(LogConfigModel configuration)
        {
            string contenuJson = JsonConvert.SerializeObject(configuration, Formatting.Indented);

            File.WriteAllText(_cheminFichier, contenuJson);
        }
    }
}
// Ce fichier sauvegarde les paramètres logs dans :
// bin/Debug/net8.0-windows/Data/logs_config.json