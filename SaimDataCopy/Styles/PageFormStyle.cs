using System.Drawing;
using System.Windows.Forms;


namespace SaimDataCopy.Styles
{
    // Cette classe contient le design des éléments utilisés dans les pages.
    // Exemple : titre, label, TextBox, ComboBox, badge "requis".
    public static class PageFormStyle
    {
        //Couleur principale de l'application.

        private static readonly Color BleuPrincipal = Color.FromArgb(30, 96, 190);

        //couleur du texte normal
        private static readonly Color TexteNormal = Color.FromArgb(30, 30, 30);

        //couleur du fond des champs.
        private static readonly Color FondChamp = Color.White;

        //style du titre principal de page.
        public static void AppliquerTitre(Label label)
        {
            label.AutoSize = true;
            label.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            label.ForeColor = Color.Black;
        }

        // Style des titres de section
        //Exemple : serveur source, serveur cible

        public static void AppliquerSousTitre(Label label)
        {
            label.AutoSize = true;
            label.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            label.ForeColor = Color.Black;
        }

        //Style des petits labels au dessus des champs.

        public static void AppliquerLabelChamp(Label label)
        {
            label.AutoSize = true;
            label.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
            label.ForeColor = TexteNormal;
        }

        // Style des textBox.
        public static void AppliquerTextBox(TextBox textBox)
        {
            textBox.Font = new Font("Segoe UI", 14F, FontStyle.Regular);
            textBox.BackColor = FondChamp;
            textBox.BorderStyle = BorderStyle.FixedSingle;
        }

        // Style des ComboBOX.
        // Style des ComboBox.
        // Style des ComboBox.
        // Style des ComboBox.
        // Style des ComboBox.
        public static void AppliquerComboBox(ComboBox comboBox)
        {
            comboBox.Font = new Font("Segoe UI", 12F, FontStyle.Regular);
            comboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox.BackColor = FondChamp;
            comboBox.FlatStyle = FlatStyle.Flat;
            comboBox.MaxDropDownItems = 3;

        }

        // Cree un petit badge rouge clair avec le text "requis".
        public static Label CreerBadgeRequis()
        {
            Label badge = new Label();

            badge.Text = "requis";
            badge.AutoSize = true;

            badge.Font = new Font("Segoe UI", 8F, FontStyle.Regular);
            badge.ForeColor = Color.FromArgb(180, 40, 40);
            badge.BackColor = Color.FromArgb(255, 235, 235);

            //petit espace a l'interieur du badge.
            badge.Padding = new Padding(4, 2, 4, 2);

            return badge;
        }

        //Stylr simple pour un bouton bleu.
        //On pourra l'utiliser plus tard pour tester connexion, enregistrer, etc.
        public static void AppliquerBoutonBleu(Button bouton)
        {
            bouton.BackColor = BleuPrincipal;
            bouton.ForeColor = Color.White;

            bouton.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            bouton.FlatStyle = FlatStyle.Flat;
            bouton.FlatAppearance.BorderSize = 0;
        }

    }
}
