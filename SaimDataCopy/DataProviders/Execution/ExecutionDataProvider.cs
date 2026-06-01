using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using SaimDataCopy.DataProviders.BasesCopier;
using SaimDataCopy.DataProviders.Configuration;
using SaimDataCopy.Models.BasesCopier;
using SaimDataCopy.Models.Configuration;
using SaimDataCopy.Models.Execution;

namespace SaimDataCopy.DataProviders.Execution
{
    // DataProvider de la page Exécution.
    // Il s'occupe seulement de charger et enregistrer les données.
    public class ExecutionDataProvider : IExecutionDataProvider
    {
        private readonly IBasesCopierDataProvider _basesCopierDataProvider;
        private readonly IConfigurationDataProvider _configurationDataProvider;
        private readonly string _cheminFichier;

        public ExecutionDataProvider()
            : this(new BasesCopierDataProvider(), new ConfigurationDataProvider())
        {
        }

        public ExecutionDataProvider(IBasesCopierDataProvider basesCopierDataProvider)
            : this(basesCopierDataProvider, new ConfigurationDataProvider())
        {
        }

        public ExecutionDataProvider(
            IBasesCopierDataProvider basesCopierDataProvider,
            IConfigurationDataProvider configurationDataProvider)
        {
            _basesCopierDataProvider = basesCopierDataProvider;
            _configurationDataProvider = configurationDataProvider;

            // Le fichier JSON sera stocké dans le dossier Data.
            // Pour l'instant, on garde ce JSON seulement pour la dernière exécution.
            string dossierData = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");

            if (!Directory.Exists(dossierData))
            {
                Directory.CreateDirectory(dossierData);
            }

            _cheminFichier = Path.Combine(dossierData, "execution_derniere.json");
        }

        public List<BaseCopieModel> ChargerBasesSelectionnees()
        {
            // On récupère les bases sauvegardées dans la page Bases à copier.
            // Cela respecte le dernier état enregistré par l'utilisateur.
            return _basesCopierDataProvider
                .ChargerBasesSauvegardees()
                .Where(b => b.Inclure)
                .OrderBy(b => b.OrdreTraitement)
                .ToList();
        }

        public List<string> ChargerTablesBaseSource(string nomBase)
        {
            List<string> tables = new List<string>();

            if (string.IsNullOrWhiteSpace(nomBase))
            {
                return tables;
            }

            string chaineConnexion = CreerChaineConnexionBaseSource(nomBase);

            string requete = @"
                SELECT TABLE_SCHEMA + '.' + TABLE_NAME AS NomTable
                FROM INFORMATION_SCHEMA.TABLES
                WHERE TABLE_TYPE = 'BASE TABLE'
                ORDER BY TABLE_SCHEMA, TABLE_NAME;
            ";

            using SqlConnection connexion = new SqlConnection(chaineConnexion);
            connexion.Open();

            using SqlCommand commande = new SqlCommand(requete, connexion);
            using SqlDataReader lecteur = commande.ExecuteReader();

            while (lecteur.Read())
            {
                string nomTable = lecteur.GetString(0);
                tables.Add(nomTable);
            }

            return tables;
        }

        public int CompterLignesTableSource(string nomBase, string nomTable)
        {
            if (string.IsNullOrWhiteSpace(nomBase) ||
                string.IsNullOrWhiteSpace(nomTable))
            {
                return 0;
            }

            string chaineConnexion = CreerChaineConnexionBaseSource(nomBase);

            string nomTableSql = ConstruireNomTableSql(nomTable);

            string requete = $"SELECT COUNT(*) FROM {nomTableSql};";

            using SqlConnection connexion = new SqlConnection(chaineConnexion);
            connexion.Open();

            using SqlCommand commande = new SqlCommand(requete, connexion);

            object? resultat = commande.ExecuteScalar();

            if (resultat == null)
            {
                return 0;
            }

            return Convert.ToInt32(resultat);
        }

