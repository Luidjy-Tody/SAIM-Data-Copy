using SaimDataCopy.Controllers.Authentification;
using SaimDataCopy.Models.BasesCopier;
using SaimDataCopy.Services.BasesCopier;
using SaimDataCopy.Views.BasesCopier;
using System.Windows.Forms;
using SaimDataCopy.Views.Commun;

namespace SaimDataCopy.Controllers.BasesCopier
{
    // Controller de la page Bases à copier.
    // Il fait le lien entre la View et le Service.
    public class BasesCopierController
    {
        private readonly IBasesCopierView _view;
        private readonly IBasesCopierService _service;
        private readonly AuthentificationController _authentificationController;

        private List<BaseCopieModel> _bases = new List<BaseCopieModel>();

        // Constructeur simple.
        // Il reçoit la View et crée le Service.
        public BasesCopierController(IBasesCopierView view)
        {
            _view = view;
            _service = new BasesCopierService();
            _authentificationController = new AuthentificationController();

            BrancherEvenements();
            ChargerPage();
        }

        // Constructeur utile plus tard pour les tests.
        // Il permet d'injecter un autre service si besoin.
        public BasesCopierController(IBasesCopierView view, IBasesCopierService service)
        {
            _view = view;
            _service = service;
            _authentificationController = new AuthentificationController();

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
            try
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

                // Après le chargement initial, la page correspond à l'état sauvegardé.
                if (_view is IPageEnregistrable pageEnregistrable)
                {
                    pageEnregistrable.MarquerCommeEnregistre();
                }
            }
            catch (Exception ex)
            {
                _bases = new List<BaseCopieModel>();

                _view.AfficherBases(_bases);

                _view.AfficherMessage(
                    "Erreur de connexion",
                    "Impossible de charger les bases depuis le serveur source." + Environment.NewLine +
                    "Vérifiez d'abord les paramètres de la page Configuration." + Environment.NewLine +
                    Environment.NewLine +
                    "Détail : " + ex.Message,
                    MessageBoxIcon.Error
                );
            }
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
            EnregistrerDepuisMainForm();
        }

        /// <summary>
        /// Enregistre les bases à copier et retourne true si l'enregistrement réussit.
        /// Cette méthode est utilisée par MainForm avant de changer de page.
        /// </summary>
        public bool EnregistrerDepuisMainForm()
        {
            // On récupère les données actuelles du tableau.
            _bases = _view.RecupererBases();

            // Le Service valide puis demande au DataProvider d'enregistrer.
            bool resultat = _service.EnregistrerBases(_bases);

            if (resultat)
            {
                if (_view is IPageEnregistrable pageEnregistrable)
                {
                    pageEnregistrable.MarquerCommeEnregistre();
                }

                EnregistrerLogUtilisateur(
                    "Enregistrement Bases à copier",
                    ConstruireDetailsBases(_bases)
                );

                _view.AfficherMessage(
                    "Enregistrement",
                    "Les bases à copier ont été enregistrées avec succès.",
                    MessageBoxIcon.Information
                );

                return true;
            }

            List<string> erreurs = _service.ValiderBases(_bases);
            string message = string.Join(Environment.NewLine, erreurs);

            _view.AfficherMessage(
                "Validation",
                message,
                MessageBoxIcon.Warning
            );

            return false;
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

        private string ConstruireDetailsBases(List<BaseCopieModel> bases)
        {
            List<BaseCopieModel> basesIncluses = bases
                .Where(b => b.Inclure)
                .OrderBy(b => b.OrdreTraitement)
                .ToList();

            List<BaseCopieModel> basesNonIncluses = bases
                .Where(b => !b.Inclure)
                .OrderBy(b => b.OrdreTraitement)
                .ToList();

            List<string> lignes = new List<string>
            {
                "Résumé :",
                "- Nombre total de bases affichées : " + bases.Count,
                "- Nombre de bases cochées : " + basesIncluses.Count,
                "- Nombre de bases non sélectionnées : " + basesNonIncluses.Count,
                "",
                "Bases cochées :"
            };

            if (basesIncluses.Count == 0)
            {
                lignes.Add("- Aucune base cochée.");
            }
            else
            {
                foreach (BaseCopieModel baseCopie in basesIncluses)
                {
                    lignes.Add(
                        $"- Ordre {baseCopie.OrdreTraitement} | Base : {baseCopie.NomBase} | Mode : {baseCopie.ModeCopie} | Statut : {baseCopie.Statut}"
                    );
                }
            }

            lignes.Add("");
            lignes.Add("Bases non sélectionnées :");

            if (basesNonIncluses.Count == 0)
            {
                lignes.Add("- Aucune base non sélectionnée.");
            }
            else
            {
                foreach (BaseCopieModel baseCopie in basesNonIncluses)
                {
                    lignes.Add(
                        $"- Ordre {baseCopie.OrdreTraitement} | Base : {baseCopie.NomBase} | Mode : {baseCopie.ModeCopie} | Statut : {baseCopie.Statut}"
                    );
                }
            }

            return string.Join(Environment.NewLine, lignes);
        }

        private void EnregistrerLogUtilisateur(string action, string details)
        {
            _ = Task.Run(async () =>
            {
                try
                {
                    await _authentificationController.AjouterLogAsync(action, details);
                }
                catch
                {
                    // L'échec du log utilisateur ne doit pas bloquer l'enregistrement métier.
                }
            });
        }
    }
}