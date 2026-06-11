using SaimDataCopy.Models.Configuration;

namespace SaimDataCopy.Helpers
{
    // Helper pour construire les chaînes de connexion selon le type de serveur.
    public static class ChaineConnexionHelper
    {
        public static string ConstruireChaineConnexionSource(ConfigurationModel configuration)
        {
            return ConstruireChaineConnexionServeur(configuration.ServeurSource);
        }

        public static string ConstruireChaineConnexionCible(ConfigurationModel configuration)
        {
            return ConstruireChaineConnexionServeur(configuration.ServeurCible);
        }

        /// <summary>
        /// Construit une chaîne de connexion selon le type du serveur.
        /// Cette méthode est utilisée par la configuration et par les DataProviders.
        /// </summary>
        public static string ConstruireChaineConnexionServeur(ServeurConfigModel serveur)
        {
            if (!string.IsNullOrWhiteSpace(serveur.ChaineConnexion))
            {
                return serveur.ChaineConnexion;
            }

            return serveur.TypeServeur switch
            {
                "MySQL" => ConstruireChaineConnexionMySql(serveur),
                "SQL Server" => ConstruireChaineConnexionSqlServer(serveur),
                _ => ConstruireChaineConnexionSqlServer(serveur)
            };
        }

        private static string ConstruireChaineConnexionSqlServer(ServeurConfigModel serveur)
        {
            string nomServeur = serveur.NomServeur.Trim();

            bool estLocalDb = nomServeur.Contains(
                "(localdb)",
                StringComparison.OrdinalIgnoreCase
            );

            bool estInstanceNommee = nomServeur.Contains('\\');

            string serveurAvecPort = nomServeur;

            if (!estLocalDb &&
                !estInstanceNommee &&
                serveur.Port > 0 &&
                !serveurAvecPort.Contains(','))
            {
                serveurAvecPort = $"{nomServeur},{serveur.Port}";
            }

            bool utiliseAuthentificationSql =
                !string.IsNullOrWhiteSpace(serveur.Identifiant);

            if (utiliseAuthentificationSql)
            {
                return
                    $"Server={serveurAvecPort};" +
                    "Database=master;" +
                    $"User Id={serveur.Identifiant};" +
                    $"Password={serveur.MotDePasse};" +
                    "TrustServerCertificate=True;";
            }

            return
                $"Server={serveurAvecPort};" +
                "Database=master;" +
                "Trusted_Connection=True;" +
                "TrustServerCertificate=True;";
        }

        private static string ConstruireChaineConnexionMySql(ServeurConfigModel serveur)
        {
            string nomServeur = serveur.NomServeur.Trim();

            int port = serveur.Port > 0
                ? serveur.Port
                : 3306;

            return
                $"Server={nomServeur};" +
                $"Port={port};" +
                $"User ID={serveur.Identifiant};" +
                $"Password={serveur.MotDePasse};" +
                "Database=mysql;" +
                "Allow User Variables=True;";
        }
    }
}