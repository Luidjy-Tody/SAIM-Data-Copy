using FontAwesome.Sharp;
using SaimDataCopy.Models.Historique;
using SaimDataCopy.Styles;
using System.Drawing;
using System.Windows.Forms;

namespace SaimDataCopy.Views.Historique
{
    // View graphique de la page Historique.
    // Elle contient seulement l'interface utilisateur.
    public class HistoriqueView : UserControl, IHistoriqueView
    {
        public event EventHandler? RechercheDemandee;
        public event Action<int>? DetailDemande;
        public event Action<int>? OuvrirLogDemande;

        private DateTimePicker dtpDate;
        private ComboBox cboOrigine;
        private ComboBox cboStatut;
        private IconButton btnRechercher;

        private Panel panelTableau;
        private FlowLayoutPanel tableauExecutions;

        private Panel panelDetail;
        private FlowLayoutPanel panelContenuDetail;
        private IconButton btnOuvrirLog;

        private int idExecutionDetailActuel;
        private bool dateFiltreSelectionnee;

        public HistoriqueView()
        {
            HistoriqueStyle.AppliquerPage(this);

            dtpDate = new DateTimePicker();
            cboOrigine = new ComboBox();
            cboStatut = new ComboBox();
            btnRechercher = new IconButton();

            panelTableau = new Panel();
            tableauExecutions = new FlowLayoutPanel();

            panelDetail = new Panel();
            panelContenuDetail = new FlowLayoutPanel();
            btnOuvrirLog = new IconButton();

            idExecutionDetailActuel = 0;
            dateFiltreSelectionnee = false;

            ConstruireInterface();
        }

        private void ConstruireInterface()
        {
            FlowLayoutPanel panelGlobal = new FlowLayoutPanel();
            panelGlobal.Dock = DockStyle.Top;
            panelGlobal.AutoSize = true;
            panelGlobal.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            panelGlobal.FlowDirection = FlowDirection.TopDown;
            panelGlobal.WrapContents = false;

            Label lblTitre = new Label();
            lblTitre.Text = "Historique des exécutions";
            HistoriqueStyle.AppliquerTitre(lblTitre);

            FlowLayoutPanel panelFiltres = CreerZoneFiltres();

            HistoriqueStyle.AppliquerCadreTableau(panelTableau);
            HistoriqueStyle.AppliquerListeTableau(tableauExecutions);

            panelTableau.Controls.Add(tableauExecutions);

            ConstruirePanelDetail();

            panelGlobal.Controls.Add(lblTitre);
            panelGlobal.Controls.Add(panelFiltres);
            panelGlobal.Controls.Add(panelTableau);
            panelGlobal.Controls.Add(panelDetail);

            Controls.Add(panelGlobal);
        }

        private FlowLayoutPanel CreerZoneFiltres()
        {
            FlowLayoutPanel panelFiltres = new FlowLayoutPanel();
            panelFiltres.AutoSize = true;
            panelFiltres.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            panelFiltres.FlowDirection = FlowDirection.LeftToRight;
            panelFiltres.WrapContents = false;
            panelFiltres.Margin = new Padding(0);

            Panel blocDate = CreerBlocFiltre("Filtre par date", dtpDate);
            HistoriqueStyle.AppliquerDatePicker(dtpDate);

            dtpDate.CustomFormat = "'jj/mm/aaaa'";

            dtpDate.ValueChanged += (sender, e) =>
            {
                dateFiltreSelectionnee = true;
                dtpDate.CustomFormat = "dd/MM/yyyy";
            };

            dtpDate.KeyDown += (sender, e) =>
            {
                if (e.KeyCode == Keys.Back || e.KeyCode == Keys.Delete)
                {
                    dateFiltreSelectionnee = false;
                    dtpDate.CustomFormat = "'jj/mm/aaaa'";
                }
            };

            Panel blocOrigine = CreerBlocFiltre("Filtre par origine", cboOrigine);

            cboOrigine.Items.AddRange(new object[]
            {
                "Tous",
                "Manuel",
                "Automatique"
            });

            cboOrigine.SelectedIndex = 0;
            HistoriqueStyle.AppliquerComboBox(cboOrigine);

            Panel blocStatut = CreerBlocFiltre("Filtre par statut", cboStatut);

            cboStatut.Items.AddRange(new object[]
            {
                "Tous",
                "Succès",
                "Avertissement",
                "Échec"
            });

            cboStatut.SelectedIndex = 0;
            HistoriqueStyle.AppliquerComboBox(cboStatut);

            Panel blocBouton = new Panel();
            blocBouton.Width = 240;
            blocBouton.Height = 65;
            blocBouton.Margin = new Padding(0);

            HistoriqueStyle.AppliquerBoutonRecherche(btnRechercher);

            // On prend la vraie hauteur de la ComboBox.
            // Comme ça le bouton a exactement la même hauteur visuelle.
            btnRechercher.Height = cboStatut.Height;
            btnRechercher.Width = cboStatut.Width;

            // Même position verticale que les autres champs.
            btnRechercher.Location = new Point(0, cboStatut.Top);

            btnRechercher.Click += (sender, e) =>
            {
                RechercheDemandee?.Invoke(this, EventArgs.Empty);
            };

            blocBouton.Controls.Add(btnRechercher);

            panelFiltres.Controls.Add(blocDate);
            panelFiltres.Controls.Add(blocOrigine);
            panelFiltres.Controls.Add(blocStatut);
            panelFiltres.Controls.Add(blocBouton);

            return panelFiltres;
        }

