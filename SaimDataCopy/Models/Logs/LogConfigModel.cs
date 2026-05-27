

namespace SaimDataCopy.Models.Logs
{
    /// <summary>
    /// Modèle qui contient les paramètres de journalisation.
    /// Cette classe représente les données de la page Paramètres Logs.
    /// </summary>
    public class LogConfigModel
    {
        /// <summary>
        /// Dossier où les fichiers logs seront enregistrés.
        /// Exemple : C:\Logs\CopieAuto
        /// </summary>
        public string RepertoireLogs { get; set; } = @"C:\Logs\CopieAuto";

        /// <summary>
        /// Format du nom des fichiers logs.
        /// Exemple : log_{date}_{heure}.txt
        /// </summary>
        public string NommageFichiers { get; set; } = "log_{date}_{heure}.txt";

        /// <summary>
        /// Nombre de jours pendant lesquels les logs sont conservés.
        /// Exemple : 30 jours
        /// </summary>
        
        public int DureeConservationJours { get; set; } = 30;

        /// <summary>
        /// Taille maximale d'un fichier log en Mo.
        /// Exemple : 10 Mo
        /// </summary>
        public int TailleMaxFichierMo { get; set; } = 10;
    }
}
