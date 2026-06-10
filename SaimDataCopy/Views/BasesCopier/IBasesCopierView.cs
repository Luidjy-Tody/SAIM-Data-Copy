using SaimDataCopy.Models.BasesCopier;

namespace SaimDataCopy.Views.BasesCopier
{
    public interface IBasesCopierView
    {
        // Événement déclenché quand l'utilisateur veut cocher toutes les bases.
        event EventHandler? CocherToutesBasesDemandee;

        // Événement déclenché quand l'utilisateur clique sur "Supprimer la sélection".
        // Attention : maintenant, cela décoche les bases au lieu de les supprimer.
        event EventHandler? DecocherToutesBasesDemandee;

        // Affiche les bases dans le tableau.
        void AfficherBases(List<BaseCopieModel> bases);

        // Affiche les choix du mode de copie dans la ComboBox du tableau.
        void AfficherModesCopie(List<string> modesCopie);

        // Récupère les données actuelles du tableau.
        List<BaseCopieModel> RecupererBases();

        // Récupère les noms des bases cochées dans le tableau.
        List<string> RecupererNomsBasesCochees();

        // Affiche un message à l'utilisateur.
        void AfficherMessage(string titre, string message, MessageBoxIcon icon);
    }
}