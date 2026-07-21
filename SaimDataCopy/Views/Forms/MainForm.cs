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
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using SaimDataCopy.Views.Commun;
using SaimDataCopy.Services.Authentification;
using SaimDataCopy.Controllers.Authentification;
using SaimDataCopy.Controllers.EspaceAdmin;
using SaimDataCopy.DataProviders.EspaceAdmin;
using SaimDataCopy.Services.EspaceAdmin;
using SaimDataCopy.Views.EspaceAdmin;
using SaimDataCopy.Models.Authentification;
using SaimDataCopy.Views.JournalActivite;
using SaimDataCopy.DataProviders.JournalActivite;
using SaimDataCopy.Services.JournalActivite;
using SaimDataCopy.Controllers.JournalActivite;




namespace SaimDataCopy.Views.Forms
{
    public partial class MainForm : Form
    {
        // Page actuellement affichée dans panelMain.
        private UserControl? pageActuelle;

        // Bouton du bas pour enregistrer les pages qui ont des paramètres.
        private Button? btnEnregistrerParametres;

        // Texte de statut affiché en bas de l'application.
        private Label? lblStatus;

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

        // Page Espace Admin gardée en mémoire.
        private EspaceAdminView? espaceAdminView;

        // Controller de la page Espace Admin.
        private EspaceAdminController? espaceAdminController;

        // Page Journal d'activité gardée en mémoire.
        private JournalActiviteView? journalActiviteView;

        // Controller de la page Journal d'activité.
        private JournalActiviteController? journalActiviteController;

        // Bouton actuellement actif dans le menu.
        private IconButton? boutonMenuActif;

        // Bouton Configuration utilisé au démarrage de l'application.
        private IconButton? boutonConfigurationMenu;

        // Gère l'utilisateur connecté et le verrouillage par inactivité.
        // On utilise une instance unique pour toute l'application.
        private readonly SessionUtilisateurService sessionUtilisateurService = SessionUtilisateurService.Instance;

        // Timer qui vérifie régulièrement si l'application doit se verrouiller.
        private readonly System.Windows.Forms.Timer timerInactivite = new System.Windows.Forms.Timer();

        private readonly AuthentificationController authentificationController = new AuthentificationController();

        private bool fermetureAutoriseeApresDeconnexion;



        // Permet de déplacer la fenêtre quand on clique sur la barre personnalisée.
        [DllImport("user32.dll")]
        private static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);

        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HTCAPTION = 0x2;

        public MainForm()
        {
            InitializeComponent();

            ConfigurerFenetrePrincipale();

            CreerBarreHaut();
            CreerMenu();
            CreerBarreBas();
            FormClosing += MainForm_FormClosing;
            ConfigurerVerrouillageInactivite();
            // Au démarrage, l'utilisateur doit d'abord s'identifier.
            AfficherPage(CreerConfigurationView());
        }


        

        // Configure la fenêtre principale pour un affichage adapté aux grands écrans.
        private void ConfigurerFenetrePrincipale()
        {
            // Important : enlève une éventuelle limite de taille définie dans le Designer.
            MaximumSize = Size.Empty;

            // Taille minimale pour éviter que l'application devienne trop petite.
            MinimumSize = new Size(1280, 720);

            // Taille utilisée si l'utilisateur quitte le mode maximisé.
            Size = new Size(1600, 900);

            StartPosition = FormStartPosition.CenterScreen;

            // Supprime la barre Windows native.
            FormBorderStyle = FormBorderStyle.None;

            MinimizeBox = true;
            MaximizeBox = true;

            // Les zones principales restent fixes.
            panelTop.Dock = DockStyle.Top;
            panelMenu.Dock = DockStyle.Left;
            panelBottom.Dock = DockStyle.Bottom;
            panelMain.Dock = DockStyle.Fill;

            // Ouvre l'application en grand écran Windows.
            WindowState = FormWindowState.Maximized;
        }

