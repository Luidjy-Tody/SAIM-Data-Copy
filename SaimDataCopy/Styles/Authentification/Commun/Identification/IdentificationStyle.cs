using System.Drawing;
using System.Windows.Forms;

namespace SaimDataCopy.Styles.Authentification.Identification
{
    public static class IdentificationStyle
    {
        public static readonly Color TextePrincipal = Color.FromArgb(15, 23, 42);
        public static readonly Color TexteSecondaire = Color.FromArgb(51, 65, 85);
        public static readonly Color TexteAide = Color.FromArgb(100, 116, 139);
        public static readonly Color BleuBouton = Color.FromArgb(15, 132, 156);
        public static readonly Color BleuBoutonSurvol = Color.FromArgb(12, 110, 132);
        public static readonly Color BordureChamp = Color.FromArgb(203, 213, 225);
        public static readonly Color FondChamp = Color.FromArgb(248, 250, 252);
        public static readonly Color Separateur = Color.FromArgb(226, 232, 240);
        public static readonly Color RougeErreur = Color.FromArgb(220, 38, 38);
        public static readonly Color VertSucces = Color.FromArgb(22, 163, 74);
        public static readonly Color VertClair = Color.FromArgb(204, 251, 241);

        public static Font TitreApplication()
        {
            return new Font("Segoe UI", 24F, FontStyle.Bold);
        }

        public static Font SousTitreApplication()
        {
            return new Font("Segoe UI", 10.5F, FontStyle.Regular);
        }

        public static Font TitreCarte()
        {
            return new Font("Segoe UI", 17F, FontStyle.Bold);
        }

        public static Font LabelChamp()
        {
            return new Font("Segoe UI", 9.5F, FontStyle.Bold);
        }

        public static Font Champ()
        {
            return new Font("Segoe UI", 10F, FontStyle.Regular);
        }

        public static Font Bouton()
        {
            return new Font("Segoe UI", 10.5F, FontStyle.Bold);
        }

        public static Font Lien()
        {
            return new Font("Segoe UI", 9F, FontStyle.Bold | FontStyle.Underline);
        }

        public static Font TextePetit()
        {
            return new Font("Segoe UI", 9F, FontStyle.Regular);
        }

        public static void AppliquerTextBox(TextBox textBox)
        {
            textBox.Font = Champ();
            textBox.ForeColor = TextePrincipal;
            textBox.BackColor = FondChamp;
            textBox.BorderStyle = BorderStyle.FixedSingle;
        }

        public static void AppliquerLabelChamp(Label label)
        {
            label.Font = LabelChamp();
            label.ForeColor = TexteSecondaire;
            label.BackColor = Color.Transparent;
        }

        public static void AppliquerLien(LinkLabel lien)
        {
            lien.Font = Lien();
            lien.LinkColor = BleuBouton;
            lien.ActiveLinkColor = BleuBoutonSurvol;
            lien.VisitedLinkColor = BleuBouton;
            lien.BackColor = Color.Transparent;
            lien.TextAlign = ContentAlignment.MiddleCenter;
        }

        public static void AppliquerBouton(Button bouton)
        {
            bouton.Font = Bouton();
            bouton.ForeColor = Color.White;
            bouton.BackColor = BleuBouton;
            bouton.FlatStyle = FlatStyle.Flat;
            bouton.Cursor = Cursors.Hand;

            bouton.FlatAppearance.BorderSize = 0;
            bouton.FlatAppearance.MouseOverBackColor = BleuBoutonSurvol;
            bouton.FlatAppearance.MouseDownBackColor = Color.FromArgb(10, 90, 110);
        }
    }
}