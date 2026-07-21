using FontAwesome.Sharp;
using SaimDataCopy.Models.JournalActivite;
using SaimDataCopy.Styles;
using System.Drawing;
using System.Windows.Forms;

namespace SaimDataCopy.Views.JournalActivite
{
    /// <summary>
    /// Interface graphique de la page Journal d'activité.
    /// </summary>
    public class JournalActiviteView : UserControl
    {
        public event EventHandler? RechercheDemandee;
        public event EventHandler? ActualisationDemandee;
        public event EventHandler? ReinitialisationDemandee;

        private readonly FlowLayoutPanel contenuPage;
        private readonly FlowLayoutPanel zoneEntete;

        private readonly IconButton btnActualiser;

        private readonly Panel carteJournal;
        private readonly Panel zoneFiltres;

        private readonly TextBox txtRecherche;
        private readonly ComboBox cboAction;

        private readonly IconButton btnRechercher;
        private readonly Button btnReinitialiser;

        private readonly DataGridView dgvJournal;

        private readonly Label lblNombreResultats;
        private readonly Label lblMessage;

        public JournalActiviteView()
        {
            contenuPage = new FlowLayoutPanel();
            zoneEntete = new FlowLayoutPanel();

            btnActualiser = new IconButton();

            carteJournal = new Panel();
            zoneFiltres = new Panel();

            txtRecherche = new TextBox();
            cboAction = new ComboBox();

            btnRechercher = new IconButton();
            btnReinitialiser = new Button();

            dgvJournal = new DataGridView();

            lblNombreResultats = new Label();
            lblMessage = new Label();

            JournalActiviteStyle.AppliquerPage(this);

            ConstruireInterface();
        }

        private void ConstruireInterface()
        {
            contenuPage.Dock = DockStyle.Fill;
            contenuPage.AutoScroll = true;
            contenuPage.FlowDirection = FlowDirection.TopDown;
            contenuPage.WrapContents = false;
            contenuPage.Padding = new Padding(22, 25, 22, 25);
            contenuPage.Margin = new Padding(0);
            contenuPage.BackColor = Color.White;

            ConstruireEntete();
            ConstruireCarteJournal();

            contenuPage.Controls.Add(zoneEntete);
            contenuPage.Controls.Add(carteJournal);

            Controls.Add(contenuPage);

            contenuPage.Resize += ContenuPage_Resize;

            AdapterDisposition();
        }

        private void ConstruireEntete()
        {
            zoneEntete.AutoSize = false;
            zoneEntete.Height = 80;
            zoneEntete.FlowDirection = FlowDirection.LeftToRight;
            zoneEntete.WrapContents = false;
            zoneEntete.Margin = new Padding(0, 0, 0, 15);
            zoneEntete.Padding = new Padding(0);
            zoneEntete.BackColor = Color.White;

            Panel blocTitres = new Panel
            {
                Height = 80,
                Margin = new Padding(0)
            };

            Label lblTitre = new Label
            {
                Text = "Journal d'activité",
                Location = new Point(0, 0)
            };

            JournalActiviteStyle.AppliquerTitre(lblTitre);

            Label lblSousTitre = new Label
            {
                Text = "Consultation des actions importantes réalisées dans l'application.",
                Location = new Point(0, 37)
            };

            JournalActiviteStyle.AppliquerSousTitre(lblSousTitre);

            blocTitres.Controls.Add(lblTitre);
            blocTitres.Controls.Add(lblSousTitre);

            Panel blocBouton = new Panel
            {
                Height = 80,
                Margin = new Padding(0)
            };

            JournalActiviteStyle.AppliquerBoutonActualiser(btnActualiser);

            btnActualiser.Click += BtnActualiser_Click;

            blocBouton.Controls.Add(btnActualiser);

            zoneEntete.Controls.Add(blocTitres);
            zoneEntete.Controls.Add(blocBouton);
        }

