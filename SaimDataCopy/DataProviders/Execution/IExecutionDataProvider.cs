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
