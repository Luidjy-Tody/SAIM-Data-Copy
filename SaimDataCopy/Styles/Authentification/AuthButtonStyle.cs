using System.Drawing;
using System.Windows.Forms;

namespace SaimDataCopy.Styles.Authentification
{
    public static class AuthButtonStyle
    {
        public static void Appliquer(Button bouton)
        {
            bouton.Height = 48;
            bouton.FlatStyle = FlatStyle.Flat;
            bouton.FlatAppearance.BorderSize = 0;
            bouton.Cursor = Cursors.Hand;

            bouton.BackColor = AuthentificationFormStyle.BleuPrincipal;
            bouton.ForeColor = Color.White;

            bouton.Font = AuthentificationFormStyle.TexteBouton();

            bouton.MouseEnter += Bouton_MouseEnter;
            bouton.MouseLeave += Bouton_MouseLeave;
            bouton.MouseDown += Bouton_MouseDown;
            bouton.MouseUp += Bouton_MouseUp;
        }

        private static void Bouton_MouseEnter(object? sender, EventArgs e)
        {
            if (sender is Button bouton)
            {
                bouton.BackColor = AuthentificationFormStyle.BleuSurvol;
            }
        }

        private static void Bouton_MouseLeave(object? sender, EventArgs e)
        {
            if (sender is Button bouton)
            {
                bouton.BackColor = AuthentificationFormStyle.BleuPrincipal;
            }
        }

        private static void Bouton_MouseDown(object? sender, MouseEventArgs e)
        {
            if (sender is Button bouton)
            {
                bouton.BackColor = AuthentificationFormStyle.BleuFonce;
            }
        }

        private static void Bouton_MouseUp(object? sender, MouseEventArgs e)
        {
            if (sender is Button bouton)
            {
                bouton.BackColor = AuthentificationFormStyle.BleuSurvol;
            }
        }
    }
}