        private void ConstruireCarteJournal()
        {
            carteJournal.Tag = "CarteJournal";
            carteJournal.Height = 650;

            JournalActiviteStyle.AppliquerCartePrincipale(carteJournal);

            Label lblTitreSection = new Label
            {
                Text = "Historique des actions",
                Location = new Point(18, 18)
            };

            JournalActiviteStyle.AppliquerTitreSection(lblTitreSection);

            lblNombreResultats.Text = "0 résultat";
            lblNombreResultats.Location = new Point(18, 47);

            JournalActiviteStyle.AppliquerMessageInformation(
                lblNombreResultats
            );

            zoneFiltres.Tag = "ZoneFiltresJournal";
            zoneFiltres.Height = 80;
            zoneFiltres.Location = new Point(18, 78);

            ConstruireFiltres();

            dgvJournal.Location = new Point(18, 175);
            dgvJournal.Height = 420;

            JournalActiviteStyle.AppliquerTableauJournal(dgvJournal);

            ConfigurerColonnesJournal();

            lblMessage.Location = new Point(18, 610);
            lblMessage.AutoSize = true;
            lblMessage.Visible = false;

            JournalActiviteStyle.AppliquerMessageInformation(lblMessage);

            carteJournal.Controls.Add(lblTitreSection);
            carteJournal.Controls.Add(lblNombreResultats);
            carteJournal.Controls.Add(zoneFiltres);
            carteJournal.Controls.Add(dgvJournal);
            carteJournal.Controls.Add(lblMessage);
        }

        private void ConstruireFiltres()
        {
            Label lblRecherche = new Label
            {
                Text = "Recherche",
                Location = new Point(0, 0)
            };

            JournalActiviteStyle.AppliquerLibelleFiltre(lblRecherche);

            txtRecherche.Location = new Point(0, 27);
            txtRecherche.Width = 340;
            txtRecherche.PlaceholderText =
                "Utilisateur, action ou détails";

            JournalActiviteStyle.AppliquerChampRecherche(txtRecherche);

            Label lblAction = new Label
            {
                Text = "Type d'action",
                Location = new Point(370, 0)
            };

            JournalActiviteStyle.AppliquerLibelleFiltre(lblAction);

            cboAction.Location = new Point(370, 27);
            cboAction.Width = 260;

            JournalActiviteStyle.AppliquerComboBoxFiltre(cboAction);

            cboAction.Items.AddRange(
                new object[]
                {
                    "Toutes les actions",
                    "Connexion",
                    "Déconnexion",
                    "Création utilisateur",
                    "Modification utilisateur",
                    "Activation utilisateur",
                    "Désactivation utilisateur",
                    "Suppression utilisateur",
                    "Enregistrement Configuration",
                    "Enregistrement Bases à copier",
                    "Enregistrement Paramètres Email",
                    "Enregistrement Paramètres Logs",
                    "Lancement de la copie"
                }
            );

            cboAction.SelectedIndex = 0;

            btnRechercher.Location = new Point(660, 27);

            JournalActiviteStyle.AppliquerBoutonRechercher(btnRechercher);

            btnReinitialiser.Location = new Point(800, 27);

            JournalActiviteStyle.AppliquerBoutonReinitialiser(
                btnReinitialiser
            );

            btnRechercher.Click += BtnRechercher_Click;
            btnReinitialiser.Click += BtnReinitialiser_Click;

            txtRecherche.KeyDown += TxtRecherche_KeyDown;

            zoneFiltres.Controls.Add(lblRecherche);
            zoneFiltres.Controls.Add(txtRecherche);
            zoneFiltres.Controls.Add(lblAction);
            zoneFiltres.Controls.Add(cboAction);
            zoneFiltres.Controls.Add(btnRechercher);
            zoneFiltres.Controls.Add(btnReinitialiser);
        }

        private void ConfigurerColonnesJournal()
        {
            dgvJournal.Columns.Clear();

            dgvJournal.Columns.Add(
                new DataGridViewTextBoxColumn
                {
                    Name = "Id",
                    HeaderText = "Id",
                    Visible = false
                }
            );

            dgvJournal.Columns.Add(
                new DataGridViewTextBoxColumn
                {
                    Name = "DateHeure",
                    HeaderText = "Date / Heure",
                    FillWeight = 85,
                    Resizable = DataGridViewTriState.False,
                    SortMode = DataGridViewColumnSortMode.NotSortable
                }
            );

            dgvJournal.Columns.Add(
                new DataGridViewTextBoxColumn
                {
                    Name = "NomUtilisateur",
                    HeaderText = "Utilisateur",
                    FillWeight = 75,
                    Resizable = DataGridViewTriState.False,
                    SortMode = DataGridViewColumnSortMode.NotSortable
                }
            );

            dgvJournal.Columns.Add(
                new DataGridViewTextBoxColumn
                {
                    Name = "Action",
                    HeaderText = "Action",
                    FillWeight = 110,
                    Resizable = DataGridViewTriState.False,
                    SortMode = DataGridViewColumnSortMode.NotSortable
                }
            );

            dgvJournal.Columns.Add(
                new DataGridViewTextBoxColumn
                {
                    Name = "Details",
                    HeaderText = "Détails",
                    FillWeight = 230,
                    Resizable = DataGridViewTriState.False,
                    SortMode = DataGridViewColumnSortMode.NotSortable
                }
            );

            dgvJournal.CellFormatting -= DgvJournal_CellFormatting;
            dgvJournal.CellFormatting += DgvJournal_CellFormatting;

            dgvJournal.CellDoubleClick -= DgvJournal_CellDoubleClick;
            dgvJournal.CellDoubleClick += DgvJournal_CellDoubleClick;
        }

