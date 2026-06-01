using SaimDataCopy.DataAccess;
using SaimDataCopy.Models.Configuration;

namespace SaimDataCopy.DataProviders.Configuration
{
    // DataProvider de configuration.
    public class ConfigurationDataProvider : IConfigurationDataProvider
    {
        public void EnregistrerConfiguration(ConfigurationModel configuration)
        {
            using SaimDbContext dbContext = SaimDbContextFactory.CreerDbContext();

            // Pour la configuration générale, on garde une seule ligne.
            ConfigurationSqlModel? configurationSql = dbContext.Configurations
                .OrderBy(c => c.Id)
                .FirstOrDefault();

            if (configurationSql == null)
            {
                configurationSql = new ConfigurationSqlModel();
                dbContext.Configurations.Add(configurationSql);
            }

            RemplirConfigurationSql(configurationSql, configuration);

            dbContext.SaveChanges();
        }

        public ConfigurationModel? ChargerConfiguration()
        {
            using SaimDbContext dbContext = SaimDbContextFactory.CreerDbContext();

            ConfigurationSqlModel? configurationSql = dbContext.Configurations
                .OrderBy(c => c.Id)
                .FirstOrDefault();

            if (configurationSql == null)
            {
                return null;
            }

            return ConvertirVersConfigurationModel(configurationSql);
        }

        private void RemplirConfigurationSql(
            ConfigurationSqlModel configurationSql,
            ConfigurationModel configuration)
        {
            // Serveur source
            configurationSql.ServeurSourceNom = configuration.ServeurSource.NomServeur;
            configurationSql.ServeurSourceChaineConnexion = configuration.ServeurSource.ChaineConnexion;
            configurationSql.ServeurSourceIdentifiant = configuration.ServeurSource.Identifiant;
            configurationSql.ServeurSourceMotDePasse = configuration.ServeurSource.MotDePasse;
            configurationSql.ServeurSourcePort = configuration.ServeurSource.Port;

            // Serveur cible
            configurationSql.ServeurCibleNom = configuration.ServeurCible.NomServeur;
            configurationSql.ServeurCibleChaineConnexion = configuration.ServeurCible.ChaineConnexion;
            configurationSql.ServeurCibleIdentifiant = configuration.ServeurCible.Identifiant;
            configurationSql.ServeurCibleMotDePasse = configuration.ServeurCible.MotDePasse;
            configurationSql.ServeurCiblePort = configuration.ServeurCible.Port;

            // Paramètres généraux
            configurationSql.ModeCopie = configuration.ModeCopie;
            configurationSql.ComportementErreur = configuration.ComportementErreur;
            configurationSql.TentativesReprise = configuration.TentativesReprise;
            configurationSql.DateModification = DateTime.Now;
        }

        private ConfigurationModel ConvertirVersConfigurationModel(
            ConfigurationSqlModel configurationSql)
        {
            return new ConfigurationModel
            {
                ServeurSource = new ServeurConfigModel
                {
                    NomServeur = configurationSql.ServeurSourceNom,
                    ChaineConnexion = configurationSql.ServeurSourceChaineConnexion,
                    Identifiant = configurationSql.ServeurSourceIdentifiant,
                    MotDePasse = configurationSql.ServeurSourceMotDePasse,
                    Port = configurationSql.ServeurSourcePort
                },

                ServeurCible = new ServeurConfigModel
                {
                    NomServeur = configurationSql.ServeurCibleNom,
                    ChaineConnexion = configurationSql.ServeurCibleChaineConnexion,
                    Identifiant = configurationSql.ServeurCibleIdentifiant,
                    MotDePasse = configurationSql.ServeurCibleMotDePasse,
                    Port = configurationSql.ServeurCiblePort
                },

                ModeCopie = configurationSql.ModeCopie,
                ComportementErreur = configurationSql.ComportementErreur,
                TentativesReprise = configurationSql.TentativesReprise
            };
        }
    }
}