        public bool VerifierOuCreerBaseCible(string nomBase)
        {
            if (string.IsNullOrWhiteSpace(nomBase))
            {
                return false;
            }

            string chaineConnexion = CreerChaineConnexionBaseCible("master");

            string nomBaseSql = ConstruireNomBaseSql(nomBase);

            string requete = $@"
        IF DB_ID(@NomBase) IS NULL
        BEGIN
            CREATE DATABASE {nomBaseSql};
            SELECT 1;
        END
        ELSE
        BEGIN
            SELECT 0;
        END;
    ";

            using SqlConnection connexion = new SqlConnection(chaineConnexion);
            connexion.Open();

            using SqlCommand commande = new SqlCommand(requete, connexion);
            commande.Parameters.AddWithValue("@NomBase", nomBase);

            object? resultat = commande.ExecuteScalar();

            if (resultat == null)
            {
                return false;
            }

            // true = la base vient d'être créée
            // false = la base existait déjà
            return Convert.ToInt32(resultat) == 1;
        }

        private string CreerChaineConnexionBaseCible(string nomBase)
        {
            ConfigurationModel? configuration =
                _configurationDataProvider.ChargerConfiguration();

            if (configuration == null)
            {
                throw new InvalidOperationException(
                    "Aucune configuration n'est enregistrée. Veuillez d'abord enregistrer la page Configuration.");
            }

            string chaineConnexionCible =
                configuration.ServeurCible.ChaineConnexion;

            SqlConnectionStringBuilder builder;

            if (!string.IsNullOrWhiteSpace(chaineConnexionCible))
            {
                builder = new SqlConnectionStringBuilder(chaineConnexionCible);
            }
            else
            {
                builder = CreerBuilderDepuisServeur(configuration.ServeurCible);
            }

            // LocalDB ne doit pas avoir de port.
            if (builder.DataSource.Contains("(localdb)", StringComparison.OrdinalIgnoreCase))
            {
                builder.DataSource = builder.DataSource.Split(',')[0];
            }

            // Pour créer une base, on se connecte d'abord à master.
            builder.InitialCatalog = nomBase;
            builder.TrustServerCertificate = true;

            return builder.ConnectionString;
        }

        private string ConstruireNomBaseSql(string nomBase)
        {
            // On protège le nom de la base avec des crochets SQL Server.
            // Exemple : DB_TestRH devient [DB_TestRH]
            string nomBaseSecurise = nomBase.Replace("]", "]]");

            return $"[{nomBaseSecurise}]";
        }

        public ExecutionTableauBordModel ChargerDernierTableauBord()
        {
            ExecutionSauvegardeModel sauvegarde = ChargerSauvegarde();

            int nombreBasesSelectionnees = ChargerBasesSelectionnees().Count;

            if (sauvegarde.TableauBord == null)
            {
                return new ExecutionTableauBordModel
                {
                    NombreBasesSelectionnees = nombreBasesSelectionnees,
                    NombreLignesCopiees = 0,
                    DureeDerniereExecution = "-"
                };
            }

            sauvegarde.TableauBord.NombreBasesSelectionnees = nombreBasesSelectionnees;

            return sauvegarde.TableauBord;
        }

        public List<ExecutionResultatBaseModel> ChargerDerniersResultats()
        {
            ExecutionSauvegardeModel sauvegarde = ChargerSauvegarde();

            return sauvegarde.Resultats ?? new List<ExecutionResultatBaseModel>();
        }

        public void EnregistrerDerniereExecution(
            ExecutionTableauBordModel tableauBord,
            List<ExecutionResultatBaseModel> resultats)
        {
            ExecutionSauvegardeModel sauvegarde = new ExecutionSauvegardeModel
            {
                TableauBord = tableauBord,
                Resultats = resultats
            };

            string contenuJson = JsonConvert.SerializeObject(sauvegarde, Formatting.Indented);

            File.WriteAllText(_cheminFichier, contenuJson);
        }

