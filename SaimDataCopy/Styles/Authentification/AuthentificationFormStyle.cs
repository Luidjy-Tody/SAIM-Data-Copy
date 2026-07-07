using System.Drawing;

namespace SaimDataCopy.Styles.Authentification
{
    public static class AuthentificationFormStyle
    {
        public static readonly Color BleuPrincipal = Color.FromArgb(14, 116, 144);
        public static readonly Color BleuSurvol = Color.FromArgb(15, 118, 110);
        public static readonly Color BleuFonce = Color.FromArgb(17, 94, 89);

        public static readonly Color Blanc = Color.White;
        public static readonly Color TextePrincipal = Color.FromArgb(15, 23, 42);
        public static readonly Color TexteSecondaire = Color.FromArgb(71, 85, 105);
        public static readonly Color TexteAide = Color.FromArgb(100, 116, 139);

        public static readonly Color FondChamp = Color.FromArgb(248, 250, 252);
        public static readonly Color BordureChamp = Color.FromArgb(203, 213, 225);
        public static readonly Color BordureFocus = BleuPrincipal;
        public static readonly Color Separateur = Color.FromArgb(226, 232, 240);

        public static readonly Color VertClair = Color.FromArgb(204, 251, 241);
        public static readonly Color RougeErreur = Color.FromArgb(220, 38, 38);
        public static readonly Color VertSucces = Color.FromArgb(16, 185, 129);

        public static Font TitreApplication()
        {
            return new Font("Segoe UI", 28F, FontStyle.Bold);
        }

        public static Font SousTitreApplication()
        {
            return new Font("Segoe UI", 13F, FontStyle.Regular);
        }

        public static Font TitreCarte()
        {
            return new Font("Segoe UI", 22F, FontStyle.Bold);
        }

        public static Font LabelChamp()
        {
            return new Font("Segoe UI", 13F, FontStyle.Regular);
        }

        public static Font TexteChamp()
        {
            return new Font("Segoe UI", 14F, FontStyle.Regular);
        }

        public static Font TexteBouton()
        {
            return new Font("Segoe UI", 15F, FontStyle.Bold);
        }

        public static Font TexteLien()
        {
            return new Font("Segoe UI", 13F, FontStyle.Underline);
        }

        public static Font TextePetit()
        {
            return new Font("Segoe UI", 12F, FontStyle.Regular);
        }
    }
}