        // Crée une barre haute personnalisée plus propre.
        private void CreerBarreHaut()
        {
            panelTop.Controls.Clear();

            panelTop.Height = 72;
            panelTop.BackColor = Color.White;
            panelTop.Dock = DockStyle.Top;
            panelTop.Padding = new Padding(22, 0, 0, 0);

            IconPictureBox icone = new IconPictureBox();
            icone.IconChar = IconChar.Database;
            icone.IconColor = Color.FromArgb(30, 96, 190);
            icone.IconSize = 26;
            icone.Size = new Size(34, 34);
            icone.Location = new Point(22, 19);
            icone.BackColor = Color.White;

            Label lblTitre = new Label();
            lblTitre.Text = "Copie automatique des données entre serveurs — SAIM LTD";
            lblTitre.Font = new Font("Segoe UI", 13F, FontStyle.Bold);
            lblTitre.ForeColor = Color.FromArgb(25, 25, 25);
            lblTitre.AutoSize = true;
            lblTitre.Location = new Point(66, 14);

            Label lblSousTitre = new Label();
            lblSousTitre.Text = "Application de synchronisation MySQL et SQL Server";
            lblSousTitre.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            lblSousTitre.ForeColor = Color.FromArgb(100, 100, 100);
            lblSousTitre.AutoSize = true;
            lblSousTitre.Location = new Point(68, 42);

            IconButton btnReduire = new IconButton();
            btnReduire.IconChar = IconChar.Minus;
            btnReduire.IconSize = 16;
            btnReduire.IconColor = Color.FromArgb(80, 80, 80);
            btnReduire.Size = new Size(55, 44);
            btnReduire.FlatStyle = FlatStyle.Flat;
            btnReduire.FlatAppearance.BorderSize = 0;
            btnReduire.BackColor = Color.White;
            btnReduire.Cursor = Cursors.Hand;
            btnReduire.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnReduire.Click += (sender, e) =>
            {
                WindowState = FormWindowState.Minimized;
            };

            IconButton btnAgrandir = new IconButton();
            btnAgrandir.IconChar = IconChar.WindowMaximize;
            btnAgrandir.IconSize = 16;
            btnAgrandir.IconColor = Color.FromArgb(80, 80, 80);
            btnAgrandir.Size = new Size(55, 44);
            btnAgrandir.FlatStyle = FlatStyle.Flat;
            btnAgrandir.FlatAppearance.BorderSize = 0;
            btnAgrandir.BackColor = Color.White;
            btnAgrandir.Cursor = Cursors.Hand;
            btnAgrandir.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnAgrandir.Click += (sender, e) =>
            {
                if (WindowState == FormWindowState.Maximized)
                {
                    WindowState = FormWindowState.Normal;
                }
                else
                {
                    WindowState = FormWindowState.Maximized;
                }
            };

            IconButton btnFermer = new IconButton();
            btnFermer.IconChar = IconChar.Xmark;
            btnFermer.IconSize = 18;
            btnFermer.IconColor = Color.FromArgb(80, 80, 80);
            btnFermer.Size = new Size(55, 44);
            btnFermer.FlatStyle = FlatStyle.Flat;
            btnFermer.FlatAppearance.BorderSize = 0;
            btnFermer.BackColor = Color.White;
            btnFermer.Cursor = Cursors.Hand;
            btnFermer.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnFermer.Click += (sender, e) =>
            {
                if (!VerifierFermetureApplication())
                {
                    return;
                }

                Close();
            };

            // Effet gris au survol du bouton Réduire.
            btnReduire.MouseEnter += (sender, e) =>
            {
                btnReduire.BackColor = Color.FromArgb(230, 230, 230);
            };

            btnReduire.MouseLeave += (sender, e) =>
            {
                btnReduire.BackColor = Color.White;
            };

            // Effet gris au survol du bouton Agrandir.
            btnAgrandir.MouseEnter += (sender, e) =>
            {
                btnAgrandir.BackColor = Color.FromArgb(230, 230, 230);
            };

            btnAgrandir.MouseLeave += (sender, e) =>
            {
                btnAgrandir.BackColor = Color.White;
            };

            // Effet rouge au survol du bouton Fermer.
            btnFermer.MouseEnter += (sender, e) =>
            {
                btnFermer.BackColor = Color.FromArgb(232, 17, 35);
                btnFermer.IconColor = Color.White;
            };

            btnFermer.MouseLeave += (sender, e) =>
            {
                btnFermer.BackColor = Color.White;
                btnFermer.IconColor = Color.FromArgb(80, 80, 80);
            };

            // Position des boutons dans le panelTop.
            btnFermer.Location = new Point(panelTop.Width - 60, 14);
            btnAgrandir.Location = new Point(panelTop.Width - 115, 14);
            btnReduire.Location = new Point(panelTop.Width - 170, 14);

            panelTop.Resize += (sender, e) =>
            {
                btnFermer.Location = new Point(panelTop.Width - 60, 14);
                btnAgrandir.Location = new Point(panelTop.Width - 115, 14);
                btnReduire.Location = new Point(panelTop.Width - 170, 14);
            };

            Panel ligneBas = new Panel();
            ligneBas.Dock = DockStyle.Bottom;
            ligneBas.Height = 1;
            ligneBas.BackColor = Color.FromArgb(220, 220, 220);

            panelTop.Controls.Add(icone);
            panelTop.Controls.Add(lblTitre);
            panelTop.Controls.Add(lblSousTitre);
            panelTop.Controls.Add(btnReduire);
            panelTop.Controls.Add(btnAgrandir);
            panelTop.Controls.Add(btnFermer);
            panelTop.Controls.Add(ligneBas);
            // Permet de déplacer la fenêtre avec la barre haute personnalisée.
            panelTop.MouseDown += PanelTop_MouseDown;
        }
        // Déplace la fenêtre quand l'utilisateur maintient la souris sur panelTop.
        private void PanelTop_MouseDown(object? sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
            {
                return;
            }

            ReleaseCapture();
            SendMessage(Handle, WM_NCLBUTTONDOWN, HTCAPTION, 0);
        }

