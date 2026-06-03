using Newtonsoft.Json;
using SaimDataCopy.Models.Email;

namespace SaimDataCopy.DataProviders.Email
{
    /// <summary>
    /// DataProvider pour les paramètres email.
    /// Il s'occupe seulement de lire et écrire les données dans un fichier JSON.
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

            EmailConfigModel? configuration = JsonConvert.DeserializeObject<EmailConfigModel>(contenuJson);

            return configuration ?? new EmailConfigModel();
        }

        /// <summary>
        /// Enregistre les paramètres email dans le fichier JSON.
        /// </summary>
        public void Enregistrer(EmailConfigModel configuration)
        {
            string? dossier = Path.GetDirectoryName(_cheminFichier);

            if (!string.IsNullOrWhiteSpace(dossier))
            {
                Directory.CreateDirectory(dossier);
            }

            string contenuJson = JsonConvert.SerializeObject(
                configuration,
                Formatting.Indented
            );

            File.WriteAllText(_cheminFichier, contenuJson);
        }
    }
}