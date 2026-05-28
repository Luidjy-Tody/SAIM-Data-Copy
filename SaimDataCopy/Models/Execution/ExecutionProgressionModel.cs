
namespace SaimDataCopy.Models.Execution
{
    // Modèle utilisé pendant l'exécution de la copie.
    // Il permet au Service d'envoyer la progression au Controller.
    public class ExecutionProgressionModel
    {
        // Pourcentage de progression entre 0 et 100.
        public int Pourcentage { get; set; }

        // Texte affiché sous la barre de progression.
        // Exemple : "Progression : 2 bases sur 3 copiées"
        public string MessageProgression { get; set; } = string.Empty;

        // Ligne de journal à ajouter dans le bloc noir.
        public ExecutionLogModel? Log { get; set; }

        // Mise à jour du tableau de bord pendant ou après la copie.
        public ExecutionTableauBordModel? TableauBord { get; set; }
    }
}