        private bool UtilisateurConnecteEstAdministrateurActif()
        {
            UtilisateurModel? utilisateurConnecte =
                sessionUtilisateurService.UtilisateurConnecte;

            return utilisateurConnecte != null &&
                   utilisateurConnecte.EstActif &&
                   utilisateurConnecte.Statut.Equals(
                       "Admin",
                       StringComparison.OrdinalIgnoreCase
                   );
        }

        // Crée tous les boutons du menu gauche.
        private void CreerMenu()
        {
            panelMenu.Controls.Clear();

            bool estAdministrateur = UtilisateurConnecteEstAdministrateurActif();

            if (estAdministrateur)
            {
                AjouterBoutonMenu(
                    "Espace Admin",
                    IconChar.UserShield,
                    () => CreerEspaceAdminView()
                );

                AjouterBoutonMenu(
                    "Journal d'activité",
                    IconChar.ListCheck,
                    () => CreerJournalActiviteView()
                );
            }

            if (boutonConfigurationMenu != null)
            {
                ActiverBoutonMenu(boutonConfigurationMenu);
            }

            AjouterBoutonMenu( "Historique",
                IconChar.Clock,
                () => CreerHistoriqueView()
            );

            AjouterBoutonMenu(
                "Exécution",
                IconChar.Play,
                () => CreerExecutionView()
            );

            AjouterBoutonMenu(
                "Paramètres Logs",
                IconChar.FileAlt,
                () => CreerLogsView()
            );

            AjouterBoutonMenu(
                "Paramètres Email",
                IconChar.Envelope,
                () => CreerEmailView()
            );

            AjouterBoutonMenu(
                "Bases à copier",
                IconChar.Database,
                () => CreerBasesCopierView()
            );

            boutonConfigurationMenu = AjouterBoutonMenu(
                "Configuration",
                IconChar.Cog,
                () => CreerConfigurationView()
            );

            
        }

        // Crée un bouton du menu.
        private IconButton AjouterBoutonMenu(string texte, IconChar icone, Func<UserControl> creerPage)
        {
            IconButton bouton = new IconButton();

            bouton.Text = texte;
            bouton.IconChar = icone;

            MenuButtonStyle.Appliquer(bouton);

            bouton.Click += (sender, e) =>
            {
                if (!VerifierAvantChangementPage())
                {
                    return;
                }

                ActiverBoutonMenu(bouton);
                AfficherPage(creerPage());
            };

            panelMenu.Controls.Add(bouton);

            return bouton;
        }

