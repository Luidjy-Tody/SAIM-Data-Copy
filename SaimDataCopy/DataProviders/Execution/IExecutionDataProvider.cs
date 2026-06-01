using SaimDataCopy.Models.BasesCopier;
using SaimDataCopy.Models.Execution;

namespace SaimDataCopy.DataProviders.Execution
{
    // Interface du DataProvider Exécution.
    // Il prépare l'accès aux données nécessaires pour lancer la copie.
    public interface IExecutionDataProvider
    {
        // Charge les bases cochées dans la configuration des bases à copier.
        List<BaseCopieModel> ChargerBasesSelectionnees();

        // Charge les tables d'une base source.
        // Exemple : DB_TestRH peut retourner dbo.EmployesTest.
        List<string> ChargerTablesBaseSource(string nomBase);

        // Compte le nombre de lignes dans une table d'une base source.
        // Exemple : DB_TestRH + dbo.EmployesTest retourne 2.
        int CompterLignesTableSource(string nomBase, string nomTable);

        // Vérifie si une base existe sur le serveur cible.
        // Si elle n'existe pas, elle est créée.
        // Retourne true si la base a été créée, false si elle existait déjà.
        bool VerifierOuCreerBaseCible(string nomBase);

        // Charge le tableau de bord de la dernière exécution.
        ExecutionTableauBordModel ChargerDernierTableauBord();

        // Charge le résumé de la dernière exécution.
        List<ExecutionResultatBaseModel> ChargerDerniersResultats();

        // Enregistre le résultat de la dernière exécution.
        void EnregistrerDerniereExecution(
            ExecutionTableauBordModel tableauBord,
            List<ExecutionResultatBaseModel> resultats);
    }
}