using FontAwesome.Sharp;
using System.Drawing;
using System.Windows.Forms;

namespace SaimDataCopy.Styles
{
    /// <summary>
    /// Styles utilisés uniquement dans la page Journal d'activité.
    /// </summary>
    public static class JournalActiviteStyle
    {
        private static readonly Color CouleurFondPage = Color.White;
        private static readonly Color CouleurTexte = Color.FromArgb(30, 30, 30);
        private static readonly Color CouleurTexteSecondaire = Color.FromArgb(90, 90, 90);
        private static readonly Color CouleurBleu = Color.FromArgb(30, 96, 190);
        private static readonly Color CouleurBordure = Color.FromArgb(220, 220, 220);
        private static readonly Color CouleurFondEnteteTableau = Color.FromArgb(245, 245, 245);

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
            label.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            label.ForeColor = CouleurTexte;
            label.Margin = new Padding(0, 0, 0, 5);
        }

        public static void AppliquerSousTitre(Label label)
        {
            label.AutoSize = true;
            label.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
            label.ForeColor = CouleurTexteSecondaire;
            label.Margin = new Padding(0, 0, 0, 22);
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

        public static void AppliquerLibelleFiltre(Label label)
        {
            label.AutoSize = true;
            label.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
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

        public static void AppliquerBoutonRechercher(IconButton bouton)
        {
            bouton.Width = 125;
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

        public static void AppliquerBoutonActualiser(IconButton bouton)
        {
            bouton.Width = 130;
            bouton.Height = 36;
            bouton.Text = " Actualiser";
            bouton.IconChar = IconChar.Rotate;
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

        public static void AppliquerBoutonReinitialiser(Button bouton)
        {
            bouton.Width = 125;
            bouton.Height = 36;
            bouton.Text = "Réinitialiser";
            bouton.BackColor = Color.White;
            bouton.ForeColor = CouleurTexte;
            bouton.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular);
            bouton.FlatStyle = FlatStyle.Flat;
            bouton.FlatAppearance.BorderColor = CouleurBordure;
            bouton.FlatAppearance.BorderSize = 1;
            bouton.Cursor = Cursors.Hand;
        }

        public static void AppliquerTableauJournal(DataGridView tableau)
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
            tableau.ColumnHeadersDefaultCellStyle.BackColor = CouleurFondEnteteTableau;
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

            tableau.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(250, 251, 253);
        }

        public static void AppliquerMessageInformation(Label label)
        {
            label.AutoSize = true;
            label.Font = new Font("Segoe UI", 9F, FontStyle.Italic);
            label.ForeColor = CouleurTexteSecondaire;
        }

        public static void AppliquerAction(DataGridViewCellStyle styleCellule, string action)
        {
            styleCellule.Alignment = DataGridViewContentAlignment.MiddleCenter;

            if (action.Equals("Connexion", StringComparison.OrdinalIgnoreCase))
            {
                styleCellule.BackColor = Color.FromArgb(225, 242, 220);
                styleCellule.ForeColor = Color.FromArgb(30, 100, 40);
                return;
            }

            if (action.Equals("Déconnexion", StringComparison.OrdinalIgnoreCase))
            {
                styleCellule.BackColor = Color.FromArgb(235, 235, 235);
                styleCellule.ForeColor = Color.FromArgb(80, 80, 80);
                return;
            }

            if (action.Contains("Suppression", StringComparison.OrdinalIgnoreCase) ||
                action.Contains("Désactivation", StringComparison.OrdinalIgnoreCase))
            {
                styleCellule.BackColor = Color.FromArgb(250, 230, 225);
                styleCellule.ForeColor = Color.FromArgb(150, 50, 35);
                return;
            }

            if (action.Contains("Activation", StringComparison.OrdinalIgnoreCase))
            {
                styleCellule.BackColor = Color.FromArgb(225, 242, 220);
                styleCellule.ForeColor = Color.FromArgb(30, 100, 40);
                return;
            }

            if (action.Contains("Création", StringComparison.OrdinalIgnoreCase) ||
                action.Contains("Modification", StringComparison.OrdinalIgnoreCase))
            {
                styleCellule.BackColor = Color.FromArgb(235, 228, 250);
                styleCellule.ForeColor = Color.FromArgb(95, 55, 155);
                return;
            }

            styleCellule.BackColor = Color.FromArgb(230, 240, 255);
            styleCellule.ForeColor = CouleurBleu;
        }
    }
}