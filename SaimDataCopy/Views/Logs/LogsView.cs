using FontAwesome.Sharp;
using SaimDataCopy.Models.Logs;
using SaimDataCopy.Styles;

namespace SaimDataCopy.Views.Logs
{
    /// <summary>
    /// Interface graphique de la page Paramètres Logs.
    /// Cette View contient seulement l'affichage.
    /// </summary>
    public class LogsView : UserControl, ILogsView
    {
        private const int LargeurControles = 682;
        private const int HauteurChamp = 38;
        private const int EspaceEntreChamps = 62;
        private const int LargeurBoutonParcourir = 125;
        private const int EspaceBouton = 8;
        private const int DecalageVertical = 25;

        private readonly TextBox _txtRepertoireLogs;
        private readonly IconButton _btnParcourir;
        private readonly TextBox _txtNommageFichiers;
        private readonly NumericUpDown _nudDureeConservation;
        private readonly NumericUpDown _nudTailleMaxFichier;

        public event EventHandler? ParcourirDemande;

        public LogsView()
        {
            Dock = DockStyle.Fill;
            BackColor = Color.White;

            _txtRepertoireLogs = new TextBox();
            _btnParcourir = new IconButton();
            _txtNommageFichiers = new TextBox();
            _nudDureeConservation = new NumericUpDown();
            _nudTailleMaxFichier = new NumericUpDown();

            CreerInterface();
        }

        private void CreerInterface()
        {
            Panel panelContenu = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                AutoScroll = true
            };

            Controls.Add(panelContenu);

            // Panel interne pour décaler toute l'interface vers la droite.
            // Comme ça, on ne modifie pas chaque champ un par un.
            Panel panelFormulaire = new Panel
            {
                Location = new Point(25, 0),
                Size = new Size(900, 650),
                BackColor = Color.White
            };

            panelContenu.Controls.Add(panelFormulaire);

            Label lblTitre = new Label
            {
                Text = "Paramètres de journalisation",
                Location = new Point(0, DecalageVertical)
            };

            LogFormStyle.AppliquerTitre(lblTitre);
            panelFormulaire.Controls.Add(lblTitre);

            int positionY = 58 + DecalageVertical;

            AjouterChampRepertoire(panelFormulaire, ref positionY);
            AjouterChampTexte(panelFormulaire, "Nommage des fichiers", _txtNommageFichiers, ref positionY);
            AjouterChampNombre(panelFormulaire, "Durée de conservation en jours", _nudDureeConservation, ref positionY, 1, 3650, 30);
            AjouterChampNombre(panelFormulaire, "Taille maximale d'un fichier en Mo", _nudTailleMaxFichier, ref positionY, 1, 1024, 10);

            AjouterBlocInformation(panelFormulaire, positionY);
        }

        private void AjouterChampRepertoire(Panel parent, ref int positionY)
        {
            Label label = new Label
            {
                Text = "Répertoire des logs",
                Location = new Point(0, positionY)
            };

            LogFormStyle.AppliquerLabel(label);
            parent.Controls.Add(label);

            positionY += 25;

            int largeurChampRepertoire = LargeurControles - LargeurBoutonParcourir - EspaceBouton;

            _txtRepertoireLogs.Location = new Point(0, positionY);
            _txtRepertoireLogs.Width = largeurChampRepertoire;
            _txtRepertoireLogs.Height = HauteurChamp;
            LogFormStyle.AppliquerChampTexte(_txtRepertoireLogs);
            parent.Controls.Add(_txtRepertoireLogs);

            _btnParcourir.Text = "Parcourir";
            _btnParcourir.IconChar = IconChar.FolderOpen;
            _btnParcourir.IconSize = 20;
            _btnParcourir.IconColor = Color.FromArgb(40, 40, 40);
            _btnParcourir.TextImageRelation = TextImageRelation.ImageBeforeText;
            _btnParcourir.ImageAlign = ContentAlignment.MiddleLeft;
            _btnParcourir.TextAlign = ContentAlignment.MiddleCenter;
            _btnParcourir.Location = new Point(largeurChampRepertoire + EspaceBouton, positionY);
            _btnParcourir.Width = LargeurBoutonParcourir;
            _btnParcourir.Height = HauteurChamp;
            LogFormStyle.AppliquerBoutonParcourir(_btnParcourir);
            _btnParcourir.Click += BtnParcourir_Click;
            parent.Controls.Add(_btnParcourir);

            positionY += EspaceEntreChamps;
        }

        private void AjouterChampTexte(Panel parent, string texteLabel, TextBox textBox, ref int positionY)
        {
            Label label = new Label
            {
                Text = texteLabel,
                Location = new Point(0, positionY)
            };

            LogFormStyle.AppliquerLabel(label);
            parent.Controls.Add(label);

            positionY += 25;

            textBox.Location = new Point(0, positionY);
            textBox.Width = LargeurControles;
            textBox.Height = HauteurChamp;
            LogFormStyle.AppliquerChampTexte(textBox);
            parent.Controls.Add(textBox);

            IconPictureBox iconeInfo = new IconPictureBox
            {
                IconChar = IconChar.CircleInfo,
                IconColor = Color.FromArgb(30, 96, 190),
                IconSize = 18,
                Size = new Size(18, 18),
                Location = new Point(LargeurControles + 12, positionY + 10),
                BackColor = Color.White
            };

            parent.Controls.Add(iconeInfo);

            positionY += EspaceEntreChamps;
        }

        private void AjouterChampNombre(
                Panel parent,
                string texteLabel,
                NumericUpDown numeric,
                ref int positionY,
                int minimum,
                int maximum,
                int valeurDefaut)
        {
            Label label = new Label
            {
                Text = texteLabel,
                Location = new Point(0, positionY)
            };

            LogFormStyle.AppliquerLabel(label);
            parent.Controls.Add(label);

            positionY += 25;

            Panel panelNombre = new Panel
            {
                Location = new Point(0, positionY),
                Size = new Size(LargeurControles, HauteurChamp)
            };

            LogFormStyle.AppliquerPanelChampNombre(panelNombre);
            parent.Controls.Add(panelNombre);

            numeric.Location = new Point(8, 7);
            numeric.Size = new Size(LargeurControles - 16, 28);
            numeric.Minimum = minimum;
            numeric.Maximum = maximum;
            numeric.Value = valeurDefaut;
            numeric.Increment = 1;

            LogFormStyle.AppliquerChampNombre(numeric);
            panelNombre.Controls.Add(numeric);

            positionY += EspaceEntreChamps;
        }

        private void AjouterBlocInformation(Panel parent, int positionY)
        {
            Panel panelInfo = new Panel
            {
                Location = new Point(0, positionY),
                Width = LargeurControles
            };

            LogFormStyle.AppliquerBlocInformation(panelInfo);
            parent.Controls.Add(panelInfo);

            IconPictureBox iconeInfo = new IconPictureBox
            {
                IconChar = IconChar.CircleInfo,
                IconColor = Color.FromArgb(30, 96, 190),
                IconSize = 18,
                Size = new Size(20, 20),
                Location = new Point(25, 18),
                BackColor = panelInfo.BackColor
            };

            panelInfo.Controls.Add(iconeInfo);

            Label lblTexte = new Label
            {
                Text = "Exemple de nom de fichier : log_2025-05-18_14-30-00.txt",
                Location = new Point(45, 14)
            };

            LogFormStyle.AppliquerTexteInformation(lblTexte);
            panelInfo.Controls.Add(lblTexte);
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
            ParcourirDemande?.Invoke(this, EventArgs.Empty);
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