        private void ActiverBoutonMenu(IconButton boutonSelectionne)
        {
            if (boutonMenuActif != null)
            {
                MenuButtonStyle.AppliquerEtatNormal(boutonMenuActif);
            }

            boutonMenuActif = boutonSelectionne;

            MenuButtonStyle.AppliquerEtatActif(boutonMenuActif);
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

        private UserControl CreerEspaceAdminView()
        {
            if (espaceAdminView == null)
            {
                espaceAdminView = new EspaceAdminView();

                EspaceAdminDataProvider espaceAdminDataProvider = new EspaceAdminDataProvider();

                EspaceAdminService espaceAdminService = new EspaceAdminService(espaceAdminDataProvider);

                espaceAdminController = new EspaceAdminController(espaceAdminView, espaceAdminService);
            }

            return espaceAdminView;
        }


        private UserControl CreerJournalActiviteView()
        {
            if (!UtilisateurConnecteEstAdministrateurActif())
            {
                return CreerPageAccesRefuse();
            }

            if (journalActiviteView == null)
            {
                journalActiviteView = new JournalActiviteView();

                JournalActiviteDataProvider journalActiviteDataProvider =
                    new JournalActiviteDataProvider();

                JournalActiviteService journalActiviteService =
                    new JournalActiviteService(journalActiviteDataProvider);

                journalActiviteController =
                    new JournalActiviteController(
                        journalActiviteView,
                        journalActiviteService
                    );
            }

            return journalActiviteView;
        }

        private static UserControl CreerPageAccesRefuse()
        {
            UserControl pageAccesRefuse = new UserControl
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White
            };

            Label lblTitre = new Label
            {
                Text = "Accès refusé",
                AutoSize = true,
                Font = new Font("Segoe UI", 22F, FontStyle.Bold),
                ForeColor = Color.FromArgb(170, 45, 45),
                Location = new Point(35, 35)
            };

            Label lblMessage = new Label
            {
                Text = "Cette page est réservée aux administrateurs actifs.",
                AutoSize = true,
                Font = new Font("Segoe UI", 11F, FontStyle.Regular),
                ForeColor = Color.FromArgb(70, 70, 70),
                Location = new Point(38, 85)
            };

            pageAccesRefuse.Controls.Add(lblTitre);
            pageAccesRefuse.Controls.Add(lblMessage);

            return pageAccesRefuse;
        }
        // Quand le mode global change dans Configuration,
        // on met à jour la page Bases à copier si elle a déjà été créée.
        private void ConfigurationController_ModeCopieGlobalModifie(string modeCopieGlobal)
        {
            // Quand la configuration change, les pages qui dépendent du serveur
            // doivent être recréées.
            // Exemple : si on passe de MySQL à SQL Server sans redémarrer,
            // l'ancien controller gardait encore l'ancien DataProvider en mémoire.
            ReinitialiserPagesDependantesConfiguration();
        }

