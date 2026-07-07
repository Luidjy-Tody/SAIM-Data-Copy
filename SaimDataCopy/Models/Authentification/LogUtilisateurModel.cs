
namespace SaimDataCopy.Models.Authentification
{
    public class LogUtilisateurModel
    {
        public int Id { get; set; }

        public int? UtilisateurId { get; set; }

        public string NomUtilisateur { get; set; } = string.Empty;

        public string Action {  get; set; } = string.Empty ;

        public string Details {  get; set; } = string.Empty ;

        public DateTime DateHeure { get; set; } = DateTime.Now ;

        public UtilisateurModel? Utilisateur { get; set; }
    }
}
