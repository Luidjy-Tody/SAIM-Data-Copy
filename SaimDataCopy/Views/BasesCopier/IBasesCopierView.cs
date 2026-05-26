using SaimDataCopy.Models.BasesCopier;

namespace SaimDataCopy.Views.BasesCopier
{
    public interface IBasesCopierView
    {
        // Événement déclenché quand l'utilisateur clique sur "Ajouter une base"
        event EventHandler? AjouterBaseDemandee;

        // Événement déclenché quand l'utilisateur clique sur "Supprimer la sélection"
        event EventHandler? SupprimerSelectionDemandee;


        // Affiche les bases dans le tableau
        void AfficherBases(List<BaseCopieModel> bases);

        // Affiche les choix du mode de copie dans la ComboBox du tableau
        void AfficherModesCopie(List<string> modesCopie);
        string? DemanderNomNouvelleBase();

        // Récupère les données actuelles du tableau
        List<BaseCopieModel> RecupererBases();

        // Récupère les noms des bases sélectionnées dans le tableau
        List<string> RecupererNomsBasesCochees();

        // Affiche un message à l'utilisateur
        void AfficherMessage(string titre, string message, MessageBoxIcon icon);

       
    }
}