using FontAwesome.Sharp;
using SaimDataCopy.Models.EspaceAdmin;
using SaimDataCopy.Styles;
using System.Drawing;
using System.Windows.Forms;
using SaimDataCopy.Models.Authentification;

namespace SaimDataCopy.Views.EspaceAdmin
{
    /// <summary>
    /// Interface graphique de la page Espace Admin.
    /// </summary>
    public class EspaceAdminView : UserControl
    {
        public event EventHandler? ActualisationDemandee;
        public event EventHandler? RechercheUtilisateursDemandee;
        public event EventHandler? NouvelUtilisateurDemande;
        public event Action<int>? ModificationUtilisateurDemandee;
        public event Action<int>? ActivationUtilisateurDemandee;
        public event Action<int>? SuppressionUtilisateurDemandee;
        public event EventHandler? EnregistrementUtilisateurDemande;
        public event EventHandler? AnnulationFormulaireDemandee;

        private readonly FlowLayoutPanel contenuPage;
        private readonly FlowLayoutPanel zoneEntete;
        private readonly FlowLayoutPanel zoneCartes;

        private readonly Label lblTotalUtilisateurs;
        private readonly Label lblComptesActifs;
        private readonly Label lblComptesInactifs;
        private readonly Label lblAdministrateurs;

        private readonly IconButton btnActualiser;

        private readonly Panel carteUtilisateurs;
        private readonly TextBox txtRechercheUtilisateur;
        private readonly ComboBox cboStatutUtilisateur;
        private readonly ComboBox cboEtatUtilisateur;
        private readonly IconButton btnRechercherUtilisateurs;
        private readonly IconButton btnNouvelUtilisateur;
        private readonly DataGridView dgvUtilisateurs;

        private readonly Panel carteFormulaire;
        private readonly Label lblTitreFormulaire;
        private readonly TextBox txtNomComplet;
        private readonly TextBox txtIdentifiant;
        private readonly TextBox txtEmail;
        private readonly TextBox txtMotDePasse;
        private readonly TextBox txtConfirmationMotDePasse;
        private readonly ComboBox cboStatutFormulaire;
        private readonly ComboBox cboEtatFormulaire;
        private readonly IconButton btnEnregistrerUtilisateur;
        private readonly Button btnReinitialiserFormulaire;
        private readonly Button btnAnnulerFormulaire;
        private readonly Label lblMessageFormulaire;

        private int? idUtilisateurSelectionne;

        private readonly Panel panelToast;
        private readonly Label lblToast;
        private readonly System.Windows.Forms.Timer timerToast;

        private readonly Image iconeModifier;
        private readonly Image iconeActiver;
        private readonly Image iconeDesactiver;
        private readonly Image iconeSupprimer;
        private readonly Label lblInfoBulleAction;

        public EspaceAdminView()
        {
            EspaceAdminStyle.AppliquerPage(this);

            contenuPage = new FlowLayoutPanel();
            zoneEntete = new FlowLayoutPanel();
            zoneCartes = new FlowLayoutPanel();

            lblTotalUtilisateurs = new Label();
            lblComptesActifs = new Label();
            lblComptesInactifs = new Label();
            lblAdministrateurs = new Label();

            btnActualiser = new IconButton();

            carteUtilisateurs = new Panel();
            txtRechercheUtilisateur = new TextBox();
            cboStatutUtilisateur = new ComboBox();
            cboEtatUtilisateur = new ComboBox();
            btnRechercherUtilisateurs = new IconButton();
            btnNouvelUtilisateur = new IconButton();
            dgvUtilisateurs = new DataGridView();

            carteFormulaire = new Panel();
            lblTitreFormulaire = new Label();
            txtNomComplet = new TextBox();
            txtIdentifiant = new TextBox();
            txtEmail = new TextBox();
            txtMotDePasse = new TextBox();
            txtConfirmationMotDePasse = new TextBox();
            cboStatutFormulaire = new ComboBox();
            cboEtatFormulaire = new ComboBox();
            btnEnregistrerUtilisateur = new IconButton();
            btnReinitialiserFormulaire = new Button();
            btnAnnulerFormulaire = new Button();
            lblMessageFormulaire = new Label();

            panelToast = new Panel();
            lblToast = new Label();
            timerToast = new System.Windows.Forms.Timer();
            lblInfoBulleAction = new Label();

            iconeModifier = IconChar.PenToSquare.ToBitmap(Color.FromArgb(30, 96, 190), 18);
            iconeActiver = IconChar.UserCheck.ToBitmap(Color.FromArgb(45, 130, 70), 18);
            iconeDesactiver = IconChar.UserSlash.ToBitmap(Color.FromArgb(180, 90, 20), 18);
            iconeSupprimer = IconChar.Trash.ToBitmap(Color.FromArgb(180, 45, 45), 18);

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
            ConstruireCartesStatistiques();
            ConstruireCarteUtilisateurs();

            contenuPage.Controls.Add(zoneEntete);
            contenuPage.Controls.Add(zoneCartes);
            contenuPage.Controls.Add(carteUtilisateurs);

            Controls.Add(contenuPage);

            ConstruireToast();

            Controls.Add(panelToast);
            panelToast.BringToFront();

            contenuPage.Resize += (sender, e) =>
            {
                AdapterDisposition();
            };

            ConstruireCarteFormulaire();
            contenuPage.Controls.Add(carteFormulaire);
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

            Panel blocTitres = new Panel();
            blocTitres.Height = 80;
            blocTitres.Margin = new Padding(0);

            Label lblTitre = new Label();
            lblTitre.Text = "Espace Admin";
            lblTitre.Location = new Point(0, 0);
            EspaceAdminStyle.AppliquerTitre(lblTitre);

            Label lblSousTitre = new Label();
            lblSousTitre.Text =
                "Gestion des utilisateurs, des rôles et de l’accès à l’application.";

            lblSousTitre.Location = new Point(0, 37);
            EspaceAdminStyle.AppliquerSousTitre(lblSousTitre);

            blocTitres.Controls.Add(lblTitre);
            blocTitres.Controls.Add(lblSousTitre);

            Panel blocBouton = new Panel();
            blocBouton.Height = 80;
            blocBouton.Margin = new Padding(0);

            EspaceAdminStyle.AppliquerBoutonActualiser(
                btnActualiser
            );

            btnActualiser.Click += (sender, e) =>
            {
                ActualisationDemandee?.Invoke(
                    this,
                    EventArgs.Empty
                );
            };

            blocBouton.Controls.Add(btnActualiser);

            zoneEntete.Controls.Add(blocTitres);
            zoneEntete.Controls.Add(blocBouton);
        }

