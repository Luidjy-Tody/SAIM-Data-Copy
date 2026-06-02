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

        // Charge les tables d'une base source.
        // Exemple : DB_TestRH peut retourner dbo.EmployesTest.
        List<string> ChargerTablesBaseSource(string nomBase);

        // Compte les lignes d'une table source.
        // Exemple : DB_TestRH + dbo.EmployesTest retourne 2.
        int CompterLignesTableSource(string nomBase, string nomTable);

        // Vérifie si une base existe sur le serveur cible.
        // Si elle n'existe pas, elle est créée.
        // Retourne true si la base a été créée, false si elle existait déjà.
        bool VerifierOuCreerBaseCible(string nomBase);
        bool VerifierOuCreerTableCible(string nomBase, string nomTable);

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