using SaimDataCopy.Models.Logs;

namespace SaimDataCopy.Views.Logs
{
    /// <summary>
    /// Interface de la page Paramètres Logs.
    /// Elle permet au Controller de communiquer avec la View.
    /// </summary>
    public interface ILogsView
    {
        /// <summary>
        /// Événement déclenché quand l'utilisateur clique sur le bouton Parcourir.
        /// </summary>
        event EventHandler? ParcourirDemande;

        /// <summary>
        /// Récupère les valeurs saisies dans l'interface.
        /// </summary>
        LogConfigModel RecupererConfiguration();

        /// <summary>
        /// Affiche les valeurs dans l'interface.
        /// </summary>
        void AfficherConfiguration(LogConfigModel configuration);

        /// <summary>
        /// Modifie le répertoire des logs dans le champ texte.
        /// Utilisé après le bouton Parcourir.
        /// </summary>
        void ModifierRepertoireLogs(string chemin);

        /// <summary>
        /// Affiche un message simple à l'utilisateur.
        /// </summary>
        void AfficherMessage(string message, bool succes);
    }
}