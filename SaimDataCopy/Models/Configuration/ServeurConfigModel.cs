namespace SaimDataCopy.Models.Configuration
{
    // Model qui représente les informations d'un serveur SQL Server.
    // Il ne contient pas de design, seulement des données.
    public class ServeurConfigModel
    {
        public string NomServeur { get; set; } = string.Empty;

        public string ChaineConnexion { get; set; } = string.Empty;

        public string Identifiant { get; set; } = string.Empty;

        public string MotDePasse { get; set; } = string.Empty;

        public int Port { get; set; }
    }
}