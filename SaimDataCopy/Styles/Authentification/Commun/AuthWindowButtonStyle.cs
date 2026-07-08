using FontAwesome.Sharp;
using System.Drawing;
using System.Windows.Forms;

namespace SaimDataCopy.Styles.Authentification.Commun
{
    public static class AuthWindowButtonStyle
    {
        public static void Appliquer(IconButton bouton)
        {
            bouton.Size = new Size(55, 44);
            bouton.IconSize = 16;
            bouton.IconColor = Color.White;
            bouton.FlatStyle = FlatStyle.Flat;
            bouton.BackColor = Color.Transparent;
            bouton.Cursor = Cursors.Hand;
            bouton.Text = string.Empty;

            bouton.FlatAppearance.BorderSize = 0;
            bouton.FlatAppearance.MouseOverBackColor = Color.FromArgb(40, 255, 255, 255);
            bouton.FlatAppearance.MouseDownBackColor = Color.FromArgb(70, 255, 255, 255);
        }

        public static void AppliquerBoutonFermer(IconButton bouton)
        {
            Appliquer(bouton);

            bouton.MouseEnter += (sender, e) =>
            {
                bouton.BackColor = Color.FromArgb(232, 17, 35);
                bouton.IconColor = Color.White;
            };

            bouton.MouseLeave += (sender, e) =>
            {
                bouton.BackColor = Color.Transparent;
                bouton.IconColor = Color.White;
            };
        }
    }
}