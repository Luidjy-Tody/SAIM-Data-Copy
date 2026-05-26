using SaimDataCopy.Models.BasesCopier;

namespace SaimDataCopy.DataProviders.BasesCopier
{
    // Interface du DataProvider pour les bases à copier.
    // Le DataProvider sera responsable de l'accès aux données.
    // Plus tard, il pourra charger et enregistrer les bases depuis SQL Server.
    public interface IBasesCopierDataProvider
    {
        // Charge la liste des bases à copier.
        List<BaseCopieModel> ChargerBases();

        // Enregistre la liste des bases à copier.
        void EnregistrerBases(List<BaseCopieModel> bases);
    }
}