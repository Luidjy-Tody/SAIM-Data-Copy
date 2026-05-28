using FontAwesome.Sharp;
using System.Drawing;
using System.Windows.Forms;

namespace SaimDataCopy.Styles
{
    /// <summary>
    /// Style utilisé uniquement pour la page Exécution.
    /// On le garde séparé pour éviter de casser les autres pages.
    /// </summary>
    public static class ExecutionStyle
    {
        private static readonly Color BleuPrincipal = Color.FromArgb(30, 96, 190);
        private static readonly Color VertPrincipal = Color.FromArgb(45, 115, 20);

        private static readonly Color FondPage = Color.White;
        private static readonly Color FondCarte = Color.FromArgb(245, 245, 245);
        private static readonly Color FondJournal = Color.FromArgb(28, 30, 30);
        private static readonly Color FondEnteteTableau = Color.FromArgb(245, 245, 245);

        private static readonly Color BordureClaire = Color.FromArgb(220, 220, 220);
        private static readonly Color TexteNormal = Color.FromArgb(35, 35, 35);
        private static readonly Color TexteSecondaire = Color.FromArgb(90, 90, 90);

        public static void AppliquerFondPage(Control control)
        {
            control.BackColor = FondPage;
        }

        public static void AppliquerTitrePage(Label label)
        {
            label.AutoSize = true;
            label.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            label.ForeColor = TexteNormal;
        }

        public static void AppliquerSousTitre(Label label)
        {
            label.AutoSize = true;
            label.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            label.ForeColor = TexteNormal;
        }

        public static void AppliquerCarteTableauBord(Panel panel)
        {
            panel.BackColor = FondCarte;
            panel.BorderStyle = BorderStyle.None;
            panel.Height = 102;
        }

        public static void AppliquerValeurCarte(Label label)
        {
            label.AutoSize = true;
            label.Font = new Font("Segoe UI", 20F, FontStyle.Bold);
            label.ForeColor = BleuPrincipal;
        }

        public static void AppliquerTitreCarte(Label label)
        {
            label.AutoSize = true;
            label.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular);
            label.ForeColor = TexteSecondaire;
        }

