using SaimDataCopy.DataProviders.Authentification;
using SaimDataCopy.Models.Authentification;
using SaimDataCopy.Helpers;
using SaimDataCopy.DataProviders.Email;
using SaimDataCopy.Models.Email;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
namespace SaimDataCopy.Services.Authentification
{
    public class AuthentificationService : IAuthentificationService
    {
        private readonly IAuthentificationDataProvider _dataProvider;

        public UtilisateurModel? UtilisateurConnecte { get; private set; }

        public AuthentificationService()
            : this(new AuthentificationDataProvider())
        {
        }

        public AuthentificationService(IAuthentificationDataProvider dataProvider)
        {
            _dataProvider = dataProvider;
        }

        public async Task<bool> ConnecterAsync(string identifiantOuEmail, string motDePasse)
        {
            if (string.IsNullOrWhiteSpace(identifiantOuEmail) || string.IsNullOrWhiteSpace(motDePasse))
            {
                return false;
            }

            identifiantOuEmail = identifiantOuEmail.Trim();

            UtilisateurModel? utilisateur =
                await _dataProvider.RecupererUtilisateurParIdentifiantOuEmailAsync(identifiantOuEmail);

            if (utilisateur == null || !utilisateur.EstActif)
            {
                return false;
            }

            bool motDePasseCorrect =
                SecuriteMotDePasseHelper.VerifierMotDePasse(motDePasse, utilisateur.MotDePasseHash);

            if (!motDePasseCorrect)
            {
                return false;
            }

            utilisateur.DerniereConnexion = DateTime.Now;
            await _dataProvider.ModifierUtilisateurAsync(utilisateur);

            UtilisateurConnecte = utilisateur;

            await AjouterLogAsync(
                utilisateur.Id,
                utilisateur.Identifiant,
                "Connexion",
                "Connexion réussie."
            );

            return true;
        }

        public async Task<bool> VerifierAuthentificationAdminAsync(string identifiantOuEmail, string motDePasse)
        {
            if (string.IsNullOrWhiteSpace(identifiantOuEmail) ||
                string.IsNullOrWhiteSpace(motDePasse))
            {
                return false;
            }

            identifiantOuEmail = identifiantOuEmail.Trim();

            UtilisateurModel? utilisateur =
                await _dataProvider.RecupererUtilisateurParIdentifiantOuEmailAsync(identifiantOuEmail);

            if (utilisateur == null || !utilisateur.EstActif)
            {
                return false;
            }

            bool estAdmin =
                utilisateur.Statut.Equals("Admin", StringComparison.OrdinalIgnoreCase);

            if (!estAdmin)
            {
                return false;
            }

            bool motDePasseCorrect =
                SecuriteMotDePasseHelper.VerifierMotDePasse(
                    motDePasse,
                    utilisateur.MotDePasseHash
                );

            return motDePasseCorrect;
        }

        public async Task<bool> InscrireAsync(
            string nomComplet,
            string identifiant,
            string email,
            string motDePasse,
            string statut)
        {
            string message = await InscrireEtRetournerMessageAsync(
                nomComplet,
                identifiant,
                email,
                motDePasse,
                statut
            );

            return message == "Compte créé avec succès. Vous pouvez vous connecter.";
        }

        public async Task<string> InscrireEtRetournerMessageAsync(
    string nomComplet,
    string identifiant,
    string email,
    string motDePasse,
    string statut)
        {
            if (string.IsNullOrWhiteSpace(nomComplet) ||
                string.IsNullOrWhiteSpace(identifiant) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(motDePasse))
            {
                return "Veuillez remplir tous les champs obligatoires.";
            }

            nomComplet = nomComplet.Trim();
            identifiant = identifiant.Trim();
            email = email.Trim();
            statut = NormaliserStatutUtilisateur(statut);

            UtilisateurModel? identifiantExistant =
                await _dataProvider.RecupererUtilisateurParIdentifiantAsync(identifiant);

            if (identifiantExistant != null)
            {
                return "Cet identifiant existe déjà.";
            }

            UtilisateurModel? emailExistant =
                await _dataProvider.RecupererUtilisateurParEmailAsync(email);

            if (emailExistant != null)
            {
                return "Cet email existe déjà.";
            }

            UtilisateurModel utilisateur = new UtilisateurModel
            {
                NomComplet = nomComplet,
                Identifiant = identifiant,
                Email = email,
                MotDePasseHash = SecuriteMotDePasseHelper.HasherMotDePasse(motDePasse),
                DateCreation = DateTime.Now,
                EstActif = true,
                Statut = statut
            };

            await _dataProvider.AjouterUtilisateurAsync(utilisateur);

            await AjouterLogAsync(
                utilisateur.Id,
                utilisateur.Identifiant,
                "Inscription",
                "Nouveau compte utilisateur créé avec le statut : " + statut + "."
            );

            return "Compte créé avec succès. Vous pouvez vous connecter.";
        }

