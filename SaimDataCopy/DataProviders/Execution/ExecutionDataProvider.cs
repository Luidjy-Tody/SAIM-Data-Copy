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

            // Cette requête récupère toutes les tables utilisateur de la base source.
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

            // Cette requête compte le nombre de lignes dans la table source.
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

        public int CompterLignesTableCible(string nomBase, string nomTable)
        {
            if (string.IsNullOrWhiteSpace(nomBase) || string.IsNullOrWhiteSpace(nomTable))
            {
                return 0;
            }

            string nomBaseCible = ObtenirNomBaseCible(nomBase);

            string chaineConnexion = CreerChaineConnexionBaseCible(nomBaseCible);

            string nomTableSql = ConstruireNomTableSql(nomTable);

            // Cette requête compte le nombre de lignes déjà présentes
            // dans la table cible.
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
                WHERE TABLE_SCHEMA = @Schema
                AND TABLE_NAME = @Table
                AND TABLE_TYPE = 'BASE TABLE';
            ";

            using SqlConnection connexionCible = new SqlConnection(chaineConnexionCible);
            connexionCible.Open();

            using SqlCommand commandeVerification = new SqlCommand(requeteVerification, connexionCible);
            commandeVerification.Parameters.AddWithValue("@Schema", schema);
            commandeVerification.Parameters.AddWithValue("@Table", table);

            object? resultatVerification = commandeVerification.ExecuteScalar();

            bool tableExiste =
                resultatVerification != null &&
                Convert.ToInt32(resultatVerification) > 0;

            if (tableExiste)
            {
                // false = la table existe déjà.
                return false;
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
                "," + Environment.NewLine + "    ",
                colonnesSql);

            // Cette requête crée le schéma s'il n'existe pas encore.
            // Ensuite elle crée la table cible avec les colonnes récupérées depuis la table source.
            string requeteCreation = $@"
                IF SCHEMA_ID(@Schema) IS NULL
                BEGIN
                    EXEC('CREATE SCHEMA {nomSchemaSql}');
                END;

                CREATE TABLE {nomTableCompletSql}
                (
                    {definitionColonnes}
                );
            ";

            using SqlCommand commandeCreation = new SqlCommand(requeteCreation, connexionCible);
            commandeCreation.Parameters.AddWithValue("@Schema", schema);
            commandeCreation.ExecuteNonQuery();

            // true = la table vient d'être créée.
            return true;
        }

        public int CopierLignesTableSourceVersCible(
            string nomBase,
            string nomTable,
            string modeCopie,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (string.IsNullOrWhiteSpace(nomBase) ||
                string.IsNullOrWhiteSpace(nomTable))
            {
                return 0;
            }

            string nomBaseCible = ObtenirNomBaseCible(nomBase);

            string chaineConnexionSource = CreerChaineConnexionBaseSource(nomBase);
            string chaineConnexionCible = CreerChaineConnexionBaseCible(nomBaseCible);

            // Sécurité importante :
            // Si la source et la cible sont exactement la même base,
            // on bloque la copie pour éviter de modifier les données source.
            if (SourceEtCibleIdentiques(chaineConnexionSource, chaineConnexionCible))
            {
                throw new InvalidOperationException(
                    "La source et la cible pointent vers la même base. Copie annulée pour éviter de modifier les données source.");
            }

            cancellationToken.ThrowIfCancellationRequested();

            switch (modeCopie)
            {
                case "Ecraser":
                case "Écraser":
                    return CopierModeEcraser(
                        nomBase,
                        nomTable,
                        chaineConnexionSource,
                        chaineConnexionCible,
                        cancellationToken);

                case "Mettre a jour":
                case "Mettre à jour":
                case "Mise à jour":
                    return CopierModeMiseAJour(
                        nomBase,
                        nomTable,
                        chaineConnexionSource,
                        chaineConnexionCible,
                        cancellationToken);

                default:
                    throw new InvalidOperationException(
                        $"Mode de copie inconnu : {modeCopie}");
            }
        }

        private int CopierModeEcraser(
            string nomBase,
            string nomTable,
            string chaineConnexionSource,
            string chaineConnexionCible,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            string nomTableSql = ConstruireNomTableSql(nomTable);

            // Cette requête lit toutes les lignes de la table source.
            // Exemple : SELECT * FROM [dbo].[EmployesTest]
            string requeteLectureSource = $"SELECT * FROM {nomTableSql};";

            using SqlConnection connexionSource = new SqlConnection(chaineConnexionSource);
            using SqlConnection connexionCible = new SqlConnection(chaineConnexionCible);

            connexionSource.OpenAsync(cancellationToken).GetAwaiter().GetResult();
            connexionCible.OpenAsync(cancellationToken).GetAwaiter().GetResult();

            cancellationToken.ThrowIfCancellationRequested();

            // Cette requête vide la table cible avant la copie.
            // DELETE est utilisé car il est plus simple et plus sûr que TRUNCATE.
            string requeteSuppressionCible = $"DELETE FROM {nomTableSql};";

            using (SqlCommand commandeSuppression = new SqlCommand(requeteSuppressionCible, connexionCible))
            {
                commandeSuppression
                    .ExecuteNonQueryAsync(cancellationToken)
                    .GetAwaiter()
                    .GetResult();
            }

            cancellationToken.ThrowIfCancellationRequested();

            using SqlCommand commandeLecture = new SqlCommand(requeteLectureSource, connexionSource);

            using SqlDataReader lecteur =
                commandeLecture.ExecuteReaderAsync(cancellationToken)
                    .GetAwaiter()
                    .GetResult();

            if (!lecteur.HasRows)
            {
                return 0;
            }

            cancellationToken.ThrowIfCancellationRequested();

            using SqlBulkCopy bulkCopy = new SqlBulkCopy(
                connexionCible,
                SqlBulkCopyOptions.KeepIdentity,
                null);

            bulkCopy.DestinationTableName = nomTableSql;

            for (int i = 0; i < lecteur.FieldCount; i++)
            {
                string nomColonne = lecteur.GetName(i);

                // On associe chaque colonne source avec la colonne cible du même nom.
                bulkCopy.ColumnMappings.Add(nomColonne, nomColonne);
            }

            bulkCopy.WriteToServerAsync(lecteur, cancellationToken)
                .GetAwaiter()
                .GetResult();

            cancellationToken.ThrowIfCancellationRequested();

            return CompterLignesTableSource(nomBase, nomTable);
        }

        private int CopierModeMiseAJour(
            string nomBase,
            string nomTable,
            string chaineConnexionSource,
            string chaineConnexionCible,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            string[] morceaux = nomTable.Split('.');

            if (morceaux.Length != 2)
            {
                throw new InvalidOperationException(
                    $"Le nom de table '{nomTable}' est invalide. Format attendu : schema.table.");
            }

            string schema = morceaux[0];
            string table = morceaux[1];

            List<string> colonnes = ChargerNomsColonnesSource(nomBase, schema, table);
            List<string> colonnesCles = ChargerColonnesClesSource(nomBase, schema, table);

            if (colonnes.Count == 0)
            {
                throw new InvalidOperationException(
                    $"Impossible de copier la table '{nomTable}' car aucune colonne source n'a été trouvée.");
            }

            if (colonnesCles.Count == 0)
            {
                throw new InvalidOperationException(
                    $"Impossible d'utiliser le mode Mise à jour pour '{nomTable}' : aucune clé primaire ou colonne unique n'a été trouvée.");
            }

            string nomTableSql = ConstruireNomTableSql(nomTable);
            string nomTableTemporaire = CreerNomTableTemporaire(table);

            // Cette requête lit toutes les lignes de la table source.
            // Ces lignes seront d'abord copiées dans une table temporaire côté cible.
            string requeteLectureSource = $"SELECT * FROM {nomTableSql};";

            using SqlConnection connexionSource = new SqlConnection(chaineConnexionSource);
            using SqlConnection connexionCible = new SqlConnection(chaineConnexionCible);

            connexionSource.OpenAsync(cancellationToken).GetAwaiter().GetResult();
            connexionCible.OpenAsync(cancellationToken).GetAwaiter().GetResult();

            cancellationToken.ThrowIfCancellationRequested();

            CreerTableTemporaireCible(
                connexionCible,
                nomTableSql,
                nomTableTemporaire);

            cancellationToken.ThrowIfCancellationRequested();

            using SqlCommand commandeLecture = new SqlCommand(requeteLectureSource, connexionSource);

            using SqlDataReader lecteur =
                commandeLecture.ExecuteReaderAsync(cancellationToken)
                    .GetAwaiter()
                    .GetResult();

            if (!lecteur.HasRows)
            {
                return 0;
            }

            cancellationToken.ThrowIfCancellationRequested();

            using SqlBulkCopy bulkCopy = new SqlBulkCopy(
                connexionCible,
                SqlBulkCopyOptions.KeepIdentity,
                null);

            bulkCopy.DestinationTableName = nomTableTemporaire;

            for (int i = 0; i < lecteur.FieldCount; i++)
            {
                string nomColonne = lecteur.GetName(i);

                // On associe chaque colonne source avec la colonne temporaire du même nom.
                bulkCopy.ColumnMappings.Add(nomColonne, nomColonne);
            }

            bulkCopy.WriteToServerAsync(lecteur, cancellationToken)
                .GetAwaiter()
                .GetResult();

            cancellationToken.ThrowIfCancellationRequested();

            string requeteMerge = ConstruireRequeteMerge(
                nomTableSql,
                nomTableTemporaire,
                colonnes,
                colonnesCles);

            using SqlCommand commandeMerge = new SqlCommand(requeteMerge, connexionCible);

            commandeMerge.ExecuteNonQueryAsync(cancellationToken)
                .GetAwaiter()
                .GetResult();

            cancellationToken.ThrowIfCancellationRequested();

            return CompterLignesTableSource(nomBase, nomTable);
        }

        private List<string> ChargerNomsColonnesSource(string nomBase, string schema, string table)
        {
            List<string> colonnes = new List<string>();

            string chaineConnexionSource = CreerChaineConnexionBaseSource(nomBase);

            // Cette requête récupère les colonnes de la table source.
            // L'ordre est important pour garder la même structure que la table source.
            string requete = @"
                SELECT COLUMN_NAME
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
                colonnes.Add(lecteur.GetString(0));
            }

            return colonnes;
        }

        private List<string> ChargerColonnesClesSource(string nomBase, string schema, string table)
        {
            List<string> colonnesCles = new List<string>();

            string chaineConnexionSource = CreerChaineConnexionBaseSource(nomBase);

            // Cette requête cherche la clé de comparaison de la table source.
            // Elle prend d'abord la clé primaire.
            // S'il n'y a pas de clé primaire, elle peut prendre un index unique.
            string requete = @"
                SELECT c.name
                FROM sys.indexes i
                INNER JOIN sys.index_columns ic 
                    ON i.object_id = ic.object_id 
                    AND i.index_id = ic.index_id
                INNER JOIN sys.columns c 
                    ON ic.object_id = c.object_id 
                    AND ic.column_id = c.column_id
                INNER JOIN sys.tables t 
                    ON i.object_id = t.object_id
                INNER JOIN sys.schemas s 
                    ON t.schema_id = s.schema_id
                WHERE s.name = @Schema
                AND t.name = @Table
                AND i.is_unique = 1
                AND i.is_hypothetical = 0
                AND i.has_filter = 0
                AND ic.is_included_column = 0
                AND i.index_id = (
                    SELECT TOP 1 i2.index_id
                    FROM sys.indexes i2
                    INNER JOIN sys.tables t2 
                        ON i2.object_id = t2.object_id
                    INNER JOIN sys.schemas s2 
                        ON t2.schema_id = s2.schema_id
                    WHERE s2.name = @Schema
                    AND t2.name = @Table
                    AND i2.is_unique = 1
                    AND i2.is_hypothetical = 0
                    AND i2.has_filter = 0
                    ORDER BY 
                        CASE WHEN i2.is_primary_key = 1 THEN 0 ELSE 1 END,
                        i2.index_id
                )
                ORDER BY ic.key_ordinal;
            ";

            using SqlConnection connexionSource = new SqlConnection(chaineConnexionSource);
            connexionSource.Open();

            using SqlCommand commande = new SqlCommand(requete, connexionSource);
            commande.Parameters.AddWithValue("@Schema", schema);
            commande.Parameters.AddWithValue("@Table", table);

            using SqlDataReader lecteur = commande.ExecuteReader();

            while (lecteur.Read())
            {
                colonnesCles.Add(lecteur.GetString(0));
            }

            return colonnesCles;
        }

        private void CreerTableTemporaireCible(
            SqlConnection connexionCible,
            string nomTableSql,
            string nomTableTemporaire)
        {
            // Cette requête crée une table temporaire avec la même structure que la table cible.
            // TOP 0 veut dire qu'on copie seulement la structure, pas les données.
            string requeteCreationTemporaire = $@"
                SELECT TOP 0 *
                INTO {nomTableTemporaire}
                FROM {nomTableSql};
            ";

            using SqlCommand commande = new SqlCommand(requeteCreationTemporaire, connexionCible);
            commande.ExecuteNonQuery();
        }

        private string ConstruireRequeteMerge(
            string nomTableSql,
            string nomTableTemporaire,
            List<string> colonnes,
            List<string> colonnesCles)
        {
            List<string> colonnesNonCles = colonnes
                .Where(c => !colonnesCles.Any(cle =>
                    cle.Equals(c, StringComparison.OrdinalIgnoreCase)))
                .ToList();

            string conditionJointure = string.Join(
                " AND ",
                colonnesCles.Select(c =>
                    $"cible.{ConstruireNomSql(c)} = source.{ConstruireNomSql(c)}"));

            string colonnesInsert = string.Join(
                ", ",
                colonnes.Select(ConstruireNomSql));

            string valeursInsert = string.Join(
                ", ",
                colonnes.Select(c => $"source.{ConstruireNomSql(c)}"));

            string clauseUpdate = string.Empty;

            if (colonnesNonCles.Count > 0)
            {
                string valeursUpdate = string.Join(
                    "," + Environment.NewLine + "        ",
                    colonnesNonCles.Select(c =>
                        $"cible.{ConstruireNomSql(c)} = source.{ConstruireNomSql(c)}"));

                clauseUpdate = $@"
                    WHEN MATCHED THEN
                        UPDATE SET
                        {valeursUpdate}";
            }

            // Cette requête fait l'équivalent SQL Server de :
            // INSERT ... ON DUPLICATE KEY UPDATE en MySQL.
            // Si la clé existe déjà, elle met à jour.
            // Si la clé n'existe pas, elle insère une nouvelle ligne.
            string requeteMerge = $@"
                MERGE INTO {nomTableSql} AS cible
                USING {nomTableTemporaire} AS source
                ON {conditionJointure}
                {clauseUpdate}
                WHEN NOT MATCHED BY TARGET THEN
                    INSERT ({colonnesInsert})
                    VALUES ({valeursInsert});
            ";

            return requeteMerge;
        }

        private string CreerNomTableTemporaire(string nomTable)
        {
            string nomNettoye = new string(
                nomTable
                    .Where(c => char.IsLetterOrDigit(c) || c == '_')
                    .ToArray());

            if (string.IsNullOrWhiteSpace(nomNettoye))
            {
                nomNettoye = "Table";
            }

            return "#Temp_" + nomNettoye + "_" + Guid.NewGuid().ToString("N");
        }

        private bool SourceEtCibleIdentiques(string chaineConnexionSource, string chaineConnexionCible)
        {
            SqlConnectionStringBuilder source =
                new SqlConnectionStringBuilder(chaineConnexionSource);

            SqlConnectionStringBuilder cible =
                new SqlConnectionStringBuilder(chaineConnexionCible);

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

        private string ObtenirNomBaseCible(string nomBaseSource)
        {
            // On garde le même nom de base côté cible.
            // Exemple :
            // Source : (localdb)\MSSQLLocalDB / DB_TestRH
            // Cible  : localhost,1433 / DB_TestRH
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

            // Cette requête récupère les définitions des colonnes de la table source.
            // Elle sert à recréer la même structure dans la table cible.
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

        public void TesterConnexionSource()
        {
            ConfigurationModel? configuration =
                _configurationDataProvider.ChargerConfiguration();

            if (configuration == null)
            {
                throw new InvalidOperationException(
                    "Aucune configuration n'est enregistrée. Veuillez d'abord enregistrer la page Configuration.");
            }

            string chaineConnexion =
                CreerChaineConnexionBaseSource("master");

            using SqlConnection connexion = new SqlConnection(chaineConnexion);

            connexion.Open();
        }

        public void TesterConnexionCible()
        {
            ConfigurationModel? configuration =
                _configurationDataProvider.ChargerConfiguration();

            if (configuration == null)
            {
                throw new InvalidOperationException(
                    "Aucune configuration n'est enregistrée. Veuillez d'abord enregistrer la page Configuration.");
            }

            string chaineConnexion =
                CreerChaineConnexionBaseCible("master");

            using SqlConnection connexion = new SqlConnection(chaineConnexion);

            connexion.Open();
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
                    "Le serveur SQL n'est pas renseigné dans la configuration.");
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
                    "Le serveur SQL n'est pas renseigné dans la configuration.");
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

            return $"{ConstruireNomSql(schema)}.{ConstruireNomSql(table)}";
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