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
            AfficherPage(new PageSimpleControl("Configuration"));

        }


        // Crée tous les boutons du menu gauche.
        // Le design des boutons se trouve dans Helpers/MenuButtonStyle.cs
        private void CreerMenu()
        {
            AjouterBoutonMenu("Historique", IconChar.Clock, "Historique");
            AjouterBoutonMenu("Exécution", IconChar.Play, "Exécution"); 
            AjouterBoutonMenu("Paramètres Logs", IconChar.FileAlt, "Paramètres Logs");
            AjouterBoutonMenu("Paramètres Email", IconChar.Envelope, "Paramètres Email");
            AjouterBoutonMenu("Bases à copier", IconChar.Database, "Bases à copier");
            AjouterBoutonMenu("Configuration", IconChar.Cog, "Configuration");

        }
        // Crée un bouton du menu.
        // texte = texte affiché sur le bouton.
        // icone = icône FontAwesome affichée à gauche.
        // titrePage = titre de la page à afficher dans panelMain.

        private void AjouterBoutonMenu(string texte, IconChar icone, string titrePage)
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
                AfficherPage(new PageSimpleControl(titrePage));
            };

            // On ajoute le bouton dans le menu gauche.
        
            panelMenu.Controls.Add(bouton);
        }
        // Affiche une page dans panelMain.

        // Le menu gauche et le bottom ne sont pas touchés.
        
        private void AfficherPage(UserControl Page)
        {
            // On supprime seulement le contenu central.
            panelMain.Controls.Clear();

            // La page prend toute la place disponible dans panelMain.
            panelMain.Controls.Add(Page);
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
