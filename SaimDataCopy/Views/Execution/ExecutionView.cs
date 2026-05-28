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
        private const int LargeurPage = 976;

        private readonly Label _lblBasesSelectionnees = new Label();
        private readonly Label _lblLignesCopiees = new Label();
        private readonly Label _lblDuree = new Label();

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

        public event EventHandler? TesterConnexionDemandee;
        public event EventHandler? LancerCopieDemandee;
        public event EventHandler? AnnulerCopieDemandee;

        public ExecutionView()
        {
            Dock = DockStyle.Fill;
            ExecutionStyle.AppliquerFondPage(this);

            CreerInterface();
        }

        private void CreerInterface()
        {
            Panel panelContenu = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true
            };

            ExecutionStyle.AppliquerFondPage(panelContenu);
            Controls.Add(panelContenu);

            Panel panelPage = new Panel
            {
                Location = new Point(25, 25),
                Size = new Size(LargeurPage, 820)
            };

            ExecutionStyle.AppliquerFondPage(panelPage);
            panelContenu.Controls.Add(panelPage);

            Label lblTitre = new Label
            {
                Text = "Lancement manuel de la copie",
                Location = new Point(0, 0)
            };

            ExecutionStyle.AppliquerTitrePage(lblTitre);
            panelPage.Controls.Add(lblTitre);

            AjouterCartesTableauBord(panelPage);
            AjouterJournal(panelPage);
            AjouterProgression(panelPage);
            AjouterTableauResultats(panelPage);
            AjouterBoutonsAction(panelPage);

            // Au démarrage, on cache la progression.
            AfficherZoneProgression(false);

            // Le bouton Annuler est désactivé tant que la copie n'est pas lancée.
            ActiverBoutonAnnuler(false);

            AjouterLog(new ExecutionLogModel
            {
                Heure = DateTime.Now.ToString("HH:mm:ss"),
                Message = "En attente du lancement de la copie...",
                Type = "Info"
            });
        }

        private void AjouterCartesTableauBord(Panel parent)
        {
            Panel carteBases = CreerCarteTableauBord(
                "Bases sélectionnées",
                _lblBasesSelectionnees,
                "0",
                new Point(0, 58)
            );

            Panel carteLignes = CreerCarteTableauBord(
                "Lignes copiées (dernière fois)",
                _lblLignesCopiees,
                "0",
                new Point(330, 58)
            );

            Panel carteDuree = CreerCarteTableauBord(
                "Durée dernière exécution",
                _lblDuree,
                "-",
                new Point(660, 58)
            );

            parent.Controls.Add(carteBases);
            parent.Controls.Add(carteLignes);
            parent.Controls.Add(carteDuree);
        }

        private Panel CreerCarteTableauBord(string titre, Label labelValeur, string valeurDefaut, Point position)
        {
            Panel panelCarte = new Panel
            {
                Location = position,
                Size = new Size(315, 102)
            };

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

            return panelCarte;
        }

        private void AjouterJournal(Panel parent)
        {
            Label lblJournal = new Label
            {
                Text = "Journal d'exécution en direct",
                Location = new Point(0, 192)
            };

            ExecutionStyle.AppliquerSousTitre(lblJournal);
            parent.Controls.Add(lblJournal);

            Panel panelJournal = new Panel
            {
                Location = new Point(0, 228),
                Size = new Size(LargeurPage, 175)
            };

            ExecutionStyle.AppliquerPanelJournal(panelJournal);
            parent.Controls.Add(panelJournal);

            _txtJournal.Location = new Point(12, 10);
            _txtJournal.Size = new Size(LargeurPage - 24, 155);

            ExecutionStyle.AppliquerJournal(_txtJournal);
            panelJournal.Controls.Add(_txtJournal);
        }

        private void AjouterProgression(Panel parent)
        {
            _panelProgression.Location = new Point(0, 416);
            _panelProgression.Size = new Size(LargeurPage, 55);
            ExecutionStyle.AppliquerFondPage(_panelProgression);
            parent.Controls.Add(_panelProgression);

            _progressBar.Location = new Point(0, 0);
            _progressBar.Width = LargeurPage;
            ExecutionStyle.AppliquerProgressBar(_progressBar);
            _panelProgression.Controls.Add(_progressBar);

            _lblProgression.Text = "Progression : 0%";
            _lblProgression.Location = new Point(0, 32);
            ExecutionStyle.AppliquerLabelProgression(_lblProgression);
            _panelProgression.Controls.Add(_lblProgression);
        }

        private void AjouterTableauResultats(Panel parent)
        {
            _lblResultats.Text = "Résumé de la dernière exécution";
            _lblResultats.Location = new Point(0, 500);

            ExecutionStyle.AppliquerSousTitre(_lblResultats);
            parent.Controls.Add(_lblResultats);

            _grilleResultats.Location = new Point(0, 536);
            _grilleResultats.Size = new Size(LargeurPage, 220);

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

            parent.Controls.Add(_grilleResultats);
        }

        private void AjouterBoutonsAction(Panel parent)
        {
            _panelActions.Location = new Point(0, 778);
            _panelActions.Size = new Size(LargeurPage, 45);
            ExecutionStyle.AppliquerFondPage(_panelActions);
            parent.Controls.Add(_panelActions);

            _btnTesterConnexion.Location = new Point(0, 0);
            ExecutionStyle.AppliquerBoutonTesterConnexion(_btnTesterConnexion);
            _btnTesterConnexion.Click += BtnTesterConnexion_Click;
            _panelActions.Controls.Add(_btnTesterConnexion);

            _btnAnnuler.Location = new Point(LargeurPage - 306, 0);
            ExecutionStyle.AppliquerBoutonAnnuler(_btnAnnuler);
            _btnAnnuler.Click += BtnAnnuler_Click;
            _panelActions.Controls.Add(_btnAnnuler);

            _btnLancerCopie.Location = new Point(LargeurPage - 170, 0);
            ExecutionStyle.AppliquerBoutonLancer(_btnLancerCopie);
            _btnLancerCopie.Click += BtnLancerCopie_Click;
            _panelActions.Controls.Add(_btnLancerCopie);
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

            _panelProgression.Visible = visible;

            if (visible)
            {
                _lblResultats.Location = new Point(0, 500);
                _grilleResultats.Location = new Point(0, 536);
                _panelActions.Location = new Point(0, 778);
            }
            else
            {
                // Quand la copie n'est pas lancée, on évite de laisser un grand vide.
                _lblResultats.Location = new Point(0, 438);
                _grilleResultats.Location = new Point(0, 474);
                _panelActions.Location = new Point(0, 716);
            }
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