using SaimDataCopy.Models.BasesCopier;

namespace SaimDataCopy.Services.BasesCopier
{
    // Interface du service Bases à copier.
    // Le service contient la logique métier de cette page.
    public interface IBasesCopierService
    {
        // Charge les bases à afficher dans la page.
        // Le service va demander au DataProvider :
        // - les bases disponibles sur le serveur source
        // - le dernier état sauvegardé
        List<BaseCopieModel> ChargerBases();

        // Retourne les modes de copie disponibles dans le tableau.
        List<string> ObtenirModesCopie();

        // Coche toutes les bases disponibles.
        List<BaseCopieModel> CocherToutesBases(List<BaseCopieModel> bases);

        // Décoche les bases sélectionnées au lieu de les supprimer.
        List<BaseCopieModel> DecocherBases(
            List<BaseCopieModel> bases,
            List<string> nomsBasesSelectionnees
        );

        // Vérifie si les bases sont valides avant enregistrement.
        List<string> ValiderBases(List<BaseCopieModel> bases);

        // Enregistre le dernier état de la page Bases à copier.
        bool EnregistrerBases(List<BaseCopieModel> bases);

        // Applique le mode global venant de la page Configuration.
        // Exemple : si Configuration = "Mise à jour",
        // toutes les bases prennent ce mode.
        void AppliquerModeCopieGlobal(string modeCopieGlobal);
    }
}