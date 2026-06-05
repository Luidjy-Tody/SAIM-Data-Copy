using FontAwesome.Sharp;
using SaimDataCopy.Models.Execution;
using SaimDataCopy.Styles;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace SaimDataCopy.Views.Execution
{
    /// <summary>
    /// Interface graphique de la page Exécution.
    /// Cette View contient seulement l'affichage.
    /// </summary>
    public class ExecutionView : UserControl, IExecutionView
    {
        private const int LargeurMinimumPage = 976;
        private const int HauteurMinimumPage = 820;
        private const int MargePage = 25;

        private readonly Panel _panelContenu = new Panel();
        private readonly Panel _panelPage = new Panel();

        private readonly Label _lblTitre = new Label();

        private readonly Panel _carteBases = new Panel();
        private readonly Panel _carteLignes = new Panel();
        private readonly Panel _carteDuree = new Panel();

        private readonly Label _lblBasesSelectionnees = new Label();
        private readonly Label _lblLignesCopiees = new Label();
        private readonly Label _lblDuree = new Label();

        private readonly Label _lblJournal = new Label();
        private readonly Panel _panelJournal = new Panel();
        private readonly RichTextBox _txtJournal = new RichTextBox();

        private readonly Panel _panelProgression = new Panel();
        private readonly ProgressBar _progressBar = new ProgressBar();
        private readonly Label _lblProgression = new Label();

        private readonly Label _lblResultats = new Label();
        private readonly DataGridView _grilleResultats = new DataGridView();

        private readonly Panel _panelActions = new Panel();
        private readonly IconButton _btnTesterConnexion = new IconButton();
        private readonly IconButton _btnAnnuler = new IconButton();
        private readonly IconButton _btnLancerCopie = new IconButton();

        private bool _progressionVisible;

        public event EventHandler? TesterConnexionDemandee;
        public event EventHandler? LancerCopieDemandee;
        public event EventHandler? AnnulerCopieDemandee;

        public ExecutionView()
        {
            Dock = DockStyle.Fill;
            ExecutionStyle.AppliquerFondPage(this);

            _progressionVisible = false;

            CreerInterface();
        }

        private void CreerInterface()
        {
            _panelContenu.Dock = DockStyle.Fill;
            _panelContenu.AutoScroll = true;
            _panelContenu.Padding = new Padding(0);

            ExecutionStyle.AppliquerFondPage(_panelContenu);
            Controls.Add(_panelContenu);

            _panelPage.Location = new Point(MargePage, MargePage);
            _panelPage.Size = new Size(LargeurMinimumPage, HauteurMinimumPage);

            ExecutionStyle.AppliquerFondPage(_panelPage);
            _panelContenu.Controls.Add(_panelPage);

            _lblTitre.Text = "Lancement manuel de la copie";
            _lblTitre.Location = new Point(0, 0);

            ExecutionStyle.AppliquerTitrePage(_lblTitre);
            _panelPage.Controls.Add(_lblTitre);

            AjouterCartesTableauBord();
            AjouterJournal();
            AjouterProgression();
            AjouterTableauResultats();
            AjouterBoutonsAction();

            AfficherZoneProgression(false);
            ActiverBoutonAnnuler(false);

            AjouterLog(new ExecutionLogModel
            {
                Heure = DateTime.Now.ToString("HH:mm:ss"),
                Message = "En attente du lancement de la copie...",
                Type = "Info"
            });

            _panelContenu.Resize += (sender, e) =>
            {
                AdapterDisposition();
            };

            AdapterDisposition();
        }

        private void AjouterCartesTableauBord()
        {
            ConfigurerCarteTableauBord(
                _carteBases,
                "Bases sélectionnées",
                _lblBasesSelectionnees,
                "0"
            );

            ConfigurerCarteTableauBord(
                _carteLignes,
                "Lignes copiées (dernière fois)",
                _lblLignesCopiees,
                "0"
            );

            ConfigurerCarteTableauBord(
                _carteDuree,
                "Durée dernière exécution",
                _lblDuree,
                "-"
            );

            _panelPage.Controls.Add(_carteBases);
            _panelPage.Controls.Add(_carteLignes);
            _panelPage.Controls.Add(_carteDuree);
        }

        private void ConfigurerCarteTableauBord(
            Panel panelCarte,
            string titre,
            Label labelValeur,
            string valeurDefaut)
        {
            panelCarte.Size = new Size(315, 102);
            ExecutionStyle.AppliquerCarteTableauBord(panelCarte);

            labelValeur.Text = valeurDefaut;
            labelValeur.Location = new Point(16, 22);
            ExecutionStyle.AppliquerValeurCarte(labelValeur);
            panelCarte.Controls.Add(labelValeur);

            Label lblTitreCarte = new Label
            {
                Text = titre,
                Location = new Point(16, 66)
            };

            ExecutionStyle.AppliquerTitreCarte(lblTitreCarte);
            panelCarte.Controls.Add(lblTitreCarte);
        }

        private void AjouterJournal()
        {
            _lblJournal.Text = "Journal d'exécution en direct";
            _lblJournal.Location = new Point(0, 192);

            ExecutionStyle.AppliquerSousTitre(_lblJournal);
            _panelPage.Controls.Add(_lblJournal);

            _panelJournal.Location = new Point(0, 228);
            _panelJournal.Size = new Size(LargeurMinimumPage, 175);

            ExecutionStyle.AppliquerPanelJournal(_panelJournal);
            _panelPage.Controls.Add(_panelJournal);

            _txtJournal.Location = new Point(12, 10);
            _txtJournal.Size = new Size(LargeurMinimumPage - 24, 155);

            ExecutionStyle.AppliquerJournal(_txtJournal);
            _panelJournal.Controls.Add(_txtJournal);
        }

        private void AjouterProgression()
        {
            _panelProgression.Location = new Point(0, 416);
            _panelProgression.Size = new Size(LargeurMinimumPage, 55);

            ExecutionStyle.AppliquerFondPage(_panelProgression);
            _panelPage.Controls.Add(_panelProgression);

            _progressBar.Location = new Point(0, 0);
            _progressBar.Width = LargeurMinimumPage;

            ExecutionStyle.AppliquerProgressBar(_progressBar);
            _panelProgression.Controls.Add(_progressBar);

            _lblProgression.Text = "Progression : 0%";
            _lblProgression.Location = new Point(0, 32);

            ExecutionStyle.AppliquerLabelProgression(_lblProgression);
            _panelProgression.Controls.Add(_lblProgression);
        }

        private void AjouterTableauResultats()
        {
            _lblResultats.Text = "Résumé de la dernière exécution";
            _lblResultats.Location = new Point(0, 438);

            ExecutionStyle.AppliquerSousTitre(_lblResultats);
            _panelPage.Controls.Add(_lblResultats);

            _grilleResultats.Location = new Point(0, 474);
            _grilleResultats.Size = new Size(LargeurMinimumPage, 220);

            ExecutionStyle.AppliquerTableauResultat(_grilleResultats);

            DataGridViewTextBoxColumn colBase = new DataGridViewTextBoxColumn
            {
                Name = "colBase",
                HeaderText = "Base",
                FillWeight = 170,
                ReadOnly = true
            };

            DataGridViewTextBoxColumn colAvant = new DataGridViewTextBoxColumn
            {
                Name = "colAvant",
                HeaderText = "Lignes avant",
                FillWeight = 130,
                ReadOnly = true
            };

            DataGridViewTextBoxColumn colApres = new DataGridViewTextBoxColumn
            {
                Name = "colApres",
                HeaderText = "Lignes après",
                FillWeight = 130,
                ReadOnly = true
            };

            DataGridViewTextBoxColumn colResultat = new DataGridViewTextBoxColumn
            {
                Name = "colResultat",
                HeaderText = "Résultat",
                FillWeight = 160,
                ReadOnly = true
            };

            _grilleResultats.Columns.AddRange(
                colBase,
                colAvant,
                colApres,
                colResultat
            );

            ExecutionStyle.DesactiverTriColonnes(_grilleResultats);

            _panelPage.Controls.Add(_grilleResultats);
        }

        private void AjouterBoutonsAction()
        {
            _panelActions.Location = new Point(0, 716);
            _panelActions.Size = new Size(LargeurMinimumPage, 45);

            ExecutionStyle.AppliquerFondPage(_panelActions);
            _panelPage.Controls.Add(_panelActions);

            ExecutionStyle.AppliquerBoutonTesterConnexion(_btnTesterConnexion);
            _btnTesterConnexion.Click += BtnTesterConnexion_Click;
            _panelActions.Controls.Add(_btnTesterConnexion);

            ExecutionStyle.AppliquerBoutonAnnuler(_btnAnnuler);
            _btnAnnuler.Click += BtnAnnuler_Click;
            _panelActions.Controls.Add(_btnAnnuler);

            ExecutionStyle.AppliquerBoutonLancer(_btnLancerCopie);
            _btnLancerCopie.Click += BtnLancerCopie_Click;
            _panelActions.Controls.Add(_btnLancerCopie);
        }

        private void AdapterDisposition()
        {
            int largeurDisponible = _panelContenu.ClientSize.Width - (MargePage * 2);
            int hauteurDisponible = _panelContenu.ClientSize.Height - (MargePage * 2);

            if (largeurDisponible < LargeurMinimumPage)
            {
                largeurDisponible = LargeurMinimumPage;
            }

            if (hauteurDisponible < HauteurMinimumPage)
            {
                hauteurDisponible = HauteurMinimumPage;
            }

            _panelPage.Size = new Size(largeurDisponible, hauteurDisponible);

            AdapterCartes(largeurDisponible);
            AdapterJournal(largeurDisponible);
            AdapterProgression(largeurDisponible);
            AdapterTableauEtBoutons(largeurDisponible, hauteurDisponible);
        }

        private void AdapterCartes(int largeurPage)
        {
            int espace = 16;
            int largeurCarte = (largeurPage - (espace * 2)) / 3;

            if (largeurCarte < 280)
            {
                largeurCarte = 280;
            }

            _carteBases.Location = new Point(0, 58);
            _carteBases.Size = new Size(largeurCarte, 102);

            _carteLignes.Location = new Point(largeurCarte + espace, 58);
            _carteLignes.Size = new Size(largeurCarte, 102);

            _carteDuree.Location = new Point((largeurCarte + espace) * 2, 58);
            _carteDuree.Size = new Size(largeurCarte, 102);
        }

        private void AdapterJournal(int largeurPage)
        {
            _lblJournal.Location = new Point(0, 192);

            _panelJournal.Location = new Point(0, 228);
            _panelJournal.Size = new Size(largeurPage, 175);

            _txtJournal.Location = new Point(12, 10);
            _txtJournal.Size = new Size(largeurPage - 24, 155);
        }

        private void AdapterProgression(int largeurPage)
        {
            _panelProgression.Location = new Point(0, 416);
            _panelProgression.Size = new Size(largeurPage, 55);

            _progressBar.Location = new Point(0, 0);
            _progressBar.Width = largeurPage;

            _lblProgression.Location = new Point(0, 32);
        }

        private void AdapterTableauEtBoutons(int largeurPage, int hauteurPage)
        {
            int yTitreResultats;
            int yGrille;

            if (_progressionVisible)
            {
                yTitreResultats = 500;
                yGrille = 536;
            }
            else
            {
                yTitreResultats = 438;
                yGrille = 474;
            }

            int hauteurPanelActions = 45;
            int margeAvantBoutons = 20;

            int yActions = hauteurPage - hauteurPanelActions;

            int hauteurGrille = yActions - yGrille - margeAvantBoutons;

            if (hauteurGrille < 220)
            {
                hauteurGrille = 220;
                yActions = yGrille + hauteurGrille + margeAvantBoutons;
            }

            _lblResultats.Location = new Point(0, yTitreResultats);

            _grilleResultats.Location = new Point(0, yGrille);
            _grilleResultats.Size = new Size(largeurPage, hauteurGrille);

            _panelActions.Location = new Point(0, yActions);
            _panelActions.Size = new Size(largeurPage, hauteurPanelActions);

            AdapterBoutonsActions(largeurPage);
        }

        private void AdapterBoutonsActions(int largeurPage)
        {
            _btnTesterConnexion.Location = new Point(0, 0);

            _btnLancerCopie.Location = new Point(
                largeurPage - _btnLancerCopie.Width,
                0
            );

            _btnAnnuler.Location = new Point(
                _btnLancerCopie.Left - _btnAnnuler.Width - 15,
                0
            );
        }

        public void AfficherTableauBord(ExecutionTableauBordModel tableauBord)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => AfficherTableauBord(tableauBord)));
                return;
            }

            _lblBasesSelectionnees.Text = tableauBord.NombreBasesSelectionnees.ToString();
            _lblLignesCopiees.Text = tableauBord.NombreLignesCopiees.ToString("N0");
            _lblDuree.Text = tableauBord.DureeDerniereExecution;
        }

        public void AfficherResultats(List<ExecutionResultatBaseModel> resultats)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => AfficherResultats(resultats)));
                return;
            }

            _grilleResultats.Rows.Clear();

            foreach (ExecutionResultatBaseModel resultat in resultats)
            {
                int index = _grilleResultats.Rows.Add(
                    resultat.NomBase,
                    resultat.LignesAvant.ToString("N0"),
                    resultat.LignesApres.ToString("N0"),
                    resultat.Resultat
                );

                AppliquerStyleLigneResultat(index);
            }

            _grilleResultats.ClearSelection();
        }

        private void AppliquerStyleLigneResultat(int index)
        {
            DataGridViewRow ligne = _grilleResultats.Rows[index];

            Color couleurFond = Color.White;

            ligne.DefaultCellStyle.BackColor = couleurFond;
            ligne.DefaultCellStyle.SelectionBackColor = couleurFond;
            ligne.DefaultCellStyle.SelectionForeColor = Color.FromArgb(35, 35, 35);

            string resultat = ligne.Cells["colResultat"].Value?.ToString() ?? string.Empty;

            ligne.Cells["colResultat"].Style.ForeColor =
                ExecutionStyle.ObtenirCouleurResultat(resultat);
        }

        public void ViderJournal()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(ViderJournal));
                return;
            }

            _txtJournal.Clear();
        }

        public void AjouterLog(ExecutionLogModel log)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => AjouterLog(log)));
                return;
            }

            Color couleurMessage = ExecutionStyle.ObtenirCouleurLog(log.Type);

            _txtJournal.SelectionStart = _txtJournal.TextLength;

            _txtJournal.SelectionColor = Color.FromArgb(175, 175, 175);
            _txtJournal.AppendText($"[{log.Heure}] ");

            _txtJournal.SelectionColor = couleurMessage;
            _txtJournal.AppendText($"{log.Message}{Environment.NewLine}");

            _txtJournal.SelectionColor = _txtJournal.ForeColor;
            _txtJournal.ScrollToCaret();
        }

        public void AfficherZoneProgression(bool visible)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => AfficherZoneProgression(visible)));
                return;
            }

            _progressionVisible = visible;
            _panelProgression.Visible = visible;

            AdapterDisposition();
        }

        public void AfficherProgression(int pourcentage, string message)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => AfficherProgression(pourcentage, message)));
                return;
            }

            int valeur = Math.Max(0, Math.Min(100, pourcentage));

            _progressBar.Value = valeur;
            _lblProgression.Text = message;
        }

        public void ActiverBoutonTesterConnexion(bool actif)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => ActiverBoutonTesterConnexion(actif)));
                return;
            }

            _btnTesterConnexion.Enabled = actif;
        }

        public void ActiverBoutonLancer(bool actif)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => ActiverBoutonLancer(actif)));
                return;
            }

            _btnLancerCopie.Enabled = actif;
        }

        public void ActiverBoutonAnnuler(bool actif)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => ActiverBoutonAnnuler(actif)));
                return;
            }

            _btnAnnuler.Enabled = actif;
        }

        public void AfficherMessage(string message, string titre)
        {
            MessageBox.Show(
                message,
                titre,
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }

        private void BtnTesterConnexion_Click(object? sender, EventArgs e)
        {
            TesterConnexionDemandee?.Invoke(this, EventArgs.Empty);
        }

        private void BtnLancerCopie_Click(object? sender, EventArgs e)
        {
            LancerCopieDemandee?.Invoke(this, EventArgs.Empty);
        }

        private void BtnAnnuler_Click(object? sender, EventArgs e)
        {
            AnnulerCopieDemandee?.Invoke(this, EventArgs.Empty);
        }
    }
}