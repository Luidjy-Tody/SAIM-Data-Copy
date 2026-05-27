using Newtonsoft.Json;
using SaimDataCopy.Models.Email;

namespace SaimDataCopy.DataProviders.Email
{
    /// <summary>
    /// DataProvider pour les paramètres email.
    /// Il s'occupe seulement de lire et écrire les données.
    /// </summary>
    public class EmailDataProvider : IEmailDataProvider
    {
        // Nom du fichier JSON où les paramètres email seront sauvegardés.
        private readonly string _cheminFichier;

        public EmailDataProvider()
        {
            // Le fichier sera créé dans le dossier de l'application.
            _cheminFichier = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "email_config.json"
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

            EmailConfigModel? configuration = JsonConvert.DeserializeObject<EmailConfigModel>( contenuJson );

            return configuration ?? new EmailConfigModel();
        }

        /// <summary>
        /// Enregistre les paramètres email dans le fichier JSON.
        /// </summary>
        
        public void Enregistrer( EmailConfigModel configuration)
        {
            string contenuJson = JsonConvert.SerializeObject( configuration, Formatting.Indented );

            File.WriteAllText(_cheminFichier, contenuJson );
        }
    }
}
