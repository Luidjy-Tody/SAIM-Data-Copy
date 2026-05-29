using FontAwesome.Sharp;
using SaimDataCopy.Controllers.BasesCopier;
using SaimDataCopy.Controllers.Configuration;
using SaimDataCopy.Controllers.Email;
using SaimDataCopy.Controllers.Execution;
using SaimDataCopy.Controllers.Historique;
using SaimDataCopy.Controllers.Logs;
using SaimDataCopy.DataProviders.Email;
using SaimDataCopy.DataProviders.Execution;
using SaimDataCopy.DataProviders.Historique;
using SaimDataCopy.DataProviders.Logs;
using SaimDataCopy.Services.Email;
using SaimDataCopy.Services.Execution;
using SaimDataCopy.Services.Historique;
using SaimDataCopy.Services.Logs;
using SaimDataCopy.Styles;
using SaimDataCopy.Views.BasesCopier;
using SaimDataCopy.Views.Configuration;
using SaimDataCopy.Views.Email;
using SaimDataCopy.Views.Execution;
using SaimDataCopy.Views.Historique;
using SaimDataCopy.Views.Logs;

namespace SaimDataCopy.Views.Forms
{
    public partial class MainForm : Form
    {
        // Page actuellement affichée dans panelMain.
        private UserControl? pageActuelle;

        // Page Configuration gardée en mémoire.
        private ConfigurationView? configurationView;

        // Controller de la page Configuration.
        private ConfigurationController? configurationController;

        // Page Bases à copier gardée en mémoire.
        private BasesCopierView? basesCopierView;

        // Controller de la page Bases à copier.
        private BasesCopierController? basesCopierController;

        // Page Paramètres Email gardée en mémoire.
        private EmailView? emailView;

        // Controller de la page Paramètres Email.
        private EmailController? emailController;

        // Page Paramètres Logs gardée en mémoire.
        private LogsView? logsView;

        // Controller de la page Paramètres Logs.
        private LogsController? logsController;

        // Page Exécution gardée en mémoire.
        private ExecutionView? executionView;

        // Controller de la page Exécution.
        private ExecutionController? executionController;

        // Page Historique gardée en mémoire.
        private HistoriqueView? historiqueView;

        // Controller de la page Historique.
        private HistoriqueController? historiqueController;

        public MainForm()
        {
            InitializeComponent();

            CreerMenu();
            CreerBarreBas();

            // Au démarrage, on affiche directement la vraie page Configuration.
            AfficherPage(CreerConfigurationView());
        }

        // Crée tous les boutons du menu gauche.
        private void CreerMenu()
        {
            AjouterBoutonMenu("Historique", IconChar.Clock, () => CreerHistoriqueView());
            AjouterBoutonMenu("Exécution", IconChar.Play, () => CreerExecutionView());
            AjouterBoutonMenu("Paramètres Logs", IconChar.FileAlt, () => CreerLogsView());
            AjouterBoutonMenu("Paramètres Email", IconChar.Envelope, () => CreerEmailView());
            AjouterBoutonMenu("Bases à copier", IconChar.Database, () => CreerBasesCopierView());
            AjouterBoutonMenu("Configuration", IconChar.Cog, () => CreerConfigurationView());
        }

        // Crée un bouton du menu.
        private void AjouterBoutonMenu(string texte, IconChar icone, Func<UserControl> creerPage)
        {
            IconButton bouton = new IconButton();

            bouton.Text = texte;
            bouton.IconChar = icone;

            // Style du bouton depuis Styles/MenuButtonStyle.cs.
            MenuButtonStyle.Appliquer(bouton);

            bouton.Click += (sender, e) =>
            {
                AfficherPage(creerPage());
            };

            panelMenu.Controls.Add(bouton);
        }

        // Crée la View Configuration et son Controller une seule fois.
        private UserControl CreerConfigurationView()
        {
            if (configurationView == null)
            {
                configurationView = new ConfigurationView();

                // Le Controller reçoit la View Configuration.
                configurationController = new ConfigurationController(configurationView);

                // Quand Configuration applique un mode global,
                // MainForm peut rafraîchir Bases à copier si cette page existe déjà.
                configurationController.ModeCopieGlobalModifie += ConfigurationController_ModeCopieGlobalModifie;
            }

            return configurationView;
        }

        // Quand le mode global change dans Configuration,
        // on met à jour la page Bases à copier si elle a déjà été créée.
        private void ConfigurationController_ModeCopieGlobalModifie(string modeCopieGlobal)
        {
            if (basesCopierController == null)
            {
                return;
            }

            basesCopierController.AppliquerModeCopieGlobal(modeCopieGlobal);
        }

        // Crée la View Bases à copier et son Controller une seule fois.
        private UserControl CreerBasesCopierView()
        {
            if (basesCopierView == null)
            {
                basesCopierView = new BasesCopierView();

                // Le Controller reçoit la View Bases à copier.
                basesCopierController = new BasesCopierController(basesCopierView);
            }

            return basesCopierView;
        }

