using MySqlConnector;
using SaimDataCopy.DataProviders.BasesCopier;
using SaimDataCopy.DataProviders.Configuration;
using SaimDataCopy.Helpers;
using SaimDataCopy.Models.BasesCopier;
using SaimDataCopy.Models.Configuration;
using SaimDataCopy.Models.Execution;

namespace SaimDataCopy.DataProviders.Execution
{
    // Version MySQL du DataProvider Exécution.
    public class MySqlExecutionDataProvider : IExecutionDataProvider
    {
        private readonly IBasesCopierDataProvider _basesCopierDataProvider;
        private readonly IConfigurationDataProvider _configurationDataProvider;

        public MySqlExecutionDataProvider(
            IBasesCopierDataProvider basesCopierDataProvider,
            IConfigurationDataProvider configurationDataProvider)
        {
            _basesCopierDataProvider = basesCopierDataProvider;
            _configurationDataProvider = configurationDataProvider;
        }

        public List<BaseCopieModel> ChargerBasesSelectionnees()
        {
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

            using MySqlConnection connexion = new MySqlConnection(chaineConnexion);
            OuvrirConnexionMySql(connexion);

            using MySqlCommand commande = connexion.CreateCommand();

            // Cette requête récupère toutes les tables utilisateur
            // dans la base MySQL source.
            commande.CommandText =
                """
                SELECT TABLE_NAME
                FROM INFORMATION_SCHEMA.TABLES
                WHERE TABLE_SCHEMA = @NomBase
                AND TABLE_TYPE = 'BASE TABLE'
                ORDER BY TABLE_NAME;
                """;

            commande.Parameters.AddWithValue("@NomBase", nomBase);

            using MySqlDataReader lecteur = commande.ExecuteReader();

            while (lecteur.Read())
            {
                tables.Add(lecteur.GetString("TABLE_NAME"));
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
            string nomTableSql = ProtegerNomMySql(nomTable);

            using MySqlConnection connexion = new MySqlConnection(chaineConnexion);
            OuvrirConnexionMySql(connexion);

            using MySqlCommand commande = connexion.CreateCommand();

            // Cette requête compte le nombre de lignes de la table source.
            commande.CommandText = $"SELECT COUNT(*) FROM {nomTableSql};";

            object? resultat = commande.ExecuteScalar();

            return resultat == null ? 0 : Convert.ToInt32(resultat);
        }

        public int CompterLignesTableCible(string nomBase, string nomTable)
        {
            if (string.IsNullOrWhiteSpace(nomBase) ||
                string.IsNullOrWhiteSpace(nomTable))
            {
                return 0;
            }

            string nomBaseCible = ObtenirNomBaseCible(nomBase);
            string chaineConnexion = CreerChaineConnexionBaseCible(nomBaseCible);
            string nomTableSql = ProtegerNomMySql(nomTable);

            using MySqlConnection connexion = new MySqlConnection(chaineConnexion);
            OuvrirConnexionMySql(connexion);

            using MySqlCommand commande = connexion.CreateCommand();

            // Cette requête compte le nombre de lignes déjà présentes
            // dans la table cible MySQL.
            commande.CommandText = $"SELECT COUNT(*) FROM {nomTableSql};";

            object? resultat = commande.ExecuteScalar();

            return resultat == null ? 0 : Convert.ToInt32(resultat);
        }

        public bool VerifierOuCreerBaseCible(string nomBase)
        {
            if (string.IsNullOrWhiteSpace(nomBase))
            {
                return false;
            }

            string nomBaseCible = ObtenirNomBaseCible(nomBase);

            ConfigurationModel? configuration = _configurationDataProvider.ChargerConfiguration();

            if (configuration == null)
            {
                throw new InvalidOperationException(
                    "Aucune configuration n'est enregistrée. Veuillez d'abord enregistrer la page Configuration.");
            }

            string chaineConnexionCible =
                ChaineConnexionHelper.ConstruireChaineConnexionCible(configuration);

            using MySqlConnection connexion = new MySqlConnection(chaineConnexionCible);
            OuvrirConnexionMySql(connexion);

            using MySqlCommand commandeVerification = connexion.CreateCommand();

            // Cette requête vérifie si la base cible existe déjà dans MySQL.
            commandeVerification.CommandText =
                """
                SELECT COUNT(*)
                FROM INFORMATION_SCHEMA.SCHEMATA
                WHERE SCHEMA_NAME = @NomBase;
                """;

            commandeVerification.Parameters.AddWithValue("@NomBase", nomBaseCible);

            object? resultat = commandeVerification.ExecuteScalar();

            bool baseExiste =
                resultat != null &&
                Convert.ToInt32(resultat) > 0;

            if (baseExiste)
            {
                return false;
            }

            using MySqlCommand commandeCreation = connexion.CreateCommand();

            string nomBaseCibleSql = ProtegerNomMySql(nomBaseCible);

            // Cette requête crée la base cible si elle n'existe pas encore.
            commandeCreation.CommandText = $"CREATE DATABASE {nomBaseCibleSql};";

            commandeCreation.ExecuteNonQuery();

            return true;
        }

        public bool VerifierOuCreerTableCible(string nomBase, string nomTable)
        {
            if (string.IsNullOrWhiteSpace(nomBase) ||
                string.IsNullOrWhiteSpace(nomTable))
            {
                return false;
            }

            string nomBaseCible = ObtenirNomBaseCible(nomBase);

            string chaineConnexionSource = CreerChaineConnexionBaseSource(nomBase);
            string chaineConnexionCible = CreerChaineConnexionBaseCible(nomBaseCible);

            using MySqlConnection connexionSource = new MySqlConnection(chaineConnexionSource);
            using MySqlConnection connexionCible = new MySqlConnection(chaineConnexionCible);

            OuvrirConnexionMySql(connexionSource);
            OuvrirConnexionMySql(connexionCible);

            using MySqlCommand commandeVerification = connexionCible.CreateCommand();

            // Cette requête vérifie si la table cible existe déjà.
            commandeVerification.CommandText =
                """
        SELECT COUNT(*)
        FROM INFORMATION_SCHEMA.TABLES
        WHERE TABLE_SCHEMA = @NomBase
        AND TABLE_NAME = @NomTable
        AND TABLE_TYPE = 'BASE TABLE';
        """;

            commandeVerification.Parameters.AddWithValue("@NomBase", nomBaseCible);
            commandeVerification.Parameters.AddWithValue("@NomTable", nomTable);

            object? resultat = commandeVerification.ExecuteScalar();

            bool tableExiste =
                resultat != null &&
                Convert.ToInt32(resultat) > 0;

            if (tableExiste)
            {
                return false;
            }

            string nomTableSql = ProtegerNomMySql(nomTable);

            using MySqlCommand commandeStructure = connexionSource.CreateCommand();

            // Cette requête récupère le script CREATE TABLE de la table source.
            commandeStructure.CommandText = $"SHOW CREATE TABLE {nomTableSql};";

            using MySqlDataReader lecteur = commandeStructure.ExecuteReader();

            if (!lecteur.Read())
            {
                throw new InvalidOperationException(
                    $"Impossible de récupérer la structure de la table source {nomTable}.");
            }

            string scriptCreationTable = lecteur.GetString(1);

            // Certains anciens serveurs MySQL ne connaissent pas utf8mb4_0900_ai_ci.
            // On remplace par une collation plus compatible.
            scriptCreationTable = scriptCreationTable.Replace(
                "utf8mb4_0900_ai_ci",
                "utf8mb4_general_ci");

            lecteur.Close();

            using MySqlCommand commandeCreation = connexionCible.CreateCommand();

            // Cette requête recrée la même table sur le serveur cible.
            commandeCreation.CommandText = scriptCreationTable;

            commandeCreation.ExecuteNonQuery();

            return true;
        }

        public int CopierLignesTableSourceVersCible(
            string nomBase,
            string nomTable,
            string modeCopie,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return modeCopie switch
            {
                "Écraser" => CopierModeEcraser(nomBase, nomTable, cancellationToken),
                "Ecraser" => CopierModeEcraser(nomBase, nomTable, cancellationToken),
                "Mise à jour" => CopierModeMiseAJour(nomBase, nomTable, cancellationToken),
                "Mettre à jour" => CopierModeMiseAJour(nomBase, nomTable, cancellationToken),

                _ => throw new InvalidOperationException(
                    $"Mode de copie inconnu : {modeCopie}")
            };
        }

        public ExecutionTableauBordModel ChargerDernierTableauBord()
        {
            return new ExecutionTableauBordModel
            {
                NombreBasesSelectionnees = ChargerBasesSelectionnees().Count,
                NombreLignesCopiees = 0,
                DureeDerniereExecution = "-"
            };
        }

        public List<ExecutionResultatBaseModel> ChargerDerniersResultats()
        {
            return new List<ExecutionResultatBaseModel>();
        }

        public void EnregistrerDerniereExecution(
            ExecutionTableauBordModel tableauBord,
            List<ExecutionResultatBaseModel> resultats)
        {
            // Temporaire.
            // On ajoutera la sauvegarde JSON MySQL plus tard.
        }

        private string CreerChaineConnexionBaseSource(string nomBase)
        {
            ConfigurationModel? configuration = _configurationDataProvider.ChargerConfiguration();

            if (configuration == null)
            {
                throw new InvalidOperationException(
                    "Aucune configuration n'est enregistrée. Veuillez d'abord enregistrer la page Configuration.");
            }

            string chaineConnexion =
                ChaineConnexionHelper.ConstruireChaineConnexionSource(configuration);

            MySqlConnectionStringBuilder builder =
                new MySqlConnectionStringBuilder(chaineConnexion);

            builder.Database = nomBase;

            return builder.ConnectionString;
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

            string chaineConnexion =
                ChaineConnexionHelper.ConstruireChaineConnexionCible(configuration);

            MySqlConnectionStringBuilder builder =
                new MySqlConnectionStringBuilder(chaineConnexion);

            builder.Database = nomBase;

            return builder.ConnectionString;
        }

        private int CopierModeMiseAJour(
            string nomBase,
            string nomTable,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            List<string> colonnes = ChargerNomsColonnesSource(nomBase, nomTable);
            List<string> colonnesCles = ChargerColonnesClesPrimairesSource(nomBase, nomTable);

            if (colonnes.Count == 0)
            {
                throw new InvalidOperationException(
                    $"Impossible de copier la table {nomTable} : aucune colonne trouvée.");
            }

            if (colonnesCles.Count == 0)
            {
                throw new InvalidOperationException(
                    $"Impossible d'utiliser le mode Mise à jour pour {nomTable} : aucune clé primaire trouvée.");
            }

            // Version simple : on relit les lignes source et on les insère avec ON DUPLICATE KEY UPDATE.
            return CopierLignesAvecOnDuplicateKeyUpdate(
                nomBase,
                nomTable,
                colonnes,
                colonnesCles,
                cancellationToken);
        }

        private int CopierModeEcraser(
            string nomBase,
            string nomTable,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            string nomBaseCible = ObtenirNomBaseCible(nomBase);

            if (SourceEtCibleIdentiques(nomBase, nomBaseCible))
            {
                throw new InvalidOperationException(
                    "La source et la cible pointent vers la même base MySQL. Copie annulée pour éviter de vider les données source.");
            }

            string chaineConnexionSource = CreerChaineConnexionBaseSource(nomBase);
            string chaineConnexionCible = CreerChaineConnexionBaseCible(nomBaseCible);

            string nomTableSql = ProtegerNomMySql(nomTable);

            using MySqlConnection connexionSource = new MySqlConnection(chaineConnexionSource);
            using MySqlConnection connexionCible = new MySqlConnection(chaineConnexionCible);

            OuvrirConnexionMySql(connexionSource);
            OuvrirConnexionMySql(connexionCible);

            cancellationToken.ThrowIfCancellationRequested();

            using MySqlCommand commandeSuppression = connexionCible.CreateCommand();

            // Cette requête vide la table cible avant la copie.
            commandeSuppression.CommandText = $"DELETE FROM {nomTableSql};";
            commandeSuppression.ExecuteNonQuery();

            cancellationToken.ThrowIfCancellationRequested();

            using MySqlCommand commandeLecture = connexionSource.CreateCommand();

            // Cette requête lit toutes les lignes de la table source.
            commandeLecture.CommandText = $"SELECT * FROM {nomTableSql};";

            using MySqlDataReader lecteur = commandeLecture.ExecuteReader();

            int lignesCopiees = 0;

            while (lecteur.Read())
            {
                cancellationToken.ThrowIfCancellationRequested();

                InsererLigneDansTableCible(
                    connexionCible,
                    nomTable,
                    lecteur);

                lignesCopiees++;
            }

            return lignesCopiees;
        }

        private int CopierLignesAvecOnDuplicateKeyUpdate(
            string nomBase,
            string nomTable,
            List<string> colonnes,
            List<string> colonnesCles,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            string nomBaseCible = ObtenirNomBaseCible(nomBase);

            if (SourceEtCibleIdentiques(nomBase, nomBaseCible))
            {
                throw new InvalidOperationException(
                    "La source et la cible pointent vers la même base MySQL. Copie annulée pour éviter de modifier les données source.");
            }

            string chaineConnexionSource = CreerChaineConnexionBaseSource(nomBase);
            string chaineConnexionCible = CreerChaineConnexionBaseCible(nomBaseCible);
            string nomTableSql = ProtegerNomMySql(nomTable);

            using MySqlConnection connexionSource = new MySqlConnection(chaineConnexionSource);
            using MySqlConnection connexionCible = new MySqlConnection(chaineConnexionCible);

            OuvrirConnexionMySql(connexionSource);
            OuvrirConnexionMySql(connexionCible);

            using MySqlCommand commandeLecture = connexionSource.CreateCommand();

            // Cette requête lit toutes les lignes de la table source.
            commandeLecture.CommandText = $"SELECT * FROM {nomTableSql};";

            using MySqlDataReader lecteur = commandeLecture.ExecuteReader();

            int lignesTraitees = 0;

            while (lecteur.Read())
            {
                cancellationToken.ThrowIfCancellationRequested();

                InsererOuMettreAJourLigneCible(
                    connexionCible,
                    nomTable,
                    colonnes,
                    colonnesCles,
                    lecteur);

                lignesTraitees++;
            }

            return lignesTraitees;
        }

        private void InsererOuMettreAJourLigneCible(
            MySqlConnection connexionCible,
            string nomTable,
            List<string> colonnes,
            List<string> colonnesCles,
            MySqlDataReader lecteur)
        {
            string nomTableSql = ProtegerNomMySql(nomTable);

            List<string> colonnesSql = new List<string>();
            List<string> parametresSql = new List<string>();
            List<string> misesAJourSql = new List<string>();

            using MySqlCommand commande = connexionCible.CreateCommand();

            for (int i = 0; i < colonnes.Count; i++)
            {
                string colonne = colonnes[i];
                string colonneSql = ProtegerNomMySql(colonne);
                string parametre = "@p" + i;

                colonnesSql.Add(colonneSql);
                parametresSql.Add(parametre);

                bool estCle = colonnesCles.Any(c =>
                    c.Equals(colonne, StringComparison.OrdinalIgnoreCase));

                if (!estCle)
                {
                    misesAJourSql.Add(
                        $"{colonneSql} = VALUES({colonneSql})");
                }

                object valeur = lecteur[colonne] == DBNull.Value
                    ? DBNull.Value
                    : lecteur[colonne];

                commande.Parameters.AddWithValue(parametre, valeur);
            }

            string colonnesInsert = string.Join(", ", colonnesSql);
            string valeursInsert = string.Join(", ", parametresSql);
            string clauseUpdate = string.Join(", ", misesAJourSql);

            // Cette requête insère une ligne si elle n'existe pas.
            // Si la clé primaire existe déjà, elle met à jour les autres colonnes.
            commande.CommandText =
                $"INSERT INTO {nomTableSql} ({colonnesInsert}) " +
                $"VALUES ({valeursInsert}) " +
                $"ON DUPLICATE KEY UPDATE {clauseUpdate};";

            commande.ExecuteNonQuery();
        }

        private void InsererLigneDansTableCible(
            MySqlConnection connexionCible,
            string nomTable,
            MySqlDataReader lecteur)
        {
            List<string> colonnes = new List<string>();
            List<string> parametres = new List<string>();

            using MySqlCommand commandeInsertion = connexionCible.CreateCommand();

            for (int i = 0; i < lecteur.FieldCount; i++)
            {
                string nomColonne = lecteur.GetName(i);
                string nomParametre = "@p" + i;

                colonnes.Add(ProtegerNomMySql(nomColonne));
                parametres.Add(nomParametre);

                object valeur = lecteur.IsDBNull(i)
                    ? DBNull.Value
                    : lecteur.GetValue(i);

                commandeInsertion.Parameters.AddWithValue(nomParametre, valeur);
            }

            string nomTableSql = ProtegerNomMySql(nomTable);
            string colonnesSql = string.Join(", ", colonnes);
            string parametresSql = string.Join(", ", parametres);

            // Cette requête insère une ligne dans la table cible.
            commandeInsertion.CommandText =
                $"INSERT INTO {nomTableSql} ({colonnesSql}) VALUES ({parametresSql});";

            commandeInsertion.ExecuteNonQuery();
        }

        private string ObtenirNomBaseCible(string nomBaseSource)
        {
            // Pour les tests MySQL sur le même serveur :
            // Source : mysql_source_test
            // Cible  : mysql_cible_test
            if (nomBaseSource.Equals("mysql_source_test", StringComparison.OrdinalIgnoreCase))
            {
                return "mysql_cible_test";
            }

            return nomBaseSource + "_cible";
        }

        private void OuvrirConnexionMySql(MySqlConnection connexion)
        {
            try
            {
                connexion.Open();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    TraduireErreurMySql(ex),
                    ex);
            }
        }

