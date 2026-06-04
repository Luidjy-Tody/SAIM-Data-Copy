using Newtonsoft.Json;
using SaimDataCopy.Helpers;
using SaimDataCopy.Models.Email;

namespace SaimDataCopy.DataProviders.Email
{
    /// <summary>
    /// DataProvider pour les paramètres email.
    /// Il s'occupe seulement de lire et écrire les données dans un fichier JSON.
    /// Le mot de passe SMTP est chiffré avant d'être sauvegardé.
    /// </summary>
    public class EmailDataProvider : IEmailDataProvider
    {
        // Chemin complet du fichier JSON des paramètres email.
        private readonly string _cheminFichier;

        public EmailDataProvider()
        {
            // On met le fichier dans le dossier Data de l'application.
            string dossierData = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "Data"
            );

            _cheminFichier = Path.Combine(
                dossierData,
                "email_parametres.json"
            );
        }

        /// <summary>
        /// Charge les paramètres email depuis le fichier JSON.
        /// Si le fichier n'existe pas, on retourne une configuration par défaut.
        /// </summary>
        public EmailConfigModel Charger()
        {
            if (!File.Exists(_cheminFichier))
            {
                return new EmailConfigModel();
            }

            string contenuJson = File.ReadAllText(_cheminFichier);

            if (string.IsNullOrWhiteSpace(contenuJson))
            {
                return new EmailConfigModel();
            }

            EmailConfigModel? configuration =
                JsonConvert.DeserializeObject<EmailConfigModel>(contenuJson);

            if (configuration == null)
            {
                return new EmailConfigModel();
            }

            // Le mot de passe est stocké chiffré dans le JSON.
            // Ici, on le déchiffre pour que l'application puisse l'utiliser.
            configuration.MotDePasseSmtp =
                SecuriteMotDePasseHelper.Dechiffrer(configuration.MotDePasseSmtp);

            return configuration;
        }

        /// <summary>
        /// Enregistre les paramètres email dans le fichier JSON.
        /// Le mot de passe SMTP est chiffré avant l'écriture.
        /// </summary>
        public void Enregistrer(EmailConfigModel configuration)
        {
            string? dossier = Path.GetDirectoryName(_cheminFichier);

            if (!string.IsNullOrWhiteSpace(dossier))
            {
                Directory.CreateDirectory(dossier);
            }

            // On crée une copie du modèle pour éviter de modifier directement
            // l'objet utilisé par la View.
            EmailConfigModel configurationASauvegarder = new EmailConfigModel
            {
                ServeurSmtp = configuration.ServeurSmtp,
                Port = configuration.Port,
                Securite = configuration.Securite,
                IdentifiantSmtp = configuration.IdentifiantSmtp,

                // Ici, on chiffre le mot de passe avant de l'écrire dans le JSON.
                MotDePasseSmtp = SecuriteMotDePasseHelper.Chiffrer(configuration.MotDePasseSmtp),

                ExpediteurFrom = configuration.ExpediteurFrom,
                DestinataireTo = configuration.DestinataireTo,
                CopieCc = configuration.CopieCc,
                CopieCacheeBcc = configuration.CopieCacheeBcc,

                Objet = configuration.Objet,
                CorpsMessage = configuration.CorpsMessage,

                ActiverEnvoiEmail = configuration.ActiverEnvoiEmail,
                JoindreFichierLog = configuration.JoindreFichierLog
            };

            string contenuJson = JsonConvert.SerializeObject(
                configurationASauvegarder,
                Formatting.Indented
            );

            File.WriteAllText(_cheminFichier, contenuJson);
        }
    }
}