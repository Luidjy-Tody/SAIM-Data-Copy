using SaimDataCopy.Models.BasesCopier;

namespace SaimDataCopy.Services.BasesCopier
{
    // Interface du service Bases à copier.
    // Elle définit les méthodes que le service doit fournir.
    public interface IBasesCopierService
    {
        List<BaseCopieModel> ChargerBasesDemo();

        List<string> ObtenirModesCopie();

        BaseCopieModel CreerNouvelleBase(List<BaseCopieModel> basesExistantes, string nomBase);

        List<BaseCopieModel> SupprimerBases(
            List<BaseCopieModel> bases,
            List<string> nomsBasesSelectionnees);

        List<string> ValiderBases(List<BaseCopieModel> bases);

        bool EnregistrerBases(List<BaseCopieModel> bases);
    }
}