        private string TraduireErreurMySql(Exception exception)
        {
            if (exception is MySqlException erreurMySql)
            {
                return erreurMySql.Number switch
                {
                    1045 => "Nom d'utilisateur ou mot de passe MySQL incorrect.",
                    1042 => "Serveur MySQL introuvable ou port incorrect.",
                    1049 => "La base de données MySQL demandée n'existe pas.",
                    2003 => "Impossible de se connecter au serveur MySQL. Vérifiez le nom du serveur et le port.",
                    2005 => "Nom du serveur MySQL invalide.",
                    1146 => "Table MySQL introuvable.",
                    1064 => "Erreur de syntaxe SQL MySQL.",
                    _ => $"Erreur MySQL ({erreurMySql.Number}) : {erreurMySql.Message}"
                };
            }

            string message = exception.Message;

            if (message.Contains("Unable to connect to any of the specified MySQL hosts", StringComparison.OrdinalIgnoreCase))
            {
                return "Impossible de se connecter au serveur MySQL. Vérifiez le nom du serveur, le port et que MySQL est démarré.";
            }

            if (message.Contains("Access denied", StringComparison.OrdinalIgnoreCase))
            {
                return "Nom d'utilisateur ou mot de passe MySQL incorrect.";
            }

            if (message.Contains("Unknown database", StringComparison.OrdinalIgnoreCase))
            {
                return "La base de données MySQL demandée n'existe pas.";
            }

            return message;
        }

