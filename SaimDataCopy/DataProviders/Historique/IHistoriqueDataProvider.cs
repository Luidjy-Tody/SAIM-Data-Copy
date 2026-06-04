using SaimDataCopy.Models.Historique;

namespace SaimDataCopy.DataProviders.Historique
{
    // Interface du DataProvider de l'historique.
    // Le DataProvider s'occupe seulement de récupérer et sauvegarder les données.
    public interface IHistoriqueDataProvider
    {
        // Charge la liste des exécutions enregistrées.
        List<HistoriqueExecutionModel> ChargerExecutions();

        // Ajoute une nouvelle exécution dans l'historique.
        void EnregistrerExecution(HistoriqueExecutionModel execution);

    }
}