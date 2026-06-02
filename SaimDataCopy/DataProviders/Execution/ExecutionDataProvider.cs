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
            string nomBaseCible = ObtenirNomBaseCible(nomBase);

            string chaineConnexion = CreerChaineConnexionBaseCible("master");

            string nomBaseSql = ConstruireNomBaseSql(nomBaseCible);
            // Cette requête vérifie si la base cible existe déjà.
            // Si elle n'existe pas, SQL Server crée la base avec CREATE DATABASE.
            // SELECT 1 = la base vient d'être créée.
            // SELECT 0 = la base existait déjà.
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
            commande.Parameters.AddWithValue("@NomBase", nomBaseCible);

            object? resultat = commande.ExecuteScalar();

            if (resultat == null)
            {
                return false;
            }

            // true = la base vient d'être créée
            // false = la base existait déjà
            return Convert.ToInt32(resultat) == 1;
        }

        public bool VerifierOuCreerTableCible(string nomBase, string nomTable)
        {
            if (string.IsNullOrWhiteSpace(nomBase) ||
                string.IsNullOrWhiteSpace(nomTable))
            {
                return false;
            }


            string[] morceaux = nomTable.Split('.');

            if (morceaux.Length != 2)

            {
                throw new InvalidOperationException(
                    $"Le nom de table '{nomTable}' est invalide. Format attendu : schema.table.");
            }

            string schema = morceaux[0];
            string table = morceaux[1];

            string nomBaseCible = ObtenirNomBaseCible(nomBase);

            string chaineConnexionCible = CreerChaineConnexionBaseCible(nomBaseCible);
            // Cette requête vérifie si la table existe déjà dans la base cible.
            // INFORMATION_SCHEMA.TABLES contient la liste des tables de la base.
            // COUNT(*) retourne 0 si la table n’existe pas, ou 1 si elle existe.
            string requeteVerification = @"
                SELECT COUNT(*)
                FROM INFORMATION_SCHEMA.TABLES
                WHERE TABLE_SCHEMA  = @Schema
                AND TABLE_NAME = @Table
                AND TABLE_TYPE = 'BASE TABLE';
            ";
            using SqlConnection connexionCible = new SqlConnection(chaineConnexionCible);
            connexionCible.Open();

            using SqlCommand commandeVerification = new SqlCommand(requeteVerification, connexionCible);
            commandeVerification.Parameters.AddWithValue("@Schema", schema);
            commandeVerification.Parameters.AddWithValue("@Table", table);

            object? resultatVerification = commandeVerification.ExecuteScalar();

            bool tableExiste = resultatVerification != null && Convert.ToInt32(resultatVerification) > 0;

            if (tableExiste)
            {
                //false = la table existant deja

                return false ;
            }

            List<string> colonnesSql =
                ChargerDefinitionsColonnesSource(nomBase, schema, table);

            if (colonnesSql.Count == 0)
            {
                throw new InvalidOperationException(
                    $"Impossible de créer la table cible '{nomTable}' car aucune colonne source n'a été trouvée.");

            }

            string nomSchemaSql = ConstruireNomSql(schema);
            string nomTableSql = ConstruireNomSql(table);
            string nomTableCompletSql = $"{nomSchemaSql}.{nomTableSql}";

            string definitionColonnes = string.Join(
                "," + Environment.NewLine + "    ", colonnesSql);

            // Cette requête crée le schéma s'il n'existe pas encore.
            // Ensuite elle crée la table cible avec les colonnes récupérées depuis la table source.
            string requeteCreation = $@"
                IF SCHEMA_ID(@Schema) IS NULL
                BEGIN
                    EXEC('CREATE SCHEMA {nomSchemaSql}');
                END;

                CREATE TABLE {nomTableCompletSql}
                (

                    {
                        definitionColonnes
                    }
                );
            ";

            using SqlCommand commandeCreation = new SqlCommand(requeteCreation, connexionCible);
            commandeCreation.Parameters.AddWithValue("@Schema", schema );
            commandeCreation.ExecuteNonQuery();

            //true = la table vient d'etre creee

            return true ;

        }

        public int CopierLignesTableSourceVersCible(string nomBase, string nomTable, string modeCopie)
        {
            if (string.IsNullOrWhiteSpace(nomBase) || string.IsNullOrWhiteSpace(nomTable))
            {
                return 0;
            }

            string nomBaseCible = ObtenirNomBaseCible(nomBase);

            string chaineConnexionSource = CreerChaineConnexionBaseSource(nomBase);
            string chaineConnexionCible = CreerChaineConnexionBaseCible(nomBaseCible);

            // Sécurité importante :
            // Si la source et la cible sont exactement la même base,
            // on bloque la copie pour éviter de dupliquer ou supprimer les données source.
            if (SourceEtCibleIdentiques(chaineConnexionSource, chaineConnexionCible))
            {
                throw new InvalidOperationException(
                    "La source et la cible pointent vers la même base. Copie annulée pour éviter de modifier les données source.");
            }

            string nomTableSql = ConstruireNomTableSql(nomTable);

            // Cette requête lit toutes les lignes de la table source.
            // Exemple : SELECT * FROM [dbo].[EmployesTest]
            // Elle sert à récupérer les données qui seront envoyées vers la table cible.
            string requeteLectureSource = $"SELECT * FROM {nomTableSql};";

            using SqlConnection connexionSource = new SqlConnection(chaineConnexionSource);
            using SqlConnection connexionCible = new SqlConnection(chaineConnexionCible);

            connexionSource.Open();
            connexionCible.Open();

            switch (modeCopie)
            {
                case "Ecraser":
                case "Écraser":
                    // Cette requête vide la table cible avant la copie.
                    // DELETE est plus sûr que TRUNCATE car il fonctionne même si la table a certaines contraintes.
                    // Exemple : DELETE FROM [dbo].[EmployesTest]
                    string requeteSuppressionCible = $"DELETE FROM {nomTableSql};";

                    using (SqlCommand commandeSuppression = new SqlCommand(requeteSuppressionCible, connexionCible))
                    {
                        commandeSuppression.ExecuteNonQuery();
                    }

                    break;

                case "Mettre a jour":
                case "Mettre à jour":
                    throw new NotSupportedException(
                        "Le mode 'Mettre à jour' n'est pas encore implémenté. Il sera ajouté après la copie simple.");

                default:
                    throw new InvalidOperationException(
                        $"Mode de copie inconnu : {modeCopie}");
            }

            using SqlCommand commandeLecture = new SqlCommand(requeteLectureSource, connexionSource);
            using SqlDataReader lecteur = commandeLecture.ExecuteReader();

            if (!lecteur.HasRows)
            {
                return 0;
            }

            using SqlBulkCopy bulkCopy = new SqlBulkCopy(
                connexionCible,
                SqlBulkCopyOptions.KeepIdentity,
                null);

            bulkCopy.DestinationTableName = nomTableSql;

            for (int i = 0; i < lecteur.FieldCount; i++)
            {
                string nomColonne = lecteur.GetName(i);

                // On associe chaque colonne source avec la colonne cible du même nom.
                // Exemple : source Nom -> cible Nom
                bulkCopy.ColumnMappings.Add(nomColonne, nomColonne);
            }

            bulkCopy.WriteToServer(lecteur);

            return CompterLignesTableSource(nomBase, nomTable);
        }

        private bool SourceEtCibleIdentiques(string chaineConnexionSource, string chaineConnexionCible)
        {
            SqlConnectionStringBuilder source = new SqlConnectionStringBuilder(chaineConnexionSource);

            SqlConnectionStringBuilder cible = new SqlConnectionStringBuilder(chaineConnexionCible);

            bool memeServeur = string.Equals(
                    source.DataSource,
                    cible.DataSource,
                    StringComparison.OrdinalIgnoreCase);

            bool memeBase = string.Equals(
                    source.InitialCatalog,
                    cible.InitialCatalog,
                    StringComparison.OrdinalIgnoreCase);

            return memeServeur && memeBase;
        }

        private string ObtenirNomBaseCible (string nomBaseSource)
        {
            string chaineConnexionSource = CreerChaineConnexionBaseSource(nomBaseSource);
            string chaineConnexionCible = CreerChaineConnexionBaseCible(nomBaseSource);

            SqlConnectionStringBuilder source = new SqlConnectionStringBuilder (chaineConnexionSource);

            SqlConnectionStringBuilder cible = new SqlConnectionStringBuilder(chaineConnexionCible);

            bool memeServeur = string.Equals(source.DataSource, cible.DataSource, StringComparison.OrdinalIgnoreCase);

            if (memeServeur)
            {
                // Cas de test local :
                // Si la source et la cible sont sur le même serveur,
                // on utilise une base cible différente pour éviter de toucher à la source.
                return $"CIBLE_{nomBaseSource}";
            }

            // Cas réel :
            // Si les serveurs sont différents, on garde le même nom de base.
            // Exemple : ServeurSource.DB_TestRH -> ServeurCible.DB_TestRH

            return nomBaseSource;

        }
        private string CreerChaineConnexionBaseCible(string nomBase)
        {
            ConfigurationModel? configuration = _configurationDataProvider.ChargerConfiguration();

            if (configuration == null)
            {
                throw new InvalidOperationException(
                    "Aucune configuration n'est enregistrée. Veuillez d'abord enregistrer la page Configuration.");
            }

            string chaineConnexionCible = configuration.ServeurCible.ChaineConnexion;

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

        private List<string> ChargerDefinitionsColonnesSource(string nomBase, string schema, string table)
        {
            List<string> colonnesSql = new List<string>();

            string chaineConnexionSource = CreerChaineConnexionBaseSource(nomBase);

            string requete = @"
        SELECT 
            COLUMN_NAME,
            DATA_TYPE,
            CHARACTER_MAXIMUM_LENGTH,
            NUMERIC_PRECISION,
            NUMERIC_SCALE,
            DATETIME_PRECISION,
            IS_NULLABLE
        FROM INFORMATION_SCHEMA.COLUMNS
        WHERE TABLE_SCHEMA = @Schema
        AND TABLE_NAME = @Table
        ORDER BY ORDINAL_POSITION;
    ";

            using SqlConnection connexionSource = new SqlConnection(chaineConnexionSource);
            connexionSource.Open();

            using SqlCommand commande = new SqlCommand(requete, connexionSource);
            commande.Parameters.AddWithValue("@Schema", schema);
            commande.Parameters.AddWithValue("@Table", table);

            using SqlDataReader lecteur = commande.ExecuteReader();

            while (lecteur.Read())
            {
                string nomColonne = lecteur.GetString(0);
                string typeDonnee = lecteur.GetString(1);

                int? longueur = lecteur.IsDBNull(2)
                    ? null
                    : Convert.ToInt32(lecteur.GetValue(2));

                int? precision = lecteur.IsDBNull(3)
                    ? null
                    : Convert.ToInt32(lecteur.GetValue(3));

                int? echelle = lecteur.IsDBNull(4)
                    ? null
                    : Convert.ToInt32(lecteur.GetValue(4));

                int? precisionDate = lecteur.IsDBNull(5)
                    ? null
                    : Convert.ToInt32(lecteur.GetValue(5));

                string nullable = lecteur.GetString(6);

                string nomColonneSql = ConstruireNomSql(nomColonne);

                string typeSql = ConstruireTypeColonneSql(
                    typeDonnee,
                    longueur,
                    precision,
                    echelle,
                    precisionDate);

                string nullSql = nullable == "YES" ? "NULL" : "NOT NULL";

                colonnesSql.Add($"{nomColonneSql} {typeSql} {nullSql}");
            }

            return colonnesSql;
        }

        private string ConstruireTypeColonneSql(
            string typeDonnee,
            int? longueur,
            int? precision,
            int? echelle,
            int? precisionDate)
        {
            string typeMinuscule = typeDonnee.ToLower();

            switch (typeMinuscule)
            {
                case "varchar":
                case "nvarchar":
                case "char":
                case "nchar":
                case "binary":
                case "varbinary":
                    if (longueur == -1)
                    {
                        return $"{typeDonnee}(MAX)";
                    }

                    if (longueur.HasValue)
                    {
                        return $"{typeDonnee}({longueur.Value})";
                    }

                    return typeDonnee;

                case "decimal":
                case "numeric":
                    if (precision.HasValue && echelle.HasValue)
                    {
                        return $"{typeDonnee}({precision.Value}, {echelle.Value})";
                    }

                    return typeDonnee;

                case "datetime2":
                case "datetimeoffset":
                case "time":
                    if (precisionDate.HasValue)
                    {
                        return $"{typeDonnee}({precisionDate.Value})";
                    }

                    return typeDonnee;

                default:
                    return typeDonnee;
            }
        }

        private string ConstruireNomSql(string nom)
        {
            string nomSecurise = nom.Replace("]", "]]");

            return $"[{nomSecurise}]";
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