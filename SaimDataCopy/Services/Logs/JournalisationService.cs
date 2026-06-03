using SaimDataCopy.DataProviders.Logs;
using SaimDataCopy.Models.Logs;

namespace SaimDataCopy.Services.Logs
{
    /// <summary>
    /// Service qui écrit réellement les messages dans des fichiers .log.
    /// Il utilise les paramètres sauvegardés dans Data/logs_parametres.json.
    /// </summary>
    public class JournalisationService : IJournalisationService
    {
        private readonly ILogsDataProvider _logsDataProvider;

        // On garde le chemin du fichier log de l'exécution actuelle.
        // Comme ça, tous les messages d'une même exécution restent dans le même fichier.
        private string _cheminFichierExecution = string.Empty;

        private enum TypeLog
        {
            Information,
            Succes,
            Avertissement,
            Erreur
        }

        public JournalisationService(ILogsDataProvider logsDataProvider)
        {
            _logsDataProvider = logsDataProvider;
        }

        public void DemarrerNouvelleExecution()
        {
            LogConfigModel configuration = _logsDataProvider.ChargerConfiguration();

            if (!Directory.Exists(configuration.RepertoireLogs))
            {
                Directory.CreateDirectory(configuration.RepertoireLogs);
            }

            _cheminFichierExecution = ConstruireCheminFichierLog(configuration);

            EcrireInformation("Nouvelle exécution démarrée.");
        }

        public void EcrireInformation(string message)
        {
            EcrireLog(TypeLog.Information, message, null);
        }

        public void EcrireSucces(string message)
        {
            EcrireLog(TypeLog.Succes, message, null);
        }

        public void EcrireAvertissement(string message)
        {
            EcrireLog(TypeLog.Avertissement, message, null);
        }

        public void EcrireErreur(string message)
        {
            EcrireLog(TypeLog.Erreur, message, null);
        }

        public void EcrireErreur(string message, Exception exception)
        {
            EcrireLog(TypeLog.Erreur, message, exception);
        }

        public void NettoyerAnciensLogs()
        {
            LogConfigModel configuration = _logsDataProvider.ChargerConfiguration();

            if (!Directory.Exists(configuration.RepertoireLogs))
            {
                return;
            }

            DateTime dateLimite = DateTime.Now.AddDays(-configuration.DureeConservationJours);

            string[] fichiersLogs = Directory.GetFiles(configuration.RepertoireLogs, "*.log");

            foreach (string fichier in fichiersLogs)
            {
                DateTime derniereModification = File.GetLastWriteTime(fichier);

                if (derniereModification < dateLimite)
                {
                    File.Delete(fichier);
                }
            }
        }

        private void EcrireLog(TypeLog typeLog, string message, Exception? exception)
        {
            LogConfigModel configuration = _logsDataProvider.ChargerConfiguration();

            if (!Directory.Exists(configuration.RepertoireLogs))
            {
                Directory.CreateDirectory(configuration.RepertoireLogs);
            }

            if (string.IsNullOrWhiteSpace(_cheminFichierExecution))
            {
                _cheminFichierExecution = ConstruireCheminFichierLog(configuration);
            }

            string ligneLog = ConstruireLigneLog(typeLog, message, exception);

            File.AppendAllText(_cheminFichierExecution, ligneLog + Environment.NewLine);

            VerifierTailleFichier(configuration);
        }

        private static string ConstruireCheminFichierLog(LogConfigModel configuration)
        {
            string date = DateTime.Now.ToString("yyyy-MM-dd");
            string heure = DateTime.Now.ToString("HH-mm-ss");

            string nomFichier = configuration.NommageFichiers
                .Replace("{date}", date)
                .Replace("{heure}", heure);

            if (!nomFichier.EndsWith(".log", StringComparison.OrdinalIgnoreCase))
            {
                nomFichier = Path.ChangeExtension(nomFichier, ".log");
            }

            return Path.Combine(configuration.RepertoireLogs, nomFichier);
        }

        private static string ConstruireLigneLog(TypeLog typeLog, string message, Exception? exception)
        {
            string dateHeure = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            string libelleType = typeLog switch
            {
                TypeLog.Information => "INFO",
                TypeLog.Succes => "SUCCES",
                TypeLog.Avertissement => "AVERTISSEMENT",
                TypeLog.Erreur => "ERREUR",
                _ => "INFO"
            };

            string ligne = $"[{dateHeure}] [{libelleType}] {message}";

            if (exception != null)
            {
                ligne += Environment.NewLine;
                ligne += "Détail erreur : " + exception.Message;
            }

            return ligne;
        }

        private void VerifierTailleFichier(LogConfigModel configuration)
        {
            if (string.IsNullOrWhiteSpace(_cheminFichierExecution))
            {
                return;
            }

            FileInfo fichier = new FileInfo(_cheminFichierExecution);

            if (!fichier.Exists)
            {
                return;
            }

            long tailleMaxOctets = configuration.TailleMaxFichierMo * 1024L * 1024L;

            if (fichier.Length <= tailleMaxOctets)
            {
                return;
            }

            string dossier = fichier.DirectoryName ?? configuration.RepertoireLogs;
            string nomSansExtension = Path.GetFileNameWithoutExtension(fichier.Name);

            string nouveauNom =
                nomSansExtension +
                "_archive_" +
                DateTime.Now.ToString("yyyyMMdd_HHmmss") +
                ".log";

            string cheminArchive = Path.Combine(dossier, nouveauNom);

            File.Move(_cheminFichierExecution, cheminArchive);

            // Après archivage, on prépare un nouveau fichier pour continuer l'écriture.
            _cheminFichierExecution = ConstruireCheminFichierLog(configuration);
        }
    }
}