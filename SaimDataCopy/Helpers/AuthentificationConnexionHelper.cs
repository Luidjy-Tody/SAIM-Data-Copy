using MySqlConnector;

namespace SaimDataCopy.Helpers
{
    public static class AuthentificationConnexionHelper
    {
        private const string Serveur = "localhost";
        private const uint Port = 3306;
        private const string BaseAuthentification = "saimdatacopy_auth";
        private const string Utilisateur = "";
        private const string MotDePasse = "";

        public static string ObtenirNomBaseAuthentification()
        {
            return BaseAuthentification;
        }

        public static string ObtenirChaineConnexionServeur()
        {
            MySqlConnectionStringBuilder builder = new MySqlConnectionStringBuilder
            {
                Server = Serveur,
                Port = Port,
                UserID = Utilisateur,
                Password = MotDePasse,
                SslMode = MySqlSslMode.None
            };

            return builder.ConnectionString;
        }

        public static string ObtenirChaineConnexionBaseAuthentification()
        {
            MySqlConnectionStringBuilder builder = new MySqlConnectionStringBuilder
            {
                Server = Serveur,
                Port = Port,
                Database = BaseAuthentification,
                UserID = Utilisateur,
                Password = MotDePasse,
                SslMode = MySqlSslMode.None
            };

            return builder.ConnectionString;
        }
    }
}