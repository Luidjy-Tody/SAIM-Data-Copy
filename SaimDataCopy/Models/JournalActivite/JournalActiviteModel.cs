namespace SaimDataCopy.Models.JournalActivite
{
    /// <summary>
    /// Représente une action affichée dans le journal d'activité.
    /// </summary>
    public class JournalActiviteModel
    {
        public int Id { get; set; }

        public int? UtilisateurId { get; set; }

        public string NomUtilisateur { get; set; } = string.Empty;

        public string Action { get; set; } = string.Empty;

        public string Details { get; set; } = string.Empty;

        public DateTime DateHeure { get; set; }
    }
}