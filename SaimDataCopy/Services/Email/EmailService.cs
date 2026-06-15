using SaimDataCopy.DataProviders.Email;
using SaimDataCopy.Models.Email;
using System.Net;
using System.Net.Mail;

namespace SaimDataCopy.Services.Email
{
    /// <summary>
    /// Service pour gérer la logique métier des paramètres email.
    /// Il valide les données, sauvegarde les paramètres et envoie les emails.
    /// </summary>
    public class EmailService : IEmailService
    {
        private readonly IEmailDataProvider _emailDataProvider;

        public EmailService(IEmailDataProvider emailDataProvider)
        {
            _emailDataProvider = emailDataProvider;
        }

        public EmailConfigModel Charger()
        {
            return _emailDataProvider.Charger();
        }

        public bool Enregistrer(EmailConfigModel configuration, out string message)
        {
            if (!ValiderConfiguration(configuration, out message))
            {
                return false;
            }

            _emailDataProvider.Enregistrer(configuration);

            message = "Paramètres email enregistrés avec succès.";
            return true;
        }

        public bool EnvoyerEmailTest(EmailConfigModel configuration, out string message)
        {
            if (!ValiderConfigurationPourTest(configuration, out message))
            {
                return false;
            }

            try
            {
                string dateActuelle = DateTime.Now.ToString("dd/MM/yyyy");
                string heureActuelle = DateTime.Now.ToString("HH:mm:ss");

                string objetTest = RemplacerVariables(
                    configuration.Objet,
                    "DB_TestRH, DB_TestVentes",
                    "Test email",
                    dateActuelle,
                    heureActuelle
                );

                string corpsTest = RemplacerVariables(
                    configuration.CorpsMessage,
                    "DB_TestRH, DB_TestVentes",
                    "Test email",
                    dateActuelle,
                    heureActuelle
                );

                using MailMessage mail = CreerMailMessage(
                    configuration,
                    "[TEST] " + objetTest,
                    corpsTest,
                    null
                );

                EnvoyerMail(configuration, mail);

                message = "E-mail de test envoyé avec succès.";
                return true;
            }
            catch (Exception ex)
            {
                message = "Erreur pendant l'envoi de l'e-mail de test : "
                    + ObtenirMessageErreurEmail(ex);

                return false;
            }
        }

        public bool EnvoyerEmailConfirmationCopie(
            string listeBases,
            string duree,
            string? cheminFichierLog,
            out string message
        )
        {
            EmailConfigModel configuration = Charger();

            // Si l'envoi email est désactivé, on ne fait rien.
            // Ce n'est pas une erreur, car l'utilisateur a choisi de désactiver l'envoi.
            if (!configuration.ActiverEnvoiEmail)
            {
                message = "Envoi email désactivé dans les paramètres.";
                return false;
            }

            if (!ValiderConfigurationPourEnvoi(configuration, out message))
            {
                return false;
            }

            try
            {
                string dateActuelle = DateTime.Now.ToString("dd/MM/yyyy");
                string heureActuelle = DateTime.Now.ToString("HH:mm:ss");

                string objet = RemplacerVariables(
                    configuration.Objet,
                    listeBases,
                    duree,
                    dateActuelle,
                    heureActuelle
                );

                string corpsMessage = RemplacerVariables(
                    configuration.CorpsMessage,
                    listeBases,
                    duree,
                    dateActuelle,
                    heureActuelle
                );

                using MailMessage mail = CreerMailMessage(
                    configuration,
                    objet,
                    corpsMessage,
                    cheminFichierLog
                );

                EnvoyerMail(configuration, mail);

                message = "E-mail de confirmation envoyé avec succès.";
                return true;
            }
            catch (Exception ex)
            {
                message = "Erreur pendant l'envoi de l'e-mail de confirmation : "
                    + ObtenirMessageErreurEmail(ex);

                return false;
            }
        }

