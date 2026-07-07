using System.Drawing;
using System.Windows.Forms;

namespace SaimDataCopy.Styles.Authentification
{
    public static class AuthCardStyle
    {
        public static void Appliquer(Panel panel)
        {
            panel.BackColor = Color.White;

            panel.Padding = new Padding(35);
            panel.Margin = new Padding(0);

            panel.BorderStyle = BorderStyle.FixedSingle;
        }

        public static void AppliquerTitre(Label label)
        {
            label.Font = AuthentificationFormStyle.TitreCarte();
            label.ForeColor = AuthentificationFormStyle.TextePrincipal;
            label.TextAlign = ContentAlignment.MiddleCenter;
            label.AutoSize = false;
        }

        public static void AppliquerSousTexte(Label label)
        {
            label.Font = AuthentificationFormStyle.LabelChamp();
            label.ForeColor = AuthentificationFormStyle.TexteAide;
            label.TextAlign = ContentAlignment.MiddleCenter;
            label.AutoSize = false;
        }

        public static void AppliquerSeparateur(Panel panel)
        {
            panel.Height = 1;
            panel.BackColor = AuthentificationFormStyle.Separateur;
        }
    }
}