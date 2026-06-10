using FontAwesome.Sharp;
using SaimDataCopy.Models.BasesCopier;
using SaimDataCopy.Styles;
using System.Drawing;
using System.Windows.Forms;
using SaimDataCopy.Views.Commun;

namespace SaimDataCopy.Views.BasesCopier
{
    public class BasesCopierView : UserControl, IBasesCopierView, IPageEnregistrable
    {
        private const int MargePage = 24;
        private const int LargeurMinimumContenu = 900;

        private readonly Panel panelContenu = new Panel();

        private readonly Label lblTitre = new Label();
        private readonly Panel panelInfo = new Panel();
        private readonly IconPictureBox iconeInfo = new IconPictureBox();
        private readonly Label lblInfo = new Label();

        private readonly DataGridView grilleBases = new DataGridView();

        private readonly Panel panelBoutons = new Panel();

        private readonly IconButton btnCocherTout = new IconButton();
        private readonly IconButton btnSupprimer = new IconButton();

        private List<string> _modesCopie = new List<string>();

        // Indique si l'utilisateur a modifié la page sans enregistrer.
        private bool _aDesModificationsNonEnregistrees = false;

        // Évite de détecter des modifications pendant le chargement du tableau.
        private bool _chargementEnCours = false;

        // Dernier état connu comme enregistré.
        private List<BaseCopieModel> _basesEnregistrees = new List<BaseCopieModel>();

        public bool ADesModificationsNonEnregistrees => _aDesModificationsNonEnregistrees;

        public event EventHandler? CocherToutesBasesDemandee;
        public event EventHandler? DecocherToutesBasesDemandee;

        public BasesCopierView()
        {
            InitialiserInterface();
        }

        private void InitialiserInterface()
        {
            Dock = DockStyle.Fill;
            BackColor = Color.White;

            panelContenu.Dock = DockStyle.Fill;
            panelContenu.Padding = new Padding(MargePage, 26, MargePage, 24);
            panelContenu.AutoScroll = true;
            panelContenu.BackColor = Color.White;
            Controls.Add(panelContenu);

            lblTitre.Text = "Bases de données à traiter";
            lblTitre.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            lblTitre.ForeColor = Color.Black;
            lblTitre.AutoSize = true;
            lblTitre.Location = new Point(MargePage, 26);
            panelContenu.Controls.Add(lblTitre);

            panelInfo.Location = new Point(MargePage, 72);
            panelInfo.Height = 48;
            panelInfo.BackColor = Color.FromArgb(235, 242, 255);
            panelContenu.Controls.Add(panelInfo);

            iconeInfo.IconChar = IconChar.CircleInfo;
            iconeInfo.IconColor = Color.FromArgb(30, 96, 190);
            iconeInfo.IconSize = 18;
            iconeInfo.Size = new Size(22, 22);
            iconeInfo.Location = new Point(16, 14);
            iconeInfo.BackColor = panelInfo.BackColor;
            panelInfo.Controls.Add(iconeInfo);

            lblInfo.Text = "Les bases sont chargées depuis le serveur source. Cochez seulement celles à copier.";
            lblInfo.Font = new Font("Segoe UI", 10, FontStyle.Regular);
            lblInfo.ForeColor = Color.Black;
            lblInfo.AutoSize = false;
            lblInfo.Height = 24;
            lblInfo.TextAlign = ContentAlignment.MiddleLeft;
            lblInfo.Location = new Point(48, 12);
            lblInfo.BackColor = panelInfo.BackColor;
            panelInfo.Controls.Add(lblInfo);

            CreerGrille();
            panelContenu.Controls.Add(grilleBases);

            panelBoutons.BackColor = Color.White;
            panelContenu.Controls.Add(panelBoutons);

            ConfigurerBoutons();

            panelContenu.Resize += (sender, e) =>
            {
                AdapterDisposition();
            };

            AdapterDisposition();
        }