        private MailMessage CreerMailMessage(
            EmailConfigModel configuration,
            string objet,
            string corpsMessage,
            string? cheminFichierLog)
        {
            MailMessage mail = new MailMessage();

            mail.From = new MailAddress(configuration.ExpediteurFrom);

            AjouterDestinataires(mail.To, configuration.DestinataireTo);
            AjouterDestinataires(mail.CC, configuration.CopieCc);
            AjouterDestinataires(mail.Bcc, configuration.CopieCacheeBcc);

            mail.Subject = objet;

            // On garde le corps saisi par l'utilisateur,
            // mais on le transforme en HTML simple pour respecter la logique du superviseur.
            mail.Body = ConvertirTexteEnHtml(corpsMessage);
            mail.IsBodyHtml = true;

            // Si l'utilisateur veut joindre le fichier log,
            // on vérifie d'abord que le fichier existe.
            if (configuration.JoindreFichierLog &&
                !string.IsNullOrWhiteSpace(cheminFichierLog) &&
                File.Exists(cheminFichierLog))
            {
                mail.Attachments.Add(new Attachment(cheminFichierLog));
            }

            return mail;
        }

        private void EnvoyerMail(EmailConfigModel configuration, MailMessage mail)
        {
            string identifiantConnexion = ObtenirIdentifiantConnexion(configuration);

            // On normalise la sécurité pour éviter les problèmes d'espace,
            // de casse ou de valeur vide.
            string securite = NormaliserSecurite(configuration.Securite);

            bool utiliserSsl = SecuriteUtiliseSsl(securite);

            // Sécurité supplémentaire :
            // Le port 587 correspond généralement à TLS / STARTTLS.
            // Donc si l'utilisateur utilise 587, on force SSL/TLS à true.
            if (configuration.Port == 587)
            {
                utiliserSsl = true;
            }

            using SmtpClient smtpClient = new SmtpClient();

            smtpClient.Host = configuration.ServeurSmtp.Trim();
            smtpClient.Port = configuration.Port;
            smtpClient.EnableSsl = utiliserSsl;
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = new NetworkCredential(
                identifiantConnexion,
                configuration.MotDePasseSmtp
            );
            smtpClient.Timeout = 30000;

            smtpClient.Send(mail);
        }

        private string NormaliserSecurite(string securite)
        {
            if (string.IsNullOrWhiteSpace(securite))
            {
                return "TLS";
            }

            return securite.Trim() switch
            {
                "TLS" => "TLS",
                "SSL" => "SSL",
                "Aucune" => "Aucune",
                _ => "TLS"
            };
        }

        private string ObtenirIdentifiantConnexion(EmailConfigModel configuration)
        {
            // Si l'utilisateur remplit "Identifiant SMTP",
            // on utilise cette adresse pour se connecter au serveur SMTP.
            // Exemple : adresse Outlook ou adresse entreprise.
            if (!string.IsNullOrWhiteSpace(configuration.IdentifiantSmtp))
            {
                return configuration.IdentifiantSmtp.Trim();
            }

            // Sinon, on utilise l'adresse expéditeur comme dans le code du superviseur.
            return configuration.ExpediteurFrom.Trim();
        }

        private string RemplacerVariables(
            string texte,
            string listeBases,
            string duree,
            string dateActuelle,
            string heureActuelle)
        {
            return texte
                .Replace("{date}", dateActuelle)
                .Replace("{heure}", heureActuelle)
                .Replace("{liste_bases}", listeBases)
                .Replace("{duree}", duree);
        }

        private string ConvertirTexteEnHtml(string texte)
        {
            string texteEncode = WebUtility.HtmlEncode(texte);

            return texteEncode
                .Replace("\r\n", "<br>")
                .Replace("\n", "<br>");
        }

        private bool ValiderConfiguration(EmailConfigModel configuration, out string message)
        {
            // Si l'envoi email est désactivé, on autorise l'enregistrement
            // même si les champs SMTP ne sont pas encore remplis.
            if (!configuration.ActiverEnvoiEmail)
            {
                message = string.Empty;
                return true;
            }

            // Si l'envoi email est activé, tous les champs nécessaires doivent être valides.
            return ValiderConfigurationPourEnvoi(configuration, out message);
        }

        private bool ValiderConfigurationPourTest(EmailConfigModel configuration, out string message)
        {
            // Pour le test email, on vérifie toujours les champs nécessaires,
            // même si l'envoi automatique est désactivé.
            return ValiderConfigurationPourEnvoi(configuration, out message);
        }

