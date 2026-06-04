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
            BackColor = Color.White;
            AutoScroll = false;
            Padding = new Padding(0);

            pageListeHistorique = new Panel();
            pageDetailExecution = new Panel();

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
            pageListeHistorique.AutoScroll = true;
            pageListeHistorique.Padding = new Padding(22, 25, 22, 25);

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

            panelGlobal.Controls.Add(lblTitre);
            panelGlobal.Controls.Add(panelFiltres);
            panelGlobal.Controls.Add(panelTableau);

            pageListeHistorique.Controls.Add(panelGlobal);
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

            btnRechercher.Height = cboStatut.Height;
            btnRechercher.Width = cboStatut.Width;
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
            // On réduit un peu "Bases traitées" et "Lignes",
            // puis on donne plus d'espace à la colonne Actions.
            ligne.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 155)); // Date / heure
            ligne.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 135)); // Origine
            ligne.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 195)); // Bases traitées
            ligne.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 125)); // Lignes
            ligne.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 95));  // Durée
            ligne.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 135)); // Statut
            ligne.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 153)); // Actions
        }

        private Panel CreerSeparateur()
        {
            Panel separateur = new Panel();
            HistoriqueStyle.AppliquerSeparateur(separateur);
            return separateur;
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

            Button bouton = new Button();
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

            AfficherPageDetail();
        }

        private Panel CreerCarteResumeExecution(HistoriqueExecutionModel execution)
        {
            Panel panel = new Panel();
            HistoriqueDetailStyle.AppliquerCarteResume(panel);

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
            HistoriqueDetailStyle.AppliquerBadgeStatut(badge, execution.Statut);
            badge.Location = new Point(890, 24);

            panel.Controls.Add(icone);
            panel.Controls.Add(lblTitre);
            panel.Controls.Add(lblSousTitre);
            panel.Controls.Add(badge);

            return panel;
        }

        private FlowLayoutPanel CreerZoneCartesInformation(HistoriqueExecutionModel execution)
        {
            FlowLayoutPanel zoneCartes = new FlowLayoutPanel();
            HistoriqueDetailStyle.AppliquerZoneCartes(zoneCartes);

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
            carte.Width = 975;
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
                lblMessage.Text = $"{etape.Statut} : {etape.Message}";
                HistoriqueDetailStyle.AppliquerMessageErreur(lblMessage);
                lblMessage.Location = new Point(0, positionY);

                carte.Controls.Add(lblMessage);

                positionY += hauteurMessage;
            }

            RichTextBox zoneLogs = new RichTextBox();
            HistoriqueDetailStyle.AppliquerZoneLogs(zoneLogs);
            zoneLogs.Location = new Point(0, positionY);
            zoneLogs.Text = ConstruireTexteLogs(etape);

            carte.Controls.Add(zoneLogs);

            return carte;
        }

        private Panel CreerEnteteBase(HistoriqueEtapeExecutionModel etape)
        {
            Panel entete = new Panel();
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
            lblLignes.Text = etape.LignesAffichage;
            HistoriqueDetailStyle.AppliquerLignesBase(lblLignes);

            Label badge = new Label();
            HistoriqueDetailStyle.AppliquerBadgeStatut(badge, etape.Statut);
            badge.Location = new Point(908, 8);

            entete.Controls.Add(icone);
            entete.Controls.Add(lblNomBase);
            entete.Controls.Add(lblLignes);
            entete.Controls.Add(badge);

            return entete;
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
        }

        private void AfficherPageDetail()
        {
            pageListeHistorique.Visible = false;
            pageDetailExecution.Visible = true;
            pageDetailExecution.BringToFront();
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