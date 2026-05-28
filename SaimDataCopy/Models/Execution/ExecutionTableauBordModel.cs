
namespace SaimDataCopy.Models.Execution
{
    // Modèle pour les 3 cartes en haut de la page Exécution.
    // Exemple : bases sélectionnées, lignes copiées, durée.
    public class ExecutionTableauBordModel
    {
        // Nombre de bases cochées dans la page Bases à copier.
        public int NombreBasesSelectionnees { get; set; }

        // Nombre total de lignes copiées pendant la dernière exécution.
        public int NombreLignesCopiees { get; set; }

        // Durée affichée dans la carte.
        // Exemple : "12s" ou "-" si aucune copie n'a encore été lancée.
        public string DureeDerniereExecution { get; set; } = "-";
    }
}
