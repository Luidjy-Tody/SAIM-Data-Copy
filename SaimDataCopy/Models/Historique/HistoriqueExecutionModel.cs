namespace SaimDataCopy.Models.Historique
{
    // Modèle qui représente une exécution dans l'historique.
    public class HistoriqueExecutionModel
    {
        public int Id { get; set; }

        public DateTime DateHeureLancement { get; set; }

        public string Origine { get; set; } = string.Empty;

        public List<string> BasesTraitees { get; set; } = new List<string>();

        public int LignesAvant { get; set; }

        public int LignesApres { get; set; }

        public int DureeSecondes { get; set; }

        public string Statut { get; set; } = string.Empty;

        public bool EmailEnvoye { get; set; }

        public DateTime? DateHeureEmail { get; set; }

        // Chemin du fichier log complet.
        // Exemple : C:\Logs\SaimDataCopy\2025-05-14.log
        public string CheminFichierLog { get; set; } = string.Empty;

        public List<HistoriqueEtapeExecutionModel> Etapes { get; set; } = new List<HistoriqueEtapeExecutionModel>();

        // Texte déjà prêt pour l'affichage dans le tableau.
        public string DateHeureAffichage
        {
            get
            {
                return DateHeureLancement.ToString("dd/MM/yyyy HH:mm");
            }
        }

        // Exemple : DB_Ventes, DB_RH, DB_Comptabilite
        public string BasesTraiteesAffichage
        {
            get
            {
                return string.Join(", ", BasesTraitees);
            }
        }

        // Exemple : 1 968 → 1 972
        public string LignesAvantApresAffichage
        {
            get
            {
                return $"{LignesAvant:N0} → {LignesApres:N0}".Replace(",", " ");
            }
        }

        // Exemple : 12s
        public string DureeAffichage
        {
            get
            {
                return $"{DureeSecondes}s";
            }
        }

        // Exemple : 14/05/2025 02:12
        public string DateEmailAffichage
        {
            get
            {
                if (DateHeureEmail == null)
                {
                    return "Non envoyé";
                }

                return DateHeureEmail.Value.ToString("dd/MM/yyyy HH:mm");
            }
        }
    }

    // Modèle qui représente une ligne de détail d'une exécution.
    public class HistoriqueEtapeExecutionModel
    {
        public string NomBase { get; set; } = string.Empty;

        public int LignesAvant { get; set; }

        public int LignesApres { get; set; }

        public string Statut { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;

        // Exemple : DB_Ventes : 1 238 → 1 240 lignes
        public string LigneAffichage
        {
            get
            {
                string texteLignes = $"{LignesAvant:N0} → {LignesApres:N0}".Replace(",", " ");
                return $"{NomBase} : {texteLignes} lignes";
            }
        }
    }
}