        private string ProtegerNomMySql(string nom)
        {
            string nomSecurise = nom.Replace("`", "``");

            return $"`{nomSecurise}`";
        }

        private bool SourceEtCibleIdentiques(string nomBaseSource, string nomBaseCible)
        {
            ConfigurationModel? configuration =
                _configurationDataProvider.ChargerConfiguration();

            if (configuration == null)
            {
                return false;
            }

            bool memeServeur =
                configuration.ServeurSource.NomServeur.Equals(
                    configuration.ServeurCible.NomServeur,
                    StringComparison.OrdinalIgnoreCase);

            bool memePort =
                configuration.ServeurSource.Port == configuration.ServeurCible.Port;

            bool memeBase =
                nomBaseSource.Equals(
                    nomBaseCible,
                    StringComparison.OrdinalIgnoreCase);

            return memeServeur && memePort && memeBase;
        }

        private List<string> ChargerNomsColonnesSource(string nomBase, string nomTable)
        {
            List<string> colonnes = new List<string>();

            string chaineConnexionSource = CreerChaineConnexionBaseSource(nomBase);

            using MySqlConnection connexionSource = new MySqlConnection(chaineConnexionSource);
            OuvrirConnexionMySql(connexionSource);

            using MySqlCommand commande = connexionSource.CreateCommand();

            // Cette requête récupère les noms des colonnes de la table source.
            // ORDINAL_POSITION garde l'ordre réel des colonnes.
            commande.CommandText =
                """
                SELECT COLUMN_NAME
                FROM INFORMATION_SCHEMA.COLUMNS
                WHERE TABLE_SCHEMA = @NomBase
                AND TABLE_NAME = @NomTable
                ORDER BY ORDINAL_POSITION;
                """;

            commande.Parameters.AddWithValue("@NomBase", nomBase);
            commande.Parameters.AddWithValue("@NomTable", nomTable);

            using MySqlDataReader lecteur = commande.ExecuteReader();

            while (lecteur.Read())
            {
                colonnes.Add(lecteur.GetString("COLUMN_NAME"));
            }

            return colonnes;
        }

        private List<string> ChargerColonnesClesPrimairesSource(string nomBase, string nomTable)
        {
            List<string> colonnesCles = new List<string>();

            string chaineConnexionSource = CreerChaineConnexionBaseSource(nomBase);

            using MySqlConnection connexionSource = new MySqlConnection(chaineConnexionSource);
            OuvrirConnexionMySql(connexionSource);

            using MySqlCommand commande = connexionSource.CreateCommand();

            // Cette requête récupère la clé primaire de la table source.
            // Le mode Mise à jour MySQL a besoin d'une clé primaire ou unique.
            commande.CommandText =
                """
                SELECT COLUMN_NAME
                FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE
                WHERE TABLE_SCHEMA = @NomBase
                AND TABLE_NAME = @NomTable
                AND CONSTRAINT_NAME = 'PRIMARY'
                ORDER BY ORDINAL_POSITION;
                """;

            commande.Parameters.AddWithValue("@NomBase", nomBase);
            commande.Parameters.AddWithValue("@NomTable", nomTable);

            using MySqlDataReader lecteur = commande.ExecuteReader();

            while (lecteur.Read())
            {
                colonnesCles.Add(lecteur.GetString("COLUMN_NAME"));
            }

            return colonnesCles;
        }
    }
}