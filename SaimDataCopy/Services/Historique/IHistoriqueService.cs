using SaimDataCopy.Models.Historique;

namespace SaimDataCopy.Services.Historique
{
    // Interface du Service de la page Historique.
    // Le Service contient la logique métier : filtre, recherche, détail, etc.
    public interface IHistoriqueService
    {
        // Recherche les exécutions selon les filtres choisis.
        List<HistoriqueExecutionModel> RechercherExecutions(
            DateTime? dateFiltre,
            string origineFiltre,
            string statutFiltre
        );

        // Récupère une exécution par son Id pour afficher le détail.
        HistoriqueExecutionModel? ObtenirExecutionParId(int idExecution);

        // Vérifie si le fichier log existe avant de l'ouvrir.
        bool FichierLogExiste(int idExecution);

        // Récupère le chemin du fichier log.
        string ObtenirCheminFichierLog(int idExecution);
    }
}