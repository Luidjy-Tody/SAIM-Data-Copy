using System.Drawing;
using System.Windows.Forms;

namespace SaimDataCopy.Helpers
{
    public static class TableGridStyle
    {
        public static void AppliquerStyle(DataGridView grille)
        {
            // Style général du tableau
            grille.BorderStyle = BorderStyle.FixedSingle;
            grille.BackgroundColor = Color.White;
            grille.GridColor = Color.FromArgb(220, 220, 220);

            // Style de l'en-tête
            grille.EnableHeadersVisualStyles = false;
            grille.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(250, 250, 250);
            grille.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
            grille.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Regular);

            // Permet d'afficher les longs titres sur plusieurs lignes.
            // Exemple : "Ordre de traitement"
            grille.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.True;

            // Aligne le texte comme dans la maquette.
            grille.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            // Empêche l'effet bleu quand on clique sur un titre de colonne.
            grille.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(250, 250, 250);
            grille.ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.Black;

            // Hauteur fixe de l'en-tête.
            // L'utilisateur n'aura plus besoin de tirer vers le bas.
            grille.ColumnHeadersHeight = 58;

            // Empêche l'utilisateur de redimensionner la hauteur de l'en-tête.
            grille.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

            // Style des cellules
            grille.DefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Regular);
            grille.DefaultCellStyle.BackColor = Color.White;
            grille.DefaultCellStyle.ForeColor = Color.FromArgb(40, 40, 40);
            grille.DefaultCellStyle.SelectionBackColor = Color.FromArgb(225, 235, 250);
            grille.DefaultCellStyle.SelectionForeColor = Color.Black;

            // Ligne alternée légèrement grise
            grille.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 248, 248);

            // Options du tableau
            grille.RowHeadersVisible = false;
            grille.RowTemplate.Height = 58;

            grille.AllowUserToAddRows = false;
            grille.AllowUserToDeleteRows = false;
            grille.AllowUserToResizeRows = false;
            // Empêche aussi le redimensionnement manuel des colonnes.
            grille.AllowUserToResizeColumns = false;

            grille.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grille.MultiSelect = true;

            grille.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        public static void DesactiverTriColonnes(DataGridView grille)
        {
            foreach (DataGridViewColumn colonne in grille.Columns)
            {
                // Les titres deviennent fixes.
                // Cliquer dessus ne changera plus l'ordre des lignes.
                colonne.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
        }
        public static void FormaterCelluleStatut(DataGridViewCellFormattingEventArgs e)
        {
            if (e.Value == null)
            {
                return;
            }

            if (e.CellStyle == null)
            {
                return;
            }

            string statut = e.Value.ToString() ?? string.Empty;

            // Couleur pour le statut "Prête"
            if (statut.Contains("Prête"))
            {
                e.CellStyle.BackColor = Color.FromArgb(230, 245, 225);
                e.CellStyle.ForeColor = Color.FromArgb(45, 120, 50);
            }
            // Couleur pour le statut "Avertissement"
            else if (statut.Contains("Avertissement"))
            {
                e.CellStyle.BackColor = Color.FromArgb(255, 242, 220);
                e.CellStyle.ForeColor = Color.FromArgb(180, 105, 0);
            }
            // Couleur pour le statut "Non sélectionnée"
            else if (statut.Contains("Non sélectionnée"))
            {
                e.CellStyle.BackColor = Color.FromArgb(240, 235, 250);
                e.CellStyle.ForeColor = Color.FromArgb(80, 80, 80);
            }

            e.CellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }
    }
}