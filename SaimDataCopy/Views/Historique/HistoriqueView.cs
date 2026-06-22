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

        private Panel pageListeHistorique;
        private Panel pageDetailExecution;

        private FlowLayoutPanel panelGlobalListe;
        private FlowLayoutPanel panelFiltres;

        private Panel blocDateFiltre;
        private Panel blocOrigineFiltre;
        private Panel blocStatutFiltre;
        private Panel blocBoutonFiltre;

        private DateTimePicker dtpDate;
        private ComboBox cboOrigine;
        private ComboBox cboStatut;
        private IconButton btnRechercher;

        private Panel panelTableau;
        private FlowLayoutPanel tableauExecutions;

        private FlowLayoutPanel contenuDetail;
        private IconButton btnRetourHaut;
        private IconButton btnRetourBas;
        private IconButton btnOuvrirLog;

        private int idExecutionDetailActuel;
        private bool dateFiltreSelectionnee;

        public HistoriqueView()
        {
            HistoriqueStyle.AppliquerPage(this);

            pageListeHistorique = new Panel();
            pageDetailExecution = new Panel();

            panelGlobalListe = new FlowLayoutPanel();
            panelFiltres = new FlowLayoutPanel();

            blocDateFiltre = new Panel();
            blocOrigineFiltre = new Panel();
            blocStatutFiltre = new Panel();
            blocBoutonFiltre = new Panel();

            dtpDate = new DateTimePicker();
            cboOrigine = new ComboBox();
            cboStatut = new ComboBox();
            btnRechercher = new IconButton();

            panelTableau = new Panel();
            tableauExecutions = new FlowLayoutPanel();

            contenuDetail = new FlowLayoutPanel();
            btnRetourHaut = new IconButton();
            btnRetourBas = new IconButton();
            btnOuvrirLog = new IconButton();

            idExecutionDetailActuel = 0;
            dateFiltreSelectionnee = false;

            ConstruireInterface();
        }

        private void ConstruireInterface()
        {
            ConstruirePageListeHistorique();
            ConstruirePageDetailExecution();

            Controls.Add(pageDetailExecution);
            Controls.Add(pageListeHistorique);

            AfficherPageListe();
        }

        private void ConstruirePageListeHistorique()
        {
            pageListeHistorique.Dock = DockStyle.Fill;
            pageListeHistorique.BackColor = Color.White;
            pageListeHistorique.AutoScroll = false;
            pageListeHistorique.Padding = new Padding(22, 25, 22, 25);

            panelGlobalListe.Dock = DockStyle.Fill;
            panelGlobalListe.AutoScroll = true;
            panelGlobalListe.FlowDirection = FlowDirection.TopDown;
            panelGlobalListe.WrapContents = false;
            panelGlobalListe.Margin = new Padding(0);
            panelGlobalListe.Padding = new Padding(0);

            Label lblTitre = new Label();
            lblTitre.Text = "Historique des exécutions";
            HistoriqueStyle.AppliquerTitre(lblTitre);

            FlowLayoutPanel zoneFiltres = CreerZoneFiltres();

            HistoriqueStyle.AppliquerCadreTableau(panelTableau);
            HistoriqueStyle.AppliquerListeTableau(tableauExecutions);

            panelTableau.Controls.Add(tableauExecutions);

            panelGlobalListe.Controls.Add(lblTitre);
            panelGlobalListe.Controls.Add(zoneFiltres);
            panelGlobalListe.Controls.Add(panelTableau);

            pageListeHistorique.Controls.Add(panelGlobalListe);

            panelGlobalListe.Resize += (sender, e) =>
            {
                AdapterDispositionListe();
            };

            tableauExecutions.Resize += (sender, e) =>
            {
                MettreAJourLargeursTableau();
            };
        }

        private FlowLayoutPanel CreerZoneFiltres()
        {
            panelFiltres.AutoSize = false;
            panelFiltres.Height = 65;
            panelFiltres.FlowDirection = FlowDirection.LeftToRight;
            panelFiltres.WrapContents = false;
            panelFiltres.Margin = new Padding(0);
            panelFiltres.Padding = new Padding(0);

            blocDateFiltre = CreerBlocFiltre("Filtre par date", dtpDate);
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

            blocOrigineFiltre = CreerBlocFiltre("Filtre par origine", cboOrigine);

            cboOrigine.Items.AddRange(new object[]
            {
                "Tous",
                "Manuel",
                "Automatique"
            });

            cboOrigine.SelectedIndex = 0;
            HistoriqueStyle.AppliquerComboBox(cboOrigine);

            blocStatutFiltre = CreerBlocFiltre("Filtre par statut", cboStatut);

            cboStatut.Items.AddRange(new object[]
            {
                "Tous",
                "Succès",
                "Avertissement",
                "Échec"
            });

            cboStatut.SelectedIndex = 0;
            HistoriqueStyle.AppliquerComboBox(cboStatut);

            blocBoutonFiltre = new Panel();
            blocBoutonFiltre.Width = 240;
            blocBoutonFiltre.Height = 65;
            blocBoutonFiltre.Margin = new Padding(0);

            HistoriqueStyle.AppliquerBoutonRecherche(btnRechercher);

            btnRechercher.Height = cboStatut.Height;
            btnRechercher.Location = new Point(0, 25);

            btnRechercher.Click += (sender, e) =>
            {
                RechercheDemandee?.Invoke(this, EventArgs.Empty);
            };

            blocBoutonFiltre.Controls.Add(btnRechercher);

            panelFiltres.Controls.Add(blocDateFiltre);
            panelFiltres.Controls.Add(blocOrigineFiltre);
            panelFiltres.Controls.Add(blocStatutFiltre);
            panelFiltres.Controls.Add(blocBoutonFiltre);

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

        private void AdapterDispositionListe()
        {
            int largeurContenu = ObtenirLargeurContenuListe();

            panelFiltres.Width = largeurContenu;
            panelTableau.Width = largeurContenu;

            int hauteurTableau = panelGlobalListe.ClientSize.Height - 140;

            if (hauteurTableau < 360)
            {
                hauteurTableau = 360;
            }

            panelTableau.Height = hauteurTableau;

            AdapterZoneFiltres(largeurContenu);
            MettreAJourLargeursTableau();
        }

        private int ObtenirLargeurContenuListe()
        {
            int largeur = panelGlobalListe.ClientSize.Width - 5;

            if (largeur < 900)
            {
                largeur = 900;
            }

            return largeur;
        }

        private void AdapterZoneFiltres(int largeurContenu)
        {
            int margeEntreBlocs = 12;
            int largeurDisponible = largeurContenu - (margeEntreBlocs * 3);
            int largeurBloc = largeurDisponible / 4;

            if (largeurBloc < 200)
            {
                largeurBloc = 200;
            }

            blocDateFiltre.Width = largeurBloc;
            blocOrigineFiltre.Width = largeurBloc;
            blocStatutFiltre.Width = largeurBloc;
            blocBoutonFiltre.Width = largeurBloc;

            blocDateFiltre.Margin = new Padding(0, 0, margeEntreBlocs, 0);
            blocOrigineFiltre.Margin = new Padding(0, 0, margeEntreBlocs, 0);
            blocStatutFiltre.Margin = new Padding(0, 0, margeEntreBlocs, 0);
            blocBoutonFiltre.Margin = new Padding(0);

            dtpDate.Width = blocDateFiltre.Width;
            cboOrigine.Width = blocOrigineFiltre.Width;
            cboStatut.Width = blocStatutFiltre.Width;

            btnRechercher.Width = blocBoutonFiltre.Width;
            btnRechercher.Height = cboStatut.Height;
            btnRechercher.Location = new Point(0, 25);
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
                lblVide.Width = ObtenirLargeurTableauInterieur();
                lblVide.Height = 70;
                lblVide.TextAlign = ContentAlignment.MiddleCenter;
                lblVide.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
                lblVide.ForeColor = Color.FromArgb(80, 80, 80);
                lblVide.BackColor = Color.White;
                lblVide.Margin = new Padding(0);

                tableauExecutions.Controls.Add(lblVide);
                tableauExecutions.Controls.Add(CreerSeparateur());

                MettreAJourLargeursTableau();
                return;
            }

            foreach (HistoriqueExecutionModel execution in executions)
            {
                TableLayoutPanel ligne = CreerLigneDonnee(execution);
                tableauExecutions.Controls.Add(ligne);
                tableauExecutions.Controls.Add(CreerSeparateur());
            }

            MettreAJourLargeursTableau();
        }

        private TableLayoutPanel CreerLigneEntete()
        {
            TableLayoutPanel ligne = new TableLayoutPanel();
            ligne.ColumnCount = 7;
            ligne.RowCount = 1;

            ligne.RowStyles.Clear();
            ligne.RowStyles.Add(new RowStyle(SizeType.Absolute, 66));

            AjouterColonnes(ligne);
            HistoriqueStyle.AppliquerLigneEntete(ligne);

            AjouterCelluleEntete(ligne, "Date/heure de\nlancement", 0);
            AjouterCelluleEntete(ligne, "Origine", 1);
            AjouterCelluleEntete(ligne, "Bases traitées", 2);
            AjouterCelluleEntete(ligne, "Lignes\navant", 3);
            AjouterCelluleEntete(ligne, "Durée", 4);
            AjouterCelluleEntete(ligne, "Statut", 5);
            AjouterCelluleEntete(ligne, "Actions", 6);

            ligne.Width = ObtenirLargeurTableauInterieur();

            return ligne;
        }

        private TableLayoutPanel CreerLigneDonnee(HistoriqueExecutionModel execution)
        {
            TableLayoutPanel ligne = new TableLayoutPanel();
            ligne.ColumnCount = 7;
            ligne.RowCount = 1;

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

            ligne.Width = ObtenirLargeurTableauInterieur();

            return ligne;
        }

        private void AjouterColonnes(TableLayoutPanel ligne)
        {
            ligne.ColumnStyles.Clear();

            // Les colonnes utilisent des pourcentages.
            // Comme ça, le tableau s'adapte quand la fenêtre devient plus grande.
            ligne.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16F)); // Date / heure
            ligne.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 14F)); // Origine
            ligne.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F)); // Bases traitées
            ligne.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 14F)); // Lignes
            ligne.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10F)); // Durée
            ligne.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 13F)); // Statut
            ligne.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 13F)); // Actions
        }

        private Panel CreerSeparateur()
        {
            Panel separateur = new Panel();
            HistoriqueStyle.AppliquerSeparateur(separateur);
            separateur.Width = ObtenirLargeurTableauInterieur();

            return separateur;
        }

        private int ObtenirLargeurTableauInterieur()
        {
            int largeur = panelTableau.ClientSize.Width - panelTableau.Padding.Left - panelTableau.Padding.Right;

            if (largeur < 850)
            {
                largeur = 850;
            }

            return largeur;
        }

        private void MettreAJourLargeursTableau()
        {
            int largeur = ObtenirLargeurTableauInterieur();

            foreach (Control controle in tableauExecutions.Controls)
            {
                controle.Width = largeur;

                if (controle is TableLayoutPanel ligne)
                {
                    ligne.Width = largeur;
                }
            }
        }

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
            CentrerDansCellule(panel, badge, 14);

            ligne.Controls.Add(panel, colonne, 0);
        }

        private void AjouterCelluleBoutonDetail(TableLayoutPanel ligne, int idExecution, int colonne)
        {
            Panel panel = new Panel();
            panel.Dock = DockStyle.Fill;
            panel.BackColor = Color.White;

            IconButton bouton = new IconButton();
            HistoriqueStyle.AppliquerBoutonDetail(bouton);

            bouton.Click += (sender, e) =>
            {
                DetailDemande?.Invoke(idExecution);
            };

            panel.Controls.Add(bouton);
            CentrerDansCellule(panel, bouton, 14);

            ligne.Controls.Add(panel, colonne, 0);
        }

        private void ConstruirePageDetailExecution()
        {
            HistoriqueDetailStyle.AppliquerPage(pageDetailExecution);

            contenuDetail.Dock = DockStyle.Top;
            contenuDetail.AutoSize = true;
            contenuDetail.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            contenuDetail.FlowDirection = FlowDirection.TopDown;
            contenuDetail.WrapContents = false;
            contenuDetail.Margin = new Padding(0);

            pageDetailExecution.Controls.Add(contenuDetail);

            pageDetailExecution.Resize += (sender, e) =>
            {
                AdapterDispositionDetail();
            };
        }

        public void AfficherDetail(HistoriqueExecutionModel execution)
        {
            idExecutionDetailActuel = execution.Id;

            contenuDetail.Controls.Clear();

            Label lblTitre = new Label();
            lblTitre.Text = "Détail de l'exécution";
            HistoriqueDetailStyle.AppliquerTitre(lblTitre);

            HistoriqueDetailStyle.AppliquerBoutonRetour(btnRetourHaut);
            btnRetourHaut.Click -= BtnRetour_Click;
            btnRetourHaut.Click += BtnRetour_Click;

            Panel carteResume = CreerCarteResumeExecution(execution);
            FlowLayoutPanel zoneCartes = CreerZoneCartesInformation(execution);

            Label lblTitreSection = new Label();
            lblTitreSection.Text = "DÉTAIL PAR BASE DE DONNÉES";
            HistoriqueDetailStyle.AppliquerTitreSection(lblTitreSection);

            contenuDetail.Controls.Add(lblTitre);
            contenuDetail.Controls.Add(btnRetourHaut);
            contenuDetail.Controls.Add(carteResume);
            contenuDetail.Controls.Add(zoneCartes);
            contenuDetail.Controls.Add(lblTitreSection);

            List<HistoriqueEtapeExecutionModel> etapesAffichage = ObtenirEtapesPourAffichage(execution);

            foreach (HistoriqueEtapeExecutionModel etape in etapesAffichage)
            {
                Panel carteBase = CreerCarteBase(etape);
                contenuDetail.Controls.Add(carteBase);
            }

            FlowLayoutPanel zoneBoutonsBas = new FlowLayoutPanel();
            zoneBoutonsBas.AutoSize = true;
            zoneBoutonsBas.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            zoneBoutonsBas.FlowDirection = FlowDirection.LeftToRight;
            zoneBoutonsBas.WrapContents = false;
            zoneBoutonsBas.Margin = new Padding(0, 6, 0, 30);

            HistoriqueDetailStyle.AppliquerBoutonOuvrirLog(btnOuvrirLog);
            btnOuvrirLog.Click -= BtnOuvrirLog_Click;
            btnOuvrirLog.Click += BtnOuvrirLog_Click;

            HistoriqueDetailStyle.AppliquerBoutonRetourBas(btnRetourBas);
            btnRetourBas.Click -= BtnRetour_Click;
            btnRetourBas.Click += BtnRetour_Click;

            zoneBoutonsBas.Controls.Add(btnOuvrirLog);
            zoneBoutonsBas.Controls.Add(btnRetourBas);

            contenuDetail.Controls.Add(zoneBoutonsBas);

            AdapterDispositionDetail();
            AfficherPageDetail();
        }

        private int ObtenirLargeurDetail()
        {
            int largeur = pageDetailExecution.ClientSize.Width
                - pageDetailExecution.Padding.Left
                - pageDetailExecution.Padding.Right
                - SystemInformation.VerticalScrollBarWidth;

            if (largeur < 850)
            {
                largeur = 850;
            }

            return largeur;
        }

        private void AdapterDispositionDetail()
        {
            int largeur = ObtenirLargeurDetail();

            contenuDetail.Width = largeur;

            foreach (Control controle in contenuDetail.Controls)
            {
                switch (controle.Tag)
                {
                    case "CarteResume":
                        controle.Width = largeur;
                        AdapterCarteResume((Panel)controle);
                        break;

                    case "ZoneCartes":
                        controle.Width = largeur;
                        AdapterZoneCartesInformation((FlowLayoutPanel)controle, largeur);
                        break;

                    case "CarteBase":
                        controle.Width = largeur;
                        AdapterCarteBase((Panel)controle);
                        break;
                }
            }
        }

        private void AdapterZoneCartesInformation(FlowLayoutPanel zoneCartes, int largeurDisponible)
        {
            // On veut 3 cartes par ligne comme dans la maquette.
            int espaceEntreCartes = 14;
            int nombreColonnes = 3;

            int largeurCarte = (largeurDisponible - (espaceEntreCartes * (nombreColonnes - 1))) / nombreColonnes;

            if (largeurCarte < 280)
            {
                largeurCarte = 280;
            }

            foreach (Control controle in zoneCartes.Controls)
            {
                controle.Width = largeurCarte;
                controle.Height = 92;
                controle.Margin = new Padding(0, 0, espaceEntreCartes, 14);
            }
        }

        private Panel CreerCarteResumeExecution(HistoriqueExecutionModel execution)
        {
            Panel panel = new Panel();
            panel.Tag = "CarteResume";
            HistoriqueDetailStyle.AppliquerCarteResume(panel);
            panel.Width = ObtenirLargeurDetail();

            IconPictureBox icone = new IconPictureBox();
            icone.IconChar = ObtenirIconeStatut(execution.Statut);
            icone.IconColor = ObtenirCouleurStatut(execution.Statut);
            icone.IconSize = 24;
            icone.Size = new Size(28, 28);
            icone.Location = new Point(16, 22);
            icone.BackColor = Color.Transparent;

            Label lblTitre = new Label();
            lblTitre.Text = $"Exécution du {execution.DateHeureAffichage}";
            lblTitre.Location = new Point(48, 20);
            HistoriqueDetailStyle.AppliquerTitreResume(lblTitre);

            Label lblSousTitre = new Label();
            lblSousTitre.Text = execution.Origine == "Automatique"
                ? "Déclenchement automatique"
                : "Déclenchement manuel";
            lblSousTitre.Location = new Point(48, 45);
            HistoriqueDetailStyle.AppliquerSousTitreResume(lblSousTitre);

            Label badge = new Label();
            badge.Tag = "BadgeResume";
            HistoriqueDetailStyle.AppliquerBadgeStatut(badge, execution.Statut);

            panel.Controls.Add(icone);
            panel.Controls.Add(lblTitre);
            panel.Controls.Add(lblSousTitre);
            panel.Controls.Add(badge);

            panel.Resize += (sender, e) =>
            {
                AdapterCarteResume(panel);
            };

            AdapterCarteResume(panel);

            return panel;
        }

        private void AdapterCarteResume(Panel panel)
        {
            foreach (Control controle in panel.Controls)
            {
                if (controle.Tag?.ToString() == "BadgeResume")
                {
                    controle.Location = new Point(
                        panel.Width - controle.Width - 20,
                        24
                    );
                }
            }
        }

        private FlowLayoutPanel CreerZoneCartesInformation(HistoriqueExecutionModel execution)
        {
            FlowLayoutPanel zoneCartes = new FlowLayoutPanel();
            zoneCartes.Tag = "ZoneCartes";
            HistoriqueDetailStyle.AppliquerZoneCartes(zoneCartes);
            zoneCartes.Width = ObtenirLargeurDetail();

            zoneCartes.Controls.Add(CreerCarteInfo(
                IconChar.Server,
                "Serveur source",
                execution.ServeurSourceAffichage));

            zoneCartes.Controls.Add(CreerCarteInfo(
                IconChar.Server,
                "Serveur cible",
                execution.ServeurCibleAffichage));

            zoneCartes.Controls.Add(CreerCarteInfo(
                IconChar.Database,
                "Bases traitées",
                $"{execution.BasesTraitees.Count}\n{execution.BasesTraiteesAffichage}"));

            zoneCartes.Controls.Add(CreerCarteInfo(
                IconChar.CalendarAlt,
                "Date de début",
                execution.DateHeureAffichage));

            zoneCartes.Controls.Add(CreerCarteInfo(
                IconChar.Clock,
                "Durée totale",
                execution.DureeAffichage));

            string etatEmail = execution.EmailEnvoye ? "Envoyé" : "Non envoyé";

            zoneCartes.Controls.Add(CreerCarteInfo(
                IconChar.Envelope,
                "E-mail de rapport",
                $"{etatEmail}\n{execution.EmailRapportAffichage}"));

            return zoneCartes;
        }

        private Panel CreerCarteInfo(IconChar iconeCarte, string titre, string valeur)
        {
            Panel panel = new Panel();
            HistoriqueDetailStyle.AppliquerCarteInfo(panel);

            IconPictureBox icone = new IconPictureBox();
            HistoriqueDetailStyle.AppliquerIconeCarte(icone, iconeCarte);

            Label lblTitre = new Label();
            lblTitre.Text = titre;
            HistoriqueDetailStyle.AppliquerLabelCarte(lblTitre);

            Label lblValeur = new Label();
            lblValeur.Text = valeur;
            HistoriqueDetailStyle.AppliquerValeurCarte(lblValeur);

            panel.Controls.Add(icone);
            panel.Controls.Add(lblTitre);
            panel.Controls.Add(lblValeur);

            return panel;
        }

        private List<HistoriqueEtapeExecutionModel> ObtenirEtapesPourAffichage(
            HistoriqueExecutionModel execution)
        {
            if (execution.Etapes.Count > 0)
            {
                return execution.Etapes;
            }

            List<HistoriqueEtapeExecutionModel> etapes = new List<HistoriqueEtapeExecutionModel>();

            foreach (string nomBase in execution.BasesTraitees)
            {
                HistoriqueEtapeExecutionModel etape = new HistoriqueEtapeExecutionModel
                {
                    NomBase = nomBase,
                    LignesAvant = execution.LignesAvant,
                    LignesApres = execution.LignesApres,
                    Statut = execution.Statut,
                    Message = "Détail non disponible pour cette ancienne exécution.",
                    Logs = new List<string>
                    {
                        $"[{execution.DateHeureLancement:HH:mm:ss}] Exécution enregistrée dans l'historique.",
                        $"[{execution.DateHeureLancement:HH:mm:ss}] Base traitée : {nomBase}",
                        $"[{execution.DateHeureLancement:HH:mm:ss}] Statut : {execution.Statut}",
                        $"[{execution.DateHeureLancement:HH:mm:ss}] Lignes : {execution.LignesAvant} → {execution.LignesApres}"
                    }
                };

                etapes.Add(etape);
            }

            return etapes;
        }

        private Panel CreerCarteBase(HistoriqueEtapeExecutionModel etape)
        {
            int hauteurEntete = 46;
            int hauteurMessage = 0;
            int hauteurLogs = 140;

            bool afficherMessage =
                !string.IsNullOrWhiteSpace(etape.Message) &&
                etape.Statut != "Succès";

            if (afficherMessage)
            {
                hauteurMessage = 30;
            }

            int hauteurTotale = hauteurEntete + hauteurMessage + hauteurLogs;

            Panel carte = new Panel();
            carte.Tag = "CarteBase";
            carte.Width = ObtenirLargeurDetail();
            carte.Height = hauteurTotale;
            carte.Margin = new Padding(0, 0, 0, 12);
            carte.BackColor = Color.White;

            HistoriqueDetailStyle.AppliquerCarteBase(carte, etape.Statut);

            Panel entete = CreerEnteteBase(etape);
            entete.Location = new Point(0, 0);
            carte.Controls.Add(entete);

            int positionY = hauteurEntete;

            if (afficherMessage)
            {
                Label lblMessage = new Label();
                lblMessage.Tag = "MessageErreur";
                lblMessage.Text = $"{etape.Statut} : {etape.Message}";
                HistoriqueDetailStyle.AppliquerMessageErreur(lblMessage);
                lblMessage.Location = new Point(0, positionY);
                lblMessage.Width = carte.Width;

                carte.Controls.Add(lblMessage);

                positionY += hauteurMessage;
            }

            RichTextBox zoneLogs = new RichTextBox();
            zoneLogs.Tag = "ZoneLogs";
            HistoriqueDetailStyle.AppliquerZoneLogs(zoneLogs);
            zoneLogs.Location = new Point(0, positionY);
            zoneLogs.Width = carte.Width;
            zoneLogs.Text = ConstruireTexteLogs(etape);

            carte.Controls.Add(zoneLogs);

            carte.Resize += (sender, e) =>
            {
                AdapterCarteBase(carte);
            };

            AdapterCarteBase(carte);

            return carte;
        }

        private Panel CreerEnteteBase(HistoriqueEtapeExecutionModel etape)
        {
            Panel entete = new Panel();
            entete.Tag = "EnteteBase";
            entete.Width = ObtenirLargeurDetail();

            HistoriqueDetailStyle.AppliquerEnteteBase(entete, etape.Statut);

            IconPictureBox icone = new IconPictureBox();
            icone.IconChar = ObtenirIconeStatut(etape.Statut);
            icone.IconColor = ObtenirCouleurStatut(etape.Statut);
            icone.IconSize = 20;
            icone.Size = new Size(24, 24);
            icone.Location = new Point(14, 10);
            icone.BackColor = Color.Transparent;

            Label lblNomBase = new Label();
            lblNomBase.Text = etape.NomBase;
            HistoriqueDetailStyle.AppliquerTitreBase(lblNomBase);

            Label lblLignes = new Label();
            lblLignes.Tag = "LignesBase";
            lblLignes.Text = etape.LignesAffichage;
            HistoriqueDetailStyle.AppliquerLignesBase(lblLignes);

            Label badge = new Label();
            badge.Tag = "BadgeBase";
            HistoriqueDetailStyle.AppliquerBadgeStatut(badge, etape.Statut);

            entete.Controls.Add(icone);
            entete.Controls.Add(lblNomBase);
            entete.Controls.Add(lblLignes);
            entete.Controls.Add(badge);

            AdapterEnteteBase(entete);

            entete.Resize += (sender, e) =>
            {
                AdapterEnteteBase(entete);
            };

            return entete;
        }

        private void AdapterCarteBase(Panel carte)
        {
            foreach (Control controle in carte.Controls)
            {
                switch (controle.Tag)
                {
                    case "EnteteBase":
                        controle.Width = carte.Width;
                        AdapterEnteteBase((Panel)controle);
                        break;

                    case "MessageErreur":
                        controle.Width = carte.Width;
                        break;

                    case "ZoneLogs":
                        controle.Width = carte.Width;
                        break;
                }
            }
        }

        private void AdapterEnteteBase(Panel entete)
        {
            Control? badge = null;
            Control? lignes = null;

            foreach (Control controle in entete.Controls)
            {
                if (controle.Tag?.ToString() == "BadgeBase")
                {
                    badge = controle;
                }

                if (controle.Tag?.ToString() == "LignesBase")
                {
                    lignes = controle;
                }
            }

            if (badge != null)
            {
                badge.Location = new Point(
                    entete.Width - badge.Width - 16,
                    8
                );
            }

            if (lignes != null && badge != null)
            {
                lignes.Location = new Point(
                    badge.Left - lignes.Width - 15,
                    11
                );
            }
        }

        private string ConstruireTexteLogs(HistoriqueEtapeExecutionModel etape)
        {
            if (etape.Logs.Count == 0)
            {
                return $"Aucun log détaillé enregistré pour {etape.NomBase}.";
            }

            return string.Join(Environment.NewLine, etape.Logs);
        }

        private IconChar ObtenirIconeStatut(string statut)
        {
            return statut switch
            {
                "Succès" => IconChar.CheckCircle,
                "Avertissement" => IconChar.ExclamationTriangle,
                "Échec" => IconChar.TimesCircle,
                _ => IconChar.InfoCircle
            };
        }

        private Color ObtenirCouleurStatut(string statut)
        {
            return statut switch
            {
                "Succès" => Color.FromArgb(60, 130, 45),
                "Avertissement" => Color.FromArgb(180, 100, 0),
                "Échec" => Color.FromArgb(170, 40, 40),
                _ => Color.FromArgb(70, 70, 70)
            };
        }

        private void BtnRetour_Click(object? sender, EventArgs e)
        {
            AfficherPageListe();
        }

        private void BtnOuvrirLog_Click(object? sender, EventArgs e)
        {
            if (idExecutionDetailActuel > 0)
            {
                OuvrirLogDemande?.Invoke(idExecutionDetailActuel);
            }
        }

        private void AfficherPageListe()
        {
            pageDetailExecution.Visible = false;
            pageListeHistorique.Visible = true;
            pageListeHistorique.BringToFront();

            AdapterDispositionListe();
        }

        private void AfficherPageDetail()
        {
            pageListeHistorique.Visible = false;
            pageDetailExecution.Visible = true;
            pageDetailExecution.BringToFront();

            AdapterDispositionDetail();
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