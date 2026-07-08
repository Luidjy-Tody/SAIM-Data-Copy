namespace SaimDataCopy.Models.Authentification
{
    public class UtilisateurModel
    {
        public int Id { get; set; }

        public string NomComplet { get; set; } = string.Empty;

        public string Identifiant { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string MotDePasseHash { get; set; } = string.Empty;

        public DateTime DateCreation { get; set; } = DateTime.Now;

        public DateTime? DerniereConnexion { get; set; }

        public bool EstActif { get; set; } = true;

        public string Statut { get; set; } = "User";
    }
}