using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace SaimDataCopy.Styles
{
    public static class TableGridStyle
    {
        public static void AppliquerStyle(DataGridView grille)
        {
            // Style général du tableau
            grille.BorderStyle = BorderStyle.FixedSingle;
            grille.BackgroundColor = Color.White;
            grille.GridColor = Color.FromArgb(225, 225, 225);

            // Bordures plus proches de la maquette
            grille.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            grille.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;

            grille.EnableHeadersVisualStyles = false;

            // En-tête du tableau
            grille.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(248, 248, 248);
            grille.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(20, 20, 20);
            grille.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Regular);
            grille.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            grille.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.False;
            grille.ColumnHeadersDefaultCellStyle.Padding = new Padding(4, 0, 0, 0);

            // Empêche l'effet bleu sur l'en-tête
            grille.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(248, 248, 248);
            grille.ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.FromArgb(20, 20, 20);

            // Hauteur de l'en-tête
            grille.ColumnHeadersHeight = 42;
            grille.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

            // Style des cellules
            grille.DefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Regular);
            grille.DefaultCellStyle.BackColor = Color.White;
            grille.DefaultCellStyle.ForeColor = Color.FromArgb(35, 35, 35);
            grille.DefaultCellStyle.SelectionBackColor = Color.White;
            grille.DefaultCellStyle.SelectionForeColor = Color.FromArgb(35, 35, 35);
            grille.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            grille.DefaultCellStyle.Padding = new Padding(8, 0, 8, 0);

            // Ligne alternée légèrement grise
            grille.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 248, 248);
            grille.AlternatingRowsDefaultCellStyle.ForeColor = Color.FromArgb(35, 35, 35);
            grille.AlternatingRowsDefaultCellStyle.SelectionBackColor = Color.FromArgb(248, 248, 248);
            grille.AlternatingRowsDefaultCellStyle.SelectionForeColor = Color.FromArgb(35, 35, 35);

            // Options du tableau
            grille.RowHeadersVisible = false;
            grille.RowTemplate.Height = 60;

            grille.AllowUserToAddRows = false;
            grille.AllowUserToDeleteRows = false;
            grille.AllowUserToResizeRows = false;
            grille.AllowUserToResizeColumns = false;

            // CellSelect évite l'effet bleu sur toute la ligne
            grille.SelectionMode = DataGridViewSelectionMode.CellSelect;
            grille.MultiSelect = false;

            grille.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        public static void DesactiverTriColonnes(DataGridView grille)
        {
            foreach (DataGridViewColumn colonne in grille.Columns)
            {
                colonne.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
        }

        public static void FormaterCelluleStatut(DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }

            DataGridViewCellStyle? style = e.CellStyle;

            if (style == null)
            {
                return;
            }

            // On garde le fond normal de la ligne.
            // Le badge coloré sera dessiné dans CellPainting.
            style.BackColor = e.RowIndex % 2 == 0
                ? Color.White
                : Color.FromArgb(248, 248, 248);

            style.SelectionBackColor = style.BackColor;

            // On cache le texte normal pour éviter qu'il apparaisse derrière le badge.
            style.ForeColor = Color.Transparent;
            style.SelectionForeColor = Color.Transparent;
        }

        public static void DessinerCellulesSpeciales(DataGridView grille, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
            {
                return;
            }

            if (e.ColumnIndex >= grille.Columns.Count)
            {
                return;
            }

            DataGridViewColumn? colonne = grille.Columns[e.ColumnIndex];

            if (colonne == null)
            {
                return;
            }

            switch (colonne.Name)
            {
                case "colOrdre":
                    DessinerCelluleOrdre(e);
                    break;

                case "colStatut":
                    DessinerBadgeStatut(grille, e);
                    break;
            }
        }

        private static void DessinerCelluleOrdre(DataGridViewCellPaintingEventArgs e)
        {
            Graphics? graphics = e.Graphics;

            if (graphics == null)
            {
                return;
            }

            Color couleurFond = e.RowIndex % 2 == 0
                ? Color.White
                : Color.FromArgb(248, 248, 248);

            using SolidBrush fondLigne = new SolidBrush(couleurFond);
            graphics.FillRectangle(fondLigne, e.CellBounds);

            using Pen ligne = new Pen(Color.FromArgb(225, 225, 225));
            graphics.DrawLine(
                ligne,
                e.CellBounds.Left,
                e.CellBounds.Bottom - 1,
                e.CellBounds.Right,
                e.CellBounds.Bottom - 1
            );

            // Petit champ comme dans la maquette
            Rectangle champ = new Rectangle(
                e.CellBounds.X + 10,
                e.CellBounds.Y + 13,
                65,
                34
            );

            using SolidBrush fondChamp = new SolidBrush(Color.White);
            using Pen bordureChamp = new Pen(Color.FromArgb(215, 215, 215));

            graphics.FillRectangle(fondChamp, champ);
            graphics.DrawRectangle(bordureChamp, champ);

            string texte = e.Value?.ToString() ?? string.Empty;

            Rectangle zoneTexte = new Rectangle(
                champ.X + 10,
                champ.Y,
                champ.Width - 20,
                champ.Height
            );

            using Font police = new Font("Segoe UI", 10, FontStyle.Regular);

            TextRenderer.DrawText(
                graphics,
                texte,
                police,
                zoneTexte,
                Color.FromArgb(35, 35, 35),
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter
            );

            e.Handled = true;
        }

        public static void DessinerBadgeStatut(DataGridView grille, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
            {
                return;
            }

            if (e.ColumnIndex >= grille.Columns.Count)
            {
                return;
            }

            DataGridViewColumn? colonne = grille.Columns[e.ColumnIndex];

            if (colonne == null || colonne.Name != "colStatut")
            {
                return;
            }

            Graphics? graphics = e.Graphics;

            if (graphics == null)
            {
                return;
            }

            string statut = e.Value?.ToString() ?? string.Empty;

            Color couleurFond;
            Color couleurTexte;
            string texte;

            if (statut.Contains("Prête"))
            {
                couleurFond = Color.FromArgb(230, 245, 225);
                couleurTexte = Color.FromArgb(40, 120, 50);
                texte = "✓ Prête";
            }
            else if (statut.Contains("Avertissement"))
            {
                couleurFond = Color.FromArgb(255, 242, 220);
                couleurTexte = Color.FromArgb(180, 105, 0);
                texte = "▲ Avertissement";
            }
            else if (statut.Contains("Non sélectionnée"))
            {
                couleurFond = Color.FromArgb(240, 235, 250);
                couleurTexte = Color.FromArgb(80, 80, 80);
                texte = "■ Non sélectionnée";
            }
            else
            {
                return;
            }

            Color couleurLigne = e.RowIndex % 2 == 0
                ? Color.White
                : Color.FromArgb(248, 248, 248);

            using SolidBrush fondLigne = new SolidBrush(couleurLigne);
            graphics.FillRectangle(fondLigne, e.CellBounds);

            using Pen ligne = new Pen(Color.FromArgb(225, 225, 225));
            graphics.DrawLine(
                ligne,
                e.CellBounds.Left,
                e.CellBounds.Bottom - 1,
                e.CellBounds.Right,
                e.CellBounds.Bottom - 1
            );

            using Font police = new Font("Segoe UI", 9, FontStyle.Regular);

            Size tailleTexte = TextRenderer.MeasureText(texte, police);

            Rectangle badge = new Rectangle(
                e.CellBounds.X + 12,
                e.CellBounds.Y + (e.CellBounds.Height - 28) / 2,
                tailleTexte.Width + 22,
                28
            );

            using GraphicsPath chemin = CreerRectangleArrondi(badge, 5);
            using SolidBrush fond = new SolidBrush(couleurFond);

            graphics.FillPath(fond, chemin);

            Rectangle zoneTexte = new Rectangle(
                badge.X + 10,
                badge.Y,
                badge.Width - 20,
                badge.Height
            );

            TextRenderer.DrawText(
                graphics,
                texte,
                police,
                zoneTexte,
                couleurTexte,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter
            );

            e.Handled = true;
        }

        private static GraphicsPath CreerRectangleArrondi(Rectangle rectangle, int rayon)
        {
            GraphicsPath chemin = new GraphicsPath();

            int diametre = rayon * 2;

            chemin.AddArc(rectangle.X, rectangle.Y, diametre, diametre, 180, 90);
            chemin.AddArc(rectangle.Right - diametre, rectangle.Y, diametre, diametre, 270, 90);
            chemin.AddArc(rectangle.Right - diametre, rectangle.Bottom - diametre, diametre, diametre, 0, 90);
            chemin.AddArc(rectangle.X, rectangle.Bottom - diametre, diametre, diametre, 90, 90);

            chemin.CloseFigure();

            return chemin;
        }
    }
}