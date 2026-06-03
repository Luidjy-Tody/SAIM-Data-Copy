namespace SaimDataCopy.Services.Logs
{
    /// <summary>
    /// Interface du service de journalisation.
    /// Ce service sert à écrire les messages dans des fichiers .log.
    /// </summary>
    public interface IJournalisationService
    {
        /// <summary>
        /// Prépare un nouveau fichier log pour une nouvelle exécution.
        /// Comme ça, tous les messages d'une même copie restent dans le même fichier.
        /// </summary>
        void DemarrerNouvelleExecution();

        /// <summary>
        /// Écrit un message d'information dans le fichier log.
        /// </summary>
        void EcrireInformation(string message);

        /// <summary>
        /// Écrit un message de succès dans le fichier log.
        /// </summary>
        void EcrireSucces(string message);

        /// <summary>
        /// Écrit un message d'avertissement dans le fichier log.
        /// </summary>
        void EcrireAvertissement(string message);

        /// <summary>
        /// Écrit un message d'erreur dans le fichier log.
        /// </summary>
        void EcrireErreur(string message);

        /// <summary>
        /// Écrit une erreur avec le détail de l'exception.
        /// </summary>
        void EcrireErreur(string message, Exception exception);

        /// <summary>
        /// Supprime les anciens fichiers logs selon la durée de conservation.
        /// </summary>
        void NettoyerAnciensLogs();
    }
}