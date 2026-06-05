using SaimDataCopy.Models.Logs;
using SaimDataCopy.Services.Logs;
using SaimDataCopy.Views.Logs;

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
            try
            {
                LogConfigModel configuration = _view.RecupererConfiguration();

                _service.EnregistrerConfiguration(configuration);

                _view.AfficherMessage(
                    "Paramètres logs enregistrés avec succès.", 
                    true
                 );
            }

            catch(Exception ex)
            {
                _view.AfficherMessage(
                    "Erreur de l'enregistrement : " + ex.Message,
                    false
                    
            );

            }

        }

    }
}