        private string CreerChaineConnexionBaseSource(string nomBase)
        {
            ConfigurationModel? configuration =
                _configurationDataProvider.ChargerConfiguration();

            if (configuration == null)
            {
                throw new InvalidOperationException(
                    "Aucune configuration n'est enregistrée. Veuillez d'abord enregistrer la page Configuration.");
            }

            string chaineConnexionSource =
                configuration.ServeurSource.ChaineConnexion;

            SqlConnectionStringBuilder builder;

            if (!string.IsNullOrWhiteSpace(chaineConnexionSource))
            {
                builder = new SqlConnectionStringBuilder(chaineConnexionSource);
            }
            else
            {
                builder = CreerBuilderDepuisServeur(configuration.ServeurSource);
            }

            // On corrige le serveur si c'est LocalDB.
            // LocalDB ne doit pas avoir de port.
            if (builder.DataSource.Contains("(localdb)", StringComparison.OrdinalIgnoreCase))
            {
                builder.DataSource = builder.DataSource.Split(',')[0];
            }

            // On garde le serveur source sauvegardé,
            // mais on change seulement la base utilisée.
            builder.InitialCatalog = nomBase;
            builder.TrustServerCertificate = true;

            return builder.ConnectionString;
        }
        private SqlConnectionStringBuilder CreerBuilderDepuisServeur(ServeurConfigModel serveur)
        {
            if (string.IsNullOrWhiteSpace(serveur.NomServeur))
            {
                throw new InvalidOperationException(
                    "Le serveur source n'est pas renseigné dans la configuration.");
            }

            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();

            string dataSource = serveur.NomServeur.Trim();

            bool estLocalDb =
                dataSource.Contains("(localdb)", StringComparison.OrdinalIgnoreCase);

            bool estInstanceNommee =
                dataSource.Contains('\\');

            string port = Convert.ToString(serveur.Port) ?? "";

            // On ajoute le port seulement pour un vrai serveur réseau.
            // On ne l'ajoute pas pour LocalDB ou pour une instance nommée.
            if (!estLocalDb &&
                !estInstanceNommee &&
                !string.IsNullOrWhiteSpace(port) &&
                port != "0" &&
                !dataSource.Contains(','))
            {
                dataSource = $"{dataSource},{port}";
            }

            builder.DataSource = dataSource;
            builder.InitialCatalog = "master";
            builder.TrustServerCertificate = true;

            if (string.IsNullOrWhiteSpace(serveur.Identifiant))
            {
                // Authentification Windows.
                builder.IntegratedSecurity = true;
            }
            else
            {
                builder.UserID = serveur.Identifiant;
                builder.Password = serveur.MotDePasse;
            }

            return builder;
        }

        private string CreerChaineConnexionDepuisServeur(ServeurConfigModel serveur)
        {
            if (string.IsNullOrWhiteSpace(serveur.NomServeur))
            {
                throw new InvalidOperationException(
                    "Le serveur source n'est pas renseigné dans la configuration.");
            }

            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();

            string dataSource = serveur.NomServeur.Trim();
            string port = Convert.ToString(serveur.Port) ?? "";

            if (!string.IsNullOrWhiteSpace(port) &&
                port != "0" &&
                !dataSource.Contains(","))
            {
                dataSource = $"{dataSource},{port}";
            }

            builder.DataSource = dataSource;
            builder.InitialCatalog = "master";
            builder.TrustServerCertificate = true;

            if (string.IsNullOrWhiteSpace(serveur.Identifiant))
            {
                // Si aucun identifiant n'est donné, on utilise l'authentification Windows.
                builder.IntegratedSecurity = true;
            }
            else
            {
                builder.UserID = serveur.Identifiant;
                builder.Password = serveur.MotDePasse;
            }

            return builder.ConnectionString;
        }

        private string ConstruireNomTableSql(string nomTable)
        {
            string[] morceaux = nomTable.Split('.');

            if (morceaux.Length != 2)
            {
                throw new InvalidOperationException(
                    $"Le nom de table '{nomTable}' est invalide. Format attendu : schema.table.");
            }

            string schema = morceaux[0];
            string table = morceaux[1];

            return $"[{schema}].[{table}]";
        }

        private ExecutionSauvegardeModel ChargerSauvegarde()
        {
            if (!File.Exists(_cheminFichier))
            {
                return new ExecutionSauvegardeModel();
            }

            string contenuJson = File.ReadAllText(_cheminFichier);

            if (string.IsNullOrWhiteSpace(contenuJson))
            {
                return new ExecutionSauvegardeModel();
            }

            ExecutionSauvegardeModel? sauvegarde =
                JsonConvert.DeserializeObject<ExecutionSauvegardeModel>(contenuJson);

            return sauvegarde ?? new ExecutionSauvegardeModel();
        }

        // Modèle privé utilisé seulement pour sauvegarder le JSON.
        private class ExecutionSauvegardeModel
        {
            public ExecutionTableauBordModel? TableauBord { get; set; }

            public List<ExecutionResultatBaseModel> Resultats { get; set; } =
                new List<ExecutionResultatBaseModel>();
        }
    }
}