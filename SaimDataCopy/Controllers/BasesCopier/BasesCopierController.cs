using SaimDataCopy.Models.BasesCopier;
using SaimDataCopy.Services.BasesCopier;
using SaimDataCopy.Views.BasesCopier;

namespace SaimDataCopy.Controllers.BasesCopier
{
    // Controller de la page Bases à copier.
    // Il fait le lien entre la View et le Service.
    public class BasesCopierController
    {
        private readonly IBasesCopierView _view;
        private readonly IBasesCopierService _service;

        private List<BaseCopieModel> _bases = new List<BaseCopieModel>();

        // Constructeur simple.
        // Il reçoit la View et crée le Service.
        public BasesCopierController(IBasesCopierView view)
        {
            _view = view;
            _service = new BasesCopierService();

            BrancherEvenements();

            ChargerPage();
        }

        // Constructeur utile plus tard pour les tests.
        // Il permet d'injecter un autre service si besoin.
        public BasesCopierController(IBasesCopierView view, IBasesCopierService service)
        {
            _view = view;
            _service = service;

            BrancherEvenements();

            ChargerPage();
        }

        private void BrancherEvenements()
        {
            // Le Controller écoute les actions demandées par la View.
            _view.AjouterBaseDemandee += AjouterBase;
            _view.SupprimerSelectionDemandee += SupprimerSelection;
        }

        private void ChargerPage()
        {
            // On récupère les modes de copie depuis le Service.
            List<string> modesCopie = _service.ObtenirModesCopie();

            // On donne ces choix à la View pour remplir le ComboBox du tableau.
            _view.AfficherModesCopie(modesCopie);

            // On charge les bases depuis le Service.
            // Le Service demande maintenant au DataProvider de fournir les données.
            _bases = _service.ChargerBasesDemo();

            // On demande à la View d’afficher les bases dans le tableau.
            _view.AfficherBases(_bases);
        }

        private void AjouterBase(object? sender, EventArgs e)
        {
            // On demande le nom de la nouvelle base à l'utilisateur.
            string? nomBase = _view.DemanderNomNouvelleBase();

            // Si l'utilisateur annule ou laisse vide, on n'ajoute rien.
            if (string.IsNullOrWhiteSpace(nomBase))
            {
                return;
            }

            // On récupère l’état actuel du tableau.
            _bases = _view.RecupererBases();

            // Le Service crée la nouvelle base.
            BaseCopieModel nouvelleBase = _service.CreerNouvelleBase(_bases, nomBase);

            // On ajoute la nouvelle base dans la liste.
            _bases.Add(nouvelleBase);

            // On recharge le tableau.
            _view.AfficherBases(_bases);
        }

        private void SupprimerSelection(object? sender, EventArgs e)
        {
            // On récupère seulement les bases cochées.
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

            // On récupère l’état actuel du tableau.
            _bases = _view.RecupererBases();

            // Le Service supprime seulement les bases sélectionnées.
            _bases = _service.SupprimerBases(_bases, nomsSelectionnes);

            // On recharge le tableau.
            _view.AfficherBases(_bases);
        }

        public void Enregistrer()
        {
            // On récupère les données actuelles du tableau.
            _bases = _view.RecupererBases();

            // Le Service valide puis demande au DataProvider d'enregistrer.
            bool resultat = _service.EnregistrerBases(_bases);

            if (resultat)
            {
                _view.AfficherMessage(
                    "Enregistrement",
                    "Les bases à copier ont été enregistrées avec succès.",
                    MessageBoxIcon.Information
                );
            }
            else
            {
                List<string> erreurs = _service.ValiderBases(_bases);
                string message = string.Join(Environment.NewLine, erreurs);

                _view.AfficherMessage(
                    "Validation",
                    message,
                    MessageBoxIcon.Warning
                );
            }
        }
    }
}