        private bool ValiderConfigurationPourEnvoi(EmailConfigModel configuration, out string message)
        {
            if (string.IsNullOrWhiteSpace(configuration.ServeurSmtp))
            {
                message = "Le serveur SMTP est obligatoire.";
                return false;
            }

            if (configuration.Port <= 0)
            {
                message = "Le port SMTP doit être supérieur à 0.";
                return false;
            }

            if (!SecuriteEstValide(configuration.Securite))
            {
                message = "La sécurité doit être TLS, SSL ou Aucune.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(configuration.ExpediteurFrom))
            {
                message = "L'expéditeur est obligatoire.";
                return false;
            }

            if (!EmailsSontValides(configuration.ExpediteurFrom))
            {
                message = "L'adresse de l'expéditeur est invalide.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(configuration.DestinataireTo))
            {
                message = "Le destinataire est obligatoire.";
                return false;
            }

            if (!EmailsSontValides(configuration.DestinataireTo))
            {
                message = "Le champ Destinataire To contient une adresse e-mail invalide.";
                return false;
            }

            if (!EmailsSontValides(configuration.CopieCc))
            {
                message = "Le champ Copie CC contient une adresse e-mail invalide.";
                return false;
            }

            if (!EmailsSontValides(configuration.CopieCacheeBcc))
            {
                message = "Le champ Copie cachée BCC contient une adresse e-mail invalide.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(configuration.MotDePasseSmtp))
            {
                message = "Le mot de passe SMTP est obligatoire.";
                return false;
            }

            message = string.Empty;
            return true;
        }

        private bool SecuriteEstValide(string securite)
        {
            string securiteNormalisee = NormaliserSecurite(securite);

            return securiteNormalisee switch
            {
                "TLS" => true,
                "SSL" => true,
                "Aucune" => true,
                _ => false
            };
        }

        private bool SecuriteUtiliseSsl(string securite)
        {
            string securiteNormalisee = NormaliserSecurite(securite);

            return securiteNormalisee switch
            {
                "TLS" => true,
                "SSL" => true,
                "Aucune" => false,
                _ => true
            };
        }

        private void AjouterDestinataires(MailAddressCollection liste, string adresses)
        {
            if (string.IsNullOrWhiteSpace(adresses))
            {
                return;
            }

            foreach (string adresse in SeparerEmails(adresses))
            {
                liste.Add(new MailAddress(adresse));
            }
        }

        private bool EmailsSontValides(string adresses)
        {
            if (string.IsNullOrWhiteSpace(adresses))
            {
                return true;
            }

            foreach (string adresse in SeparerEmails(adresses))
            {
                try
                {
                    _ = new MailAddress(adresse);
                }
                catch
                {
                    return false;
                }
            }

            return true;
        }

        private List<string> SeparerEmails(string adresses)
        {
            return adresses
                .Split(',', ';')
                .Select(adresse => adresse.Trim())
                .Where(adresse => !string.IsNullOrWhiteSpace(adresse))
                .ToList();
        }

        private string ObtenirMessageErreurEmail(Exception exception)
        {
            if (exception is SmtpException erreurSmtp)
            {
                return erreurSmtp.StatusCode switch
                {
                    SmtpStatusCode.MailboxUnavailable =>
                        "Adresse e-mail introuvable ou boîte mail indisponible.",

                    SmtpStatusCode.ClientNotPermitted =>
                        "Le serveur SMTP a refusé l'envoi. Vérifiez les droits du compte e-mail.",

                    SmtpStatusCode.MustIssueStartTlsFirst =>
                        "Le serveur SMTP demande TLS. Vérifiez le champ Sécurité.",

                    SmtpStatusCode.GeneralFailure =>
                        "Connexion au serveur SMTP impossible. Vérifiez le serveur, le port et le réseau.",

                    _ =>
                        $"{erreurSmtp.Message} ({erreurSmtp.StatusCode})"
                };
            }

            if (exception is FormatException)
            {
                return "Une adresse e-mail est mal écrite.";
            }

            return exception.Message;
        }
    }
}