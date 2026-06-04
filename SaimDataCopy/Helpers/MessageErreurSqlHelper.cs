using Microsoft.Data.SqlClient;

namespace SaimDataCopy.Helpers
{
    // Helper pour transformer les erreurs SQL techniques
    // en messages simples pour l'utilisateur.
    public static class MessageErreurSqlHelper
    {
        public static string ObtenirMessageSimple(Exception exception)
        {
            SqlException? erreurSql = TrouverSqlException(exception);

            if (erreurSql != null)
            {
                return ObtenirMessageDepuisSqlException(erreurSql);
            }

            string message = exception.Message;

            if (message.Contains("Format of the initialization string", StringComparison.OrdinalIgnoreCase))
            {
                return "La chaîne de connexion SQL est invalide. Vérifie le serveur, le port ou la chaîne de connexion.";
            }

            if (message.Contains("Login failed", StringComparison.OrdinalIgnoreCase))
            {
                return "Identifiant ou mot de passe SQL incorrect.";
            }

            if (message.Contains("network-related", StringComparison.OrdinalIgnoreCase) ||
                message.Contains("server was not found", StringComparison.OrdinalIgnoreCase))
            {
                return "Serveur SQL introuvable ou inaccessible. Vérifie le nom du serveur et le port.";
            }

            if (message.Contains("Cannot open database", StringComparison.OrdinalIgnoreCase))
            {
                return "La base de données demandée est introuvable ou inaccessible.";
            }

            return "Erreur SQL : " + exception.Message;
        }

        private static string ObtenirMessageDepuisSqlException(SqlException exception)
        {
            foreach (SqlError erreur in exception.Errors)
            {
                switch (erreur.Number)
                {
                    case 18456:
                        return "Identifiant ou mot de passe SQL incorrect.";

                    case 4060:
                        return "La base de données demandée est introuvable ou inaccessible.";

                    case 2:
                    case 26:
                    case 53:
                    case 11001:
                        return "Serveur SQL introuvable. Vérifie le nom du serveur ou l'instance SQL.";

                    case 10060:
                    case 10061:
                    case 258:
                        return "Connexion au serveur SQL impossible. Vérifie le port, le pare-feu ou si SQL Server est démarré.";

                    default:
                        break;
                }
            }

            return "Erreur SQL : " + exception.Message;
        }

        private static SqlException? TrouverSqlException(Exception exception)
        {
            Exception? erreurActuelle = exception;

            while (erreurActuelle != null)
            {
                if (erreurActuelle is SqlException erreurSql)
                {
                    return erreurSql;
                }

                erreurActuelle = erreurActuelle.InnerException;
            }

            return null;
        }
    }
}