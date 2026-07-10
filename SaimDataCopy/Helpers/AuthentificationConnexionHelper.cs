using Microsoft.EntityFrameworkCore;
using MySqlConnector;

namespace SaimDataCopy.Helpers
{
    public static class AuthentificationConnexionHelper
    {
        // Ton encadreur modifie seulement ces valeurs.
        private const string Serveur = "localhost";
        private const uint Port = 3306;
        private const string BaseAuthentification = "saimdatacopy_auth";
        private const string Utilisateur = "root";
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
                SslMode = MySqlSslMode.None,
                CharacterSet = "latin1"
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
                SslMode = MySqlSslMode.None,
                CharacterSet = "latin1"
            };

            return builder.ConnectionString;
        }

        public static ServerVersion ObtenirVersionServeurMySql()
        {
            return ServerVersion.Parse("5.1.73-mysql");
        }
    }
}