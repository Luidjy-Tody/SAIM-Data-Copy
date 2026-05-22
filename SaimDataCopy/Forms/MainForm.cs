using FontAwesome.Sharp;
using SaimDataCopy.Helpers;
using SaimDataCopy.UserControls;

namespace SaimDataCopy.Forms
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            CreerMenu();
            CreerBarreBas();

            // Page affichée au démarrage de l'application.
            AfficherPage(new ConfigurationControl());

        }


        // Crée tous les boutons du menu gauche.
        // Le design des boutons se trouve dans Helpers/MenuButtonStyle.cs
        private void CreerMenu()
        {
            AjouterBoutonMenu("Historique", IconChar.Clock, () => new PageSimpleControl("Historique"));
            AjouterBoutonMenu("Exécution", IconChar.Play, () => new PageSimpleControl("Exécution"));
            AjouterBoutonMenu("Paramètres Logs", IconChar.FileAlt, () => new PageSimpleControl("Paramètres Logs"));
            AjouterBoutonMenu("Paramètres Email", IconChar.Envelope, () => new PageSimpleControl("Paramètres Email"));
            AjouterBoutonMenu("Bases à copier", IconChar.Database, () => new PageSimpleControl("Bases à copier"));

            // Ici on appelle la vraie page ConfigurationControl.
            AjouterBoutonMenu("Configuration", IconChar.Cog, () => new ConfigurationControl());

        }
        // Crée un bouton du menu.
        // texte = texte affiché sur le bouton.
        // icone = icône FontAwesome affichée à gauche.
        // titrePage = titre de la page à afficher dans panelMain.

        private void AjouterBoutonMenu(string texte, IconChar icone, Func<UserControl> creerPage)
            /* On appelle AjouterBoutonMenu avec () => new PageSimpleControl(...);
            donc ce n’est pas un simple texte string;
            c’est une fonction qui crée une page;
            donc le paramètre doit être Func<UserControl>.*/
        {
            IconButton bouton= new IconButton();

            bouton.Text = texte;
            bouton.IconChar = icone;

            // On applique le style depuis le dossier Helpers/MenuButtonStyle.cs

            MenuButtonStyle.Appliquer(bouton);
            // Quand on clique sur le bouton,
            // on change seulement le contenu de panelMain.
            bouton.Click += (sender, e) =>
            {
                AfficherPage(creerPage());
            };

            // On ajoute le bouton dans le menu gauche.
        
            panelMenu.Controls.Add(bouton);
        }
        // Affiche une page dans panelMain.

        // Le menu gauche et le bottom ne sont pas touchés.
        
        private void AfficherPage(UserControl page)
        {
            // On supprime seulement le contenu central.
            panelMain.Controls.Clear();

            // Très important :
            // la page doit prendre toute la place dans panelMain.
            page.Dock = DockStyle.Fill;

            // La page prend toute la place disponible dans panelMain.
            panelMain.Controls.Add(page);
        }
        private void CreerBarreBas()
        {
            Label lblStatus = new Label();
            lblStatus.Text = "Prêt";

            // On applique le style lblStatus depuis le dossier Helpers/MenuButtonStyle.cs

            MenuLabelStyle.Appliquer(lblStatus);


            Button btnEnregistrerParametres = new Button();


            btnEnregistrerParametres.Text = "Enregistrer les paramètres";

            // On applique le style btnEnregistrerParametres depuis le dossier Helpers/MenuButtonStyle.cs


            MenuButtonStyle.Appliquer(btnEnregistrerParametres);

            // On ajoute le label dans le PanelBottom a droite.

            panelBottom.Controls.Add(btnEnregistrerParametres);

            // On ajoute le label dans le PanelBottom gauche.

            panelBottom.Controls.Add(lblStatus);

        }

    }
}
