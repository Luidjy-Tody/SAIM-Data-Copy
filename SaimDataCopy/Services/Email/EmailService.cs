using MailKitSmtpClient = MailKit.Net.Smtp.SmtpClient;
using MailKit.Security;
using MimeKit;
using SaimDataCopy.DataProviders.Email;
using SaimDataCopy.Models.Email;
using System.Net.Mail;

namespace SaimDataCopy.Services.Email
{
    /// <summary>
    /// Service pour gérer la logique métier des paramètres email.
    /// Ici on valide les données, on sauvegarde et on peut envoyer un email de test.
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
                MimeMessage email = new MimeMessage();

                email.From.Add(MailboxAddress.Parse(configuration.ExpediteurFrom));
                AjouterDestinataires(email.To, configuration.DestinataireTo);
                AjouterDestinataires(email.Cc, configuration.CopieCc);
                AjouterDestinataires(email.Bcc, configuration.CopieCacheeBcc);

                email.Subject = configuration.Objet;

                email.Body = new TextPart("plain")
                {
                    Text = configuration.CorpsMessage
                };

                using MailKitSmtpClient smtpClient = new MailKitSmtpClient();
                SecureSocketOptions optionSecurite = ObtenirOptionSecurite(configuration.Securite);

                smtpClient.Connect(
                    configuration.ServeurSmtp,
                    configuration.Port,
                    optionSecurite
                );

                smtpClient.Authenticate(
                    configuration.IdentifiantSmtp,
                    configuration.MotDePasseSmtp
                );

                smtpClient.Send(email);
                smtpClient.Disconnect(true);

                message = "E-mail de test envoyé avec succès.";
                return true;
            }
            catch (Exception ex)
            {
                message = "Erreur pendant l'envoi de l'e-mail de test : " + ex.Message;
                return false;
            }
        }

        private bool ValiderConfiguration(EmailConfigModel configuration, out string message)
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

            message = string.Empty;
            return true;
        }

        private bool ValiderConfigurationPourTest(EmailConfigModel configuration, out string message)
        {
            if (!ValiderConfiguration(configuration, out message))
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(configuration.ExpediteurFrom))
            {
                message = "L'expéditeur est obligatoire pour envoyer un e-mail de test.";
                return false;
            }

            if (!EmailsSontValides(configuration.ExpediteurFrom))
            {
                message = "L'adresse de l'expéditeur est invalide.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(configuration.IdentifiantSmtp))
            {
                message = "L'identifiant SMTP est obligatoire pour envoyer un e-mail de test.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(configuration.MotDePasseSmtp))
            {
                message = "Le mot de passe SMTP est obligatoire pour envoyer un e-mail de test.";
                return false;
            }

            return true;
        }

        private SecureSocketOptions ObtenirOptionSecurite(string securite)
        {
            return securite switch
            {
                "TLS" => SecureSocketOptions.StartTls,
                "SSL" => SecureSocketOptions.SslOnConnect,
                "Aucune" => SecureSocketOptions.None,
                _ => SecureSocketOptions.Auto
            };
        }

        private bool SecuriteEstValide(string securite)
        {
            return securite switch
            {
                "TLS" => true,
                "SSL" => true,
                "Aucune" => true,
                _ => false
            };
        }

        private void AjouterDestinataires(InternetAddressList liste, string adresses)
        {
            if (string.IsNullOrWhiteSpace(adresses))
            {
                return;
            }

            foreach (string adresse in SeparerEmails(adresses))
            {
                liste.Add(MailboxAddress.Parse(adresse));
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
    }
}