        public void AfficherJournal(List<JournalActiviteModel> logs)
        {
            dgvJournal.Rows.Clear();

            foreach (JournalActiviteModel log in logs)
            {
                dgvJournal.Rows.Add(
                    log.Id,
                    log.DateHeure.ToString("dd/MM/yyyy HH:mm:ss"),
                    string.IsNullOrWhiteSpace(log.NomUtilisateur)
                        ? "Utilisateur inconnu"
                        : log.NomUtilisateur,
                    log.Action,
                    log.Details
                );
            }

            lblNombreResultats.Text = logs.Count switch
            {
                0 => "Aucun résultat",
                1 => "1 résultat",
                _ => $"{logs.Count} résultats"
            };

            lblMessage.Visible = logs.Count == 0;

            if (logs.Count == 0)
            {
                lblMessage.Text =
                    "Aucune action ne correspond aux critères sélectionnés.";
            }

            DefinirChargement(false);
        }

        public void AfficherChargement()
        {
            dgvJournal.Rows.Clear();

            lblNombreResultats.Text = "Chargement...";
            lblMessage.Text = "Chargement du journal d'activité...";
            lblMessage.Visible = true;

            DefinirChargement(true);
        }

        public void AfficherErreur()
        {
            dgvJournal.Rows.Clear();

            lblNombreResultats.Text = "Erreur";
            lblMessage.Text =
                "Impossible de charger le journal d'activité.";
            lblMessage.Visible = true;

            DefinirChargement(false);
        }

        public string ObtenirRecherche()
        {
            return txtRecherche.Text.Trim();
        }

        public string ObtenirActionSelectionnee()
        {
            return cboAction.SelectedItem?.ToString()
                ?? "Toutes les actions";
        }

        public void ReinitialiserFiltres()
        {
            txtRecherche.Clear();
            cboAction.SelectedIndex = 0;
            txtRecherche.Focus();
        }

        public void AfficherMessage(
            string message,
            string titre,
            MessageBoxIcon icone)
        {
            MessageBox.Show(
                message,
                titre,
                MessageBoxButtons.OK,
                icone
            );
        }

        private void DefinirChargement(bool chargement)
        {
            btnActualiser.Enabled = !chargement;
            btnRechercher.Enabled = !chargement;
            btnReinitialiser.Enabled = !chargement;
            txtRecherche.Enabled = !chargement;
            cboAction.Enabled = !chargement;
        }

        private void BtnActualiser_Click(
            object? sender,
            EventArgs e)
        {
            ActualisationDemandee?.Invoke(
                this,
                EventArgs.Empty
            );
        }

        private void BtnRechercher_Click(
            object? sender,
            EventArgs e)
        {
            RechercheDemandee?.Invoke(
                this,
                EventArgs.Empty
            );
        }

        private void BtnReinitialiser_Click(
            object? sender,
            EventArgs e)
        {
            ReinitialiserFiltres();

            ReinitialisationDemandee?.Invoke(
                this,
                EventArgs.Empty
            );
        }

        private void TxtRecherche_KeyDown(
            object? sender,
            KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
            {
                return;
            }

            RechercheDemandee?.Invoke(
                this,
                EventArgs.Empty
            );

            e.SuppressKeyPress = true;
        }

        private void DgvJournal_CellFormatting(
            object? sender,
            DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 ||
                e.ColumnIndex < 0 ||
                e.Value == null)
            {
                return;
            }

            string nomColonne =
                dgvJournal.Columns[e.ColumnIndex].Name;

            if (nomColonne != "Action")
            {
                return;
            }

            DataGridViewCellStyle? styleCellule = e.CellStyle;

            if (styleCellule == null)
            {
                return;
            }

            string action = e.Value.ToString() ?? string.Empty;

            JournalActiviteStyle.AppliquerAction(
                styleCellule,
                action
            );
        }

        private void DgvJournal_CellDoubleClick(
            object? sender,
            DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }

            DataGridViewRow ligne =
                dgvJournal.Rows[e.RowIndex];

            string dateHeure =
                ligne.Cells["DateHeure"].Value?.ToString()
                ?? string.Empty;

            string utilisateur =
                ligne.Cells["NomUtilisateur"].Value?.ToString()
                ?? string.Empty;

