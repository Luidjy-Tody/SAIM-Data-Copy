using System.Drawing;
using System.Windows.Forms;

namespace SaimDataCopy.Styles.Authentification
{
    public static class AuthTextBoxStyle
    {
        public static void Appliquer(TextBox textBox)
        {
            textBox.Height = 36;
            textBox.Font = AuthentificationFormStyle.TexteChamp();
            textBox.ForeColor = AuthentificationFormStyle.TextePrincipal;
            textBox.BackColor = AuthentificationFormStyle.FondChamp;
            textBox.BorderStyle = BorderStyle.FixedSingle;

            textBox.GotFocus += TextBox_GotFocus;
            textBox.LostFocus += TextBox_LostFocus;
        }

        private static void TextBox_GotFocus(object? sender, EventArgs e)
        {
            if (sender is TextBox textBox)
            {
                textBox.BackColor = Color.White;
            }
        }

        private static void TextBox_LostFocus(object? sender, EventArgs e)
        {
            if (sender is TextBox textBox)
            {
                textBox.BackColor = AuthentificationFormStyle.FondChamp;
            }
        }
    }
}