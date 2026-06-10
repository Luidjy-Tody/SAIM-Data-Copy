using SaimDataCopy.Models.BasesCopier;
using SaimDataCopy.Services.BasesCopier;
using SaimDataCopy.Views.BasesCopier;
using System.Windows.Forms;

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
            // La View déclenche ces événements.
            // Le Controller décide ensuite quelle action faire.
            _view.CocherToutesBasesDemandee += CocherToutesBases;
            _view.DecocherToutesBasesDemandee += DecocherSelection;
        }

        private void ChargerPage()
        {
            // On récupère les modes de copie depuis le Service.
            List<string> modesCopie = _service.ObtenirModesCopie();

            // On donne ces choix à la View pour remplir la ComboBox du tableau.
            _view.AfficherModesCopie(modesCopie);

            // On charge les bases.
            // Le Service récupère le dernier état sauvegardé.
            // Si aucune sauvegarde n'existe, les bases sont cochées par défaut.
            _bases = _service.ChargerBases();

            // On demande à la View d'afficher les bases dans le tableau.
            _view.AfficherBases(_bases);
        }

        private void CocherToutesBases(object? sender, EventArgs e)
        {
            // On récupère l'état actuel du tableau.
            _bases = _view.RecupererBases();

            // Le Service coche toutes les bases.
            _bases = _service.CocherToutesBases(_bases);

            // On recharge le tableau.
            _view.AfficherBases(_bases);
        }

        private void DecocherSelection(object? sender, EventArgs e)
        {
            // décocher les bases cochées.
            List<string> nomsSelectionnes = _view.RecupererNomsBasesCochees();

            if (nomsSelectionnes.Count == 0)
            {
                _view.AfficherMessage(
                    "Sélection",
                    "Veuillez cocher au moins une base à décocher.",
                    MessageBoxIcon.Information
                );

                return;
            }

            // On récupère l'état actuel du tableau.
            _bases = _view.RecupererBases();

            // Le Service décoche les bases sélectionnées.
            _bases = _service.DecocherBases(_bases, nomsSelectionnes);

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

        public void AppliquerModeCopieGlobal(string modeCopieGlobal)
        {
            // Cette méthode sera appelée plus tard depuis Configuration.
            // Elle applique le mode global à toutes les bases sauvegardées.
            _service.AppliquerModeCopieGlobal(modeCopieGlobal);

            // Si la page Bases à copier est déjà ouverte,
            // on recharge directement l'affichage.
            ChargerPage();
        }
    }
}