        public static void AppliquerTexteSecondaire(Label label)
        {
            label.AutoSize = true;
            label.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular);
            label.ForeColor = TexteSecondaire;
        }

        public static void AppliquerPanelJournal(Panel panel)
        {
            panel.BackColor = FondJournal;
            panel.BorderStyle = BorderStyle.None;
        }

        public static void AppliquerJournal(RichTextBox journal)
        {
            journal.BackColor = FondJournal;
            journal.ForeColor = Color.White;
            journal.BorderStyle = BorderStyle.None;
            journal.Font = new Font("Consolas", 9.5F, FontStyle.Regular);

            journal.ReadOnly = true;
            journal.Multiline = true;
            journal.ScrollBars = RichTextBoxScrollBars.Vertical;
            journal.WordWrap = false;
        }

        public static void AppliquerProgressBar(ProgressBar progressBar)
        {
            progressBar.Height = 24;
            progressBar.Minimum = 0;
            progressBar.Maximum = 100;
            progressBar.Value = 0;
            progressBar.Style = ProgressBarStyle.Continuous;
        }

        public static void AppliquerLabelProgression(Label label)
        {
            label.AutoSize = true;
            label.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular);
            label.ForeColor = TexteSecondaire;
        }

        public static void AppliquerBoutonTesterConnexion(IconButton bouton)
        {
            bouton.Height = 38;
            bouton.Width = 200;

            bouton.Text = " Tester la connexion";
            bouton.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
            bouton.BackColor = Color.White;
            bouton.ForeColor = TexteNormal;

            bouton.FlatStyle = FlatStyle.Flat;
            bouton.FlatAppearance.BorderSize = 1;
            bouton.FlatAppearance.BorderColor = BordureClaire;
            bouton.Cursor = Cursors.Hand;

            bouton.IconChar = IconChar.Plug;
            bouton.IconSize = 18;
            bouton.IconColor = TexteNormal;
            bouton.TextImageRelation = TextImageRelation.ImageBeforeText;
            bouton.ImageAlign = ContentAlignment.MiddleLeft;
            bouton.TextAlign = ContentAlignment.MiddleCenter;
        }

        public static void AppliquerBoutonAnnuler(IconButton bouton)
        {
            bouton.Height = 38;
            bouton.Width = 120;

            bouton.Text = " Annuler";
            bouton.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
            bouton.BackColor = Color.White;
            bouton.ForeColor = TexteNormal;

            bouton.FlatStyle = FlatStyle.Flat;
            bouton.FlatAppearance.BorderSize = 1;
            bouton.FlatAppearance.BorderColor = BordureClaire;
            bouton.Cursor = Cursors.Hand;

            bouton.IconChar = IconChar.Xmark;
            bouton.IconSize = 18;
            bouton.IconColor = TexteNormal;
            bouton.TextImageRelation = TextImageRelation.ImageBeforeText;
            bouton.ImageAlign = ContentAlignment.MiddleLeft;
            bouton.TextAlign = ContentAlignment.MiddleCenter;
        }

        public static void AppliquerBoutonLancer(IconButton bouton)
        {
            bouton.Height = 38;
            bouton.Width = 170;

            bouton.Text = " Lancer la copie";
            bouton.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
            bouton.BackColor = VertPrincipal;
            bouton.ForeColor = Color.White;

            bouton.FlatStyle = FlatStyle.Flat;
            bouton.FlatAppearance.BorderSize = 0;
            bouton.Cursor = Cursors.Hand;

            bouton.IconChar = IconChar.Play;
            bouton.IconSize = 18;
            bouton.IconColor = Color.White;
            bouton.TextImageRelation = TextImageRelation.ImageBeforeText;
            bouton.ImageAlign = ContentAlignment.MiddleLeft;
            bouton.TextAlign = ContentAlignment.MiddleCenter;
        }

        public static void AppliquerTableauResultat(DataGridView grille)
        {
            grille.BorderStyle = BorderStyle.FixedSingle;
            grille.BackgroundColor = Color.White;
            grille.GridColor = Color.FromArgb(225, 225, 225);

            grille.EnableHeadersVisualStyles = false;

            grille.ColumnHeadersDefaultCellStyle.BackColor = FondEnteteTableau;
            grille.ColumnHeadersDefaultCellStyle.ForeColor = TexteNormal;
            grille.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
            grille.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            grille.ColumnHeadersDefaultCellStyle.Padding = new Padding(8, 0, 0, 0);

            grille.ColumnHeadersDefaultCellStyle.SelectionBackColor = FondEnteteTableau;
            grille.ColumnHeadersDefaultCellStyle.SelectionForeColor = TexteNormal;

            grille.ColumnHeadersHeight = 42;
            grille.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

            grille.DefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
            grille.DefaultCellStyle.BackColor = Color.White;
            grille.DefaultCellStyle.ForeColor = TexteNormal;
            grille.DefaultCellStyle.SelectionBackColor = Color.White;
            grille.DefaultCellStyle.SelectionForeColor = TexteNormal;
            grille.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            grille.DefaultCellStyle.Padding = new Padding(8, 0, 8, 0);

            grille.AlternatingRowsDefaultCellStyle.BackColor = Color.White;
            grille.AlternatingRowsDefaultCellStyle.ForeColor = TexteNormal;
            grille.AlternatingRowsDefaultCellStyle.SelectionBackColor = Color.White;
            grille.AlternatingRowsDefaultCellStyle.SelectionForeColor = TexteNormal;

            grille.RowHeadersVisible = false;
            grille.RowTemplate.Height = 52;

            grille.AllowUserToAddRows = false;
            grille.AllowUserToDeleteRows = false;
            grille.AllowUserToResizeRows = false;
            grille.AllowUserToResizeColumns = false;

            grille.SelectionMode = DataGridViewSelectionMode.CellSelect;
            grille.MultiSelect = false;
            grille.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grille.ReadOnly = true;
        }

        public static void DesactiverTriColonnes(DataGridView grille)
        {
            foreach (DataGridViewColumn colonne in grille.Columns)
            {
                colonne.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
        }

        public static Color ObtenirCouleurLog(string type)
        {
            return type switch
            {
                "Info" => Color.FromArgb(90, 170, 255),
                "Succes" => Color.FromArgb(110, 220, 95),
                "Avertissement" => Color.FromArgb(255, 190, 40),
                "Erreur" => Color.FromArgb(255, 100, 100),
                _ => Color.White
            };
        }

        public static Color ObtenirCouleurResultat(string resultat)
        {
            return resultat switch
            {
                "Succès" => Color.FromArgb(45, 130, 65),
                "Avertissement" => Color.FromArgb(180, 105, 0),
                "Erreur" => Color.FromArgb(190, 50, 50),
                _ => TexteNormal
            };
        }
    }
}