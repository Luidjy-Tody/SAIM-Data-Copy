using SaimDataCopy.Models;
using SaimDataCopy.Services;
using SaimDataCopy.Views.Interfaces.BasesCopier;

namespace SaimDataCopy.Controllers
{
    public class BasesCopierController
    {
        private readonly IBasesCopierView _view;
        private readonly BasesCopierService _service;

        private List<BaseCopieModel> _bases = new List<BaseCopieModel>();

        public BasesCopierController(IBasesCopierView view)
        {
            _view = view;
            _service = new BasesCopierService();

            //Le controller ecoute les actions demandees par la view
            _view.AjouterBaseDemandee += AjouterBase;
            _view.SupprimerSelectionDemandee += SupprimerSelection;

            chargerPage();
        }

    private void chargerPage()

        {
            // On récupère les modes de copie depuis le Service
            // Exemple : Écraser, Mise à jour
            List<string> modesCopie = _service.ObtenirModesCopie();

            // On donne ces choix à la View pour remplir le ComboBox du tableau

            _view.AfficherModesCopie(modesCopie);

            // Pour l’instant, on charge des données de test
            _bases = _service.ChargerBasesDemo();

            // On demande à la View d’afficher les bases dans le tableau
            _view.AfficherBases(_bases);

        }

        private void AjouterBase(object? sender, EventArgs e)
        {
            // On demande d'abord le nom de la base à l'utilisateur.
            string? nomBase = _view.DemanderNomNouvelleBase();

            // Si l'utilisateur annule ou laisse vide, on n'ajoute rien.
            if (string.IsNullOrWhiteSpace(nomBase))
            {
                return;
            }

            // On récupère l’état actuel du tableau.
            _bases = _view.RecupererBases();

            // Le Service crée la nouvelle base avec le nom écrit par l'utilisateur.
            BaseCopieModel nouvelleBase = _service.CreerNouvelleBase(_bases, nomBase);

            // On ajoute la nouvelle base dans la liste.
            _bases.Add(nouvelleBase);

            // On recharge le tableau.
            _view.AfficherBases(_bases);
        }

        private void SupprimerSelection(object? sender, EventArgs e) 
        {
            List<string> nomsSelectionnes = _view.RecupererNomsBasesCochees();

            if (nomsSelectionnes.Count == 0)
            {
                _view.AfficherMessage(
                    "Suppression",
                    "Veuillez sélectionner une ligne à supprimer.",
                    MessageBoxIcon.Information
                    );
            return;
            }

            _bases = _view.RecupererBases();

            // Le Service supprime les bases sélectionnées
            _bases = _service.SupprimerBases(_bases, nomsSelectionnes);

            _view.AfficherBases( _bases );

        }

        public void Enregistrer()
        {
            // On récupère les données actuelles du tableau
            _bases = _view.RecupererBases();

            // Le Service vérifie les règles métier
            List<string> erreurs = _service.ValiderBases(_bases);

            if (erreurs.Count > 0)
            {
                string message = string.Join(Environment.NewLine, erreurs);

                _view.AfficherMessage(
                    "Validation",
                    message,
                    MessageBoxIcon.Warning
                );

                return;
            }

            bool resultat = _service.EnregistrerBases(_bases);

            if (resultat)
            {
                _view.AfficherMessage(
                    "Enregistrement",
                    "Les bases à copier ont été enregistrées avec succès.",
                    MessageBoxIcon.Information
                );
            }
        }

    }
}
