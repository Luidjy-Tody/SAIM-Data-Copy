using FontAwesome.Sharp;
using SaimDataCopy.Helpers;

namespace SaimDataCopy.Forms
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            CreerMenu();
            CreerBarreBas();

        }


        // Crée tous les boutons du menu gauche.
        // Le design des boutons se trouve dans Helpers/MenuButtonStyle.cs
        private void CreerMenu()
        {
            AjouterBoutonMenu("Historique", IconChar.Clock);
            AjouterBoutonMenu("Exécution", IconChar.Play); 
            AjouterBoutonMenu("Paramètres Logs", IconChar.FileAlt);
            AjouterBoutonMenu("Paramètres Email", IconChar.Envelope);
            AjouterBoutonMenu("Bases à copier", IconChar.Database);
            AjouterBoutonMenu("Configuration", IconChar.Cog);

        }
        // Crée un bouton du menu.
        // texte = texte affiché sur le bouton.
        // icone = icône FontAwesome affichée à gauche.
        private void AjouterBoutonMenu(string texte, IconChar icone)
        {
            IconButton bouton= new IconButton();

            bouton.Text = texte;
            bouton.IconChar = icone;

            // On applique le style depuis le dossier Helpers/MenuButtonStyle.cs

            MenuButtonStyle.Appliquer(bouton);

            // On ajoute le bouton dans le menu gauche.

            panelMenu.Controls.Add(bouton);
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
