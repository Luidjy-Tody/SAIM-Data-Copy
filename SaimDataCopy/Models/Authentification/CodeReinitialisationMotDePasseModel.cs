namespace SaimDataCopy.Models.Authentification
{
    public class CodeReinitialisationMotDePasseModel
    {
        public int Id { get; set; }

        public int UtilisateurId { get; set; }

        public UtilisateurModel? Utilisateur { get; set; }

        public string CodeHash { get; set; } = string.Empty;

        public DateTime DateCreation { get; set; } = DateTime.Now;

        public DateTime DateExpiration { get; set; }

        public bool EstUtilise { get; set; } = false;
    }
}