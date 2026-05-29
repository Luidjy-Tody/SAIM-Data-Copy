using SaimDataCopy.Models.BasesCopier;

namespace SaimDataCopy.DataProviders.BasesCopier
{
    // Interface du DataProvider pour les bases à copier.
    // Le DataProvider s'occupe seulement de charger et enregistrer les données.
    public interface IBasesCopierDataProvider
    {
        // Charge les bases disponibles sur le serveur source.
        // Pour l'instant, ce sera encore simulé.
        // Plus tard, cette méthode lira vraiment SQL Server.
        List<BaseCopieModel> ChargerBasesDepuisServeurSource();

        // Charge le dernier état sauvegardé par l'utilisateur.
        // Exemple : bases cochées/décochées + mode de copie choisi.
        List<BaseCopieModel> ChargerBasesSauvegardees();

        // Enregistre le dernier état de la page Bases à copier.
        void EnregistrerBases(List<BaseCopieModel> bases);

        // Applique le mode de copie global venant de la page Configuration.
        // Exemple : si Configuration = "Mise à jour",
        // toutes les bases prennent ce mode.
        void AppliquerModeCopieGlobal(string modeCopieGlobal);
    }
}