        private void ReinitialiserPagesDependantesConfiguration()
        {
            // La page Bases à copier dépend du type de serveur source.
            // On la supprime du cache pour forcer sa recréation au prochain affichage.
            if (basesCopierView != null && pageActuelle != basesCopierView)
            {
                basesCopierView.Dispose();
                basesCopierView = null;
                basesCopierController = null;
            }

            // La page Exécution dépend aussi du type de serveur et des bases sélectionnées.
            // Elle doit aussi être recréée après un changement SQL Server / MySQL.
            if (executionView != null && pageActuelle != executionView)
            {
                executionView.Dispose();
                executionView = null;
                executionController = null;
            }
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

                // Le Service choisit automatiquement
                // SQL Server ou MySQL via la Factory.
                ExecutionService executionService = new ExecutionService();

                // Le Controller reçoit la View et le Service.
                executionController = new ExecutionController( executionView, executionService );
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

        /// <summary>
        /// Vérifie si la page actuelle contient des modifications non enregistrées.
        /// Retourne true si on peut changer de page.
        /// Retourne false si on doit rester sur la page actuelle.
        /// </summary>
        private bool VerifierAvantChangementPage()
        {
            if (pageActuelle is not IPageEnregistrable pageEnregistrable)
            {
                return true;
            }

            if (!pageEnregistrable.ADesModificationsNonEnregistrees)
            {
                return true;
            }

            DialogResult choix = MessageBox.Show(
                "Vous avez des modifications non enregistrées. Voulez-vous les enregistrer avant de quitter cette page ?",
                "Modifications non enregistrées",
                MessageBoxButtons.YesNoCancel,
                MessageBoxIcon.Warning
            );

            switch (choix)
            {
                case DialogResult.Yes:
                    return EnregistrerPageActuelleAvantChangement();

                case DialogResult.No:
                    pageEnregistrable.AnnulerModificationsNonEnregistrees();
                    return true;

                case DialogResult.Cancel:
                    return false;

                default:
                    return false;
            }
        }

        /// <summary>
        /// Enregistre la page actuelle selon son type.
        /// Retourne true seulement si l'enregistrement réussit.
        /// </summary>
        private bool EnregistrerPageActuelleAvantChangement()
        {
            switch (pageActuelle)
            {
                case ConfigurationView:
                    return configurationController?.EnregistrerDepuisMainForm() == true;

                case BasesCopierView:
                    return basesCopierController?.EnregistrerDepuisMainForm() == true;

                case EmailView:
                    return emailController?.EnregistrerDepuisMainForm() == true;

                case LogsView:
                    return logsController?.EnregistrerDepuisMainForm() == true;

                default:
                    return true;
            }
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
            MettreAJourBoutonEnregistrer();
            MettreAJourStatutPage();
        }

        // Affiche le bouton Enregistrer seulement sur les pages qui ont des paramètres.
        private void MettreAJourBoutonEnregistrer()
        {
            if (btnEnregistrerParametres == null)
            {
                return;
            }

            btnEnregistrerParametres.Visible =
                pageActuelle is ConfigurationView ||
                pageActuelle is BasesCopierView ||
                pageActuelle is EmailView ||
                pageActuelle is LogsView;
        }

        // Met à jour le texte de statut selon la page affichée.
        private void MettreAJourStatutPage()
        {
            if (lblStatus == null)
            {
                return;
            }

            lblStatus.Text = pageActuelle switch
            {
                ConfigurationView => "Page actuelle : Configuration",
                BasesCopierView => "Page actuelle : Bases à copier",
                EmailView => "Page actuelle : Paramètres Email",
                LogsView => "Page actuelle : Paramètres Logs",
                ExecutionView => "Page actuelle : Exécution",
                HistoriqueView => "Page actuelle : Historique",
                EspaceAdminView => "Page actuelle : Espace Admin",
                JournalActiviteView => "Page actuelle : Journal d'activité",


                _ => "Prêt"
            };
        }
        private void CreerBarreBas()
        {
            lblStatus = new Label();
            lblStatus.Text = "Prêt";

            // Style du label dans Styles.
            MenuLabelStyle.Appliquer(lblStatus);

            btnEnregistrerParametres = new Button();
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
                case ConfigurationView:
                    configurationController?.EnregistrerDepuisMainForm();
                    break;

                // Si la page actuelle est Bases à copier,
                // on appelle directement le Controller de cette page.
                case BasesCopierView:
                    basesCopierController?.EnregistrerDepuisMainForm();
                    break;

                // Si la page actuelle est Paramètres Email,
                // on demande à la View de déclencher l'enregistrement.
                case EmailView:
                    emailController?.EnregistrerDepuisMainForm();
                    break;

                // Si la page actuelle est Paramètres Logs,
                // on appelle le Controller pour enregistrer.
                case LogsView:
                    logsController?.EnregistrerDepuisMainForm();
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


                case EspaceAdminView:
                    MessageBox.Show(
                        "Utilisez le formulaire de l'Espace Admin pour enregistrer un utilisateur.",
                        "Espace Admin",
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

        private async void MainForm_FormClosing(object? sender, FormClosingEventArgs e)
        {
            if (!VerifierFermetureApplication())
            {
                e.Cancel = true;
                return;
            }

            if (fermetureAutoriseeApresDeconnexion)
            {
                return;
            }

            e.Cancel = true;
            fermetureAutoriseeApresDeconnexion = true;

            try
            {
                timerInactivite.Stop();

                await authentificationController.DeconnecterEtJournaliserAsync("Déconnexion automatique après fermeture de la fenêtre principale.");
            }
            catch
            {
                authentificationController.Deconnecter();
            }

            Close();
        }

        private bool VerifierFermetureApplication()
        {
            if (executionController == null || !executionController.EstTraitementEnCours())
            {
                return true;
            }

            DialogResult choix = MessageBox.Show(
                "Une copie ou un test de connexion est actuellement en cours.\n\n" +
                "Voulez-vous annuler l'exécution en cours ?\n\n" +
                "L'application restera ouverte jusqu'à l'arrêt complet de l'exécution.",
                "Exécution en cours",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (choix == DialogResult.Yes)
            {
                executionController.AnnulerTraitementEnCoursDepuisMainForm();
            }

            return false;
        }

        

        private void ConfigurerVerrouillageInactivite()
        {
            KeyPreview = true;

            MouseMove += MainForm_ActiviteUtilisateur;
            MouseClick += MainForm_ActiviteUtilisateur;
            KeyDown += MainForm_ActiviteUtilisateur;

            timerInactivite.Interval = 10000; // Vérifie toutes les 10 secondes.
            timerInactivite.Tick += TimerInactivite_Tick;
            timerInactivite.Start();
        }

        private void MainForm_ActiviteUtilisateur(object? sender, EventArgs e)
        {
            if (sessionUtilisateurService.EstVerrouille)
            {
                return;
            }

            sessionUtilisateurService.ActualiserActivite();
        }

        private void TimerInactivite_Tick(object? sender, EventArgs e)
        {
            if (!sessionUtilisateurService.DoitVerrouiller())
            {
                return;
            }

            sessionUtilisateurService.Verrouiller();

            MessageBox.Show(
                "Application verrouillée après 5 minutes d'inactivité.",
                "Verrouillage",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }
    }
}