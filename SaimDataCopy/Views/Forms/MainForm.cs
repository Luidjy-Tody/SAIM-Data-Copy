using FontAwesome.Sharp;
using SaimDataCopy.Controllers;
using SaimDataCopy.Helpers;
using SaimDataCopy.Views.Interfaces;
using SaimDataCopy.Views.UserControls;
using System.Drawing;
using System.Windows.Forms;

namespace SaimDataCopy.Views.Forms
{
    public partial class MainForm : Form
    {
        // Page actuellement affichée dans panelMain.
        private UserControl? pageActuelle;

        // Controller de la page Configuration.
        // On le garde en mémoire pour respecter MVC.
        private ConfigurationController? configurationController;

        public MainForm()
        {
            InitializeComponent();

            CreerMenu();
            CreerBarreBas();

            // Au démarrage, on affiche directement la vraie View MVC de configuration.
            AfficherPage(CreerConfigurationView());
        }

        // Crée tous les boutons du menu gauche.
        // Le design des boutons se trouve dans Helpers/MenuButtonStyle.cs
        private void CreerMenu()
        {
            AjouterBoutonMenu("Historique", IconChar.Clock, () => new PageSimpleControl(""));
            AjouterBoutonMenu("Exécution", IconChar.Play, () => new PageSimpleControl(""));
            AjouterBoutonMenu("Paramètres Logs", IconChar.FileAlt, () => new PageSimpleControl(""));
            AjouterBoutonMenu("Paramètres Email", IconChar.Envelope, () => new PageSimpleControl(""));
            AjouterBoutonMenu("Bases à copier", IconChar.Database, () => new PageSimpleControl(""));

            // Ici on appelle la vraie page Configuration en MVC.
            AjouterBoutonMenu("Configuration", IconChar.Cog, () => CreerConfigurationView());
        }

        // Crée un bouton du menu.
        // creerPage permet de créer la page à afficher quand on clique.
        private void AjouterBoutonMenu(string texte, IconChar icone, Func<UserControl> creerPage)
        {
            IconButton bouton = new IconButton();

            bouton.Text = texte;
            bouton.IconChar = icone;

            // Style du bouton depuis Helpers.
            MenuButtonStyle.Appliquer(bouton);

            bouton.Click += (sender, e) =>
            {
                // Quand on clique, on affiche la page demandée dans panelMain.
                AfficherPage(creerPage());
            };

            panelMenu.Controls.Add(bouton);
        }

        // Crée la View Configuration et son Controller.
        // C'est ici qu'on relie View + Controller.
        private UserControl CreerConfigurationView()
        {
            ConfigurationView configurationView = new ConfigurationView();

            // Le Controller reçoit la View.
            // Il écoutera les actions de cette View.
            configurationController = new ConfigurationController(configurationView);

            return configurationView;
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

            btnEnregistrerParametres.Click += (sender, e) =>
            {
                // Si la page actuelle est une View de configuration,
                // on demande l'enregistrement au Controller via l'interface.
                if (pageActuelle is IConfigurationView configurationView)
                {
                    configurationView.DemanderEnregistrement();
                }
                else
                {
                    MessageBox.Show(
                        "Cette page n'a pas encore de paramètres à enregistrer.",
                        "Information",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                }
            };

            panelBottom.Controls.Add(btnEnregistrerParametres);
            panelBottom.Controls.Add(lblStatus);
        }
    }
}