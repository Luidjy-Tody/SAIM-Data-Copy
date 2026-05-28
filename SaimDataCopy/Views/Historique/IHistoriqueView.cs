using SaimDataCopy.Models.Historique;

namespace SaimDataCopy.Views.Historique
{
    // Interface de la page Historique.
    // Elle permet au Controller de communiquer avec la View.
    public interface IHistoriqueView
    {
        // Événement déclenché quand l'utilisateur clique sur Rechercher.
        event EventHandler? RechercheDemandee;

        // Événement déclenché quand l'utilisateur clique sur Voir détail.
        event Action<int>? DetailDemande;

        // Événement déclenché quand l'utilisateur clique sur Ouvrir le fichier log.
        event Action<int>? OuvrirLogDemande;

        // Récupère la date choisie dans le filtre.
        DateTime? ObtenirDateFiltre();

        // Récupère l'origine choisie : Tous, Manuel, Automatique.
        string ObtenirOrigineFiltre();

        // Récupère le statut choisi : Tous, Succès, Avertissement, Échec.
        string ObtenirStatutFiltre();

        // Affiche les exécutions dans le tableau.
        void AfficherExecutions(List<HistoriqueExecutionModel> executions);

        // Affiche le détail d'une exécution en bas de la page.
        void AfficherDetail(HistoriqueExecutionModel execution);

        // Affiche un message simple à l'utilisateur.
        void AfficherMessage(string message, string titre, MessageBoxIcon icone);
    }
}