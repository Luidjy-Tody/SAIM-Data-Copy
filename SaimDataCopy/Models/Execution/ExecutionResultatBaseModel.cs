

namespace SaimDataCopy.Models.Execution
{
    // Modèle utilisé pour afficher une ligne dans le tableau
    // "Résumé de la dernière exécution".
    public class ExecutionResultatBaseModel
    {
        // Nom de la base copiée.
        // Exemple : DB_Ventes
        public string NomBase { get; set; } = string.Empty;

        // Nombre de lignes avant la copie.
        public int LignesApres { get; set; }

        // Nombre de lignes apres la copie.
        public int LignesAvant { get; set; }



        // Résultat de la copie.
        // Exemple : Succès, Avertissement, Erreur.
        public string Resultat {  get; set; } = string.Empty ;

        // Message complémentaire si besoin.
        // Exemple : "Table vide" ou "Copie terminée".
        public string Message { get; set; } = string.Empty;
    }
}
