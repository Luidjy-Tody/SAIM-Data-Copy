namespace SaimDataCopy.Models.Configuration
{
    // Model qui représente les informations d'un serveur.
    // Il peut être SQL Server ou MySQL.
    public class ServeurConfigModel
    {
        // SQL Server ou MySQL
        public string TypeServeur { get; set; } = "SQL Server";

        public string NomServeur { get; set; } = string.Empty;

        public string ChaineConnexion { get; set; } = string.Empty;

        public string Identifiant { get; set; } = string.Empty;

        public string MotDePasse { get; set; } = string.Empty;

        public int Port { get; set; }
    }
}