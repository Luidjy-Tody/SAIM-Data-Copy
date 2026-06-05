using FontAwesome.Sharp;
using SaimDataCopy.Models.Logs;
using SaimDataCopy.Styles;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace SaimDataCopy.Views.Logs
{
    /// <summary>
    /// Interface graphique de la page Paramètres Logs.
    /// Cette View contient seulement l'affichage.
    /// </summary>
    public class LogsView : UserControl, ILogsView
    {
        private const int LargeurMinimumContenu = 700;
        private const int HauteurChamp = 38;
        private const int LargeurBoutonParcourir = 125;
        private const int EspaceBouton = 8;

        private readonly Panel panelContenu = new Panel();
        private readonly TableLayoutPanel layoutPrincipal = new TableLayoutPanel();

        private readonly TextBox _txtRepertoireLogs = new TextBox();
        private readonly IconButton _btnParcourir = new IconButton();
        private readonly TextBox _txtNommageFichiers = new TextBox();
        private readonly NumericUpDown _nudDureeConservation = new NumericUpDown();
        private readonly NumericUpDown _nudTailleMaxFichier = new NumericUpDown();

        public LogsView()
        {
            Dock = DockStyle.Fill;
            BackColor = Color.White;

            CreerInterface();
        }

        private void CreerInterface()
        {
            panelContenu.Dock = DockStyle.Fill;
            panelContenu.BackColor = Color.White;
            panelContenu.AutoScroll = true;
            panelContenu.Padding = new Padding(25, 25, 25, 25);

            Controls.Add(panelContenu);

            layoutPrincipal.ColumnCount = 1;
            layoutPrincipal.RowCount = 0;
            layoutPrincipal.Dock = DockStyle.Top;
            layoutPrincipal.AutoSize = true;
            layoutPrincipal.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            layoutPrincipal.BackColor = Color.White;
            layoutPrincipal.Margin = new Padding(0);
            layoutPrincipal.Padding = new Padding(0);

            layoutPrincipal.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            panelContenu.Controls.Add(layoutPrincipal);

            int ligne = 0;

            AjouterTitre(ref ligne);
            AjouterChampPleineLargeur(ref ligne, CreerChampRepertoire());
            AjouterChampPleineLargeur(ref ligne, CreerChampTexte("Nommage des fichiers", _txtNommageFichiers, true));
            AjouterChampPleineLargeur(ref ligne, CreerChampNombre("Durée de conservation en jours", _nudDureeConservation, 1, 3650, 30));
            AjouterChampPleineLargeur(ref ligne, CreerChampNombre("Taille maximale d'un fichier en Mo", _nudTailleMaxFichier, 1, 1024, 10));
            AjouterChampPleineLargeur(ref ligne, CreerBlocInformation());

            panelContenu.Resize += (sender, e) =>
            {
                AdapterLargeurContenu();
            };

            AdapterLargeurContenu();
        }

        private void AdapterLargeurContenu()
        {
            int largeur = panelContenu.ClientSize.Width
                - panelContenu.Padding.Left
                - panelContenu.Padding.Right
                - SystemInformation.VerticalScrollBarWidth;

            if (largeur < LargeurMinimumContenu)
            {
                largeur = LargeurMinimumContenu;
            }

            layoutPrincipal.Width = largeur;
        }

        private void AjouterTitre(ref int ligne)
        {
            Label lblTitre = new Label();
            lblTitre.Text = "Paramètres de journalisation";
            lblTitre.Margin = new Padding(0, 0, 0, 28);

            LogFormStyle.AppliquerTitre(lblTitre);

            AjouterLigneAuto();
            layoutPrincipal.Controls.Add(lblTitre, 0, ligne);

            ligne++;
        }

        private void AjouterChampPleineLargeur(ref int ligne, Panel champ)
        {
            AjouterLigneAuto();

            champ.Dock = DockStyle.Fill;
            champ.Margin = new Padding(0, 0, 0, 22);

            layoutPrincipal.Controls.Add(champ, 0, ligne);

            ligne++;
        }

        private void AjouterLigneAuto()
        {
            layoutPrincipal.RowCount++;
            layoutPrincipal.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        }

        private Panel CreerChampRepertoire()
        {
            Panel panel = CreerPanelChamp(70);

            Label label = new Label();
            label.Text = "Répertoire des logs";
            label.Location = new Point(0, 0);

            LogFormStyle.AppliquerLabel(label);
            panel.Controls.Add(label);

            _txtRepertoireLogs.Location = new Point(0, 25);
            _txtRepertoireLogs.Height = HauteurChamp;

            LogFormStyle.AppliquerChampTexte(_txtRepertoireLogs);
            panel.Controls.Add(_txtRepertoireLogs);

            _btnParcourir.Text = "Parcourir";
            _btnParcourir.IconChar = IconChar.FolderOpen;
            _btnParcourir.IconSize = 20;
            _btnParcourir.IconColor = Color.FromArgb(40, 40, 40);
            _btnParcourir.TextImageRelation = TextImageRelation.ImageBeforeText;
            _btnParcourir.ImageAlign = ContentAlignment.MiddleLeft;
            _btnParcourir.TextAlign = ContentAlignment.MiddleCenter;
            _btnParcourir.Height = HauteurChamp;

            LogFormStyle.AppliquerBoutonParcourir(_btnParcourir);

            _btnParcourir.Click += BtnParcourir_Click;
            panel.Controls.Add(_btnParcourir);

            panel.Resize += (sender, e) =>
            {
                int largeurChamp = panel.Width - LargeurBoutonParcourir - EspaceBouton;

                if (largeurChamp < 300)
                {
                    largeurChamp = 300;
                }

                _txtRepertoireLogs.Width = largeurChamp;

                _btnParcourir.Location = new Point(
                    largeurChamp + EspaceBouton,
                    25
                );

                _btnParcourir.Width = LargeurBoutonParcourir;
            };

            return panel;
        }

        private Panel CreerChampTexte(string texteLabel, TextBox textBox, bool afficherIconeInfo)
        {
            Panel panel = CreerPanelChamp(70);

            Label label = new Label();
            label.Text = texteLabel;
            label.Location = new Point(0, 0);

            LogFormStyle.AppliquerLabel(label);
            panel.Controls.Add(label);

            textBox.Location = new Point(0, 25);
            textBox.Height = HauteurChamp;

            LogFormStyle.AppliquerChampTexte(textBox);
            panel.Controls.Add(textBox);

            IconPictureBox? iconeInfo = null;

            if (afficherIconeInfo)
            {
                iconeInfo = new IconPictureBox();
                iconeInfo.IconChar = IconChar.CircleInfo;
                iconeInfo.IconColor = Color.FromArgb(30, 96, 190);
                iconeInfo.IconSize = 18;
                iconeInfo.Size = new Size(18, 18);
                iconeInfo.BackColor = Color.White;

                panel.Controls.Add(iconeInfo);
            }

            panel.Resize += (sender, e) =>
            {
                int margeIcone = afficherIconeInfo ? 35 : 0;

                textBox.Width = panel.Width - margeIcone;

                if (iconeInfo != null)
                {
                    iconeInfo.Location = new Point(
                        panel.Width - 22,
                        35
                    );
                }
            };

            return panel;
        }

        private Panel CreerChampNombre(
            string texteLabel,
            NumericUpDown numeric,
            int minimum,
            int maximum,
            int valeurDefaut)
        {
            Panel panel = CreerPanelChamp(70);

            Label label = new Label();
            label.Text = texteLabel;
            label.Location = new Point(0, 0);

            LogFormStyle.AppliquerLabel(label);
            panel.Controls.Add(label);

            Panel panelNombre = new Panel();
            panelNombre.Location = new Point(0, 25);
            panelNombre.Height = HauteurChamp;

            LogFormStyle.AppliquerPanelChampNombre(panelNombre);
            panel.Controls.Add(panelNombre);

            numeric.Location = new Point(8, 7);
            numeric.Height = 28;
            numeric.Minimum = minimum;
            numeric.Maximum = maximum;
            numeric.Value = valeurDefaut;
            numeric.Increment = 1;

            LogFormStyle.AppliquerChampNombre(numeric);
            panelNombre.Controls.Add(numeric);

            panel.Resize += (sender, e) =>
            {
                panelNombre.Width = panel.Width;
                numeric.Width = panelNombre.Width - 16;
            };

            return panel;
        }

        private Panel CreerBlocInformation()
        {
            Panel panelInfo = new Panel();
            panelInfo.Height = 48;
            panelInfo.BackColor = Color.White;

            LogFormStyle.AppliquerBlocInformation(panelInfo);

            IconPictureBox iconeInfo = new IconPictureBox();
            iconeInfo.IconChar = IconChar.CircleInfo;
            iconeInfo.IconColor = Color.FromArgb(30, 96, 190);
            iconeInfo.IconSize = 18;
            iconeInfo.Size = new Size(20, 20);
            iconeInfo.Location = new Point(16, 14);
            iconeInfo.BackColor = panelInfo.BackColor;

            panelInfo.Controls.Add(iconeInfo);

            Label lblTexte = new Label();
            lblTexte.Text = "Exemple de nom de fichier : log_2025-05-18_14-30-00.txt";
            lblTexte.Location = new Point(45, 13);
            lblTexte.Height = 24;
            lblTexte.AutoSize = false;
            lblTexte.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            LogFormStyle.AppliquerTexteInformation(lblTexte);

            panelInfo.Controls.Add(lblTexte);

            panelInfo.Resize += (sender, e) =>
            {
                lblTexte.Width = panelInfo.Width - lblTexte.Left - 15;
            };

            return panelInfo;
        }

        private Panel CreerPanelChamp(int hauteur)
        {
            Panel panel = new Panel();
            panel.Height = hauteur;
            panel.BackColor = Color.White;

            return panel;
        }

        public LogConfigModel RecupererConfiguration()
        {
            return new LogConfigModel
            {
                RepertoireLogs = _txtRepertoireLogs.Text.Trim(),
                NommageFichiers = _txtNommageFichiers.Text.Trim(),
                DureeConservationJours = (int)_nudDureeConservation.Value,
                TailleMaxFichierMo = (int)_nudTailleMaxFichier.Value
            };
        }

        public void AfficherConfiguration(LogConfigModel configuration)
        {
            _txtRepertoireLogs.Text = configuration.RepertoireLogs;
            _txtNommageFichiers.Text = configuration.NommageFichiers;

            _nudDureeConservation.Value = AjusterValeur(
                configuration.DureeConservationJours,
                _nudDureeConservation.Minimum,
                _nudDureeConservation.Maximum
            );

            _nudTailleMaxFichier.Value = AjusterValeur(
                configuration.TailleMaxFichierMo,
                _nudTailleMaxFichier.Minimum,
                _nudTailleMaxFichier.Maximum
            );
        }

        public void ModifierRepertoireLogs(string chemin)
        {
            _txtRepertoireLogs.Text = chemin;
        }

        public void AfficherMessage(string message, bool succes)
        {
            MessageBoxIcon icone = succes ? MessageBoxIcon.Information : MessageBoxIcon.Error;
            string titre = succes ? "Succès" : "Erreur";

            MessageBox.Show(message, titre, MessageBoxButtons.OK, icone);
        }

        private void BtnParcourir_Click(object? sender, EventArgs e)
        {
            string cheminInitial = _txtRepertoireLogs.Text.Trim();

            using DossierLogsSelectionForm fenetreSelection =
                new DossierLogsSelectionForm(cheminInitial);

            Form? fenetreParent = FindForm();

            DialogResult resultat;

            if (fenetreParent != null)
            {
                resultat = fenetreSelection.ShowDialog(fenetreParent);
            }
            else
            {
                resultat = fenetreSelection.ShowDialog();
            }

            if (resultat == DialogResult.OK &&
                !string.IsNullOrWhiteSpace(fenetreSelection.DossierSelectionne))
            {
                _txtRepertoireLogs.Text = fenetreSelection.DossierSelectionne;
            }
        }

        private static decimal AjusterValeur(int valeur, decimal minimum, decimal maximum)
        {
            if (valeur < minimum)
            {
                return minimum;
            }

            if (valeur > maximum)
            {
                return maximum;
            }

            return valeur;
        }
    }
}