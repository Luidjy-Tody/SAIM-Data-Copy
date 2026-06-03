using Newtonsoft.Json;
using SaimDataCopy.Models.Logs;

namespace SaimDataCopy.DataProviders.Logs
{
    /// <summary>
    /// DataProvider pour les paramètres de journalisation.
    /// Il s'occupe seulement de charger et sauvegarder les données dans un fichier JSON.
    /// </summary>
    public class LogsDataProvider : ILogsDataProvider
    {
        private readonly string _cheminFichier;

        public LogsDataProvider()
        {
            // Dossier Data dans le dossier de l'application.
            string dossierData = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");

            if (!Directory.Exists(dossierData))
            {
                Directory.CreateDirectory(dossierData);
            }

            // Nouveau fichier demandé pour les paramètres logs.
            _cheminFichier = Path.Combine(dossierData, "logs_parametres.json");
        }

        public LogConfigModel ChargerConfiguration()
        {
            // Si le fichier JSON n'existe pas encore,
            // on retourne les valeurs par défaut du modèle.
            if (!File.Exists(_cheminFichier))
            {
                return new LogConfigModel();
            }

            string contenuJson = File.ReadAllText(_cheminFichier);

            // Si le fichier existe mais qu'il est vide,
            // on retourne aussi les valeurs par défaut.
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