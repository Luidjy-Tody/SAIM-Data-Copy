using SaimDataCopy.Models.Execution;

namespace SaimDataCopy.Views.Execution
{
    // Interface de la page Exécution.
    // Elle permet au Controller de communiquer avec la View
    // sans mélanger la logique métier avec l'interface graphique.
    public interface IExecutionView
    {
        // Événement lancé quand l'utilisateur clique sur "Tester la connexion".
        event EventHandler? TesterConnexionDemandee;

        // Événement lancé quand l'utilisateur clique sur "Lancer la copie".
        event EventHandler? LancerCopieDemandee;

        // Événement lancé quand l'utilisateur clique sur "Annuler".
        event EventHandler? AnnulerCopieDemandee;

        // Affiche les informations du petit tableau de bord.
        void AfficherTableauBord(ExecutionTableauBordModel tableauBord);

        // Affiche les résultats dans le tableau résumé.
        void AfficherResultats(List<ExecutionResultatBaseModel> resultats);

        // Vide le journal d'exécution.
        void ViderJournal();

        // Ajoute une ligne dans le journal noir.
        void AjouterLog(ExecutionLogModel log);

        // Affiche ou cache la zone de progression.
        void AfficherZoneProgression(bool visible);

        // Met à jour la barre de progression.
        void AfficherProgression(int pourcentage, string message);

        // Active ou désactive le bouton "Tester la connexion".
        void ActiverBoutonTesterConnexion(bool actif);

        // Active ou désactive le bouton "Lancer la copie".
        void ActiverBoutonLancer(bool actif);

        // Active ou désactive le bouton "Annuler".
        void ActiverBoutonAnnuler(bool actif);

        // Affiche un message simple à l'utilisateur.
        void AfficherMessage(string message, string titre);
    }
}