        // Crée la View Paramètres Email et son Controller une seule fois.
        private UserControl CreerEmailView()
        {
            if (emailView == null)
            {
                emailView = new EmailView();

                // Le DataProvider s'occupe de charger et sauvegarder les données.
                EmailDataProvider emailDataProvider = new EmailDataProvider();

                // Le Service contient la logique métier.
                EmailService emailService = new EmailService(emailDataProvider);

                // Le Controller reçoit la View et le Service.
                emailController = new EmailController(emailView, emailService);
            }

            return emailView;
        }

        // Crée la View Paramètres Logs et son Controller une seule fois.
        private UserControl CreerLogsView()
        {
            if (logsView == null)
            {
                logsView = new LogsView();

                // Le DataProvider s'occupe de charger et sauvegarder les données.
                LogsDataProvider logsDataProvider = new LogsDataProvider();

                // Le Service contient la logique métier et les validations.
                LogsService logsService = new LogsService(logsDataProvider);

                // Le Controller reçoit la View et le Service.
                logsController = new LogsController(logsView, logsService);

                // On charge les valeurs enregistrées ou les valeurs par défaut.
                logsController.ChargerPage();
            }

            return logsView;
        }

        // Crée la View Exécution et son Controller une seule fois.
        private UserControl CreerExecutionView()
        {
            if (executionView == null)
            {
                executionView = new ExecutionView();

                // Le DataProvider charge les bases sélectionnées
                // et sauvegarde la dernière exécution.
                ExecutionDataProvider executionDataProvider = new ExecutionDataProvider();

                // Le Service contient la logique métier de l'exécution.
                ExecutionService executionService = new ExecutionService(executionDataProvider);

                // Le Controller reçoit la View et le Service.
                executionController = new ExecutionController(executionView, executionService);
            }

            return executionView;
        }

        // Crée la View Historique et son Controller une seule fois.
        private UserControl CreerHistoriqueView()
        {
            if (historiqueView == null)
            {
                historiqueView = new HistoriqueView();

                // Le DataProvider récupère les exécutions enregistrées.
                HistoriqueDataProvider historiqueDataProvider = new HistoriqueDataProvider();

                // Le Service contient la logique métier de l'historique.
                HistoriqueService historiqueService = new HistoriqueService(historiqueDataProvider);

                // Le Controller reçoit la View et le Service.
                historiqueController = new HistoriqueController(historiqueView, historiqueService);
            }

            return historiqueView;
        }

        // Affiche une page dans panelMain.
        // Le menu gauche et le bottom ne sont pas supprimés.
        private void AfficherPage(UserControl page)
        {
            panelMain.Controls.Clear();

            page.Dock = DockStyle.Fill;
            panelMain.Controls.Add(page);

            // On garde en mémoire la page affichée.
            pageActuelle = page;
        }

        private void CreerBarreBas()
        {
            Label lblStatus = new Label();
            lblStatus.Text = "Prêt";

            // Style du label dans Styles.
            MenuLabelStyle.Appliquer(lblStatus);

            Button btnEnregistrerParametres = new Button();
            btnEnregistrerParametres.Text = "Enregistrer les paramètres";

            // Style du bouton dans Styles.
            MenuButtonStyle.Appliquer(btnEnregistrerParametres);

            // Quand on clique sur Enregistrer, on vérifie la page actuelle.
            btnEnregistrerParametres.Click += BtnEnregistrerParametres_Click;

            panelBottom.Controls.Add(btnEnregistrerParametres);
            panelBottom.Controls.Add(lblStatus);
        }

        private void BtnEnregistrerParametres_Click(object? sender, EventArgs e)
        {
            switch (pageActuelle)
            {
                // Si la page actuelle est Configuration,
                // on demande à la View de déclencher l'enregistrement.
                case ConfigurationView configurationView:
                    configurationView.DemanderEnregistrement();
                    break;

                // Si la page actuelle est Bases à copier,
                // on appelle directement le Controller de cette page.
                case BasesCopierView:
                    basesCopierController?.Enregistrer();
                    break;

                // Si la page actuelle est Paramètres Email,
                // on demande à la View de déclencher l'enregistrement.
                case EmailView emailView:
                    emailView.DemanderEnregistrement();
                    break;

                // Si la page actuelle est Paramètres Logs,
                // on appelle le Controller pour enregistrer.
                case LogsView:
                    logsController?.DemanderEnregistrement();
                    break;

                // La page Exécution n'a pas de paramètres à enregistrer.
                case ExecutionView:
                    MessageBox.Show(
                        "La page Exécution ne possède pas de paramètres à enregistrer. Utilisez le bouton Lancer la copie.",
                        "Information",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                    break;

                // La page Historique n'a pas de paramètres à enregistrer.
                case HistoriqueView:
                    MessageBox.Show(
                        "La page Historique ne possède pas de paramètres à enregistrer.",
                        "Information",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                    break;

                // Pour les pages temporaires qui n'ont pas encore d'enregistrement.
                default:
                    MessageBox.Show(
                        "Cette page n'a pas encore de paramètres à enregistrer.",
                        "Information",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                    break;
            }
        }
    }
}