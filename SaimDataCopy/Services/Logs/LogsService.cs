using SaimDataCopy.DataProviders.Logs;
using SaimDataCopy.Models.Logs;

namespace SaimDataCopy.Services.Logs
{
    /// <summary>
    /// Service pour les paramètres logs.
    /// Il contient la logique métier et les validations.
    /// </summary>
    public class LogsService : ILogsService
    {
        private readonly ILogsDataProvider _logsDataProvider;

        public LogsService(ILogsDataProvider logsDataProvider)
        {
            _logsDataProvider = logsDataProvider;
        }

        public LogConfigModel ChargerConfiguration()
        {
            return _logsDataProvider.ChargerConfiguration();
        }

        public void EnregistrerConfiguration(LogConfigModel configuration)
        {
            if (!ValiderConfiguration(configuration, out string messageErreur))
            {
                throw new ArgumentException(messageErreur);
            }

            _logsDataProvider.EnregistrerConfiguration(configuration);
        }

        public bool ValiderConfiguration(LogConfigModel configuration, out string messageErreur)
        {
            messageErreur = string.Empty;

            if (string.IsNullOrWhiteSpace(configuration.RepertoireLogs))
            {
                messageErreur = "Veuillez sélectionner le répertoire des logs.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(configuration.NommageFichiers))
            {
                messageErreur = "Veuillez saisir le nommage des fichiers logs.";
                return false;
            }

            if (configuration.NommageFichiers.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
            {
                messageErreur = "Le nommage des fichiers contient des caractères invalides.";
                return false;
            }

            messageErreur = configuration.DureeConservationJours switch
            {
                < 1 => "La durée de conservation doit être supérieure à 0 jour.",
                > 3650 => "La durée de conservation ne doit pas dépasser 3650 jours.",
                _ => string.Empty
            };

            if (!string.IsNullOrWhiteSpace(messageErreur))
            {
                return false;
            }

            messageErreur = configuration.TailleMaxFichierMo switch
            {
                < 1 => "La taille maximale d'un fichier doit être supérieure à 0 Mo.",
                > 1024 => "La taille maximale d'un fichier ne doit pas dépasser 1024 Mo.",
                _ => string.Empty
            };

            return string.IsNullOrWhiteSpace(messageErreur);
        }
    }
}