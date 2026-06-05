using System.Drawing;
using System.Windows.Forms;
using FontAwesome.Sharp;

namespace SaimDataCopy.Styles
{
    /// <summary>
    /// Style utilisé uniquement pour la page Paramètres Logs.
    /// Le design de cette page reste séparé des autres pages.
    /// </summary>
    public static class LogFormStyle
    {
        private static readonly Color CouleurBordure = Color.FromArgb(215, 215, 215);
        private static readonly Color CouleurBleu = Color.FromArgb(30, 96, 190);
        private static readonly Color CouleurFondInfo = Color.FromArgb(232, 240, 255);

        public static void AppliquerTitre(Label label)
        {
            label.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            label.ForeColor = Color.Black;
            label.AutoSize = true;
        }

        public static void AppliquerLabel(Label label)
        {
            label.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
            label.ForeColor = Color.Black;
            label.AutoSize = true;
        }

        public static void AppliquerChampTexte(TextBox textBox)
        {
            textBox.Font = new Font("Segoe UI", 10F);
            textBox.BorderStyle = BorderStyle.FixedSingle;
            textBox.AutoSize = false;
            textBox.Height = 38;
            textBox.Padding = new Padding(6, 0, 6, 0);
        }

        public static void AppliquerChampNombre(NumericUpDown numeric)
        {
            numeric.Font = new Font("Segoe UI", 10F);
            numeric.BorderStyle = BorderStyle.None;
            numeric.AutoSize = false;
            numeric.Height = 28;
            numeric.TextAlign = HorizontalAlignment.Left;
        }

        public static void AppliquerPanelChampNombre(Panel panel)
        {
            panel.BackColor = Color.White;
            panel.BorderStyle = BorderStyle.FixedSingle;
            panel.Height = 38;
        }


        public static void AppliquerBoutonParcourir(IconButton bouton)
        {
            bouton.Font = new Font("Segoe UI", 10F);
            bouton.Height = 38;
            bouton.Width = 125;
            bouton.FlatStyle = FlatStyle.Flat;
            bouton.BackColor = Color.White;
            bouton.ForeColor = Color.Black;
            bouton.FlatAppearance.BorderColor = Color.FromArgb(215, 215, 215);
            bouton.FlatAppearance.BorderSize = 1;
            bouton.Cursor = Cursors.Hand;

            bouton.IconChar = FontAwesome.Sharp.IconChar.FolderOpen;
            bouton.IconSize = 20;
            bouton.IconColor = Color.FromArgb(40, 40, 40);
            bouton.TextImageRelation = TextImageRelation.ImageBeforeText;
            bouton.ImageAlign = ContentAlignment.MiddleLeft;
            bouton.TextAlign = ContentAlignment.MiddleCenter;
        }


        public static void AppliquerBlocInformation(Panel panel)
        {
            panel.BackColor = CouleurFondInfo;
            panel.Height = 48;
        }

        public static void AppliquerTexteInformation(Label label)
        {
            label.Font = new Font("Segoe UI", 10F);
            label.ForeColor = Color.Black;
            label.AutoSize = true;
        }

        public static void AppliquerIconeInformation(Label label)
        {
            label.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            label.ForeColor = Color.FromArgb(30, 96, 190);
            label.AutoSize = true;
            label.TextAlign = ContentAlignment.MiddleCenter;
        }
    }
}