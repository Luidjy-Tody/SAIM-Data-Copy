using System.Drawing;
using System.Windows.Forms;

namespace SaimDataCopy.Styles.Authentification
{
    public static class AuthLabelStyle
    {
        public static void AppliquerLabelChamp(Label label)
        {
            label.Font = AuthentificationFormStyle.LabelChamp();
            label.ForeColor = AuthentificationFormStyle.TexteSecondaire;
            label.AutoSize = false;
            label.TextAlign = ContentAlignment.MiddleLeft;
        }

        public static void AppliquerErreur(Label label)
        {
            label.Font = AuthentificationFormStyle.LabelChamp();
            label.ForeColor = AuthentificationFormStyle.RougeErreur;
            label.AutoSize = false;
            label.TextAlign = ContentAlignment.MiddleCenter;
        }

        public static void AppliquerSucces(Label label)
        {
            label.Font = AuthentificationFormStyle.LabelChamp();
            label.ForeColor = AuthentificationFormStyle.VertSucces;
            label.AutoSize = false;
            label.TextAlign = ContentAlignment.MiddleCenter;
        }

        public static void AppliquerLien(LinkLabel lien)
        {
            lien.Font = AuthentificationFormStyle.TexteLien();
            lien.LinkColor = AuthentificationFormStyle.BleuPrincipal;
            lien.ActiveLinkColor = AuthentificationFormStyle.BleuSurvol;
            lien.VisitedLinkColor = AuthentificationFormStyle.BleuPrincipal;
            lien.TextAlign = ContentAlignment.MiddleCenter;
            lien.AutoSize = false;
        }
    }
}