using SaimDataCopy.Models.Execution;

namespace SaimDataCopy.Services.Execution
{
    // Interface du service Exécution.
    // Elle définit ce que la logique métier doit fournir au Controller.
    public interface IExecutionService
    {
        // Charge le tableau de bord affiché au démarrage.
        ExecutionTableauBordModel ChargerTableauBordInitial();

        // Charge le résumé de la dernière exécution.
        List<ExecutionResultatBaseModel> ChargerDerniersResultats();

        // Teste la connexion avant de lancer la copie.
        // Pour l'instant, on prépare une version simple et propre.
        // Plus tard, on pourra remplacer par un vrai test EF Core / SQL Server.
        Task<bool> TesterConnexionAsync(
            IProgress<ExecutionProgressionModel> progression,
            CancellationToken cancellationToken);

        // Lance la copie des bases.
        Task<List<ExecutionResultatBaseModel>> LancerCopieAsync(
            IProgress<ExecutionProgressionModel> progression,
            CancellationToken cancellationToken);
    }
}