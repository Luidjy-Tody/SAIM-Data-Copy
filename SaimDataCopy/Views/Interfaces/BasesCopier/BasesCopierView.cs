using SaimDataCopy.Controllers;
using SaimDataCopy.Helpers;
using SaimDataCopy.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace SaimDataCopy.Views.Interfaces.BasesCopier
{
    public class BasesCopierView : UserControl, IBasesCopierView
    {
        private readonly BasesCopierController _controller;

        private readonly Label lblTitre = new Label();
        private readonly Panel panelInfo = new Panel();
        private readonly Label lblInfo = new Label();

        private readonly DataGridView grilleBases = new DataGridView();

        private readonly Panel panelBoutons = new Panel();


        private readonly Button btnAjouter = new Button();
        private readonly Button btnSupprimer = new Button();

        private List<string> _modesCopie = new List<string>();

        public event EventHandler? AjouterBaseDemandee;
        public event EventHandler? SupprimerSelectionDemandee;

        public BasesCopierView()
        {
            InitialiserInterface();

            // Le Controller reçoit cette View.
            // Ensuite il charge les données et écoute les boutons.
            _controller = new BasesCopierController(this);
        }

        private void InitialiserInterface()
        {
            Dock = DockStyle.Fill;
            BackColor = Color.White;

            Panel panelContenu = new Panel();
            panelContenu.Dock = DockStyle.Fill;
            panelContenu.Padding = new Padding(24, 26, 24, 24);
            panelContenu.AutoScroll = true;
            panelContenu.BackColor = Color.White;
            Controls.Add(panelContenu);

            lblTitre.Text = "Bases de données à traiter";
            lblTitre.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            lblTitre.ForeColor = Color.Black;
            lblTitre.AutoSize = true;
            lblTitre.Location = new Point(24, 26);
            panelContenu.Controls.Add(lblTitre);

            panelInfo.Location = new Point(24, 72);
            panelInfo.Size = new Size(995, 48);
            panelInfo.BackColor = Color.FromArgb(235, 242, 255);
            panelContenu.Controls.Add(panelInfo);

            lblInfo.Text = "ⓘ   Cochez les bases à inclure. L'ordre de traitement peut être modifié.";
            lblInfo.Font = new Font("Segoe UI", 10, FontStyle.Regular);
            lblInfo.ForeColor = Color.Black;
            lblInfo.AutoSize = true;
            lblInfo.Location = new Point(16, 14);
            panelInfo.Controls.Add(lblInfo);

            CreerGrille();
            panelContenu.Controls.Add(grilleBases);
            panelBoutons.Location = new Point(24, 445);
            panelBoutons.Size = new Size(460, 45);
            panelBoutons.BackColor = Color.White;
            panelContenu.Controls.Add(panelBoutons);

            btnAjouter.Text = "+  Ajouter une base";
            btnAjouter.Font = new Font("Segoe UI", 10, FontStyle.Regular);
            btnAjouter.Size = new Size(185, 38);
            btnAjouter.Location = new Point(0, 0);
            btnAjouter.FlatStyle = FlatStyle.Flat;
            btnAjouter.FlatAppearance.BorderColor = Color.FromArgb(210, 210, 210);
            btnAjouter.BackColor = Color.White;
            btnAjouter.Cursor = Cursors.Hand;
            btnAjouter.Click += BtnAjouter_Click;
            panelBoutons.Controls.Add(btnAjouter);

            btnSupprimer.Text = "Supprimer la sélection";
            btnSupprimer.Font = new Font("Segoe UI", 10, FontStyle.Regular);
            btnSupprimer.Size = new Size(225, 38);
            btnSupprimer.Location = new Point(198, 0);
            btnSupprimer.FlatStyle = FlatStyle.Flat;
            btnSupprimer.FlatAppearance.BorderColor = Color.FromArgb(210, 210, 210);
            btnSupprimer.BackColor = Color.White;
            btnSupprimer.Cursor = Cursors.Hand;
            btnSupprimer.Click += BtnSupprimer_Click;
            panelBoutons.Controls.Add(btnSupprimer);
        }

        private void CreerGrille()
        {
            grilleBases.Location = new Point(24, 145);
            grilleBases.Width = 995;

            // On enlève le scroll interne du tableau.
            // Si le tableau devient grand, c'est le panel principal qui va scroller.
            grilleBases.ScrollBars = ScrollBars.None;

            // Permet d'entrer directement en édition avec un seul clic.
            grilleBases.EditMode = DataGridViewEditMode.EditOnEnter;

            TableGridStyle.AppliquerStyle(grilleBases);

            DataGridViewCheckBoxColumn colInclure = new DataGridViewCheckBoxColumn();
            colInclure.Name = "colInclure";
            colInclure.HeaderText = "Inclure";
            colInclure.FillWeight = 70;

            DataGridViewTextBoxColumn colNomBase = new DataGridViewTextBoxColumn();
            colNomBase.Name = "colNomBase";
            colNomBase.HeaderText = "Nom de la base";
            // Le nom reste modifiable si l'utilisateur s'est trompé.
            colNomBase.ReadOnly = false;
            colNomBase.FillWeight = 150;

            DataGridViewTextBoxColumn colOrdre = new DataGridViewTextBoxColumn();
            colOrdre.Name = "colOrdre";
            colOrdre.HeaderText = "Ordre de traitement";
            colOrdre.FillWeight = 140;

            DataGridViewComboBoxColumn colMode = new DataGridViewComboBoxColumn();
            colMode.Name = "colMode";
            colMode.HeaderText = "Mode de copie";
            colMode.FillWeight = 150;

            // Style plus proche de la maquette.
            colMode.DisplayStyle = DataGridViewComboBoxDisplayStyle.ComboBox;
            colMode.DisplayStyleForCurrentCellOnly = false;
            colMode.FlatStyle = FlatStyle.Flat;

            // Évite l'effet bleu trop grand quand on clique sur la cellule.
            colMode.DefaultCellStyle.BackColor = Color.White;
            colMode.DefaultCellStyle.ForeColor = Color.Black;
            colMode.DefaultCellStyle.SelectionBackColor = Color.White;
            colMode.DefaultCellStyle.SelectionForeColor = Color.Black;

            DataGridViewTextBoxColumn colStatut = new DataGridViewTextBoxColumn();
            colStatut.Name = "colStatut";
            colStatut.HeaderText = "Statut";
            colStatut.ReadOnly = true;
            colStatut.FillWeight = 160;

            DataGridViewTextBoxColumn colDerniereCopie = new DataGridViewTextBoxColumn();
            colDerniereCopie.Name = "colDerniereCopie";
            colDerniereCopie.HeaderText = "Dernière copie";
            colDerniereCopie.ReadOnly = true;
            colDerniereCopie.FillWeight = 180;

            grilleBases.Columns.AddRange(
                colInclure,
                colNomBase,
                colOrdre,
                colMode,
                colStatut,
                colDerniereCopie
            );

            TableGridStyle.DesactiverTriColonnes(grilleBases);

            grilleBases.CellFormatting += GrilleBases_CellFormatting;
            grilleBases.CurrentCellDirtyStateChanged += GrilleBases_CurrentCellDirtyStateChanged;
            grilleBases.CellValueChanged += GrilleBases_CellValueChanged;

            // Vérifie que l'ordre de traitement est bien un nombre.
            grilleBases.CellValidating += GrilleBases_CellValidating;

            // Améliore l'affichage de la ComboBox du mode de copie.
            grilleBases.EditingControlShowing += GrilleBases_EditingControlShowing;

            // Permet d'ouvrir directement la liste du mode de copie avec un seul clic.
            grilleBases.CellClick += GrilleBases_CellClick;

            // Évite les erreurs si une valeur ComboBox n'est pas encore prête.
            grilleBases.DataError += (s, e) => e.ThrowException = false;
        }

        public void AfficherModesCopie(List<string> modesCopie)
        {
            _modesCopie = modesCopie;

            if (grilleBases.Columns["colMode"] is DataGridViewComboBoxColumn colMode)
            {
                colMode.Items.Clear();

                foreach (string mode in _modesCopie)
                {
                    colMode.Items.Add(mode);
                }
            }
        }

        public void AfficherBases(List<BaseCopieModel> bases)
        {
            grilleBases.Rows.Clear();

            foreach (BaseCopieModel baseCopie in bases)
            {
                int index = grilleBases.Rows.Add(
                    baseCopie.Inclure,
                    baseCopie.NomBase,
                    baseCopie.OrdreTraitement,
                    baseCopie.ModeCopie,
                    ObtenirStatutAffiche(baseCopie.Statut),
                    baseCopie.DerniereCopie.HasValue
                        ? baseCopie.DerniereCopie.Value.ToString("dd/MM/yyyy HH:mm")
                        : "—"
                );

                grilleBases.Rows[index].Tag = baseCopie;
            }

            AjusterHauteurTableau();
        }

        private void AjusterHauteurTableau()
        {
            int hauteurEntete = grilleBases.ColumnHeadersHeight;
            int hauteurLignes = grilleBases.Rows.Count * grilleBases.RowTemplate.Height;

            // Petite marge pour éviter que la dernière ligne soit coupée.
            int margeTableau = 3;

            grilleBases.Height = hauteurEntete + hauteurLignes + margeTableau;

            // Position commune pour les deux boutons.
            int positionBoutonsY = grilleBases.Bottom + 18;

            // Hauteur fixe pour éviter une différence visuelle.
            int hauteurBouton = 38;

            panelBoutons.Location = new Point(24, positionBoutonsY);
            panelBoutons.Size = new Size(460, 45);

            btnAjouter.SetBounds(0, 0, 185, hauteurBouton);
            btnSupprimer.SetBounds(198, 0, 225, hauteurBouton);
        }

        public string? DemanderNomNouvelleBase()
        {
            Form fenetre = new Form();
            fenetre.Text = "Ajouter une base";
            fenetre.StartPosition = FormStartPosition.CenterScreen;
            fenetre.FormBorderStyle = FormBorderStyle.FixedDialog;
            fenetre.MaximizeBox = false;
            fenetre.MinimizeBox = false;
            fenetre.ClientSize = new Size(380, 150);
            fenetre.BackColor = Color.White;

            Label lblNom = new Label();
            lblNom.Text = "Nom de la base :";
            lblNom.Font = new Font("Segoe UI", 10, FontStyle.Regular);
            lblNom.Location = new Point(20, 20);
            lblNom.AutoSize = true;

            TextBox txtNom = new TextBox();
            txtNom.Font = new Font("Segoe UI", 10, FontStyle.Regular);
            txtNom.Location = new Point(20, 50);
            txtNom.Width = 330;

            Button btnOk = new Button();
            btnOk.Text = "Ajouter";
            btnOk.Size = new Size(100, 32);
            btnOk.Location = new Point(145, 95);
            btnOk.DialogResult = DialogResult.OK;

            Button btnAnnuler = new Button();
            btnAnnuler.Text = "Annuler";
            btnAnnuler.Size = new Size(100, 32);
            btnAnnuler.Location = new Point(250, 95);
            btnAnnuler.DialogResult = DialogResult.Cancel;

            fenetre.Controls.Add(lblNom);
            fenetre.Controls.Add(txtNom);
            fenetre.Controls.Add(btnOk);
            fenetre.Controls.Add(btnAnnuler);

            fenetre.AcceptButton = btnOk;
            fenetre.CancelButton = btnAnnuler;

            DialogResult resultat = fenetre.ShowDialog();

            if (resultat != DialogResult.OK)
            {
                return null;
            }

            return txtNom.Text.Trim();
        }

        public List<BaseCopieModel> RecupererBases()
        {
            List<BaseCopieModel> bases = new List<BaseCopieModel>();

            foreach (DataGridViewRow row in grilleBases.Rows)
            {
                if (row.Tag is not BaseCopieModel ancienneBase)
                {
                    continue;
                }

                bool inclure = Convert.ToBoolean(row.Cells["colInclure"].Value);

                int.TryParse(
                    row.Cells["colOrdre"].Value?.ToString(),
                    out int ordreTraitement
                );

                string modeCopie = row.Cells["colMode"].Value?.ToString() ?? "Écraser";

                BaseCopieModel baseCopie = new BaseCopieModel
                {
                    Inclure = inclure,
                    NomBase = row.Cells["colNomBase"].Value?.ToString()?.Trim() ?? string.Empty,
                    OrdreTraitement = ordreTraitement,
                    ModeCopie = modeCopie,
                    Statut = inclure ? ancienneBase.Statut : "Non sélectionnée",
                    DerniereCopie = ancienneBase.DerniereCopie
                };

                bases.Add(baseCopie);
            }

            return bases;
        }

        public List<string> RecupererNomsBasesCochees()
        {
            List<string> noms = new List<string>();
            foreach (DataGridViewRow row in grilleBases.Rows)
            {
                bool estCoche = Convert.ToBoolean(row.Cells["colInclure"].Value);

                if (!estCoche)
                {
                    continue;
                }

                string? nomBase = row.Cells["colNomBase"].Value?.ToString();

                if (!string.IsNullOrWhiteSpace(nomBase))
                {
                    noms.Add(nomBase.Trim());
                }
            }

            return noms;
        }

        public void DemanderEnregistrement()
        {
            _controller.Enregistrer();
        }

        public void AfficherMessage(string titre, string message, MessageBoxIcon icon)
        {
            MessageBox.Show(
                message,
                titre,
                MessageBoxButtons.OK,
                icon
            );
        }

        private string ObtenirStatutAffiche(string statut)
        {
            return statut switch
            {
                "Prête" => "✓ Prête",
                "Avertissement" => "▲ Avertissement",
                "Non sélectionnée" => "■ Non sélectionnée",
                _ => statut
            };
        }

        private void BtnAjouter_Click(object? sender, EventArgs e)
        {
            AjouterBaseDemandee?.Invoke(this, EventArgs.Empty);
        }

        private void BtnSupprimer_Click(object? sender, EventArgs e)
        {
            SupprimerSelectionDemandee?.Invoke(this, EventArgs.Empty);
        }

        private void GrilleBases_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }

            if (grilleBases.Columns[e.ColumnIndex].Name == "colStatut")
            {
                TableGridStyle.FormaterCelluleStatut(e);
            }
        }

        private void GrilleBases_CurrentCellDirtyStateChanged(object? sender, EventArgs e)
        {
            if (grilleBases.IsCurrentCellDirty)
            {
                grilleBases.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        private void GrilleBases_CellValueChanged(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }

            if (grilleBases.Columns[e.ColumnIndex].Name != "colInclure")
            {
                return;
            }

            DataGridViewRow row = grilleBases.Rows[e.RowIndex];
            bool inclure = Convert.ToBoolean(row.Cells["colInclure"].Value);

            if (!inclure)
            {
                row.Cells["colStatut"].Value = "■ Non sélectionnée";
            }
            else if (row.Cells["colStatut"].Value?.ToString()?.Contains("Non sélectionnée") == true)
            {
                row.Cells["colStatut"].Value = "✓ Prête";
            }
        }

        private void GrilleBases_CellValidating(object? sender, DataGridViewCellValidatingEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }

            if (grilleBases.Columns[e.ColumnIndex].Name != "colOrdre")
            {
                return;
            }

            string valeur = e.FormattedValue?.ToString() ?? string.Empty;

            if (!int.TryParse(valeur, out int ordre) || ordre <= 0)
            {
                MessageBox.Show(
                    "L'ordre de traitement doit être un nombre supérieur à 0.",
                    "Validation",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                e.Cancel = true;
            }
        }

        private void GrilleBases_EditingControlShowing(
    object? sender,
    DataGridViewEditingControlShowingEventArgs e)
        {
            if (grilleBases.CurrentCell == null)
            {
                return;
            }

            if (grilleBases.CurrentCell.OwningColumn.Name != "colMode")
            {
                return;
            }

            if (e.Control is ComboBox comboBox)
            {
                comboBox.DropDownStyle = ComboBoxStyle.DropDownList;
                comboBox.FlatStyle = FlatStyle.Flat;
                comboBox.BackColor = Color.White;
                comboBox.ForeColor = Color.Black;
            }
        }

        private void GrilleBases_CellClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }

            DataGridViewColumn? colonne = grilleBases.Columns[e.ColumnIndex];

            if (colonne?.Name != "colMode")
            {
                return;
            }

            // On force la cellule cliquée à devenir la cellule active.
            grilleBases.CurrentCell = grilleBases.Rows[e.RowIndex].Cells[e.ColumnIndex];

            // On lance l'édition directement.
            grilleBases.BeginEdit(true);

            // Si la cellule est une ComboBox, on ouvre directement la liste.
            if (grilleBases.EditingControl is ComboBox comboBox)
            {
                comboBox.DroppedDown = true;
            }
        }

    }
}