        private void ConstruireCartesStatistiques()
        {
            EspaceAdminStyle.AppliquerZoneCartes(
                zoneCartes
            );

            Panel carteTotal = CreerCarteStatistique(
                IconChar.Users,
                Color.FromArgb(30, 96, 190),
                "Total utilisateurs",
                lblTotalUtilisateurs
            );

            Panel carteActifs = CreerCarteStatistique(
                IconChar.UserCheck,
                Color.FromArgb(45, 130, 70),
                "Comptes actifs",
                lblComptesActifs
            );

            Panel carteInactifs = CreerCarteStatistique(
                IconChar.UserSlash,
                Color.FromArgb(180, 90, 20),
                "Comptes inactifs",
                lblComptesInactifs
            );

            Panel carteAdministrateurs =
                CreerCarteStatistique(
                    IconChar.UserShield,
                    Color.FromArgb(115, 70, 180),
                    "Administrateurs",
                    lblAdministrateurs
                );

            zoneCartes.Controls.Add(carteTotal);
            zoneCartes.Controls.Add(carteActifs);
            zoneCartes.Controls.Add(carteInactifs);
            zoneCartes.Controls.Add(carteAdministrateurs);
        }

        private void ConstruireCarteUtilisateurs()
        {
            carteUtilisateurs.Tag = "CarteUtilisateurs";
            carteUtilisateurs.Height = 520;

            EspaceAdminStyle.AppliquerCartePrincipale(carteUtilisateurs);

            Label lblTitre = new Label();
            lblTitre.Text = "Utilisateurs";
            lblTitre.Location = new Point(18, 18);
            EspaceAdminStyle.AppliquerTitreSection(lblTitre);

            Panel zoneFiltres = new Panel();
            zoneFiltres.Tag = "ZoneFiltresUtilisateurs";
            zoneFiltres.Height = 75;
            zoneFiltres.Location = new Point(18, 58);

            CreerFiltreUtilisateurs(zoneFiltres);

            dgvUtilisateurs.Location = new Point(18, 145);
            dgvUtilisateurs.Height = 350;
            EspaceAdminStyle.AppliquerTableauUtilisateurs(dgvUtilisateurs);

            ConfigurerColonnesUtilisateurs();
            ConfigurerInfoBulleActions();

            carteUtilisateurs.Controls.Add(lblTitre);
            carteUtilisateurs.Controls.Add(zoneFiltres);
            carteUtilisateurs.Controls.Add(dgvUtilisateurs);
            carteUtilisateurs.Controls.Add(lblInfoBulleAction);

            lblInfoBulleAction.BringToFront();
        }

        private void ConfigurerInfoBulleActions()
        {
            lblInfoBulleAction.AutoSize = false;
            lblInfoBulleAction.Height = 30;
            lblInfoBulleAction.BackColor = Color.FromArgb(45, 45, 48);
            lblInfoBulleAction.ForeColor = Color.White;
            lblInfoBulleAction.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            lblInfoBulleAction.TextAlign = ContentAlignment.MiddleCenter;
            lblInfoBulleAction.Padding = new Padding(8, 0, 8, 0);
            lblInfoBulleAction.BorderStyle = BorderStyle.FixedSingle;
            lblInfoBulleAction.Visible = false;
        }

        private void ConstruireToast()
        {
            panelToast.Width = 360;
            panelToast.Height = 52;
            panelToast.BackColor = Color.FromArgb(225, 242, 220);
            panelToast.BorderStyle = BorderStyle.FixedSingle;
            panelToast.Visible = false;
            panelToast.Anchor = AnchorStyles.Top | AnchorStyles.Right;

            lblToast.Dock = DockStyle.Fill;
            lblToast.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            lblToast.ForeColor = Color.FromArgb(30, 100, 40);
            lblToast.TextAlign = ContentAlignment.MiddleLeft;
            lblToast.Padding = new Padding(16, 0, 12, 0);

            panelToast.Controls.Add(lblToast);

            timerToast.Interval = 3500;
            timerToast.Tick += TimerToast_Tick;

            Resize += (sender, e) =>
            {
                PositionnerToast();
            };

            PositionnerToast();
        }