        private Panel CreerBlocFiltre(string texteLabel, Control controle)
        {
            Panel panel = new Panel();
            panel.Width = 240;
            panel.Height = 65;
            panel.Margin = new Padding(0, 0, 12, 0);

            Label label = new Label();
            label.Text = texteLabel;
            label.Location = new Point(0, 0);
            HistoriqueStyle.AppliquerLabelFiltre(label);

            controle.Location = new Point(0, 25);

            panel.Controls.Add(label);
            panel.Controls.Add(controle);

            return panel;
        }

        public void AfficherExecutions(List<HistoriqueExecutionModel> executions)
        {
            tableauExecutions.Controls.Clear();

            TableLayoutPanel ligneEntete = CreerLigneEntete();
            tableauExecutions.Controls.Add(ligneEntete);
            tableauExecutions.Controls.Add(CreerSeparateur());

            if (executions.Count == 0)
            {
                Label lblVide = new Label();
                lblVide.Text = "Aucune exécution trouvée avec ces filtres.";
                lblVide.Width = 993;
                lblVide.Height = 70;
                lblVide.TextAlign = ContentAlignment.MiddleCenter;
                lblVide.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
                lblVide.ForeColor = Color.FromArgb(80, 80, 80);
                lblVide.BackColor = Color.White;
                lblVide.Margin = new Padding(0);

                tableauExecutions.Controls.Add(lblVide);
                tableauExecutions.Controls.Add(CreerSeparateur());

                return;
            }

            foreach (HistoriqueExecutionModel execution in executions)
            {
                TableLayoutPanel ligne = CreerLigneDonnee(execution);
                tableauExecutions.Controls.Add(ligne);
                tableauExecutions.Controls.Add(CreerSeparateur());
            }
        }

        private TableLayoutPanel CreerLigneEntete()
        {
            TableLayoutPanel ligne = new TableLayoutPanel();
            ligne.ColumnCount = 7;
            ligne.RowCount = 1;

            // Important : on force la hauteur de la ligne.
            ligne.RowStyles.Clear();
            ligne.RowStyles.Add(new RowStyle(SizeType.Absolute, 66));

            AjouterColonnes(ligne);
            HistoriqueStyle.AppliquerLigneEntete(ligne);

            AjouterCelluleEntete(ligne, "Date/heure de\nlancement", 0);
            AjouterCelluleEntete(ligne, "Origine", 1);
            AjouterCelluleEntete(ligne, "Bases traitées", 2);
            AjouterCelluleEntete(ligne, "Lignes avant\n→ après", 3);
            AjouterCelluleEntete(ligne, "Durée", 4);
            AjouterCelluleEntete(ligne, "Statut", 5);
            AjouterCelluleEntete(ligne, "Actions", 6);

            return ligne;
        }