            string action =
                ligne.Cells["Action"].Value?.ToString()
                ?? string.Empty;

            string details =
                ligne.Cells["Details"].Value?.ToString()
                ?? string.Empty;

            AfficherDetailsJournal(
                dateHeure,
                utilisateur,
                action,
                details
            );
        }

        private static void AfficherDetailsJournal(
            string dateHeure,
            string utilisateur,
            string action,
            string details)
        {
            Form fenetreDetails = new Form
            {
                Text = "Détails de l'action",
                StartPosition = FormStartPosition.CenterParent,
                Size = new Size(750, 560),
                MinimumSize = new Size(650, 480),
                BackColor = Color.White,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false
            };

            Label lblAction = new Label
            {
                Text = action,
                Location = new Point(22, 20),
                AutoSize = true,
                Font = new Font(
                    "Segoe UI",
                    14F,
                    FontStyle.Bold
                ),
                ForeColor = Color.FromArgb(30, 30, 30)
            };

            Label lblUtilisateur = new Label
            {
                Text = $"Utilisateur : {utilisateur}",
                Location = new Point(24, 65),
                AutoSize = true,
                Font = new Font(
                    "Segoe UI",
                    9.5F,
                    FontStyle.Regular
                ),
                ForeColor = Color.FromArgb(80, 80, 80)
            };

            Label lblDate = new Label
            {
                Text = $"Date : {dateHeure}",
                Location = new Point(24, 90),
                AutoSize = true,
                Font = new Font(
                    "Segoe UI",
                    9.5F,
                    FontStyle.Regular
                ),
                ForeColor = Color.FromArgb(80, 80, 80)
            };

            Label lblTitreDetails = new Label
            {
                Text = "Détails",
                Location = new Point(22, 130),
                AutoSize = true,
                Font = new Font(
                    "Segoe UI",
                    10F,
                    FontStyle.Bold
                ),
                ForeColor = Color.FromArgb(30, 30, 30)
            };

            TextBox txtDetails = new TextBox
            {
                Text = details,
                Location = new Point(24, 160),
                Width = 685,
                Height = 300,
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Both,
                WordWrap = true,
                Font = new Font(
                    "Segoe UI",
                    10F,
                    FontStyle.Regular
                ),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Anchor =
                    AnchorStyles.Top |
                    AnchorStyles.Bottom |
                    AnchorStyles.Left |
                    AnchorStyles.Right
            };

            Button btnFermer = new Button
            {
                Text = "Fermer",
                Width = 110,
                Height = 36,
                Location = new Point(
                    fenetreDetails.ClientSize.Width - 134,
                    fenetreDetails.ClientSize.Height - 55
                ),
                BackColor = Color.FromArgb(30, 96, 190),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Anchor =
                    AnchorStyles.Bottom |
                    AnchorStyles.Right
            };

            btnFermer.FlatAppearance.BorderSize = 0;

            btnFermer.Click += (sender, e) =>
            {
                fenetreDetails.Close();
            };

            fenetreDetails.Controls.Add(lblAction);
            fenetreDetails.Controls.Add(lblUtilisateur);
            fenetreDetails.Controls.Add(lblDate);
            fenetreDetails.Controls.Add(lblTitreDetails);
            fenetreDetails.Controls.Add(txtDetails);
            fenetreDetails.Controls.Add(btnFermer);

            fenetreDetails.ShowDialog();
        }

        private void ContenuPage_Resize(
            object? sender,
            EventArgs e)
        {
            AdapterDisposition();
        }

        private void AdapterDisposition()
        {
            int largeurDisponible =
                contenuPage.ClientSize.Width
                - contenuPage.Padding.Left
                - contenuPage.Padding.Right
                - SystemInformation.VerticalScrollBarWidth;

            if (largeurDisponible < 900)
            {
                largeurDisponible = 900;
            }

            zoneEntete.Width = largeurDisponible;
            carteJournal.Width = largeurDisponible;

            AdapterEntete(largeurDisponible);

            zoneFiltres.Width = largeurDisponible - 36;
            dgvJournal.Width = largeurDisponible - 36;
        }

        private void AdapterEntete(
            int largeurDisponible)
        {
            if (zoneEntete.Controls.Count < 2)
            {
                return;
            }

            Control blocTitres = zoneEntete.Controls[0];
            Control blocBouton = zoneEntete.Controls[1];

            blocBouton.Width = btnActualiser.Width;

            blocTitres.Width =
                largeurDisponible
                - blocBouton.Width;

            btnActualiser.Location = new Point(
                blocBouton.Width - btnActualiser.Width,
                5
            );
        }
    }
}