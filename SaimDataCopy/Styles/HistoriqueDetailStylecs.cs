using FontAwesome.Sharp;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace SaimDataCopy.Styles
{
    // Style utilisé seulement pour la page Détail de l'exécution.
    // Ne pas mélanger avec HistoriqueStyle.cs.
    public static class HistoriqueDetailStyle
    {
        private static readonly Color BleuPrimaire = ColorTranslator.FromHtml("#1A5FB4");
        private static readonly Color VertAction = ColorTranslator.FromHtml("#3B6D11");
        private static readonly Color BgPrincipal = Color.White;
        private static readonly Color BgSecondaire = ColorTranslator.FromHtml("#EEF2F7");
        private static readonly Color Bordure = ColorTranslator.FromHtml("#CBD5E1");
        private static readonly Color TextePrimaire = ColorTranslator.FromHtml("#1E293B");
        private static readonly Color TexteSecondaire = ColorTranslator.FromHtml("#64748B");
        private static readonly Color Rouge = ColorTranslator.FromHtml("#B91C1C");
        private static readonly Color Orange = ColorTranslator.FromHtml("#B45309");
        private static readonly Color BgConsole = ColorTranslator.FromHtml("#1E1E2E");
        private static readonly Color VertConsole = ColorTranslator.FromHtml("#A3E635");

        public static void AppliquerPage(Panel panel)
        {
            panel.Dock = DockStyle.Fill;
            panel.BackColor = Color.White;
            panel.AutoScroll = true;
            panel.Padding = new Padding(22, 25, 22, 25);
            panel.Visible = false;
        }

        public static void AppliquerTitre(Label label)
        {
            label.AutoSize = true;
            label.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            label.ForeColor = TextePrimaire;
            label.Margin = new Padding(0, 0, 0, 16);
        }

        public static void AppliquerBoutonRetour(IconButton bouton)
        {
            bouton.Width = 230;
            bouton.Height = 34;

            bouton.Text = "Retour à l'historique";
            bouton.IconChar = IconChar.ArrowLeft;
            bouton.IconSize = 16;
            bouton.IconColor = BleuPrimaire;

            bouton.TextImageRelation = TextImageRelation.ImageBeforeText;
            bouton.ImageAlign = ContentAlignment.MiddleLeft;
            bouton.TextAlign = ContentAlignment.MiddleCenter;

            bouton.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular);
            bouton.ForeColor = BleuPrimaire;
            bouton.BackColor = Color.Transparent;

            bouton.FlatStyle = FlatStyle.Flat;
            bouton.FlatAppearance.BorderSize = 0;

            bouton.Cursor = Cursors.Hand;
            bouton.Margin = new Padding(0, 0, 0, 14);
        }

        public static void AppliquerCarteResume(Panel panel)
        {
            panel.Width = 975;
            panel.Height = 76;
            panel.BackColor = Color.White;
            panel.BorderStyle = BorderStyle.None;
            panel.Padding = new Padding(16);
            panel.Margin = new Padding(0, 0, 0, 16);

            AjouterBordureArrondie(panel, Bordure, 4);
        }

        public static void AppliquerTitreResume(Label label)
        {
            label.AutoSize = true;
            label.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            label.ForeColor = TextePrimaire;
        }

        public static void AppliquerSousTitreResume(Label label)
        {
            label.AutoSize = true;
            label.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            label.ForeColor = TexteSecondaire;
        }

        public static void AppliquerBadgeStatut(Label badge, string statut)
        {
            badge.AutoSize = true;
            badge.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            badge.TextAlign = ContentAlignment.MiddleCenter;
            badge.Padding = new Padding(8, 3, 8, 3);

            switch (statut)
            {
                case "Succès":
                    badge.Text = "Succès";
                    badge.ForeColor = ColorTranslator.FromHtml("#166534");
                    badge.BackColor = ColorTranslator.FromHtml("#DCFCE7");
                    break;

                case "Avertissement":
                    badge.Text = "Partiel";
                    badge.ForeColor = ColorTranslator.FromHtml("#854D0E");
                    badge.BackColor = ColorTranslator.FromHtml("#FEF9C3");
                    break;

                case "Échec":
                    badge.Text = "Échec";
                    badge.ForeColor = ColorTranslator.FromHtml("#991B1B");
                    badge.BackColor = ColorTranslator.FromHtml("#FEE2E2");
                    break;

                default:
                    badge.Text = statut;
                    badge.ForeColor = TexteSecondaire;
                    badge.BackColor = Color.White;
                    break;
            }
        }

        public static void AppliquerZoneCartes(FlowLayoutPanel panel)
        {
            panel.Width = 990;
            panel.AutoSize = true;
            panel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            panel.FlowDirection = FlowDirection.LeftToRight;
            panel.WrapContents = true;
            panel.Margin = new Padding(0, 0, 0, 18);
        }

        public static void AppliquerCarteInfo(Panel panel)
        {
            panel.Width = 316;
            panel.Height = 92;
            panel.BackColor = Color.White;
            panel.BorderStyle = BorderStyle.None;
            panel.Padding = new Padding(14);
            panel.Margin = new Padding(0, 0, 10, 12);

            AjouterBordureArrondie(panel, Bordure, 4);
        }

        public static void AppliquerIconeCarte(IconPictureBox icone, IconChar iconChar)
        {
            icone.IconChar = iconChar;
            icone.IconSize = 20;
            icone.IconColor = BleuPrimaire;
            icone.Size = new Size(24, 24);
            icone.Location = new Point(12, 31);
            icone.BackColor = Color.Transparent;
        }

        public static void AppliquerLabelCarte(Label label)
        {
            label.AutoSize = true;
            label.Font = new Font("Segoe UI", 8.5F, FontStyle.Regular);
            label.ForeColor = TexteSecondaire;
            label.Location = new Point(42, 10);
        }

        public static void AppliquerValeurCarte(Label label)
        {
            label.AutoSize = false;
            label.Width = 250;
            label.Height = 46;
            label.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            label.ForeColor = TextePrimaire;
            label.Location = new Point(42, 34);
        }

        public static void AppliquerTitreSection(Label label)
        {
            label.AutoSize = true;
            label.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            label.ForeColor = TexteSecondaire;
            label.Margin = new Padding(0, 0, 0, 8);
        }

        public static void AppliquerCarteBase(Panel panel, string statut)
        {
            panel.Width = 975;
            panel.AutoSize = true;
            panel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            panel.BackColor = Color.White;
            panel.BorderStyle = BorderStyle.None;
            panel.Margin = new Padding(0, 0, 0, 10);
            panel.Padding = new Padding(0);

            AjouterBordureArrondie(panel, Bordure, 4);
        }

        public static void AppliquerEnteteBase(Panel panel, string statut)
        {
            panel.Width = 975;
            panel.Height = 46;
            panel.Margin = new Padding(0);
            panel.Padding = new Padding(0);
            panel.BackColor = ObtenirFondStatut(statut);

            panel.Paint += (sender, e) =>
            {
                using SolidBrush brush = new SolidBrush(ObtenirCouleurStatut(statut));
                e.Graphics.FillRectangle(brush, 0, 0, 4, panel.Height);
            };
        }

        public static void AppliquerTitreBase(Label label)
        {
            label.AutoSize = true;
            label.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            label.ForeColor = TextePrimaire;
            label.Location = new Point(42, 11);
        }

        public static void AppliquerLignesBase(Label label)
        {
            label.AutoSize = false;
            label.Width = 180;
            label.Height = 24;
            label.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            label.ForeColor = TexteSecondaire;
            label.TextAlign = ContentAlignment.MiddleRight;
            label.Location = new Point(720, 8);
        }

        public static void AppliquerMessageErreur(Label label)
        {
            label.AutoSize = false;
            label.Width = 975;
            label.Height = 30;
            label.Font = new Font("Segoe UI", 8.5F, FontStyle.Regular);
            label.ForeColor = Orange;
            label.BackColor = ColorTranslator.FromHtml("#FFFBEB");
            label.Padding = new Padding(12, 7, 12, 7);
            label.Margin = new Padding(0);
        }

        public static void AppliquerZoneLogs(RichTextBox richTextBox)
        {
            richTextBox.Width = 975;
            richTextBox.Height = 140;
            richTextBox.BackColor = BgConsole;
            richTextBox.ForeColor = VertConsole;
            richTextBox.Font = new Font("Consolas", 9F, FontStyle.Regular);
            richTextBox.BorderStyle = BorderStyle.None;
            richTextBox.ReadOnly = true;
            richTextBox.Margin = new Padding(0);
            richTextBox.ScrollBars = RichTextBoxScrollBars.Vertical;
        }

        public static void AppliquerBoutonOuvrirLog(IconButton bouton)
        {
            bouton.Width = 285;
            bouton.Height = 34;

            bouton.Text = "Ouvrir le fichier de log complet";
            bouton.IconChar = IconChar.FolderOpen;
            bouton.IconSize = 16;
            bouton.IconColor = BleuPrimaire;

            bouton.TextImageRelation = TextImageRelation.ImageBeforeText;
            bouton.ImageAlign = ContentAlignment.MiddleLeft;
            bouton.TextAlign = ContentAlignment.MiddleCenter;

            bouton.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            bouton.ForeColor = BleuPrimaire;
            bouton.BackColor = Color.White;

            bouton.FlatStyle = FlatStyle.Flat;
            bouton.FlatAppearance.BorderColor = BleuPrimaire;
            bouton.FlatAppearance.BorderSize = 1;

            bouton.Cursor = Cursors.Hand;
            bouton.Margin = new Padding(0, 6, 10, 0);
        }

        public static void AppliquerBoutonRetourBas(IconButton bouton)
        {
            bouton.Width = 210;
            bouton.Height = 34;

            bouton.Text = "Retour à l'historique";
            bouton.IconChar = IconChar.ArrowLeft;
            bouton.IconSize = 16;
            bouton.IconColor = BleuPrimaire;

            bouton.TextImageRelation = TextImageRelation.ImageBeforeText;
            bouton.ImageAlign = ContentAlignment.MiddleLeft;
            bouton.TextAlign = ContentAlignment.MiddleCenter;

            bouton.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            bouton.ForeColor = BleuPrimaire;
            bouton.BackColor = Color.White;

            bouton.FlatStyle = FlatStyle.Flat;
            bouton.FlatAppearance.BorderColor = BleuPrimaire;
            bouton.FlatAppearance.BorderSize = 1;

            bouton.Cursor = Cursors.Hand;
            bouton.Margin = new Padding(0, 6, 0, 0);
        }

        private static Color ObtenirCouleurStatut(string statut)
        {
            return statut switch
            {
                "Succès" => VertAction,
                "Avertissement" => Orange,
                "Échec" => Rouge,
                _ => TexteSecondaire
            };
        }

        private static Color ObtenirFondStatut(string statut)
        {
            return statut switch
            {
                "Succès" => ColorTranslator.FromHtml("#F0F7EB"),
                "Avertissement" => ColorTranslator.FromHtml("#FEF3C7"),
                "Échec" => ColorTranslator.FromHtml("#FEF2F2"),
                _ => BgSecondaire
            };
        }

        private static void AjouterBordureArrondie(Control controle, Color couleurBordure, int rayon)
        {
            controle.Paint += (sender, e) =>
            {
                using Pen pen = new Pen(couleurBordure, 1);
                Rectangle rectangle = new Rectangle(
                    0,
                    0,
                    controle.Width - 1,
                    controle.Height - 1
                );

                using GraphicsPath chemin = new GraphicsPath();
                chemin.AddArc(rectangle.X, rectangle.Y, rayon * 2, rayon * 2, 180, 90);
                chemin.AddArc(rectangle.Right - rayon * 2, rectangle.Y, rayon * 2, rayon * 2, 270, 90);
                chemin.AddArc(rectangle.Right - rayon * 2, rectangle.Bottom - rayon * 2, rayon * 2, rayon * 2, 0, 90);
                chemin.AddArc(rectangle.X, rectangle.Bottom - rayon * 2, rayon * 2, rayon * 2, 90, 90);
                chemin.CloseFigure();

                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                e.Graphics.DrawPath(pen, chemin);
            };
        }
    }
}