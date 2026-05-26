using FontAwesome.Sharp;
using SaimDataCopy.Controllers.BasesCopier;
using SaimDataCopy.Controllers.Configuration;
using SaimDataCopy.Helpers;
using SaimDataCopy.Views.BasesCopier;
using SaimDataCopy.Views.Configuration;
using SaimDataCopy.Views.Commun;

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
            AjouterBoutonMenu("Historique", IconChar.Clock, () => new PageSimpleView("Historique"));
            AjouterBoutonMenu("Exécution", IconChar.Play, () => new PageSimpleView("Exécution"));
            AjouterBoutonMenu("Paramètres Logs", IconChar.FileAlt, () => new PageSimpleView("Paramètres Logs"));
            AjouterBoutonMenu("Paramètres Email", IconChar.Envelope, () => new PageSimpleView("Paramètres Email"));

            // Ici on appelle la vraie page Bases à copier en MVC.
            AjouterBoutonMenu("Bases à copier", IconChar.Database, () => CreerBasesCopierView());

            // Ici on appelle la vraie page Configuration en MVC.
            AjouterBoutonMenu("Configuration", IconChar.Cog, () => CreerConfigurationView());
        }

        // Crée un bouton du menu.
        private void AjouterBoutonMenu(string texte, IconChar icone, Func<UserControl> creerPage)
        {
            IconButton bouton = new IconButton();

            bouton.Text = texte;
            bouton.IconChar = icone;

            // Style du bouton depuis Helpers/MenuButtonStyle.cs
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
            }

            return configurationView;
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

            // Style du label dans Helpers.
            MenuLabelStyle.Appliquer(lblStatus);

            Button btnEnregistrerParametres = new Button();
            btnEnregistrerParametres.Text = "Enregistrer les paramètres";

            // Style du bouton dans Helpers.
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