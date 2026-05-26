using SaimDataCopy.Services.Configuration;
using SaimDataCopy.Views.Configuration;
using System;

namespace SaimDataCopy.Controllers.Configuration
{
    // Controller de la page Configuration.
    // Il coordonne la View et le Service.
    public class ConfigurationController
    {
        private readonly ConfigurationView _view;
        private readonly IConfigurationService _configurationService;

        // Constructeur simple.
        // Il reçoit la View et crée le Service.
        public ConfigurationController(ConfigurationView view)
        {
            _view = view;
            _configurationService = new ConfigurationService();

            // Quand la View demande un enregistrement,
            // le Controller lance la méthode EnregistrerConfiguration.
            _view.EnregistrerConfigurationDemande += EnregistrerConfiguration;
        }

        // Constructeur utile si plus tard on veut injecter un autre service.
        // Exemple : pour les tests ou pour changer l'implémentation du service.
        public ConfigurationController(ConfigurationView view, IConfigurationService configurationService)
        {
            _view = view;
            _configurationService = configurationService;

            _view.EnregistrerConfigurationDemande += EnregistrerConfiguration;
        }

        private void EnregistrerConfiguration(object? sender, EventArgs e)
        {
            // La View récupère les valeurs saisies
            // et les transforme en Model.
            var configuration = _view.RecupererConfiguration();

            // Le Controller demande au Service d'enregistrer.
            // Le Service va valider puis appeler le DataProvider.
            bool estEnregistre = _configurationService.EnregistrerConfiguration(configuration, out string message);

            if (estEnregistre)
            {
                _view.AfficherMessageSucces(message);
            }
            else
            {
                _view.AfficherMessageErreur(message);
            }
        }
    }
}