using SaimDataCopy.Models.Email;

namespace SaimDataCopy.Views.Email
{
    /// <summary>
    /// Interface de la vue Paramètres Email.
    /// Elle permet au Controller de communiquer avec la View.
    /// </summary>
    public interface IEmailView
    {
        /// <summary>
        /// Événement déclenché quand l'utilisateur demande l'enregistrement.
        /// </summary>
        event EventHandler? EnregistrementDemande;

        /// <summary>
        /// Événement déclenché quand l'utilisateur clique sur le bouton de test.
        /// </summary>
        event EventHandler? TestEmailDemande;

        /// <summary>
        /// Remplit l'interface avec les paramètres email.
        /// </summary>
        void AfficherConfiguration(EmailConfigModel configuration);

        /// <summary>
        /// Récupère les valeurs saisies dans l'interface.
        /// </summary>
        EmailConfigModel RecupererConfiguration();

        /// <summary>
        /// Demande l'enregistrement depuis MainForm.
        /// </summary>
        void DemanderEnregistrement();
    }
}