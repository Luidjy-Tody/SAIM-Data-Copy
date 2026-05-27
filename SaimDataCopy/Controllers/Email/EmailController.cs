using SaimDataCopy.Models.Email;
using SaimDataCopy.Services.Email;
using SaimDataCopy.Views.Email;

namespace SaimDataCopy.Controllers.Email
{
    /// <summary>
    /// Controller de la page Paramètres Email.
    /// Il coordonne les actions entre la View et le Service.
    /// </summary>
    public class EmailController
    {
        private readonly IEmailView _view;
        private readonly IEmailService _emailService;

        public EmailController(IEmailView view, IEmailService emailService)
        {
            _view = view;
            _emailService = emailService;

            // On écoute les actions demandées par la View.
            _view.EnregistrementDemande += View_EnregistrementDemande;
            _view.TestEmailDemande += View_TestEmailDemande;

            // Au démarrage de la page, on charge les paramètres sauvegardés.
            ChargerConfiguration();
        }

        /// <summary>
        /// Charge les paramètres email et les affiche dans la View.
        /// </summary>
        private void ChargerConfiguration()
        {
            try
            {
                EmailConfigModel configuration = _emailService.Charger();

                _view.AfficherConfiguration(configuration);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Erreur pendant le chargement des paramètres email : " + ex.Message,
                    "Erreur",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        /// <summary>
        /// Action appelée quand l'utilisateur demande l'enregistrement.
        /// </summary>
        private void View_EnregistrementDemande(object? sender, EventArgs e)
        {
            EmailConfigModel configuration = _view.RecupererConfiguration();

            bool resultat = _emailService.Enregistrer(configuration, out string message);

            MessageBox.Show(
                message,
                resultat ? "Succès" : "Validation",
                MessageBoxButtons.OK,
                resultat ? MessageBoxIcon.Information : MessageBoxIcon.Warning
            );
        }

        /// <summary>
        /// Action appelée quand l'utilisateur clique sur Tester l'envoi email.
        /// </summary>
        private void View_TestEmailDemande(object? sender, EventArgs e)
        {
            EmailConfigModel configuration = _view.RecupererConfiguration();

            bool resultat = _emailService.EnvoyerEmailTest(configuration, out string message);

            MessageBox.Show(
                message,
                resultat ? "Succès" : "Erreur",
                MessageBoxButtons.OK,
                resultat ? MessageBoxIcon.Information : MessageBoxIcon.Error
            );
        }
    }
}