        private void PositionnerToast()
        {
            panelToast.Location = new Point(ClientSize.Width - panelToast.Width - 25, 20);
        }

        private void TimerToast_Tick(object? sender, EventArgs e)
        {
            timerToast.Stop();
            panelToast.Visible = false;
        }

        private void CreerFiltreUtilisateurs(Panel zoneFiltres)
        {
            Label lblRecherche = new Label();
            lblRecherche.Text = "Recherche";
            lblRecherche.Location = new Point(0, 0);
            lblRecherche.AutoSize = true;
            lblRecherche.Font = new Font("Segoe UI", 9F, FontStyle.Regular);

            txtRechercheUtilisateur.Location = new Point(0, 27);
            txtRechercheUtilisateur.Width = 300;
            txtRechercheUtilisateur.PlaceholderText = "Rechercher par nom, identifiant ou email";
            EspaceAdminStyle.AppliquerChampRecherche(txtRechercheUtilisateur);

            Label lblStatut = new Label();
            lblStatut.Text = "Statut";
            lblStatut.Location = new Point(330, 0);
            lblStatut.AutoSize = true;
            lblStatut.Font = new Font("Segoe UI", 9F, FontStyle.Regular);

            cboStatutUtilisateur.Location = new Point(330, 27);
            cboStatutUtilisateur.Items.AddRange(new object[] { "Tous", "Admin", "User" });
            cboStatutUtilisateur.SelectedIndex = 0;
            EspaceAdminStyle.AppliquerComboBoxFiltre(cboStatutUtilisateur);

            Label lblEtat = new Label();
            lblEtat.Text = "État";
            lblEtat.Location = new Point(480, 0);
            lblEtat.AutoSize = true;
            lblEtat.Font = new Font("Segoe UI", 9F, FontStyle.Regular);

            cboEtatUtilisateur.Location = new Point(480, 27);
            cboEtatUtilisateur.Items.AddRange(new object[] { "Tous", "Actif", "Inactif" });
            cboEtatUtilisateur.SelectedIndex = 0;
            EspaceAdminStyle.AppliquerComboBoxFiltre(cboEtatUtilisateur);

            btnRechercherUtilisateurs.Location = new Point(630, 27);
            EspaceAdminStyle.AppliquerBoutonRechercherUtilisateurs(btnRechercherUtilisateurs);

            btnNouvelUtilisateur.Location = new Point(765, 27);
            EspaceAdminStyle.AppliquerBoutonNouvelUtilisateur(btnNouvelUtilisateur);

            btnRechercherUtilisateurs.Click += (sender, e) =>
            {
                RechercheUtilisateursDemandee?.Invoke(this, EventArgs.Empty);
            };

            btnNouvelUtilisateur.Click += (sender, e) =>
            {
                NouvelUtilisateurDemande?.Invoke(this, EventArgs.Empty);
            };

            txtRechercheUtilisateur.KeyDown += (sender, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    RechercheUtilisateursDemandee?.Invoke(this, EventArgs.Empty);
                    e.SuppressKeyPress = true;
                }
            };

            zoneFiltres.Controls.Add(lblRecherche);
            zoneFiltres.Controls.Add(txtRechercheUtilisateur);
            zoneFiltres.Controls.Add(lblStatut);
            zoneFiltres.Controls.Add(cboStatutUtilisateur);
            zoneFiltres.Controls.Add(lblEtat);
            zoneFiltres.Controls.Add(cboEtatUtilisateur);
            zoneFiltres.Controls.Add(btnRechercherUtilisateurs);
            zoneFiltres.Controls.Add(btnNouvelUtilisateur);
        }

