

namespace SaimDataCopy.Models.Execution
{
    // Modèle utilisé pour représenter une ligne du journal d'exécution.
    public class ExecutionLogModel
    {
        // Heure affichée dans le journal.
        // Exemple : 09:42:02
        public string Heure {  get; set; } = string.Empty;

        // Message affiché dans le journal.
        // Exemple : Connexion PROD-SRV-01 : OK
        public string Message {  get; set; } = string.Empty;

        // Type de message.
        // On l'utilise pour choisir la couleur dans l'interface.
        // Exemple : Info, Succes, Avertissement, Erreur
        public string Type { get; set; } = "Info";
    }
}
