using SaimDataCopy.Models.Logs;
using SaimDataCopy.Services.Logs;
using SaimDataCopy.Views.Logs;
using SaimDataCopy.Views.Commun;

namespace SaimDataCopy.Controllers.Logs
{
    /// <summary>
    /// Controller de la page Paramètres Logs.
    /// Il coordonne les actions entre la View et le Service.
    /// </summary>
    public class LogsController
    {
        private readonly ILogsView _view;
        private readonly ILogsService _service;

        public LogsController(ILogsView view, ILogsService service)
        {
            _view = view;
            _service = service;

            // Quand l'utilisateur clique sur Parcourir,
            // le Controller réagit à l'événement.

        }

        /// <summary>
        /// Charge les paramètres logs au démarrage de la page.
        /// </summary>
        /// 
        public void ChargerPage()
        {
            try
            {
                LogConfigModel configuration = _service.ChargerConfiguration();
                _view.AfficherConfiguration(configuration);
            }
            catch (Exception ex)
            {
                _view.AfficherMessage(
                    "Erreur lors du chargement des paramètres logs : " + ex.Message,
                    false
            );

            }
        }

        /// <summary>
        /// Méthode appelée par MainForm quand l'utilisateur clique sur
        /// le bouton Enregistrer les paramètres.
        /// </summary>
        /// 

        public void DemanderEnregistrement()
        {
            EnregistrerDepuisMainForm();
        }

        /// <summary>
        /// Enregistre les paramètres logs et retourne true si l'enregistrement réussit.
        /// Cette méthode sera utilisée par MainForm avant de changer de page.
        /// </summary>
        public bool EnregistrerDepuisMainForm()
        {
            try
            {
                LogConfigModel configuration = _view.RecupererConfiguration();

                _service.EnregistrerConfiguration(configuration);

                // Si la View gère les modifications non enregistrées,
                // on indique que les paramètres actuels sont maintenant sauvegardés.
                if (_view is IPageEnregistrable pageEnregistrable)
                {
                    pageEnregistrable.MarquerCommeEnregistre();
                }

                _view.AfficherMessage(
                    "Paramètres logs enregistrés avec succès.",
                    true
                );

                return true;
            }
            catch (Exception ex)
            {
                _view.AfficherMessage(
                    "Erreur de l'enregistrement : " + ex.Message,
                    false
                );

                return false;
            }
        }

    }
}