        private void ConfigurerColonnesUtilisateurs()
        {
            dgvUtilisateurs.Columns.Clear();

            dgvUtilisateurs.AllowUserToResizeColumns = false;
            dgvUtilisateurs.AllowUserToResizeRows = false;

            dgvUtilisateurs.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Id",
                HeaderText = "Id",
                FillWeight = 35,
                Resizable = DataGridViewTriState.False
            });

            dgvUtilisateurs.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "NomComplet",
                HeaderText = "Nom complet",
                FillWeight = 100,
                Resizable = DataGridViewTriState.False
            });

            dgvUtilisateurs.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Identifiant",
                HeaderText = "Identifiant",
                FillWeight = 80,
                Resizable = DataGridViewTriState.False
            });

            dgvUtilisateurs.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Email",
                HeaderText = "Email",
                FillWeight = 100,
                Resizable = DataGridViewTriState.False
            });

            dgvUtilisateurs.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Statut",
                HeaderText = "Statut",
                FillWeight = 60,
                Resizable = DataGridViewTriState.False
            });

            dgvUtilisateurs.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Etat",
                HeaderText = "État",
                FillWeight = 55,
                Resizable = DataGridViewTriState.False
            });

            dgvUtilisateurs.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "DateCreation",
                HeaderText = "Date création",
                FillWeight = 80,
                Resizable = DataGridViewTriState.False
            });

            dgvUtilisateurs.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "DerniereConnexion",
                HeaderText = "Dernière connexion",
                FillWeight = 80,
                Resizable = DataGridViewTriState.False
            });

            DataGridViewTextBoxColumn colonneActions = new DataGridViewTextBoxColumn
            {
                Name = "Actions",
                HeaderText = "Actions",
                Width = 130,
                MinimumWidth = 125,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                Resizable = DataGridViewTriState.False,
                ReadOnly = true,
                SortMode = DataGridViewColumnSortMode.NotSortable
            };

            colonneActions.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colonneActions.DefaultCellStyle.NullValue = null;

            dgvUtilisateurs.Columns.Add(colonneActions);

            dgvUtilisateurs.DefaultCellStyle.SelectionBackColor = Color.FromArgb(225, 235, 250);
            dgvUtilisateurs.DefaultCellStyle.SelectionForeColor = Color.Black;


            dgvUtilisateurs.CellFormatting -= DgvUtilisateurs_CellFormatting;
            dgvUtilisateurs.CellPainting -= DgvUtilisateurs_CellPainting;
            dgvUtilisateurs.CellMouseClick -= DgvUtilisateurs_CellMouseClick;
            dgvUtilisateurs.MouseMove -= DgvUtilisateurs_MouseMove;
            dgvUtilisateurs.MouseLeave -= DgvUtilisateurs_MouseLeave;

            dgvUtilisateurs.CellFormatting += DgvUtilisateurs_CellFormatting;
            dgvUtilisateurs.CellPainting += DgvUtilisateurs_CellPainting;
            dgvUtilisateurs.CellMouseClick += DgvUtilisateurs_CellMouseClick;
            dgvUtilisateurs.MouseMove += DgvUtilisateurs_MouseMove;
            dgvUtilisateurs.MouseLeave += DgvUtilisateurs_MouseLeave;
        }






        public void AfficherUtilisateurs(List<UtilisateurModel> utilisateurs)
        {
            dgvUtilisateurs.Rows.Clear();

            foreach (UtilisateurModel utilisateur in utilisateurs)
            {
                string derniereConnexion = utilisateur.DerniereConnexion.HasValue ? utilisateur.DerniereConnexion.Value.ToString("dd/MM/yyyy HH:mm")
                    : "Jamais";

                int indexLigne = dgvUtilisateurs.Rows.Add(
                    utilisateur.Id,
                    utilisateur.NomComplet,
                    utilisateur.Identifiant,
                    utilisateur.Email,
                    utilisateur.Statut,
                    utilisateur.EstActif ? "Actif" : "Inactif",
                    utilisateur.DateCreation.ToString("dd/MM/yyyy"),
                    derniereConnexion);

                DataGridViewRow ligne = dgvUtilisateurs.Rows[indexLigne];

                ligne.Cells["Actions"].Tag = utilisateur.EstActif;


            }


            btnRechercherUtilisateurs.Enabled = true;
        }

        private static (Rectangle modifier, Rectangle activation, Rectangle supprimer) ObtenirZonesActions(Rectangle cellule)
        {
            const int tailleIcone = 18;
            const int espacement = 14;

            int largeurTotale = tailleIcone * 3 + espacement * 2;
            int positionX = cellule.Left + (cellule.Width - largeurTotale) / 2;
            int positionY = cellule.Top + (cellule.Height - tailleIcone) / 2;

            Rectangle zoneModifier = new Rectangle(
                positionX,
                positionY,
                tailleIcone,
                tailleIcone
            );

            Rectangle zoneActivation = new Rectangle(
                positionX + tailleIcone + espacement,
                positionY,
                tailleIcone,
                tailleIcone
            );

            Rectangle zoneSupprimer = new Rectangle(
                positionX + (tailleIcone + espacement) * 2,
                positionY,
                tailleIcone,
                tailleIcone
            );

            return (zoneModifier, zoneActivation, zoneSupprimer);
        }

        private void DgvUtilisateurs_MouseMove(object? sender, MouseEventArgs e)
        {
            DataGridView.HitTestInfo zoneSurvolee = dgvUtilisateurs.HitTest(e.X, e.Y);

            if (zoneSurvolee.RowIndex < 0 ||
                zoneSurvolee.ColumnIndex < 0 ||
                dgvUtilisateurs.Columns[zoneSurvolee.ColumnIndex].Name != "Actions")
            {
                MasquerInfoBulleAction();
                return;
            }

            Rectangle rectangleCellule = dgvUtilisateurs.GetCellDisplayRectangle(
                zoneSurvolee.ColumnIndex,
                zoneSurvolee.RowIndex,
                false
            );

            (
                Rectangle zoneModifier,
                Rectangle zoneActivation,
                Rectangle zoneSupprimer
            ) = ObtenirZonesActions(rectangleCellule);

            Point positionSourisDansTableau = new Point(e.X, e.Y);
            string texteAction = string.Empty;

            if (zoneModifier.Contains(positionSourisDansTableau))
            {
                texteAction = "Modifier l'utilisateur";
            }
            else if (zoneActivation.Contains(positionSourisDansTableau))
            {
                object? valeurEtat = dgvUtilisateurs
                    .Rows[zoneSurvolee.RowIndex]
                    .Cells["Etat"]
                    .Value;

                bool estActif = string.Equals(
                    valeurEtat?.ToString(),
                    "Actif",
                    StringComparison.OrdinalIgnoreCase
                );

                texteAction = estActif
                    ? "Désactiver l'utilisateur"
                    : "Activer l'utilisateur";
            }
            else if (zoneSupprimer.Contains(positionSourisDansTableau))
            {
                texteAction = "Supprimer l'utilisateur";
            }

            if (string.IsNullOrWhiteSpace(texteAction))
            {
                MasquerInfoBulleAction();
                return;
            }

            dgvUtilisateurs.Cursor = Cursors.Hand;
            AfficherInfoBulleAction(texteAction, e.X, e.Y);
        }

        private void AfficherInfoBulleAction(string texte, int positionXDansTableau, int positionYDansTableau)
        {
            lblInfoBulleAction.Text = texte;

            Size tailleTexte = TextRenderer.MeasureText(
                texte,
                lblInfoBulleAction.Font
            );

            lblInfoBulleAction.Width = tailleTexte.Width + 24;
            lblInfoBulleAction.Height = 30;

            Point positionDansCarte = carteUtilisateurs.PointToClient(
                dgvUtilisateurs.PointToScreen(
                    new Point(
                        positionXDansTableau + 15,
                        positionYDansTableau + 20
                    )
                )
            );

            int positionX = positionDansCarte.X;
            int positionY = positionDansCarte.Y;

            if (positionX + lblInfoBulleAction.Width > carteUtilisateurs.ClientSize.Width - 5)
            {
                positionX = carteUtilisateurs.ClientSize.Width - lblInfoBulleAction.Width - 10;
            }

            if (positionY + lblInfoBulleAction.Height > carteUtilisateurs.ClientSize.Height - 5)
            {
                positionY = positionDansCarte.Y - lblInfoBulleAction.Height - 10;
            }

            lblInfoBulleAction.Location = new Point(
                Math.Max(5, positionX),
                Math.Max(5, positionY)
            );

            lblInfoBulleAction.Visible = true;
            lblInfoBulleAction.BringToFront();
        }

        private void DgvUtilisateurs_MouseLeave(object? sender, EventArgs e)
        {
            MasquerInfoBulleAction();
        }

        private void MasquerInfoBulleAction()
        {
            dgvUtilisateurs.Cursor = Cursors.Default;
            lblInfoBulleAction.Visible = false;
        }

        private void DgvUtilisateurs_CellPainting(object? sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
            {
                return;
            }

            if (dgvUtilisateurs.Columns[e.ColumnIndex].Name != "Actions")
            {
                return;
            }

            e.Paint(
                e.CellBounds,
                DataGridViewPaintParts.Background |
                DataGridViewPaintParts.Border |
                DataGridViewPaintParts.SelectionBackground
            );

            Graphics? graphics = e.Graphics;

            if (graphics == null)
            {
                return;
            }

            int tailleIcone = 18;
            int espacement = 14;
            int largeurTotale = tailleIcone * 3 + espacement * 2;
            int xDepart = e.CellBounds.Left + (e.CellBounds.Width - largeurTotale) / 2;
            int y = e.CellBounds.Top + (e.CellBounds.Height - tailleIcone) / 2;

            object? valeurEtat = dgvUtilisateurs.Rows[e.RowIndex].Cells["Etat"].Value;

            bool estActif = string.Equals(
                valeurEtat?.ToString(),
                "Actif",
                StringComparison.OrdinalIgnoreCase
            );

            graphics.DrawImage(
                iconeModifier,
                new Rectangle(xDepart, y, tailleIcone, tailleIcone)
            );

            graphics.DrawImage(
                estActif ? iconeDesactiver : iconeActiver,
                new Rectangle(
                    xDepart + tailleIcone + espacement,
                    y,
                    tailleIcone,
                    tailleIcone
                )
            );

            graphics.DrawImage(
                iconeSupprimer,
                new Rectangle(
                    xDepart + (tailleIcone + espacement) * 2,
                    y,
                    tailleIcone,
                    tailleIcone
                )
            );

            e.Handled = true;
        }

        private void DgvUtilisateurs_CellMouseClick(
    object? sender,
    DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex < 0 ||
                e.ColumnIndex < 0 ||
                dgvUtilisateurs.Columns[e.ColumnIndex].Name != "Actions")
            {
                return;
            }

            object? valeurId =
                dgvUtilisateurs.Rows[e.RowIndex].Cells["Id"].Value;

            if (valeurId == null ||
                !int.TryParse(valeurId.ToString(), out int idUtilisateur))
            {
                return;
            }

            Rectangle rectangleCellule =
                dgvUtilisateurs.GetCellDisplayRectangle(
                    e.ColumnIndex,
                    e.RowIndex,
                    false
                );

            Point positionClic = new Point(
                rectangleCellule.Left + e.X,
                rectangleCellule.Top + e.Y
            );

            (
                Rectangle zoneModifier,
                Rectangle zoneActivation,
                Rectangle zoneSupprimer
            ) = ObtenirZonesActions(rectangleCellule);

            if (zoneModifier.Contains(positionClic))
            {
                ModificationUtilisateurDemandee?.Invoke(idUtilisateur);
                return;
            }

            if (zoneActivation.Contains(positionClic))
            {
                ActivationUtilisateurDemandee?.Invoke(idUtilisateur);
                return;
            }

            if (zoneSupprimer.Contains(positionClic))
            {
                SuppressionUtilisateurDemandee?.Invoke(idUtilisateur);
            }
        }




        public void AfficherChargementUtilisateurs()
        {
            dgvUtilisateurs.Rows.Clear();
            btnRechercherUtilisateurs.Enabled = false;
        }

        public void AfficherErreurUtilisateurs()
        {
            dgvUtilisateurs.Rows.Clear();
            btnRechercherUtilisateurs.Enabled = true;
        }

        public string ObtenirRechercheUtilisateur()
        {
            return txtRechercheUtilisateur.Text.Trim();
        }
        public string ObtenirStatutUtilisateurFiltre()
        {
            return cboStatutUtilisateur.SelectedItem?.ToString() ?? "Tous";
        }
        public string ObtenirEtatUtilisateurFiltre()
        {
            return cboEtatUtilisateur.SelectedItem?.ToString() ?? "Tous";
        }

        private void DgvUtilisateurs_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.Value == null || e.ColumnIndex < 0 || e.ColumnIndex >= dgvUtilisateurs.Columns.Count)
            {
                return;
            }

            DataGridViewCellStyle? styleCellule = e.CellStyle;

            if (styleCellule == null)
            {
                return;
            }

            string nomColonne = dgvUtilisateurs.Columns[e.ColumnIndex].Name;
            string valeur = e.Value.ToString() ?? string.Empty;

            if (nomColonne == "Statut")
            {
                if (valeur.Equals("Admin", StringComparison.OrdinalIgnoreCase))
                {
                    styleCellule.BackColor = Color.FromArgb(235, 228, 250);
                    styleCellule.ForeColor = Color.FromArgb(95, 55, 155);
                }
                else
                {
                    styleCellule.BackColor = Color.FromArgb(230, 240, 255);
                    styleCellule.ForeColor = Color.FromArgb(30, 96, 190);
                }

                styleCellule.Alignment = DataGridViewContentAlignment.MiddleCenter;
                return;
            }

            if (nomColonne == "Etat")
            {
                if (valeur.Equals("Actif", StringComparison.OrdinalIgnoreCase))
                {
                    styleCellule.BackColor = Color.FromArgb(225, 242, 220);
                    styleCellule.ForeColor = Color.FromArgb(30, 100, 40);
                }
                else
                {
                    styleCellule.BackColor = Color.FromArgb(250, 230, 225);
                    styleCellule.ForeColor = Color.FromArgb(150, 50, 35);
                }

                styleCellule.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
        }


        private Panel CreerCarteStatistique(
            IconChar iconeCarte,
            Color couleurIcone,
            string titre,
            Label labelValeur)
        {
            Panel carte = new Panel();
            carte.Tag = "CarteStatistique";

            EspaceAdminStyle.AppliquerCarteStatistique(
                carte
            );

            IconPictureBox icone =
                new IconPictureBox();

            EspaceAdminStyle.AppliquerIconeCarte(
                icone,
                iconeCarte,
                couleurIcone
            );

            Label lblTitre = new Label();
            lblTitre.Text = titre;
            lblTitre.Location = new Point(58, 17);

            EspaceAdminStyle.AppliquerTitreStatistique(
                lblTitre
            );

            labelValeur.Text = "0";
            labelValeur.Location = new Point(58, 43);

            EspaceAdminStyle.AppliquerValeurStatistique(
                labelValeur
            );

            carte.Controls.Add(icone);
            carte.Controls.Add(lblTitre);
            carte.Controls.Add(labelValeur);

            return carte;
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
            zoneCartes.Width = largeurDisponible;

            AdapterEntete(largeurDisponible);
            AdapterCartes(largeurDisponible);

            carteUtilisateurs.Width = largeurDisponible;

            Control? zoneFiltres = carteUtilisateurs.Controls
                .Cast<Control>()
                .FirstOrDefault(controle => controle.Tag?.ToString() == "ZoneFiltresUtilisateurs");

            if (zoneFiltres != null)
            {
                zoneFiltres.Width = largeurDisponible - 36;
            }

            dgvUtilisateurs.Width = largeurDisponible - 36;

            carteFormulaire.Width = largeurDisponible;
        }

        private void AdapterEntete(int largeurDisponible)
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
                blocBouton.Width
                - btnActualiser.Width,
                5
            );
        }

        private void AdapterCartes(
            int largeurDisponible)
        {
            int nombreCartes = 4;
            int margeEntreCartes = 14;

            int largeurCarte =
                (
                    largeurDisponible
                    - margeEntreCartes
                    * (nombreCartes - 1)
                )
                / nombreCartes;

            if (largeurCarte < 200)
            {
                largeurCarte = 200;
            }

            foreach (Control controle in zoneCartes.Controls)
            {
                controle.Width = largeurCarte;
                controle.Height = 105;

                bool derniereCarte =
                    controle
                    == zoneCartes.Controls[
                        zoneCartes.Controls.Count - 1
                    ];

                controle.Margin = derniereCarte
                    ? new Padding(0)
                    : new Padding(
                        0,
                        0,
                        margeEntreCartes,
                        0
                    );
            }
        }

        public void AfficherChargementStatistiques()
        {
            lblTotalUtilisateurs.Text = "...";
            lblComptesActifs.Text = "...";
            lblComptesInactifs.Text = "...";
            lblAdministrateurs.Text = "...";

            btnActualiser.Enabled = false;
        }

        public void AfficherStatistiques(
            StatistiquesUtilisateursModel statistiques)
        {
            lblTotalUtilisateurs.Text =
                statistiques.TotalUtilisateurs.ToString();

            lblComptesActifs.Text =
                statistiques.ComptesActifs.ToString();

            lblComptesInactifs.Text =
                statistiques.ComptesInactifs.ToString();

            lblAdministrateurs.Text =
                statistiques.Administrateurs.ToString();

            btnActualiser.Enabled = true;
        }

        public void AfficherErreurStatistiques(string message)
        {
            lblTotalUtilisateurs.Text = "Erreur";
            lblComptesActifs.Text = "Erreur";
            lblComptesInactifs.Text = "Erreur";
            lblAdministrateurs.Text = "Erreur";

            btnActualiser.Enabled = true;
        }

        public void AfficherMessage(string message, string titre, MessageBoxIcon icone)
        {
            MessageBox.Show(message, titre, MessageBoxButtons.OK, icone);
        }

        private void ConstruireCarteFormulaire()
        {
            carteFormulaire.Tag = "CarteFormulaire";
            carteFormulaire.Height = 540;

            EspaceAdminStyle.AppliquerCartePrincipale(carteFormulaire);

            lblTitreFormulaire.Text = "Créer un utilisateur";
            lblTitreFormulaire.Location = new Point(18, 18);
            EspaceAdminStyle.AppliquerTitreSection(lblTitreFormulaire);

            CreerChampFormulaire("Nom complet", txtNomComplet, 18, 65, 360);
            CreerChampFormulaire("Identifiant", txtIdentifiant, 398, 65, 300);
            CreerChampFormulaire("Email", txtEmail, 718, 65, 400);

            CreerChampFormulaire("Mot de passe", txtMotDePasse, 18, 145, 360);
            CreerChampFormulaire("Confirmer mot de passe", txtConfirmationMotDePasse, 398, 145, 360);

            txtMotDePasse.UseSystemPasswordChar = true;
            txtConfirmationMotDePasse.UseSystemPasswordChar = true;

            CreerComboFormulaire("Statut", cboStatutFormulaire, 18, 225, 260);
            cboStatutFormulaire.Items.AddRange(new object[] { "Admin", "User" });
            cboStatutFormulaire.SelectedItem = "User";

            CreerComboFormulaire("Compte actif", cboEtatFormulaire, 298, 225, 260);
            cboEtatFormulaire.Items.AddRange(new object[] { "Oui", "Non" });
            cboEtatFormulaire.SelectedItem = "Oui";

            Label lblAide = new Label();
            lblAide.Text = "Laissez les champs mot de passe vides pendant une modification pour conserver le mot de passe actuel.";
            lblAide.Location = new Point(18, 315);
            lblAide.AutoSize = true;
            lblAide.Font = new Font("Segoe UI", 9F, FontStyle.Italic);
            lblAide.ForeColor = Color.FromArgb(90, 90, 90);

            btnEnregistrerUtilisateur.Text = " Enregistrer utilisateur";
            btnEnregistrerUtilisateur.IconChar = IconChar.FloppyDisk;
            btnEnregistrerUtilisateur.IconSize = 18;
            btnEnregistrerUtilisateur.IconColor = Color.White;
            btnEnregistrerUtilisateur.Width = 230;
            btnEnregistrerUtilisateur.Height = 40;
            btnEnregistrerUtilisateur.Location = new Point(18, 355);
            btnEnregistrerUtilisateur.BackColor = Color.FromArgb(30, 96, 190);
            btnEnregistrerUtilisateur.ForeColor = Color.White;
            btnEnregistrerUtilisateur.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            btnEnregistrerUtilisateur.FlatStyle = FlatStyle.Flat;
            btnEnregistrerUtilisateur.FlatAppearance.BorderSize = 0;
            btnEnregistrerUtilisateur.Cursor = Cursors.Hand;
            btnEnregistrerUtilisateur.TextImageRelation = TextImageRelation.ImageBeforeText;
            btnEnregistrerUtilisateur.ImageAlign = ContentAlignment.MiddleCenter;
            btnEnregistrerUtilisateur.TextAlign = ContentAlignment.MiddleCenter;
            btnEnregistrerUtilisateur.Padding = new Padding(0);

            btnReinitialiserFormulaire.Text = "Réinitialiser";
            btnReinitialiserFormulaire.Width = 150;
            btnReinitialiserFormulaire.Height = 40;
            btnReinitialiserFormulaire.Location = new Point(265, 355);

            btnAnnulerFormulaire.Text = "Annuler";
            btnAnnulerFormulaire.Width = 130;
            btnAnnulerFormulaire.Height = 40;
            btnAnnulerFormulaire.Location = new Point(430, 355);

            lblMessageFormulaire.Location = new Point(18, 420);
            lblMessageFormulaire.Width = 800;
            lblMessageFormulaire.Height = 45;
            lblMessageFormulaire.Padding = new Padding(12);
            lblMessageFormulaire.Visible = false;

            btnEnregistrerUtilisateur.Click += (sender, e) =>
            {
                EnregistrementUtilisateurDemande?.Invoke(this, EventArgs.Empty);
            };

            btnReinitialiserFormulaire.Click += (sender, e) =>
            {
                if (idUtilisateurSelectionne.HasValue)
                {
                    ModificationUtilisateurDemandee?.Invoke(idUtilisateurSelectionne.Value);
                }
                else
                {
                    ReinitialiserChampsFormulaire();
                    txtNomComplet.Focus();
                }
            };

            btnAnnulerFormulaire.Click += (sender, e) =>
            {
                AnnulationFormulaireDemandee?.Invoke(this, EventArgs.Empty);
            };

            carteFormulaire.Controls.Add(lblTitreFormulaire);
            carteFormulaire.Controls.Add(lblAide);
            carteFormulaire.Controls.Add(btnEnregistrerUtilisateur);
            carteFormulaire.Controls.Add(btnReinitialiserFormulaire);
            carteFormulaire.Controls.Add(btnAnnulerFormulaire);
            carteFormulaire.Controls.Add(lblMessageFormulaire);

            carteFormulaire.Visible = false;
        }


        private void CreerChampFormulaire(string titre, TextBox textBox, int x, int y, int largeur)
        {
            Label label = new Label();
            label.Text = titre;
            label.Location = new Point(x, y);
            label.AutoSize = true;
            label.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular);

            textBox.Location = new Point(x, y + 27);
            textBox.Width = largeur;
            textBox.Height = 34;
            textBox.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
            textBox.BorderStyle = BorderStyle.FixedSingle;

            carteFormulaire.Controls.Add(label);
            carteFormulaire.Controls.Add(textBox);
        }

        private void CreerComboFormulaire(string titre, ComboBox comboBox, int x, int y, int largeur)
        {
            Label label = new Label();
            label.Text = titre;
            label.Location = new Point(x, y);
            label.AutoSize = true;
            label.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular);

            comboBox.Location = new Point(x, y + 27);
            comboBox.Width = largeur;
            comboBox.Height = 34;
            comboBox.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
            comboBox.DropDownStyle = ComboBoxStyle.DropDownList;

            carteFormulaire.Controls.Add(label);
            carteFormulaire.Controls.Add(comboBox);
        }



        public void AfficherFormulaireCreation()
        {
            idUtilisateurSelectionne = null;

            lblTitreFormulaire.Text = "Créer un utilisateur";

            ReinitialiserChampsFormulaire();

            carteFormulaire.Visible = true;
            contenuPage.ScrollControlIntoView(carteFormulaire);
            txtNomComplet.Focus();
        }

        private void ReinitialiserChampsFormulaire()
        {
            txtNomComplet.Clear();
            txtIdentifiant.Clear();
            txtEmail.Clear();
            txtMotDePasse.Clear();
            txtConfirmationMotDePasse.Clear();

            cboStatutFormulaire.SelectedItem = "User";
            cboEtatFormulaire.SelectedItem = "Oui";

            lblMessageFormulaire.Visible = false;
        }
        public void AfficherFormulaireModification(UtilisateurModel utilisateur)
        {
            idUtilisateurSelectionne = utilisateur.Id;

            lblTitreFormulaire.Text = "Modifier un utilisateur";

            txtNomComplet.Text = utilisateur.NomComplet;
            txtIdentifiant.Text = utilisateur.Identifiant;
            txtEmail.Text = utilisateur.Email;
            txtMotDePasse.Clear();
            txtConfirmationMotDePasse.Clear();

            cboStatutFormulaire.SelectedItem = utilisateur.Statut;
            cboEtatFormulaire.SelectedItem = utilisateur.EstActif ? "Oui" : "Non";

            lblMessageFormulaire.Visible = false;

            carteFormulaire.Visible = true;
            contenuPage.ScrollControlIntoView(carteFormulaire);
            txtNomComplet.Focus();
        }
        public void AfficherMessageFormulaire(string message, bool succes)
        {
            lblMessageFormulaire.Text = message;
            lblMessageFormulaire.Visible = true;

            if (succes)
            {
                lblMessageFormulaire.BackColor = Color.FromArgb(225, 242, 220);
                lblMessageFormulaire.ForeColor = Color.FromArgb(30, 100, 40);
            }
            else
            {
                lblMessageFormulaire.BackColor = Color.FromArgb(250, 225, 225);
                lblMessageFormulaire.ForeColor = Color.FromArgb(150, 35, 35);
            }
        }
        public void DefinirFormulaireEnChargement(bool chargement)
        {
            btnEnregistrerUtilisateur.Enabled = !chargement;
            btnReinitialiserFormulaire.Enabled = !chargement;
            btnAnnulerFormulaire.Enabled = !chargement;

            btnEnregistrerUtilisateur.Text = chargement
                ? " Enregistrement..."
                : " Enregistrer utilisateur";
        }
        public int? ObtenirIdUtilisateurSelectionne()
        {
            return idUtilisateurSelectionne;
        }

        public string ObtenirNomComplet()
        {
            return txtNomComplet.Text;
        }

        public string ObtenirIdentifiant()
        {
            return txtIdentifiant.Text;
        }

        public string ObtenirEmail()
        {
            return txtEmail.Text;
        }

        public string ObtenirMotDePasse()
        {
            return txtMotDePasse.Text;
        }

        public string ObtenirConfirmationMotDePasse()
        {
            return txtConfirmationMotDePasse.Text;
        }

        public string ObtenirStatutFormulaire()
        {
            return cboStatutFormulaire.SelectedItem?.ToString() ?? "User";
        }

        public bool ObtenirEtatActifFormulaire()
        {
            return cboEtatFormulaire.SelectedItem?.ToString() == "Oui";
        }

        public void MasquerFormulaireUtilisateur()
        {
            idUtilisateurSelectionne = null;

            ReinitialiserChampsFormulaire();

            carteFormulaire.Visible = false;
        }


        public void AfficherToastSucces(string message)
        {
            timerToast.Stop();

            lblToast.Text = message;
            panelToast.BackColor = Color.FromArgb(225, 242, 220);
            lblToast.ForeColor = Color.FromArgb(30, 100, 40);

            PositionnerToast();

            panelToast.Visible = true;
            panelToast.BringToFront();

            timerToast.Start();
        }
    }
}