        public async Task<string> DemanderCodeReinitialisationMotDePasseAsync(string identifiantOuEmail)
        {
            if (string.IsNullOrWhiteSpace(identifiantOuEmail))
            {
                return "Veuillez saisir votre email ou votre identifiant.";
            }

            identifiantOuEmail = identifiantOuEmail.Trim();

            UtilisateurModel? utilisateur =
                await _dataProvider.RecupererUtilisateurParIdentifiantOuEmailAsync(identifiantOuEmail);

            // On retourne un message neutre pour ne pas afficher clairement
            // si un compte existe ou non.
            if (utilisateur == null || !utilisateur.EstActif)
            {
                return "Si un compte actif correspond à cette information, un code de réinitialisation a été envoyé par email.";
            }

            string code = GenererCodeReinitialisation();

            await _dataProvider.MarquerCodesUtilisateurCommeUtilisesAsync(utilisateur.Id);

            CodeReinitialisationMotDePasseModel codeReinitialisation =
                new CodeReinitialisationMotDePasseModel
                {
                    UtilisateurId = utilisateur.Id,
                    CodeHash = SecuriteMotDePasseHelper.HasherMotDePasse(code),
                    DateCreation = DateTime.Now,
                    DateExpiration = DateTime.Now.AddMinutes(10),
                    EstUtilise = false
                };

            await _dataProvider.AjouterCodeReinitialisationAsync(codeReinitialisation);

            bool emailEnvoye = EnvoyerCodeReinitialisationParEmail(
                utilisateur,
                code,
                out string messageErreurEmail
            );

            if (!emailEnvoye)
            {
                await _dataProvider.MarquerCodesUtilisateurCommeUtilisesAsync(utilisateur.Id);

                return "Erreur pendant l'envoi du code : " + messageErreurEmail;
            }

            await AjouterLogAsync(
                utilisateur.Id,
                utilisateur.Identifiant,
                "Demande réinitialisation mot de passe",
                "Un code de réinitialisation a été généré et envoyé par email."
            );

            return "Si un compte actif correspond à cette information, un code de réinitialisation a été envoyé par email.";
        }

        private static string GenererCodeReinitialisation()
        {
            int nombre = RandomNumberGenerator.GetInt32(100000, 1000000);

            return nombre.ToString();
        }

        private bool EnvoyerCodeReinitialisationParEmail(
            UtilisateurModel utilisateur,
            string code,
            out string messageErreur)
        {
            messageErreur = string.Empty;

            try
            {
                EmailDataProvider emailDataProvider = new EmailDataProvider();
                EmailConfigModel configuration = emailDataProvider.Charger();

                if (!ConfigurationEmailValidePourReinitialisation(configuration, out messageErreur))
                {
                    return false;
                }

                using MailMessage mail = new MailMessage();

                mail.From = new MailAddress(configuration.ExpediteurFrom);
                mail.To.Add(utilisateur.Email);
                mail.Subject = "Code de réinitialisation - SaimDataCopy";

                mail.Body =
                    "Bonjour " + utilisateur.NomComplet + "," + Environment.NewLine +
                    Environment.NewLine +
                    "Votre code de réinitialisation SaimDataCopy est :" + Environment.NewLine +
                    Environment.NewLine +
                    code + Environment.NewLine +
                    Environment.NewLine +
                    "Ce code est valable pendant 10 minutes." + Environment.NewLine +
                    "Si vous n'avez pas demandé cette action, ignorez simplement cet email." + Environment.NewLine +
                    Environment.NewLine +
                    "SaimDataCopy";

                using SmtpClient client = new SmtpClient(
                    configuration.ServeurSmtp,
                    configuration.Port
                );

                client.EnableSsl = SecuriteEmailAvecSsl(configuration.Securite);

                if (!string.IsNullOrWhiteSpace(configuration.IdentifiantSmtp))
                {
                    client.Credentials = new NetworkCredential(
                        configuration.IdentifiantSmtp,
                        configuration.MotDePasseSmtp
                    );
                }

                client.Send(mail);

                return true;
            }
            catch (Exception ex)
            {
                messageErreur = ex.Message;
                return false;
            }
        }

        private static bool ConfigurationEmailValidePourReinitialisation(
            EmailConfigModel configuration,
            out string message)
        {
            if (!configuration.ActiverEnvoiEmail)
            {
                message = "l'envoi email est désactivé dans les paramètres.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(configuration.ServeurSmtp))
            {
                message = "le serveur SMTP n'est pas renseigné.";
                return false;
            }

            if (configuration.Port <= 0)
            {
                message = "le port SMTP est invalide.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(configuration.ExpediteurFrom))
            {
                message = "l'adresse expéditeur n'est pas renseignée.";
                return false;
            }

            message = string.Empty;
            return true;
        }

        private static bool SecuriteEmailAvecSsl(string securite)
        {
            if (string.IsNullOrWhiteSpace(securite))
            {
                return false;
            }

            return securite.Contains("SSL", StringComparison.OrdinalIgnoreCase) ||
                   securite.Contains("TLS", StringComparison.OrdinalIgnoreCase) ||
                   securite.Contains("STARTTLS", StringComparison.OrdinalIgnoreCase);
        }

        private static string NormaliserStatutUtilisateur(string statut)
        {
            if (string.IsNullOrWhiteSpace(statut))
            {
                return "User";
            }

            statut = statut.Trim();

            if (statut.Equals("Admin", StringComparison.OrdinalIgnoreCase))
            {
                return "Admin";
            }

            return "User";
        }

        public async Task<bool> ExisteAuMoinsUnUtilisateurAsync()
        {
            return await _dataProvider.ExisteAuMoinsUnUtilisateurAsync();
        }

        public async Task AjouterLogAsync(
            int? utilisateurId,
            string nomUtilisateur,
            string action,
            string details)
        {
            LogUtilisateurModel log = new LogUtilisateurModel
            {
                UtilisateurId = utilisateurId,
                NomUtilisateur = nomUtilisateur,
                Action = action,
                Details = details,
                DateHeure = DateTime.Now
            };

            await _dataProvider.AjouterLogAsync(log);
        }
    }
}