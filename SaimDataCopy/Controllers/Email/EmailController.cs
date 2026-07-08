using SaimDataCopy.Controllers.Authentification;
using SaimDataCopy.Models.Email;
using SaimDataCopy.Services.Email;
using SaimDataCopy.Views.Email;
using SaimDataCopy.Views.Commun;

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
        private readonly AuthentificationController _authentificationController;

        public EmailController(IEmailView view, IEmailService emailService)
        {
            _view = view;
            _emailService = emailService;
            _authentificationController = new AuthentificationController();

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
            EnregistrerDepuisMainForm();
        }

        /// <summary>
        /// Enregistre les paramètres email et retourne true si l'enregistrement réussit.
        /// Cette méthode sera utilisée par MainForm avant de changer de page.
        /// </summary>
        public bool EnregistrerDepuisMainForm()
        {
            EmailConfigModel configuration = _view.RecupererConfiguration();

            bool resultat = _emailService.Enregistrer(configuration, out string message);

            MessageBox.Show(
                message,
                resultat ? "Succès" : "Validation",
                MessageBoxButtons.OK,
                resultat ? MessageBoxIcon.Information : MessageBoxIcon.Warning
            );

            if (!resultat)
            {
                return false;
            }

            // Si la View gère les modifications non enregistrées,
            // on indique que les paramètres actuels sont maintenant sauvegardés.
            if (_view is IPageEnregistrable pageEnregistrable)
            {
                pageEnregistrable.MarquerCommeEnregistre();
            }

            EnregistrerLogUtilisateur(
                "Enregistrement Paramètres Email",
                ConstruireDetailsEmail(configuration)
            );

            return true;
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

        private string ConstruireDetailsEmail(EmailConfigModel configuration)
        {
            return
                "Serveur SMTP :" + Environment.NewLine +
                "- Serveur SMTP renseigné : " + OuiNon(!string.IsNullOrWhiteSpace(configuration.ServeurSmtp)) + Environment.NewLine +
                "- Port SMTP renseigné : " + OuiNon(configuration.Port > 0) + Environment.NewLine +
                "- Sécurité : " + AfficherValeur(configuration.Securite) + Environment.NewLine +
                "- Identifiant SMTP renseigné : " + OuiNon(!string.IsNullOrWhiteSpace(configuration.IdentifiantSmtp)) + Environment.NewLine +
                "- Mot de passe SMTP renseigné : " + OuiNon(!string.IsNullOrWhiteSpace(configuration.MotDePasseSmtp)) + Environment.NewLine +
                Environment.NewLine +
                "Destinataires :" + Environment.NewLine +
                "- Expéditeur renseigné : " + OuiNon(!string.IsNullOrWhiteSpace(configuration.ExpediteurFrom)) + Environment.NewLine +
                "- Nombre de destinataires To : " + CompterEmails(configuration.DestinataireTo) + Environment.NewLine +
                "- Nombre de destinataires CC : " + CompterEmails(configuration.CopieCc) + Environment.NewLine +
                "- Nombre de destinataires BCC : " + CompterEmails(configuration.CopieCacheeBcc) + Environment.NewLine +
                Environment.NewLine +
                "Message :" + Environment.NewLine +
                "- Objet renseigné : " + OuiNon(!string.IsNullOrWhiteSpace(configuration.Objet)) + Environment.NewLine +
                "- Corps renseigné : " + OuiNon(!string.IsNullOrWhiteSpace(configuration.CorpsMessage)) + Environment.NewLine +
                Environment.NewLine +
                "Options :" + Environment.NewLine +
                "- Envoi email activé : " + OuiNon(configuration.ActiverEnvoiEmail) + Environment.NewLine +
                "- Joindre fichier log : " + OuiNon(configuration.JoindreFichierLog);
        }

        private void EnregistrerLogUtilisateur(string action, string details)
        {
            _ = Task.Run(async () =>
            {
                try
                {
                    await _authentificationController.AjouterLogAsync(action, details);
                }
                catch
                {
                    // L'échec du log utilisateur ne doit pas bloquer l'enregistrement métier.
                }
            });
        }

        private int CompterEmails(string adresses)
        {
            if (string.IsNullOrWhiteSpace(adresses))
            {
                return 0;
            }

            return adresses
                .Split(',', ';')
                .Select(adresse => adresse.Trim())
                .Count(adresse => !string.IsNullOrWhiteSpace(adresse));
        }

        private string AfficherValeur(string valeur)
        {
            return string.IsNullOrWhiteSpace(valeur) ? "Non renseigné" : valeur;
        }

        private string OuiNon(bool valeur)
        {
            return valeur ? "Oui" : "Non";
        }
    }
}