        private void ConfigurerBoutons()
        {
            btnCocherTout.Text = " Cocher toutes les bases";
            btnCocherTout.IconChar = IconChar.CheckSquare;
            btnCocherTout.IconSize = 18;
            btnCocherTout.IconColor = Color.FromArgb(40, 40, 40);
            btnCocherTout.TextImageRelation = TextImageRelation.ImageBeforeText;
            btnCocherTout.Font = new Font("Segoe UI", 10, FontStyle.Regular);
            btnCocherTout.Size = new Size(260, 38);
            btnCocherTout.Location = new Point(0, 0);
            btnCocherTout.TextAlign = ContentAlignment.MiddleCenter;
            btnCocherTout.ImageAlign = ContentAlignment.MiddleLeft;
            btnCocherTout.FlatStyle = FlatStyle.Flat;
            btnCocherTout.FlatAppearance.BorderColor = Color.FromArgb(210, 210, 210);
            btnCocherTout.FlatAppearance.BorderSize = 1;
            btnCocherTout.BackColor = Color.White;
            btnCocherTout.Cursor = Cursors.Hand;
            btnCocherTout.Click += BtnCocherTout_Click;
            panelBoutons.Controls.Add(btnCocherTout);

            btnSupprimer.Text = " Decocher toutes les bases";
            btnSupprimer.IconChar = IconChar.CircleMinus;
            btnSupprimer.IconSize = 18;
            btnSupprimer.IconColor = Color.FromArgb(40, 40, 40);
            btnSupprimer.TextImageRelation = TextImageRelation.ImageBeforeText;
            btnSupprimer.Font = new Font("Segoe UI", 10, FontStyle.Regular);
            btnSupprimer.Size = new Size(240, 38);
            btnSupprimer.Location = new Point(275, 0);
            btnSupprimer.TextAlign = ContentAlignment.MiddleCenter;
            btnSupprimer.ImageAlign = ContentAlignment.MiddleLeft;
            btnSupprimer.FlatStyle = FlatStyle.Flat;
            btnSupprimer.FlatAppearance.BorderColor = Color.FromArgb(210, 210, 210);
            btnSupprimer.FlatAppearance.BorderSize = 1;
            btnSupprimer.BackColor = Color.White;
            btnSupprimer.Cursor = Cursors.Hand;
            btnSupprimer.Click += BtnSupprimer_Click;
            panelBoutons.Controls.Add(btnSupprimer);
        }

