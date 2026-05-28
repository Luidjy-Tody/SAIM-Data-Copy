using FontAwesome.Sharp;
using System.Drawing;
using System.Windows.Forms;

namespace SaimDataCopy.Styles
{
    // Style utilisé seulement pour la page Historique.
    // On ne mélange pas avec les autres styles.
    public static class HistoriqueStyle
    {
        private static readonly Color CouleurBleu = Color.FromArgb(30, 96, 190);
        private static readonly Color CouleurFondPage = Color.White;
        private static readonly Color CouleurBordure = Color.FromArgb(220, 220, 220);
        private static readonly Color CouleurEnteteTableau = Color.FromArgb(245, 245, 245);
        private static readonly Color CouleurTexte = Color.FromArgb(30, 30, 30);
        private static readonly Color CouleurTexteSecondaire = Color.FromArgb(55, 65, 75);

        // Appliquer le style général de la page.
        public static void AppliquerPage(UserControl page)
        {
            page.BackColor = CouleurFondPage;
            page.AutoScroll = true;
            page.Padding = new Padding(22, 25, 22, 25);
        }

        // Style du grand titre.
        public static void AppliquerTitre(Label label)
        {
            label.AutoSize = true;
            label.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            label.ForeColor = CouleurTexte;
            label.Margin = new Padding(0, 0, 0, 22);
        }

        // Style des labels de filtre.
        public static void AppliquerLabelFiltre(Label label)
        {
            label.AutoSize = true;
            label.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
            label.ForeColor = CouleurTexte;
        }

        // Style du filtre date.
        public static void AppliquerDatePicker(DateTimePicker datePicker)
        {
            datePicker.Width = 240;
            datePicker.Height = 36;
            datePicker.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
            datePicker.Format = DateTimePickerFormat.Custom;
            datePicker.CustomFormat = "'jj/mm/aaaa'";
            datePicker.ShowCheckBox = false;
        }

        // Style des ComboBox de filtre.
        public static void AppliquerComboBox(ComboBox comboBox)
        {
            comboBox.Width = 240;
            comboBox.Height = 36;
            comboBox.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
            comboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox.FlatStyle = FlatStyle.System;
        }

        // Style du bouton Rechercher.
        public static void AppliquerBoutonRecherche(IconButton bouton)
        {
            bouton.Width = 240;
            bouton.Height = 30;

            bouton.Text = "Rechercher";
            bouton.IconChar = IconChar.Search;
            bouton.IconSize = 16;
            bouton.IconColor = Color.White;

            bouton.TextImageRelation = TextImageRelation.ImageBeforeText;
            bouton.ImageAlign = ContentAlignment.MiddleCenter;
            bouton.TextAlign = ContentAlignment.MiddleCenter;

            bouton.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
            bouton.ForeColor = Color.White;
            bouton.BackColor = CouleurBleu;

            bouton.FlatStyle = FlatStyle.Flat;
            bouton.FlatAppearance.BorderSize = 0;

            bouton.Padding = new Padding(0);
            bouton.Margin = new Padding(0);
            bouton.Cursor = Cursors.Hand;
        }

        // Cadre extérieur du tableau.
        // Le BackColor sert aussi à faire une bordure propre.
        public static void AppliquerCadreTableau(Panel panel)
        {
            panel.Width = 995;
            panel.Height = 336;
            panel.BackColor = CouleurBordure;
            panel.Padding = new Padding(1);
            panel.Margin = new Padding(0, 24, 0, 0);
        }

        // Contenu intérieur du tableau.
        public static void AppliquerListeTableau(FlowLayoutPanel panel)
        {
            panel.Dock = DockStyle.Fill;
            panel.BackColor = Color.White;
            panel.FlowDirection = FlowDirection.TopDown;
            panel.WrapContents = false;
            panel.AutoScroll = false;
            panel.Padding = new Padding(0);
            panel.Margin = new Padding(0);
        }

        // Ligne d'en-tête du tableau.
        public static void AppliquerLigneEntete(TableLayoutPanel ligne)
        {
            ligne.Width = 993;
            ligne.Height = 66;
            ligne.BackColor = CouleurEnteteTableau;
            ligne.Margin = new Padding(0);
            ligne.Padding = new Padding(0);
            ligne.CellBorderStyle = TableLayoutPanelCellBorderStyle.None;
        }

        // Ligne de données du tableau.
        public static void AppliquerLigneDonnee(TableLayoutPanel ligne)
        {
            ligne.Width = 993;
            ligne.Height = 88;
            ligne.BackColor = Color.White;
            ligne.Margin = new Padding(0);
            ligne.Padding = new Padding(0);
            ligne.CellBorderStyle = TableLayoutPanelCellBorderStyle.None;
        }

        // Séparateur horizontal entre les lignes.
        public static void AppliquerSeparateur(Panel separateur)
        {
            separateur.Width = 993;
            separateur.Height = 1;
            separateur.BackColor = CouleurBordure;
            separateur.Margin = new Padding(0);
            separateur.Padding = new Padding(0);
        }

