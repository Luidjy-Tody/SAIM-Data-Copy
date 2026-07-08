using SaimDataCopy.Styles.Authentification.Identification;

namespace SaimDataCopy.Styles.Authentification.MotDePasseOublie
{
    public static class MotDePasseOublieStyle
    {
        public static readonly Color TextePrincipal = IdentificationStyle.TextePrincipal;
        public static readonly Color TexteSecondaire = IdentificationStyle.TexteSecondaire;
        public static readonly Color TexteAide = IdentificationStyle.TexteAide;
        public static readonly Color BleuBouton = IdentificationStyle.BleuBouton;
        public static readonly Color BleuBoutonSurvol = IdentificationStyle.BleuBoutonSurvol;
        public static readonly Color BordureChamp = IdentificationStyle.BordureChamp;
        public static readonly Color FondChamp = IdentificationStyle.FondChamp;
        public static readonly Color Separateur = IdentificationStyle.Separateur;
        public static readonly Color RougeErreur = IdentificationStyle.RougeErreur;
        public static readonly Color VertSucces = IdentificationStyle.VertSucces;
        public static readonly Color VertClair = IdentificationStyle.VertClair;

        public static Font TitreApplication() => IdentificationStyle.TitreApplication();
        public static Font SousTitreApplication() => IdentificationStyle.SousTitreApplication();
        public static Font TitreCarte() => IdentificationStyle.TitreCarte();
        public static Font LabelChamp() => IdentificationStyle.LabelChamp();
        public static Font Champ() => IdentificationStyle.Champ();
        public static Font Bouton() => IdentificationStyle.Bouton();
        public static Font Lien() => IdentificationStyle.Lien();
        public static Font TextePetit() => IdentificationStyle.TextePetit();

        public static void AppliquerTextBox(TextBox textBox) => IdentificationStyle.AppliquerTextBox(textBox);
        public static void AppliquerLabelChamp(Label label) => IdentificationStyle.AppliquerLabelChamp(label);
        public static void AppliquerLien(LinkLabel lien) => IdentificationStyle.AppliquerLien(lien);
        public static void AppliquerBouton(Button bouton) => IdentificationStyle.AppliquerBouton(bouton);
    }
}