        private void CreerGrille()
        {
            grilleBases.Location = new Point(MargePage, 145);
            grilleBases.ScrollBars = ScrollBars.Vertical;
            grilleBases.EditMode = DataGridViewEditMode.EditOnEnter;

            TableGridStyle.AppliquerStyle(grilleBases);

            DataGridViewCheckBoxColumn colInclure = new DataGridViewCheckBoxColumn();
            colInclure.Name = "colInclure";
            colInclure.HeaderText = "Inclure";
            colInclure.FillWeight = 90;
            colInclure.MinimumWidth = 90;

            DataGridViewTextBoxColumn colNomBase = new DataGridViewTextBoxColumn();
            colNomBase.Name = "colNomBase";
            colNomBase.HeaderText = "Nom de la base";
            colNomBase.ReadOnly = true;
            colNomBase.FillWeight = 170;

            DataGridViewTextBoxColumn colOrdre = new DataGridViewTextBoxColumn();
            colOrdre.Name = "colOrdre";
            colOrdre.HeaderText = "Ordre de traitement";
            colOrdre.FillWeight = 180;

            DataGridViewComboBoxColumn colMode = new DataGridViewComboBoxColumn();
            colMode.Name = "colMode";
            colMode.HeaderText = "Mode de copie";
            colMode.FillWeight = 170;
            colMode.DisplayStyle = DataGridViewComboBoxDisplayStyle.DropDownButton;
            colMode.DisplayStyleForCurrentCellOnly = false;
            colMode.FlatStyle = FlatStyle.Flat;

            colMode.DefaultCellStyle.BackColor = Color.White;
            colMode.DefaultCellStyle.ForeColor = Color.Black;
            colMode.DefaultCellStyle.SelectionBackColor = Color.White;
            colMode.DefaultCellStyle.SelectionForeColor = Color.Black;

            DataGridViewTextBoxColumn colStatut = new DataGridViewTextBoxColumn();
            colStatut.Name = "colStatut";
            colStatut.HeaderText = "Statut";
            colStatut.ReadOnly = true;
            colStatut.FillWeight = 200;

            DataGridViewTextBoxColumn colDerniereCopie = new DataGridViewTextBoxColumn();
            colDerniereCopie.Name = "colDerniereCopie";
            colDerniereCopie.HeaderText = "Dernière copie";
            colDerniereCopie.ReadOnly = true;
            colDerniereCopie.FillWeight = 165;

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
            grilleBases.CellPainting += GrilleBases_CellPainting;
            grilleBases.CurrentCellDirtyStateChanged += GrilleBases_CurrentCellDirtyStateChanged;
            grilleBases.CellValueChanged += GrilleBases_CellValueChanged;
            grilleBases.CellValidating += GrilleBases_CellValidating;
            grilleBases.EditingControlShowing += GrilleBases_EditingControlShowing;
            grilleBases.CellClick += GrilleBases_CellClick;

            grilleBases.DataError += (s, e) => e.ThrowException = false;
        }

        private void AdapterDisposition()
        {
            int largeurContenu = panelContenu.ClientSize.Width - (MargePage * 2);

            if (largeurContenu < LargeurMinimumContenu)
            {
                largeurContenu = LargeurMinimumContenu;
            }

            panelInfo.Width = largeurContenu;

            lblInfo.Width = panelInfo.Width - lblInfo.Left - 15;

            grilleBases.Width = largeurContenu;

            AjusterHauteurTableau();

            panelBoutons.Width = largeurContenu;
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
            _chargementEnCours = true;

            try
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

                    AppliquerStyleLigne(index);
                }

