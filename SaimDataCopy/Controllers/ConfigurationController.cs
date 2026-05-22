using SaimDataCopy.Models;
using SaimDataCopy.Services;
using SaimDataCopy.Views.Interfaces;
using System;

namespace SaimDataCopy.Controllers
{
    // Controller de la page Configuration.
    // Il fait le lien entre la View et le Service.
    public class ConfigurationController
    {
        private readonly IConfigurationView _view;
        private readonly ConfigurationService _configurationService;

        public ConfigurationController(IConfigurationView view)
        {
            // On garde une référence vers la View.
            _view = view;

            // On crée le Service qui contient la logique métier.
            _configurationService = new ConfigurationService();

            // Quand la View demande un enregistrement,
            // le Controller lance la méthode EnregistrerConfiguration.
            _view.EnregistrerConfigurationDemande += EnregistrerConfiguration;
        }

        private void EnregistrerConfiguration(object? sender, EventArgs e)
        {
            // La View récupère les données de l'interface
            // et les transforme en Model.
            ConfigurationModel configuration = _view.RecupererConfiguration();

            // Le Service vérifie si les données sont correctes.
            bool estValide = _configurationService.ValiderConfiguration(configuration, out string message);

            if (estValide)
            {
                // Si tout est correct, on affiche un message de succès.
                _view.AfficherMessageSucces(message);
            }
            else
            {
                // Sinon, on affiche le message d'erreur.
                _view.AfficherMessageErreur(message);
            }
        }
    }
}