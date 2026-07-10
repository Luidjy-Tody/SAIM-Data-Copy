using SaimDataCopy.Controllers.Authentification;
using SaimDataCopy.Models.Configuration;
using SaimDataCopy.Services.BasesCopier;
using SaimDataCopy.Services.Configuration;
using SaimDataCopy.Views.Configuration;
using SaimDataCopy.Views.Commun;

namespace SaimDataCopy.Controllers.Configuration
{
    // Controller de la page Configuration.
    // Il coordonne la View et les Services.
    public class ConfigurationController
    {
        private readonly ConfigurationView _view;
        private readonly IConfigurationService _configurationService;
        private readonly IBasesCopierService _basesCopierService;
        private readonly AuthentificationController _authentificationController;

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
            _authentificationController = new AuthentificationController();

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
            _authentificationController = new AuthentificationController();

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
            EnregistrerDepuisMainForm();
        }

        /// <summary>
        /// Enregistre la configuration et retourne true si tout s'est bien passé.
        /// Cette méthode sera utilisée par MainForm avant de changer de page.
        /// </summary>
        public bool EnregistrerDepuisMainForm()
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
                return false;
            }

            try
            {
                // La configuration globale est le paramètre maître.
                // Après l'enregistrement, on recrée un nouveau service BasesCopierService
                // pour qu'il relise la configuration actuelle et choisisse le bon DataProvider
                // SQL Server ou MySQL.
                IBasesCopierService basesCopierServiceActuel = new BasesCopierService();

                // On applique le mode global à toutes les bases à copier.
                // Exemple : si Configuration = "Mise à jour",
                // toutes les bases sauvegardées prennent "Mise à jour".
                basesCopierServiceActuel.AppliquerModeCopieGlobal(configuration.ModeCopie);

                // On prévient MainForm pour recréer les pages qui dépendent de la configuration.
                ModeCopieGlobalModifie?.Invoke(configuration.ModeCopie);

                // Si la View gère les modifications non enregistrées,
                // on indique que l'état actuel est maintenant sauvegardé.
                if (_view is IPageEnregistrable pageEnregistrable)
                {
                    pageEnregistrable.MarquerCommeEnregistre();
                }

                EnregistrerLogUtilisateur(
                    "Enregistrement Configuration",
                    ConstruireDetailsConfiguration(configuration) +
                    Environment.NewLine +
                    "Statut : configuration enregistrée et mode global appliqué aux bases à copier."
                );

                _view.AfficherMessageSucces(
                    message + Environment.NewLine +
                    "Le mode de copie global a été appliqué aux bases à copier."
                );

                return true;
            }
            catch (Exception ex)
            {
                EnregistrerLogUtilisateur(
                    "Enregistrement Configuration",
                    ConstruireDetailsConfiguration(configuration) +
                    Environment.NewLine +
                    "Statut : configuration enregistrée, mais synchronisation avec les bases échouée." +
                    Environment.NewLine +
                    "Erreur : " + ex.Message
                );

                _view.AfficherMessageErreur(
                    "La configuration a été enregistrée, mais la synchronisation avec les bases a échoué : "
                    + ex.Message
                );

                return false;
            }
        }

        private string ConstruireDetailsConfiguration(ConfigurationModel configuration)
        {
            return
                "Serveur source :" + Environment.NewLine +
                "- Type : " + AfficherValeur(configuration.ServeurSource.TypeServeur) + Environment.NewLine +
                "- Nom serveur : " + AfficherValeur(configuration.ServeurSource.NomServeur) + Environment.NewLine +
                "- Port : " + AfficherPort(configuration.ServeurSource.Port) + Environment.NewLine +
                "- Identifiant : " + AfficherValeur(configuration.ServeurSource.Identifiant) + Environment.NewLine +
                "- Mot de passe : " + AfficherMotDePasse(configuration.ServeurSource.MotDePasse) + Environment.NewLine +
                "- Chaîne de connexion : " + AfficherChaineConnexion(configuration.ServeurSource.ChaineConnexion) + Environment.NewLine +
                Environment.NewLine +
                "Serveur cible :" + Environment.NewLine +
                "- Type : " + AfficherValeur(configuration.ServeurCible.TypeServeur) + Environment.NewLine +
                "- Nom serveur : " + AfficherValeur(configuration.ServeurCible.NomServeur) + Environment.NewLine +
                "- Port : " + AfficherPort(configuration.ServeurCible.Port) + Environment.NewLine +
                "- Identifiant : " + AfficherValeur(configuration.ServeurCible.Identifiant) + Environment.NewLine +
                "- Mot de passe : " + AfficherMotDePasse(configuration.ServeurCible.MotDePasse) + Environment.NewLine +
                "- Chaîne de connexion : " + AfficherChaineConnexion(configuration.ServeurCible.ChaineConnexion) + Environment.NewLine +
                Environment.NewLine +
                "Paramètres copie :" + Environment.NewLine +
                "- Mode copie global : " + AfficherValeur(configuration.ModeCopie) + Environment.NewLine +
                "- Comportement erreur : " + AfficherValeur(configuration.ComportementErreur) + Environment.NewLine +
                "- Tentatives reprise : " + configuration.TentativesReprise;
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

        private string AfficherValeur(string valeur)
        {
            return string.IsNullOrWhiteSpace(valeur) ? "Non renseigné" : valeur;
        }

        private string AfficherPort(int port)
        {
            return port > 0 ? port.ToString() : "Non renseigné";
        }

        private string AfficherMotDePasse(string motDePasse)
        {
            return string.IsNullOrWhiteSpace(motDePasse)
                ? "Non renseigné"
                : "Renseigné (masqué)";
        }

        private string AfficherChaineConnexion(string chaineConnexion)
        {
            if (string.IsNullOrWhiteSpace(chaineConnexion))
            {
                return "Non renseignée";
            }

            if (ContientInformationSensible(chaineConnexion))
            {
                return "Renseignée (masquée car elle peut contenir un mot de passe)";
            }

            return chaineConnexion;
        }

        private bool ContientInformationSensible(string valeur)
        {
            return valeur.Contains("password", StringComparison.OrdinalIgnoreCase) ||
                   valeur.Contains("pwd", StringComparison.OrdinalIgnoreCase) ||
                   valeur.Contains("motdepasse", StringComparison.OrdinalIgnoreCase) ||
                   valeur.Contains("mot_de_passe", StringComparison.OrdinalIgnoreCase);
        }


    }
}