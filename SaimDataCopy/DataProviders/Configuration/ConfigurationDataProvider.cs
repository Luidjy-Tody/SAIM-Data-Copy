using Newtonsoft.Json;
using SaimDataCopy.Helpers;
using SaimDataCopy.Models.Configuration;

namespace SaimDataCopy.DataProviders.Configuration
{
    // DataProvider de configuration.
    // Il sauvegarde et charge les paramètres depuis un fichier JSON.
    // Les mots de passe source et cible sont chiffrés avec DPAPI.
    public class ConfigurationDataProvider : IConfigurationDataProvider
    {
        private readonly string _cheminFichierConfiguration;

        public ConfigurationDataProvider()
        {
            // Le fichier JSON sera stocké dans le dossier Data de l'application.
            string dossierData = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");

            if (!Directory.Exists(dossierData))
            {
                Directory.CreateDirectory(dossierData);
            }

            _cheminFichierConfiguration = Path.Combine(dossierData, "configuration_execution.json");
        }

        public void EnregistrerConfiguration(ConfigurationModel configuration)
        {
            // On crée une copie pour éviter de modifier directement l'objet utilisé par la View.
            ConfigurationModel configurationASauvegarder = new ConfigurationModel
            {
                ServeurSource = new ServeurConfigModel
                {
                    NomServeur = configuration.ServeurSource.NomServeur,
                    ChaineConnexion = configuration.ServeurSource.ChaineConnexion,
                    Identifiant = configuration.ServeurSource.Identifiant,

                    // On chiffre le mot de passe source avant de l'écrire dans le JSON.
                    MotDePasse = SecuriteMotDePasseHelper.Chiffrer(
                        configuration.ServeurSource.MotDePasse),

                    Port = configuration.ServeurSource.Port
                },

                ServeurCible = new ServeurConfigModel
                {
                    NomServeur = configuration.ServeurCible.NomServeur,
                    ChaineConnexion = configuration.ServeurCible.ChaineConnexion,
                    Identifiant = configuration.ServeurCible.Identifiant,

                    // On chiffre le mot de passe cible avant de l'écrire dans le JSON.
                    MotDePasse = SecuriteMotDePasseHelper.Chiffrer(
                        configuration.ServeurCible.MotDePasse),

                    Port = configuration.ServeurCible.Port
                },

                ModeCopie = configuration.ModeCopie,
                ComportementErreur = configuration.ComportementErreur,
                TentativesReprise = configuration.TentativesReprise
            };

            // Sauvegarde principale dans le fichier JSON.
            string contenuJson = JsonConvert.SerializeObject(
                configurationASauvegarder,
                Formatting.Indented
            );

            File.WriteAllText(_cheminFichierConfiguration, contenuJson);
        }

        public ConfigurationModel? ChargerConfiguration()
        {
            if (!File.Exists(_cheminFichierConfiguration))
            {
                return null;
            }

            string contenuJson = File.ReadAllText(_cheminFichierConfiguration);

            if (string.IsNullOrWhiteSpace(contenuJson))
            {
                return null;
            }

            try
            {
                ConfigurationModel? configuration =
                    JsonConvert.DeserializeObject<ConfigurationModel>(contenuJson);

                if (configuration == null)
                {
                    return null;
                }

                // On déchiffre le mot de passe source après lecture du JSON.
                configuration.ServeurSource.MotDePasse =
                    SecuriteMotDePasseHelper.Dechiffrer(
                        configuration.ServeurSource.MotDePasse);

                // On déchiffre le mot de passe cible après lecture du JSON.
                configuration.ServeurCible.MotDePasse =
                    SecuriteMotDePasseHelper.Dechiffrer(
                        configuration.ServeurCible.MotDePasse);

                return configuration;
            }
            catch (JsonException)
            {
                throw new InvalidOperationException(
                    "Le fichier configuration_execution.json est invalide. Vérifiez son contenu JSON.");
            }
        }
    }
}