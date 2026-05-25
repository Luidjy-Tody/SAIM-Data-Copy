using SaimDataCopy.Models;

namespace SaimDataCopy.Views.Interfaces.Configuration
{
    // Interface entre la View et le Controller.
    // Le Controller utilise cette interface au lieu de toucher directement aux TextBox.
    public interface IConfigurationView
    {
        // Récupère les données saisies dans l'interface.
        ConfigurationModel RecupererConfiguration();

        // Affiche un message de succès à l'utilisateur.
        void AfficherMessageSucces(string message);

        // Affiche un message d'erreur à l'utilisateur.
        void AfficherMessageErreur(string message);

        // Méthode appelée par MainForm quand l'utilisateur clique sur
        // le bouton "Enregistrer les paramètres" dans la barre du bas.
        void DemanderEnregistrement();

        // Événement déclenché quand l'utilisateur demande l'enregistrement.
        event EventHandler? EnregistrerConfigurationDemande;
    }
}