        private TableLayoutPanel CreerLigneDonnee(HistoriqueExecutionModel execution)
        {
            TableLayoutPanel ligne = new TableLayoutPanel();
            ligne.ColumnCount = 7;
            ligne.RowCount = 1;

            // Important : on force aussi la hauteur des lignes de données.
            ligne.RowStyles.Clear();
            ligne.RowStyles.Add(new RowStyle(SizeType.Absolute, 88));

            AjouterColonnes(ligne);
            HistoriqueStyle.AppliquerLigneDonnee(ligne);

            AjouterCelluleTexte(ligne, execution.DateHeureAffichage, 0);
            AjouterCelluleBadgeOrigine(ligne, execution.Origine, 1);
            AjouterCelluleTexte(ligne, execution.BasesTraiteesAffichage, 2);
            AjouterCelluleTexte(ligne, execution.LignesAvantApresAffichage, 3);
            AjouterCelluleTexte(ligne, execution.DureeAffichage, 4);
            AjouterCelluleBadgeStatut(ligne, execution.Statut, 5);
            AjouterCelluleBoutonDetail(ligne, execution.Id, 6);

            return ligne;
        }

        private void AjouterColonnes(TableLayoutPanel ligne)
        {
            ligne.ColumnStyles.Clear();

            // Total = 993.
            ligne.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 160)); // Date / heure
            ligne.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 135)); // Origine
            ligne.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 220)); // Bases traitées
            ligne.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 135)); // Lignes
            ligne.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 95));  // Durée
            ligne.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 135)); // Statut
            ligne.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 113)); // Actions
        }

        private Panel CreerSeparateur()
        {
            Panel separateur = new Panel();
            HistoriqueStyle.AppliquerSeparateur(separateur);
            return separateur;
        }

        // Centre un contrôle verticalement dans une cellule.
        // Exemple : badge Automatique, badge Succès, bouton Voir détail.
        private void CentrerDansCellule(Panel panel, Control controle, int margeGauche)
        {
            controle.Left = margeGauche;
            controle.Top = (panel.Height - controle.Height) / 2;

            panel.Resize += (sender, e) =>
            {
                controle.Left = margeGauche;
                controle.Top = (panel.Height - controle.Height) / 2;
            };
        }

        private void AjouterCelluleEntete(TableLayoutPanel ligne, string texte, int colonne)
        {
            Label label = new Label();
            label.Text = texte;

            HistoriqueStyle.AppliquerCelluleEntete(label);

            ligne.Controls.Add(label, colonne, 0);
        }

        private void AjouterCelluleTexte(TableLayoutPanel ligne, string texte, int colonne)
        {
            Label label = new Label();
            label.Text = texte;

            HistoriqueStyle.AppliquerCellule(label);

            ligne.Controls.Add(label, colonne, 0);
        }

        private void AjouterCelluleBadgeOrigine(TableLayoutPanel ligne, string origine, int colonne)
        {
            Panel panel = new Panel();
            panel.Dock = DockStyle.Fill;
            panel.BackColor = Color.White;

            Label badge = new Label();
            HistoriqueStyle.AppliquerBadgeOrigine(badge, origine);

            panel.Controls.Add(badge);

            // On centre le badge verticalement dans la cellule.
            CentrerDansCellule(panel, badge, 14);

            ligne.Controls.Add(panel, colonne, 0);
        }

        private void AjouterCelluleBadgeStatut(TableLayoutPanel ligne, string statut, int colonne)
        {
            Panel panel = new Panel();
            panel.Dock = DockStyle.Fill;
            panel.BackColor = Color.White;

            Label badge = new Label();
            HistoriqueStyle.AppliquerBadgeStatut(badge, statut);

            panel.Controls.Add(badge);

            // On centre le badge verticalement dans la cellule.
            CentrerDansCellule(panel, badge, 14);

            ligne.Controls.Add(panel, colonne, 0);
        }

        private void AjouterCelluleBoutonDetail(TableLayoutPanel ligne, int idExecution, int colonne)
        {
            Panel panel = new Panel();
            panel.Dock = DockStyle.Fill;
            panel.BackColor = Color.White;

            Button bouton = new Button();
            HistoriqueStyle.AppliquerBoutonDetail(bouton);

            bouton.Click += (sender, e) =>
            {
                DetailDemande?.Invoke(idExecution);
            };

            panel.Controls.Add(bouton);

            // On centre le bouton verticalement dans la cellule.
            CentrerDansCellule(panel, bouton, 14);

            ligne.Controls.Add(panel, colonne, 0);
        }

        private void ConstruirePanelDetail()
        {
            HistoriqueStyle.AppliquerPanelDetail(panelDetail);

            panelContenuDetail.Dock = DockStyle.Fill;
            panelContenuDetail.FlowDirection = FlowDirection.TopDown;
            panelContenuDetail.WrapContents = false;
            panelContenuDetail.AutoScroll = true;

            btnOuvrirLog.Margin = new Padding(0, 12, 0, 0);
            HistoriqueStyle.AppliquerBoutonOuvrirLog(btnOuvrirLog);

            btnOuvrirLog.Click += (sender, e) =>
            {
                if (idExecutionDetailActuel > 0)
                {
                    OuvrirLogDemande?.Invoke(idExecutionDetailActuel);
                }
            };

            panelDetail.Controls.Add(panelContenuDetail);
        }

        public void AfficherDetail(HistoriqueExecutionModel execution)
        {
            idExecutionDetailActuel = execution.Id;

            panelContenuDetail.Controls.Clear();

            Label lblTitreDetail = new Label();
            lblTitreDetail.Text = $"Détail de l’exécution du {execution.DateHeureAffichage}";
            HistoriqueStyle.AppliquerTitreDetail(lblTitreDetail);

            Label lblOrigine = CreerLabelDetail($"Origine : {execution.Origine}");
            Label lblStatut = CreerLabelDetail($"Statut : {execution.Statut}");
            Label lblBases = CreerLabelDetail($"Bases traitées : {execution.BasesTraiteesAffichage}");
            Label lblLignes = CreerLabelDetail($"Total lignes : {execution.LignesAvantApresAffichage}");
            Label lblDuree = CreerLabelDetail($"Durée : {execution.DureeAffichage}");

            string texteEmail = execution.EmailEnvoye
                ? $"Email envoyé : Oui, le {execution.DateEmailAffichage}"
                : "Email envoyé : Non";

            Label lblEmail = CreerLabelDetail(texteEmail);

            Label lblSousTitre = new Label();
            lblSousTitre.Text = "Détail par base :";
            HistoriqueStyle.AppliquerTitreDetail(lblSousTitre);

            panelContenuDetail.Controls.Add(lblTitreDetail);
            panelContenuDetail.Controls.Add(lblOrigine);
            panelContenuDetail.Controls.Add(lblStatut);
            panelContenuDetail.Controls.Add(lblBases);
            panelContenuDetail.Controls.Add(lblLignes);
            panelContenuDetail.Controls.Add(lblDuree);
            panelContenuDetail.Controls.Add(lblEmail);
            panelContenuDetail.Controls.Add(lblSousTitre);

            foreach (HistoriqueEtapeExecutionModel etape in execution.Etapes)
            {
                Label lblEtape = CreerLabelDetail(
                    $"{etape.LigneAffichage} - {etape.Statut} - {etape.Message}"
                );

                panelContenuDetail.Controls.Add(lblEtape);
            }

            panelContenuDetail.Controls.Add(btnOuvrirLog);

            panelDetail.Visible = true;
        }

        private Label CreerLabelDetail(string texte)
        {
            Label label = new Label();
            label.Text = texte;

            HistoriqueStyle.AppliquerTexteDetail(label);

            return label;
        }

        public DateTime? ObtenirDateFiltre()
        {
            if (!dateFiltreSelectionnee)
            {
                return null;
            }

            return dtpDate.Value.Date;
        }

        public string ObtenirOrigineFiltre()
        {
            return cboOrigine.SelectedItem?.ToString() ?? "Tous";
        }

        public string ObtenirStatutFiltre()
        {
            return cboStatut.SelectedItem?.ToString() ?? "Tous";
        }

        public void AfficherMessage(string message, string titre, MessageBoxIcon icone)
        {
            MessageBox.Show(
                message,
                titre,
                MessageBoxButtons.OK,
                icone
            );
        }
    }
}