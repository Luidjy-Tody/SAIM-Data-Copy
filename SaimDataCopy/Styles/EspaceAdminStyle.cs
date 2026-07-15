using System.Drawing;
using System.Windows.Forms;
using FontAwesome.Sharp;
namespace SaimDataCopy.Styles
{
    /// <summary>
    /// Styles utilisés uniquement dans la page Espace Admin.
    /// </summary>
    public static class EspaceAdminStyle
    {
        private static readonly Color CouleurFondPage = Color.White;
        private static readonly Color CouleurTexte = Color.FromArgb(30, 30, 30);
        private static readonly Color CouleurTexteSecondaire = Color.FromArgb(90, 90, 90);

        private static readonly Color CouleurBleu = Color.FromArgb(30, 96, 190);

        private static readonly Color CouleurBordure =
            Color.FromArgb(220, 220, 220);

        private static readonly Color CouleurFondCarte =
            Color.FromArgb(248, 250, 253);
        public static void AppliquerPage(UserControl page)
        {
            page.Dock = DockStyle.Fill;
            page.BackColor = CouleurFondPage;
            page.AutoScroll = false;
            page.Padding = new Padding(0);
        }

        public static void AppliquerTitre(Label label)
        {
            label.AutoSize = true;
            label.Font = new Font(
                "Segoe UI",
                14F,
                FontStyle.Bold
            );

            label.ForeColor = CouleurTexte;
            label.Margin = new Padding(0, 0, 0, 5);
        }

        public static void AppliquerSousTitre(Label label)
        {
            label.AutoSize = true;
            label.Font = new Font(
                "Segoe UI",
                10F,
                FontStyle.Regular
            );

            label.ForeColor = CouleurTexteSecondaire;
            label.Margin = new Padding(0, 0, 0, 22);
        }

        public static void AppliquerCartePreparation(Panel panel)
        {
            panel.Height = 150;
            panel.BackColor = Color.FromArgb(248, 250, 253);
            panel.BorderStyle = BorderStyle.FixedSingle;
            panel.Margin = new Padding(0, 10, 0, 0);
            panel.Padding = new Padding(22);
        }

        public static void AppliquerTitreCarte(Label label)
        {
            label.AutoSize = true;
            label.Font = new Font(
                "Segoe UI",
                11F,
                FontStyle.Bold
            );

            label.ForeColor = CouleurTexte;
        }

        public static void AppliquerTexteCarte(Label label)
        {
            label.AutoSize = true;
            label.Font = new Font(
                "Segoe UI",
                10F,
                FontStyle.Regular
            );

            label.ForeColor = CouleurTexteSecondaire;
        }

        public static void AppliquerZoneCartes(
    FlowLayoutPanel panel)
        {
            panel.AutoSize = false;
            panel.Height = 125;
            panel.FlowDirection = FlowDirection.LeftToRight;
            panel.WrapContents = false;
            panel.Margin = new Padding(0, 0, 0, 22);
            panel.Padding = new Padding(0);
            panel.BackColor = Color.White;
        }

        public static void AppliquerCarteStatistique(
            Panel panel)
        {
            panel.Height = 105;
            panel.BackColor = CouleurFondCarte;
            panel.BorderStyle = BorderStyle.FixedSingle;
            panel.Margin = new Padding(0, 0, 14, 0);
            panel.Padding = new Padding(16);
        }

        public static void AppliquerIconeCarte(
            IconPictureBox icone,
            IconChar iconChar,
            Color couleur)
        {
            icone.IconChar = iconChar;
            icone.IconColor = couleur;
            icone.IconSize = 24;
            icone.Size = new Size(30, 30);
            icone.Location = new Point(16, 17);
            icone.BackColor = Color.Transparent;
        }

        public static void AppliquerTitreStatistique(
            Label label)
        {
            label.AutoSize = true;
            label.Font = new Font(
                "Segoe UI",
                9.5F,
                FontStyle.Regular
            );

            label.ForeColor = CouleurTexteSecondaire;
        }

        public static void AppliquerValeurStatistique(
            Label label)
        {
            label.AutoSize = true;
            label.Font = new Font(
                "Segoe UI",
                19F,
                FontStyle.Bold
            );

            label.ForeColor = CouleurTexte;
        }