        // Style d'une cellule d'en-tête.
        public static void AppliquerCelluleEntete(Label label)
        {
            label.Dock = DockStyle.Fill;

            // Un peu marqué, mais pas trop lourd.
            label.Font = new Font("Segoe UI", 10F, FontStyle.Regular);

            label.ForeColor = CouleurTexte;
            label.BackColor = CouleurEnteteTableau;

            // Aligné verticalement au milieu.
            label.TextAlign = ContentAlignment.MiddleLeft;

            // Moins de padding pour éviter que les mots se coupent.
            label.Padding = new Padding(10, 0, 6, 0);

            label.AutoSize = false;
        }

        // Style d'une cellule texte.
        public static void AppliquerCellule(Label label)
        {
            label.Dock = DockStyle.Fill;
            label.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
            label.ForeColor = CouleurTexteSecondaire;
            label.BackColor = Color.White;
            label.TextAlign = ContentAlignment.MiddleLeft;
            label.Padding = new Padding(14, 0, 8, 0);
            label.AutoSize = false;
        }

        // Style du badge origine : Manuel / Automatique.
        public static void AppliquerBadgeOrigine(Label badge, string origine)
        {
            badge.AutoSize = true;
            badge.Text = origine;
            badge.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular);
            badge.TextAlign = ContentAlignment.MiddleCenter;
            badge.Padding = new Padding(8, 3, 8, 3);

            switch (origine)
            {
                case "Manuel":
                    badge.ForeColor = CouleurBleu;
                    badge.BackColor = Color.FromArgb(230, 240, 255);
                    break;

                case "Automatique":
                    badge.ForeColor = CouleurTexteSecondaire;
                    badge.BackColor = Color.FromArgb(242, 242, 242);
                    break;

                default:
                    badge.ForeColor = CouleurTexteSecondaire;
                    badge.BackColor = Color.FromArgb(242, 242, 242);
                    break;
            }
        }

        // Style du badge statut : Succès / Avertissement / Échec.
        public static void AppliquerBadgeStatut(Label badge, string statut)
        {
            badge.AutoSize = true;
            badge.Text = statut;
            badge.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular);
            badge.TextAlign = ContentAlignment.MiddleCenter;
            badge.Padding = new Padding(10, 4, 10, 4);

            switch (statut)
            {
                case "Succès":
                    badge.ForeColor = Color.FromArgb(30, 90, 35);
                    badge.BackColor = Color.FromArgb(225, 242, 220);
                    break;

                case "Avertissement":
                    badge.ForeColor = Color.FromArgb(150, 90, 0);
                    badge.BackColor = Color.FromArgb(255, 240, 215);
                    break;

                case "Échec":
                    badge.ForeColor = Color.FromArgb(150, 35, 35);
                    badge.BackColor = Color.FromArgb(250, 225, 225);
                    break;

                default:
                    badge.ForeColor = CouleurTexteSecondaire;
                    badge.BackColor = Color.FromArgb(242, 242, 242);
                    break;
            }
        }

        // Style du bouton Voir détail.
        public static void AppliquerBoutonDetail(Button bouton)
        {
            bouton.Width = 90;
            bouton.Height = 62;
            bouton.Text = "Voir\ndétail";

            bouton.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
            bouton.ForeColor = CouleurTexte;
            bouton.BackColor = Color.White;

            bouton.FlatStyle = FlatStyle.Flat;
            bouton.FlatAppearance.BorderColor = CouleurBordure;
            bouton.FlatAppearance.BorderSize = 1;

            bouton.Cursor = Cursors.Hand;
        }

        // Style du panel de détail.
        public static void AppliquerPanelDetail(Panel panel)
        {
            panel.Width = 995;
            panel.Height = 260;
            panel.BackColor = Color.White;
            panel.BorderStyle = BorderStyle.FixedSingle;
            panel.Padding = new Padding(16);
            panel.Margin = new Padding(0, 25, 0, 0);
            panel.Visible = false;
        }

        // Style du titre dans le détail.
        public static void AppliquerTitreDetail(Label label)
        {
            label.AutoSize = true;
            label.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            label.ForeColor = CouleurTexte;
            label.Margin = new Padding(0, 0, 0, 14);
        }

        // Style du texte dans le détail.
        public static void AppliquerTexteDetail(Label label)
        {
            label.AutoSize = true;
            label.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
            label.ForeColor = CouleurTexte;
            label.Margin = new Padding(0, 0, 0, 8);
        }

        // Style du bouton pour ouvrir le fichier log complet.
        public static void AppliquerBoutonOuvrirLog(IconButton bouton)
        {
            bouton.Width = 290;
            bouton.Height = 38;

            bouton.Text = "Ouvrir le fichier de log complet";
            bouton.IconChar = IconChar.FolderOpen;
            bouton.IconSize = 18;
            bouton.IconColor = CouleurTexteSecondaire;

            bouton.TextImageRelation = TextImageRelation.ImageBeforeText;
            bouton.ImageAlign = ContentAlignment.MiddleLeft;
            bouton.TextAlign = ContentAlignment.MiddleCenter;

            bouton.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
            bouton.ForeColor = CouleurTexte;
            bouton.BackColor = Color.White;

            bouton.FlatStyle = FlatStyle.Flat;
            bouton.FlatAppearance.BorderColor = CouleurBordure;
            bouton.FlatAppearance.BorderSize = 1;

            bouton.Cursor = Cursors.Hand;
        }
    }
}