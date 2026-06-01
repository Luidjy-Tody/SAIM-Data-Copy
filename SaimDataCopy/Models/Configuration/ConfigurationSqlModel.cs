

namespace SaimDataCopy.Models.Configuration
{
    // Model utilisé par Entity Framework Core pour sauvegarder
    // la configuration dans SQL Server.
    //
    // On garde ce Model séparé de ConfigurationModel
    // pour ne pas casser la View actuelle.
    public class ConfigurationSqlModel
    {
        public int Id { get; set; }

        // Informations du serveur source
        public string ServeurSourceNom { get; set; } = string.Empty;

        public string ServeurSourceChaineConnexion { get; set; } = string.Empty;

        public string ServeurSourceIdentifiant {  get; set; } = string.Empty;

        public string ServeurSourceMotDePasse {  get; set; } = string.Empty;

        public int ServeurSourcePort { get; set; }

        // Informations du serveur cible
        public string ServeurCibleNom {  get; set; } = string.Empty;

        public string ServeurCibleChaineConnexion { get; set; } = string.Empty;

        public string ServeurCibleIdentifiant { get; set; } = string.Empty;

        public string ServeurCibleMotDePasse { get; set; } = string.Empty;

        public int ServeurCiblePort { get; set; }

        // Paramètres généraux de copie

        public string ModeCopie {  get; set; } = "Écraser";

        public string ComportementErreur { get; set; } = "Continuer avec les autres";

        public int TentativesReprise { get; set; } = 1;

        // Date de dernière modification de la configuration
        
        public DateTime DateModification {  get; set; } = DateTime.Now;

    }
}
