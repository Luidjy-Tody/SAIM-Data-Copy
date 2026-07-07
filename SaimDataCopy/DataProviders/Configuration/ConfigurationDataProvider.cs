using Newtonsoft.Json;
using SaimDataCopy.Helpers;
using SaimDataCopy.Models.Configuration;

namespace SaimDataCopy.DataProviders.Configuration
{
    // DataProvider de configuration.
    // Il sauvegarde et charge les paramètres depuis un fichier JSON.
    public class ConfigurationDataProvider : IConfigurationDataProvider
    {
        private readonly string _cheminFichierConfiguration;

        public ConfigurationDataProvider()
        {
            string dossierData = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");

            if (!Directory.Exists(dossierData))
            {
                Directory.CreateDirectory(dossierData);
            }

            _cheminFichierConfiguration = Path.Combine(dossierData, "configuration_execution.json");
        }

        public void EnregistrerConfiguration(ConfigurationModel configuration)
        {
            ConfigurationModel configurationASauvegarder = new ConfigurationModel
            {
                ServeurSource = new ServeurConfigModel
                {
                    TypeServeur = configuration.ServeurSource.TypeServeur,
                    NomServeur = configuration.ServeurSource.NomServeur,

                    // On chiffre toute la chaîne de connexion si elle est remplie.
                    ChaineConnexion = SecuriteMotDePasseHelper.Chiffrer(configuration.ServeurSource.ChaineConnexion),

                    Identifiant = configuration.ServeurSource.Identifiant,

                    // On chiffre aussi le mot de passe séparé.
                    MotDePasse = SecuriteMotDePasseHelper.Chiffrer(configuration.ServeurSource.MotDePasse),

                    Port = configuration.ServeurSource.Port
                },

                ServeurCible = new ServeurConfigModel
                {
                    TypeServeur = configuration.ServeurCible.TypeServeur,
                    NomServeur = configuration.ServeurCible.NomServeur,

                    // On chiffre toute la chaîne de connexion si elle est remplie.
                    ChaineConnexion = SecuriteMotDePasseHelper.Chiffrer(configuration.ServeurCible.ChaineConnexion),

                    Identifiant = configuration.ServeurCible.Identifiant,

                    // On chiffre aussi le mot de passe séparé.
                    MotDePasse = SecuriteMotDePasseHelper.Chiffrer(configuration.ServeurCible.MotDePasse),

                    Port = configuration.ServeurCible.Port
                },

                ModeCopie = configuration.ModeCopie,
                ComportementErreur = configuration.ComportementErreur,
                TentativesReprise = configuration.TentativesReprise
            };

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

                // On déchiffre la chaîne de connexion source.
                configuration.ServeurSource.ChaineConnexion = SecuriteMotDePasseHelper.Dechiffrer(configuration.ServeurSource.ChaineConnexion);

                // On déchiffre le mot de passe source.
                configuration.ServeurSource.MotDePasse = SecuriteMotDePasseHelper.Dechiffrer(configuration.ServeurSource.MotDePasse);

                // On déchiffre la chaîne de connexion cible.
                configuration.ServeurCible.ChaineConnexion = SecuriteMotDePasseHelper.Dechiffrer(configuration.ServeurCible.ChaineConnexion);

                // On déchiffre le mot de passe cible.
                configuration.ServeurCible.MotDePasse = SecuriteMotDePasseHelper.Dechiffrer(configuration.ServeurCible.MotDePasse);

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