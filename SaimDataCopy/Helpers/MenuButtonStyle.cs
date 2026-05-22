using FontAwesome.Sharp;
using System.Drawing;
using System.Windows.Forms;

namespace SaimDataCopy.Helpers
{
    // Classe séparée pour gérer le style des boutons du menu gauche.
    public static class MenuButtonStyle
    {
        // Couleurs principales utilisées dans le menu.
        private static readonly Color BleuPrincipal = Color.FromArgb(30, 96, 190);
        private static readonly Color GrisMenu = Color.FromArgb(245, 245, 245);
        private static readonly Color TexteNormal = Color.FromArgb(30, 30, 30);
        private static readonly Color IconeNormale = Color.FromArgb(80, 80, 80);

        // Applique le même style à chaque bouton du menu gauche.
        public static void Appliquer(IconButton bouton)
        {
            // Position du bouton dans le menu.
            bouton.Dock = DockStyle.Top;
            bouton.Height = 50;

            // Texte et icône.
            bouton.TextAlign = ContentAlignment.MiddleLeft;
            bouton.ImageAlign = ContentAlignment.MiddleLeft;
            bouton.TextImageRelation = TextImageRelation.ImageBeforeText;

            // Style de l'icône FontAwesome.
            bouton.IconSize = 20;
            bouton.IconColor = IconeNormale;

            // Espacement à gauche.
            bouton.Padding = new Padding(20, 0, 0, 0);

            // Style général.
            bouton.FlatStyle = FlatStyle.Flat;
            bouton.FlatAppearance.BorderSize = 0;
            bouton.BackColor = GrisMenu;
            bouton.ForeColor = TexteNormal;
            bouton.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
        }

        // Style du bouton "Enregistrer les paramètres" dans la barre du bas.
        public static void Appliquer(Button btnEnregistrerParametres)
        {
            btnEnregistrerParametres.Dock = DockStyle.Right;
            btnEnregistrerParametres.Width = 220;
            btnEnregistrerParametres.Height = 35;

            btnEnregistrerParametres.BackColor = BleuPrincipal;
            btnEnregistrerParametres.ForeColor = Color.White;

            btnEnregistrerParametres.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnEnregistrerParametres.FlatStyle = FlatStyle.Flat;
            btnEnregistrerParametres.FlatAppearance.BorderSize = 0;
        }
    }

    // Classe qui contient le design du label "Prêt" dans la barre du bas.
    public static class MenuLabelStyle
    {
        private static readonly Color TexteNormal = Color.FromArgb(30, 30, 30);

        public static void Appliquer(Label lblStatus)
        {
            lblStatus.Dock = DockStyle.Left;
            lblStatus.Width = 120;

            lblStatus.TextAlign = ContentAlignment.MiddleLeft;
            lblStatus.Padding = new Padding(15, 0, 0, 0);

            lblStatus.ForeColor = TexteNormal;
            lblStatus.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
        }

    }

}