        public static void AppliquerBoutonActualiser(
            IconButton bouton)
        {
            bouton.Width = 130;
            bouton.Height = 36;

            bouton.Text = " Actualiser";
            bouton.IconChar = IconChar.Rotate;
            bouton.IconSize = 16;
            bouton.IconColor = Color.White;

            bouton.TextImageRelation =
                TextImageRelation.ImageBeforeText;

            bouton.TextAlign = ContentAlignment.MiddleCenter;
            bouton.ImageAlign = ContentAlignment.MiddleCenter;

            bouton.BackColor = CouleurBleu;
            bouton.ForeColor = Color.White;

            bouton.Font = new Font(
                "Segoe UI",
                9.5F,
                FontStyle.Regular
            );

            bouton.FlatStyle = FlatStyle.Flat;
            bouton.FlatAppearance.BorderSize = 0;
            bouton.Cursor = Cursors.Hand;
        }

        public static void AppliquerCartePrincipale(Panel panel)
        {
            panel.BackColor = Color.White;
            panel.BorderStyle = BorderStyle.FixedSingle;
            panel.Padding = new Padding(18);
            panel.Margin = new Padding(0, 0, 0, 22);
        }

        public static void AppliquerTitreSection(Label label)
        {
            label.AutoSize = true;
            label.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            label.ForeColor = CouleurTexte;
        }

        public static void AppliquerChampRecherche(TextBox textBox)
        {
            textBox.Height = 36;
            textBox.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
            textBox.BorderStyle = BorderStyle.FixedSingle;
        }

        public static void AppliquerComboBoxFiltre(ComboBox comboBox)
        {
            comboBox.Height = 36;
            comboBox.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
            comboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        public static void AppliquerBoutonNouvelUtilisateur(IconButton bouton)
        {
            bouton.Width = 175;
            bouton.Height = 36;
            bouton.Text = " Nouvel utilisateur";
            bouton.IconChar = IconChar.UserPlus;
            bouton.IconSize = 17;
            bouton.IconColor = Color.White;
            bouton.BackColor = CouleurBleu;
            bouton.ForeColor = Color.White;
            bouton.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            bouton.TextImageRelation = TextImageRelation.ImageBeforeText;
            bouton.TextAlign = ContentAlignment.MiddleCenter;
            bouton.ImageAlign = ContentAlignment.MiddleCenter;
            bouton.FlatStyle = FlatStyle.Flat;
            bouton.FlatAppearance.BorderSize = 0;
            bouton.Cursor = Cursors.Hand;
        }

        public static void AppliquerBoutonRechercherUtilisateurs(IconButton bouton)
        {
            bouton.Width = 120;
            bouton.Height = 36;
            bouton.Text = " Rechercher";
            bouton.IconChar = IconChar.Search;
            bouton.IconSize = 16;
            bouton.IconColor = Color.White;
            bouton.BackColor = CouleurBleu;
            bouton.ForeColor = Color.White;
            bouton.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular);
            bouton.TextImageRelation = TextImageRelation.ImageBeforeText;
            bouton.TextAlign = ContentAlignment.MiddleCenter;
            bouton.ImageAlign = ContentAlignment.MiddleCenter;
            bouton.FlatStyle = FlatStyle.Flat;
            bouton.FlatAppearance.BorderSize = 0;
            bouton.Cursor = Cursors.Hand;
        }

        public static void AppliquerTableauUtilisateurs(DataGridView tableau)
        {
            tableau.BackgroundColor = Color.White;
            tableau.BorderStyle = BorderStyle.None;
            tableau.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            tableau.GridColor = CouleurBordure;

            tableau.AllowUserToAddRows = false;
            tableau.AllowUserToDeleteRows = false;
            tableau.AllowUserToResizeRows = false;
            tableau.AllowUserToOrderColumns = false;
            tableau.MultiSelect = false;
            tableau.ReadOnly = true;
            tableau.RowHeadersVisible = false;
            tableau.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            tableau.AutoGenerateColumns = false;
            tableau.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            tableau.EnableHeadersVisualStyles = false;
            tableau.ColumnHeadersHeight = 45;
            tableau.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245);
            tableau.ColumnHeadersDefaultCellStyle.ForeColor = CouleurTexte;
            tableau.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            tableau.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            tableau.RowTemplate.Height = 48;
            tableau.DefaultCellStyle.BackColor = Color.White;
            tableau.DefaultCellStyle.ForeColor = CouleurTexteSecondaire;
            tableau.DefaultCellStyle.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular);
            tableau.DefaultCellStyle.SelectionBackColor = Color.FromArgb(230, 240, 255);
            tableau.DefaultCellStyle.SelectionForeColor = CouleurTexte;
            tableau.DefaultCellStyle.Padding = new Padding(5, 0, 5, 0);
        }


    }
}