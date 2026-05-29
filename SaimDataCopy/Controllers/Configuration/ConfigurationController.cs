using SaimDataCopy.Models.Configuration;
using SaimDataCopy.Services.BasesCopier;
using SaimDataCopy.Services.Configuration;
using SaimDataCopy.Views.Configuration;

namespace SaimDataCopy.Controllers.Configuration
{
    // Controller de la page Configuration.
    // Il coordonne la View et les Services.
    public class ConfigurationController
    {
        private readonly ConfigurationView _view;
        private readonly IConfigurationService _configurationService;
        private readonly IBasesCopierService _basesCopierService;

        // Événement envoyé à MainForm pour prévenir que le mode global a changé.
        // MainForm pourra ensuite rafraîchir la page Bases à copier si elle existe déjà.
        public event Action<string>? ModeCopieGlobalModifie;

        // Constructeur simple.
        // Il reçoit la View et crée les Services.
        public ConfigurationController(ConfigurationView view)
        {
            _view = view;
            _configurationService = new ConfigurationService();
            _basesCopierService = new BasesCopierService();

            BrancherEvenements();
            ChargerConfiguration();
        }

        // Constructeur utile si plus tard on veut injecter d'autres services.
        // Exemple : pour les tests ou pour changer l'implémentation.
        public ConfigurationController(
            ConfigurationView view,
            IConfigurationService configurationService,
            IBasesCopierService basesCopierService)
        {
            _view = view;
            _configurationService = configurationService;
            _basesCopierService = basesCopierService;

            BrancherEvenements();
            ChargerConfiguration();
        }

        private void BrancherEvenements()
        {
            // Quand la View demande un enregistrement,
            // le Controller lance la méthode EnregistrerConfiguration.
            _view.EnregistrerConfigurationDemande += EnregistrerConfiguration;
        }

        private void ChargerConfiguration()
        {
            try
            {
                ConfigurationModel configuration =
                    _configurationService.ChargerConfiguration();

                _view.AfficherConfiguration(configuration);
            }
            catch (Exception ex)
            {
                _view.AfficherMessageErreur(
                    "Erreur pendant le chargement de la configuration : " + ex.Message
                );
            }
        }

        private void EnregistrerConfiguration(object? sender, EventArgs e)
        {
            // La View récupère les valeurs saisies
            // et les transforme en Model.
            ConfigurationModel configuration = _view.RecupererConfiguration();

            // Le Controller demande au Service d'enregistrer.
            // Le Service valide puis appelle le DataProvider.
            bool estEnregistre =
                _configurationService.EnregistrerConfiguration(configuration, out string message);

            if (!estEnregistre)
            {
                _view.AfficherMessageErreur(message);
                return;
            }

            try
            {
                // Quand Configuration est enregistrée,
                // le mode global est appliqué à toutes les bases.
                _basesCopierService.AppliquerModeCopieGlobal(configuration.ModeCopie);

                // On prévient MainForm pour rafraîchir Bases à copier si la page existe déjà.
                ModeCopieGlobalModifie?.Invoke(configuration.ModeCopie);

                _view.AfficherMessageSucces(
                    message + Environment.NewLine +
                    "Le mode de copie global a été appliqué aux bases à copier."
                );
            }
            catch (Exception ex)
            {
                _view.AfficherMessageErreur(
                    "La configuration a été enregistrée, mais la synchronisation avec les bases a échoué : "
                    + ex.Message
                );
            }
        }
    }
}