                AjusterHauteurTableau();
                grilleBases.ClearSelection();
            }
            finally
            {
                _chargementEnCours = false;
            }
        }

        private void AppliquerStyleLigne(int index)
        {
            DataGridViewRow row = grilleBases.Rows[index];

            Color couleurLigne = index % 2 == 0
                ? Color.White
                : Color.FromArgb(248, 248, 248);

            row.DefaultCellStyle.BackColor = couleurLigne;
            row.DefaultCellStyle.SelectionBackColor = couleurLigne;
            row.DefaultCellStyle.SelectionForeColor = Color.FromArgb(35, 35, 35);

            row.Cells["colInclure"].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            row.Cells["colNomBase"].Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
            row.Cells["colOrdre"].Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
            row.Cells["colMode"].Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
            row.Cells["colStatut"].Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
            row.Cells["colDerniereCopie"].Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
        }

        private void AjusterHauteurTableau()
        {
            int hauteurEntete = grilleBases.ColumnHeadersHeight;
            int hauteurLignes = grilleBases.Rows.Count * grilleBases.RowTemplate.Height;
            int margeTableau = 3;

            int hauteurNecessaire = hauteurEntete + hauteurLignes + margeTableau;

            int hauteurDisponible = panelContenu.ClientSize.Height
                - panelContenu.Padding.Top
                - panelContenu.Padding.Bottom
                - grilleBases.Top
                - 80;

            if (hauteurDisponible < 220)
            {
                hauteurDisponible = 220;
            }

            int hauteurFinale = hauteurNecessaire;

            if (hauteurFinale < 220)
            {
                hauteurFinale = 220;
            }

            if (hauteurFinale > hauteurDisponible)
            {
                hauteurFinale = hauteurDisponible;
            }

            grilleBases.Height = hauteurFinale;

            int positionBoutonsY = grilleBases.Bottom + 18;
            int hauteurBouton = 38;

            panelBoutons.Location = new Point(MargePage, positionBoutonsY);
            panelBoutons.Size = new Size(grilleBases.Width, 45);

            btnCocherTout.SetBounds(0, 0, 260, hauteurBouton);
            btnSupprimer.SetBounds(275, 0, 240, hauteurBouton);
        }

        /// <summary>
        /// Appelé par le Controller après un chargement ou un enregistrement réussi.
        /// </summary>
        public void MarquerCommeEnregistre()
        {
            _basesEnregistrees = CopierBases(RecupererBases());
            _aDesModificationsNonEnregistrees = false;
        }

        /// <summary>
        /// Appelé si l'utilisateur choisit "Non".
        /// On remet les bases comme elles étaient lors du dernier enregistrement.
        /// </summary>
        public void AnnulerModificationsNonEnregistrees()
        {
            AfficherBases(CopierBases(_basesEnregistrees));
            _aDesModificationsNonEnregistrees = false;
        }

        /// <summary>
        /// Marque la page comme modifiée si le chargement n'est pas en cours.
        /// </summary>
        private void MarquerCommeModifie()
        {
            if (_chargementEnCours)
            {
                return;
            }

            _aDesModificationsNonEnregistrees = true;
        }

        /// <summary>
        /// Copie une liste de bases pour éviter de garder les mêmes références en mémoire.
        /// </summary>
        private List<BaseCopieModel> CopierBases(List<BaseCopieModel> bases)
        {
            return bases
                .Select(CopierBase)
                .ToList();
        }

        /// <summary>
        /// Copie une base à copier.
        /// </summary>
        private BaseCopieModel CopierBase(BaseCopieModel baseCopie)
        {
            return new BaseCopieModel
            {
                Inclure = baseCopie.Inclure,
                NomBase = baseCopie.NomBase,
                OrdreTraitement = baseCopie.OrdreTraitement,
                ModeCopie = baseCopie.ModeCopie,
                Statut = baseCopie.Statut,
                DerniereCopie = baseCopie.DerniereCopie,
                ExisteSurServeurSource = baseCopie.ExisteSurServeurSource,
                NomModifiable = baseCopie.NomModifiable
            };
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
                    DerniereCopie = ancienneBase.DerniereCopie,
                    ExisteSurServeurSource = ancienneBase.ExisteSurServeurSource,
                    NomModifiable = false
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

        private void BtnCocherTout_Click(object? sender, EventArgs e)
        {
            bool auMoinsUneBaseDecochee = RecupererBases()
                .Any(baseCopie => !baseCopie.Inclure);

            CocherToutesBasesDemandee?.Invoke(this, EventArgs.Empty);

            if (auMoinsUneBaseDecochee)
            {
                MarquerCommeModifie();
            }
        }

        private void BtnSupprimer_Click(object? sender, EventArgs e)
        {
            bool auMoinsUneBaseCochee = RecupererBases()
                .Any(baseCopie => baseCopie.Inclure);

            DecocherToutesBasesDemandee?.Invoke(this, EventArgs.Empty);

            if (auMoinsUneBaseCochee)
            {
                MarquerCommeModifie();
            }
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

        private void GrilleBases_CellPainting(object? sender, DataGridViewCellPaintingEventArgs e)
        {
            TableGridStyle.DessinerCellulesSpeciales(grilleBases, e);
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

            MarquerCommeModifie();

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

            grilleBases.Invalidate();
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
                comboBox.Font = new Font("Segoe UI", 10, FontStyle.Regular);
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

            grilleBases.CurrentCell = grilleBases.Rows[e.RowIndex].Cells[e.ColumnIndex];
            grilleBases.BeginEdit(true);

            if (grilleBases.EditingControl is ComboBox comboBox)
            {
                comboBox.DroppedDown = true;
            }
        }
    }
}