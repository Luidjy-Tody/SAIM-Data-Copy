using FontAwesome.Sharp;
using SaimDataCopy.Models.Email;
using SaimDataCopy.Styles;
using System.Drawing;
using System.Windows.Forms;
using SaimDataCopy.Views.Commun;

namespace SaimDataCopy.Views.Email
{
    /// <summary>
    /// Vue graphique pour la page Paramètres Email.
    /// Cette classe contient seulement l'interface WinForms.
    /// </summary>
    public class EmailView : UserControl, IEmailView, IPageEnregistrable
    {
        private const int LargeurMinimumContenu = 1000;

        public event EventHandler? EnregistrementDemande;
        public event EventHandler? TestEmailDemande;

        private readonly Panel panelContenu = new Panel();
        private readonly TableLayoutPanel layoutPrincipal = new TableLayoutPanel();

        private readonly TextBox txtServeurSmtp = new TextBox();
        private readonly TextBox txtPort = new TextBox();
        private readonly ComboBox cmbSecurite = new ComboBox();
        private readonly TextBox txtIdentifiantSmtp = new TextBox();
        private readonly TextBox txtMotDePasseSmtp = new TextBox();

        private readonly TextBox txtExpediteurFrom = new TextBox();
        private readonly TextBox txtDestinataireTo = new TextBox();
        private readonly TextBox txtCopieCc = new TextBox();
        private readonly TextBox txtCopieCacheeBcc = new TextBox();

        private readonly TextBox txtObjet = new TextBox();
        private readonly TextBox txtCorpsMessage = new TextBox();

        private readonly CheckBox chkActiverEnvoiEmail = new CheckBox();
        private readonly CheckBox chkJoindreFichierLog = new CheckBox();
        private readonly Button btnTesterEmail = new Button();

        private readonly IconButton btnAfficherMotDePasse = new IconButton();

        // Indique si l'utilisateur a modifié un champ sans enregistrer.
        private bool _aDesModificationsNonEnregistrees = false;

        // Évite de détecter des modifications pendant le chargement automatique.
        private bool _chargementEnCours = false;

        // Dernière configuration connue comme enregistrée.
        private EmailConfigModel _configurationEnregistree = new EmailConfigModel();

        public bool ADesModificationsNonEnregistrees => _aDesModificationsNonEnregistrees;

        public EmailView()
        {
            InitialiserInterface();
        }

        private void InitialiserInterface()
        {
            EmailFormStyle.AppliquerPage(this);
            AutoScroll = false;

            panelContenu.Dock = DockStyle.Fill;
            panelContenu.Padding = new Padding(25, 25, 25, 30);
            panelContenu.BackColor = Color.White;
            panelContenu.AutoScroll = true;

            Controls.Add(panelContenu);

            layoutPrincipal.ColumnCount = 6;
            layoutPrincipal.RowCount = 0;
            layoutPrincipal.Dock = DockStyle.Top;
            layoutPrincipal.AutoSize = true;
            layoutPrincipal.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            layoutPrincipal.BackColor = Color.White;
            layoutPrincipal.Margin = new Padding(0);
            layoutPrincipal.Padding = new Padding(0);

            for (int i = 0; i < 6; i++)
            {
                layoutPrincipal.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16.66F));
            }

            panelContenu.Controls.Add(layoutPrincipal);

            int ligne = 0;

            AjouterTitre(ref ligne);
            AjouterBarreInfo(ref ligne);

            AjouterTitreSection(ref ligne, "Serveur SMTP");

            AjouterTroisChamps(
                ref ligne,
                CreerChampTexte("Serveur SMTP", txtServeurSmtp, true),
                CreerChampTexte("Port", txtPort, false),
                CreerChampSecurite()
            );

            AjouterDeuxChamps(
                ref ligne,
                CreerChampTexte("Identifiant SMTP", txtIdentifiantSmtp, false),
                CreerChampMotDePasse("Mot de passe SMTP")
            );

            AjouterTitreSection(ref ligne, "Destinataires");

            AjouterDeuxChamps(
                ref ligne,
                CreerChampTexte("Expéditeur from", txtExpediteurFrom, false),
                CreerChampTexte("Destinataire To", txtDestinataireTo, true)
            );

            AjouterDeuxChamps(
                ref ligne,
                CreerChampTexte("Copie CC", txtCopieCc, false),
                CreerChampTexte("Copie cachée BCC", txtCopieCacheeBcc, false)
            );

            AjouterTitreSection(ref ligne, "Contenu du message");

            AjouterChampPleineLargeur(
                ref ligne,
                CreerChampTexte("Objet", txtObjet, false)
            );

            AjouterChampPleineLargeur(
                ref ligne,
                CreerChampMessage()
            );

            AjouterOptions(ref ligne);
            AjouterAlerte(ref ligne);
            AjouterBoutonTest(ref ligne);

            AjouterPlaceholders();

            panelContenu.Resize += (sender, e) =>
            {
                AdapterLargeurContenu();
            };

            AdapterLargeurContenu();

            BrancherDetectionModifications();
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
            lblTitre.Text = "Paramètres de notification e-mail";
            lblTitre.Margin = new Padding(0, 0, 0, 24);

            EmailFormStyle.AppliquerTitre(lblTitre);

            AjouterLigneAuto();
            layoutPrincipal.Controls.Add(lblTitre, 0, ligne);
            layoutPrincipal.SetColumnSpan(lblTitre, 6);

            ligne++;
        }

        private void AjouterBarreInfo(ref int ligne)
        {
            Panel panelInfo = new Panel();
            panelInfo.Height = 48;
            panelInfo.Margin = new Padding(0, 0, 0, 28);
            panelInfo.Dock = DockStyle.Fill;

            EmailFormStyle.AppliquerBarreInfo(panelInfo);

            IconPictureBox iconeInfo = new IconPictureBox();
            iconeInfo.IconChar = IconChar.CircleInfo;
            iconeInfo.IconColor = EmailFormStyle.CouleurBleu;
            iconeInfo.IconSize = 18;
            iconeInfo.Size = new Size(22, 22);
            iconeInfo.Location = new Point(16, 15);
            iconeInfo.BackColor = EmailFormStyle.CouleurFondInfo;

            Label lblInfo = new Label();
            lblInfo.Text = "Un e-mail de confirmation est envoyé après chaque copie réussie.";
            lblInfo.Location = new Point(50, 14);
            lblInfo.Height = 24;
            lblInfo.AutoSize = false;
            lblInfo.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            EmailFormStyle.AppliquerTexteInfo(lblInfo);

            panelInfo.Controls.Add(iconeInfo);
            panelInfo.Controls.Add(lblInfo);

            panelInfo.Resize += (sender, e) =>
            {
                lblInfo.Width = panelInfo.Width - lblInfo.Left - 15;
            };

            AjouterLigneAuto();
            layoutPrincipal.Controls.Add(panelInfo, 0, ligne);
            layoutPrincipal.SetColumnSpan(panelInfo, 6);

            ligne++;
        }

        private void AjouterTitreSection(ref int ligne, string texte)
        {
            Label label = new Label();
            label.Text = texte;
            label.Margin = new Padding(0, 6, 0, 16);

            EmailFormStyle.AppliquerTitreSection(label);

            AjouterLigneAuto();
            layoutPrincipal.Controls.Add(label, 0, ligne);
            layoutPrincipal.SetColumnSpan(label, 6);

            ligne++;
        }

        private void AjouterTroisChamps(ref int ligne, Panel champ1, Panel champ2, Panel champ3)
        {
            AjouterLigneAuto();

            champ1.Dock = DockStyle.Fill;
            champ2.Dock = DockStyle.Fill;
            champ3.Dock = DockStyle.Fill;

            champ1.Margin = new Padding(0, 0, 12, 22);
            champ2.Margin = new Padding(12, 0, 12, 22);
            champ3.Margin = new Padding(12, 0, 0, 22);

            layoutPrincipal.Controls.Add(champ1, 0, ligne);
            layoutPrincipal.SetColumnSpan(champ1, 2);

            layoutPrincipal.Controls.Add(champ2, 2, ligne);
            layoutPrincipal.SetColumnSpan(champ2, 2);

            layoutPrincipal.Controls.Add(champ3, 4, ligne);
            layoutPrincipal.SetColumnSpan(champ3, 2);

            ligne++;
        }

        private void AjouterDeuxChamps(ref int ligne, Panel champGauche, Panel champDroite)
        {
            AjouterLigneAuto();

            champGauche.Dock = DockStyle.Fill;
            champDroite.Dock = DockStyle.Fill;

            champGauche.Margin = new Padding(0, 0, 12, 22);
            champDroite.Margin = new Padding(12, 0, 0, 22);

            layoutPrincipal.Controls.Add(champGauche, 0, ligne);
            layoutPrincipal.SetColumnSpan(champGauche, 3);

            layoutPrincipal.Controls.Add(champDroite, 3, ligne);
            layoutPrincipal.SetColumnSpan(champDroite, 3);

            ligne++;
        }

        private void AjouterChampPleineLargeur(ref int ligne, Panel champ)
        {
            AjouterLigneAuto();

            champ.Dock = DockStyle.Fill;
            champ.Margin = new Padding(0, 0, 0, 22);

            layoutPrincipal.Controls.Add(champ, 0, ligne);
            layoutPrincipal.SetColumnSpan(champ, 6);

            ligne++;
        }

        private void AjouterLigneAuto()
        {
            layoutPrincipal.RowCount++;
            layoutPrincipal.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        }

        private Panel CreerChampTexte(string libelle, TextBox textBox, bool requis)
        {
            Panel panel = CreerPanelChamp(75);

            Label label = CreerLabel(libelle);
            label.Location = new Point(0, 0);
            panel.Controls.Add(label);

            if (requis)
            {
                Label badge = new Label();
                badge.Location = new Point(label.PreferredWidth + 8, 0);

                EmailFormStyle.AppliquerBadgeRequis(badge);

                panel.Controls.Add(badge);
            }

            Panel panelBordure = new Panel();
            panelBordure.Location = new Point(0, 27);
            panelBordure.Height = 40;
            panelBordure.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            EmailFormStyle.AppliquerPanelBordure(panelBordure);

            textBox.Location = new Point(10, 9);
            textBox.Height = 22;
            textBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            EmailFormStyle.AppliquerTextBoxDansBordure(textBox);

            panelBordure.Controls.Add(textBox);
            panel.Controls.Add(panelBordure);

            panel.Resize += (sender, e) =>
            {
                panelBordure.Width = panel.Width;
                textBox.Width = panelBordure.Width - 20;
            };

            return panel;
        }

        private Panel CreerChampSecurite()
        {
            Panel panel = CreerPanelChamp(75);

            Label label = CreerLabel("Sécurité");
            label.Location = new Point(0, 0);
            panel.Controls.Add(label);

            Panel panelBordure = new Panel();
            panelBordure.Location = new Point(0, 27);
            panelBordure.Height = 40;
            panelBordure.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            EmailFormStyle.AppliquerPanelBordure(panelBordure);

            cmbSecurite.Location = new Point(6, 7);
            cmbSecurite.Height = 26;
            cmbSecurite.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            EmailFormStyle.AppliquerComboBoxDansBordure(cmbSecurite);

            cmbSecurite.Items.AddRange(new object[]
            {
                "TLS",
                "SSL",
                "Aucune"
            });

            cmbSecurite.SelectedIndex = 0;

            panelBordure.Controls.Add(cmbSecurite);
            panel.Controls.Add(panelBordure);

            panel.Resize += (sender, e) =>
            {
                panelBordure.Width = panel.Width;
                cmbSecurite.Width = panelBordure.Width - 12;
            };

            return panel;
        }

        private Panel CreerChampMotDePasse(string libelle)
        {
            Panel panel = CreerPanelChamp(75);

            Label label = CreerLabel(libelle);
            label.Location = new Point(0, 0);
            panel.Controls.Add(label);

            Panel panelBordure = new Panel();
            panelBordure.Location = new Point(0, 27);
            panelBordure.Height = 40;
            panelBordure.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            EmailFormStyle.AppliquerPanelBordure(panelBordure);

            txtMotDePasseSmtp.Location = new Point(10, 9);
            txtMotDePasseSmtp.Height = 22;
            txtMotDePasseSmtp.PasswordChar = '●';
            txtMotDePasseSmtp.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            EmailFormStyle.AppliquerTextBoxDansBordure(txtMotDePasseSmtp);

            btnAfficherMotDePasse.IconChar = IconChar.Eye;
            btnAfficherMotDePasse.IconColor = Color.FromArgb(90, 90, 90);
            btnAfficherMotDePasse.IconSize = 16;
            btnAfficherMotDePasse.Size = new Size(35, 30);
            btnAfficherMotDePasse.FlatStyle = FlatStyle.Flat;
            btnAfficherMotDePasse.FlatAppearance.BorderSize = 0;
            btnAfficherMotDePasse.BackColor = Color.White;
            btnAfficherMotDePasse.Cursor = Cursors.Hand;
            btnAfficherMotDePasse.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnAfficherMotDePasse.Click += BtnAfficherMotDePasse_Click;

            panelBordure.Controls.Add(txtMotDePasseSmtp);
            panelBordure.Controls.Add(btnAfficherMotDePasse);
            panel.Controls.Add(panelBordure);

            panel.Resize += (sender, e) =>
            {
                panelBordure.Width = panel.Width;
                txtMotDePasseSmtp.Width = panelBordure.Width - 55;
                btnAfficherMotDePasse.Location = new Point(panelBordure.Width - 42, 4);
            };

            return panel;
        }

        private Panel CreerChampMessage()
        {
            Panel panel = CreerPanelChamp(185);

            Label lblCorps = CreerLabel("Corps du message");
            lblCorps.Location = new Point(0, 0);
            panel.Controls.Add(lblCorps);

            txtCorpsMessage.Location = new Point(0, 27);
            txtCorpsMessage.Height = 150;
            txtCorpsMessage.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            EmailFormStyle.AppliquerTextBoxMultiligne(txtCorpsMessage);

            panel.Controls.Add(txtCorpsMessage);

            panel.Resize += (sender, e) =>
            {
                txtCorpsMessage.Width = panel.Width;
            };

            return panel;
        }

        private Panel CreerPanelChamp(int hauteur)
        {
            Panel panel = new Panel();
            panel.Height = hauteur;
            panel.BackColor = Color.White;

            return panel;
        }

        private Label CreerLabel(string texte)
        {
            Label label = new Label();
            label.Text = texte;

            EmailFormStyle.AppliquerLabel(label);

            return label;
        }

        private void AjouterOptions(ref int ligne)
        {
            Panel panelOptions = new Panel();
            panelOptions.Height = 60;
            panelOptions.BackColor = Color.White;
            panelOptions.Margin = new Padding(0, 0, 0, 18);

            chkActiverEnvoiEmail.Text = "Activer l'envoi des e-mails";
            chkActiverEnvoiEmail.Location = new Point(0, 0);

            EmailFormStyle.AppliquerCheckBox(chkActiverEnvoiEmail);

            chkJoindreFichierLog.Text = "Joindre le fichier de log";
            chkJoindreFichierLog.Location = new Point(0, 30);

            EmailFormStyle.AppliquerCheckBox(chkJoindreFichierLog);

            panelOptions.Controls.Add(chkActiverEnvoiEmail);
            panelOptions.Controls.Add(chkJoindreFichierLog);

            AjouterLigneAuto();
            layoutPrincipal.Controls.Add(panelOptions, 0, ligne);
            layoutPrincipal.SetColumnSpan(panelOptions, 6);

            ligne++;
        }

        private void AjouterAlerte(ref int ligne)
        {
            Panel panelAlerte = new Panel();
            panelAlerte.Height = 48;
            panelAlerte.Dock = DockStyle.Fill;
            panelAlerte.Margin = new Padding(0, 0, 0, 18);

            EmailFormStyle.AppliquerAlerte(panelAlerte);

            IconPictureBox iconeAlerte = new IconPictureBox();
            iconeAlerte.IconChar = IconChar.TriangleExclamation;
            iconeAlerte.IconColor = Color.FromArgb(230, 140, 30);
            iconeAlerte.IconSize = 18;
            iconeAlerte.Size = new Size(22, 22);
            iconeAlerte.Location = new Point(16, 13);
            iconeAlerte.BackColor = Color.FromArgb(255, 245, 225);

            Label lblAlerte = new Label();
            lblAlerte.Text = "Alerte en cas d'échec — disponible dans une version ultérieure";
            lblAlerte.Location = new Point(45, 14);
            lblAlerte.Height = 24;
            lblAlerte.AutoSize = false;
            lblAlerte.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            lblAlerte.ForeColor = Color.FromArgb(20, 20, 20);
            lblAlerte.BackColor = Color.FromArgb(255, 245, 225);
            lblAlerte.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            panelAlerte.Controls.Add(iconeAlerte);
            panelAlerte.Controls.Add(lblAlerte);

            panelAlerte.Resize += (sender, e) =>
            {
                lblAlerte.Width = panelAlerte.Width - lblAlerte.Left - 15;
            };

            AjouterLigneAuto();
            layoutPrincipal.Controls.Add(panelAlerte, 0, ligne);
            layoutPrincipal.SetColumnSpan(panelAlerte, 6);

            ligne++;
        }

        private void AjouterBoutonTest(ref int ligne)
        {
            Panel panelBouton = new Panel();
            panelBouton.Height = 45;
            panelBouton.BackColor = Color.White;
            panelBouton.Margin = new Padding(0, 0, 0, 30);

            btnTesterEmail.Text = "  Envoyer un e-mail de test";
            btnTesterEmail.Location = new Point(0, 0);
            btnTesterEmail.Size = new Size(245, 38);
            btnTesterEmail.TextAlign = ContentAlignment.MiddleCenter;
            btnTesterEmail.Click += BtnTesterEmail_Click;

            EmailFormStyle.AppliquerBoutonTest(btnTesterEmail);

            panelBouton.Controls.Add(btnTesterEmail);

            AjouterLigneAuto();
            layoutPrincipal.Controls.Add(panelBouton, 0, ligne);
            layoutPrincipal.SetColumnSpan(panelBouton, 6);

            ligne++;
        }

        private void AjouterPlaceholders()
        {
            txtServeurSmtp.PlaceholderText = "smtp.saim.com";
            txtPort.PlaceholderText = "587";
            txtIdentifiantSmtp.PlaceholderText = "notifications@saim.com";
            txtMotDePasseSmtp.PlaceholderText = "••••••••";

            txtExpediteurFrom.PlaceholderText = "noreply@saim.com";
            txtDestinataireTo.PlaceholderText = "admin@saim.com";
            txtCopieCc.PlaceholderText = "manager@saim.com";
            txtCopieCacheeBcc.PlaceholderText = "";

            txtObjet.PlaceholderText = "[SAIM] Copie réussie — {date}";
        }

        /// <summary>
        /// Branche les événements qui permettent de savoir
        /// si l'utilisateur a modifié un champ.
        /// </summary>
        private void BrancherDetectionModifications()
        {
            txtServeurSmtp.TextChanged += Champ_Modifie;
            txtPort.TextChanged += Champ_Modifie;
            cmbSecurite.SelectedIndexChanged += Champ_Modifie;

            txtIdentifiantSmtp.TextChanged += Champ_Modifie;
            txtMotDePasseSmtp.TextChanged += Champ_Modifie;

            txtExpediteurFrom.TextChanged += Champ_Modifie;
            txtDestinataireTo.TextChanged += Champ_Modifie;
            txtCopieCc.TextChanged += Champ_Modifie;
            txtCopieCacheeBcc.TextChanged += Champ_Modifie;

            txtObjet.TextChanged += Champ_Modifie;
            txtCorpsMessage.TextChanged += Champ_Modifie;

            chkActiverEnvoiEmail.CheckedChanged += Champ_Modifie;
            chkJoindreFichierLog.CheckedChanged += Champ_Modifie;
        }

        /// <summary>
        /// Appelé quand un champ est modifié par l'utilisateur.
        /// </summary>
        private void Champ_Modifie(object? sender, EventArgs e)
        {
            if (_chargementEnCours)
            {
                return;
            }

            _aDesModificationsNonEnregistrees = true;
        }

        /// <summary>
        /// Appelé par le Controller après un enregistrement réussi.
        /// </summary>
        public void MarquerCommeEnregistre()
        {
            _configurationEnregistree = RecupererConfiguration();
            _aDesModificationsNonEnregistrees = false;
        }

        /// <summary>
        /// Appelé si l'utilisateur choisit "Non".
        /// On remet les valeurs comme elles étaient lors du dernier enregistrement.
        /// </summary>
        public void AnnulerModificationsNonEnregistrees()
        {
            AfficherConfiguration(_configurationEnregistree);
            _aDesModificationsNonEnregistrees = false;
        }

        public void AfficherConfiguration(EmailConfigModel configuration)
        {
            _chargementEnCours = true;

            txtServeurSmtp.Text = configuration.ServeurSmtp;
            txtPort.Text = configuration.Port == 0 ? "" : configuration.Port.ToString();
            cmbSecurite.SelectedItem = configuration.Securite;

            txtIdentifiantSmtp.Text = configuration.IdentifiantSmtp;
            txtMotDePasseSmtp.Text = configuration.MotDePasseSmtp;

            txtExpediteurFrom.Text = configuration.ExpediteurFrom;
            txtDestinataireTo.Text = configuration.DestinataireTo;
            txtCopieCc.Text = configuration.CopieCc;
            txtCopieCacheeBcc.Text = configuration.CopieCacheeBcc;

            txtObjet.Text = configuration.Objet;
            txtCorpsMessage.Text = configuration.CorpsMessage;

            chkActiverEnvoiEmail.Checked = configuration.ActiverEnvoiEmail;
            chkJoindreFichierLog.Checked = configuration.JoindreFichierLog;

            if (cmbSecurite.SelectedIndex == -1)
            {
                cmbSecurite.SelectedItem = "TLS";
            }

            _configurationEnregistree = RecupererConfiguration();
            _aDesModificationsNonEnregistrees = false;
            _chargementEnCours = false;
        }

        public EmailConfigModel RecupererConfiguration()
        {
            int.TryParse(txtPort.Text, out int port);

            return new EmailConfigModel
            {
                ServeurSmtp = txtServeurSmtp.Text.Trim(),
                Port = port,
                Securite = cmbSecurite.Text,

                IdentifiantSmtp = txtIdentifiantSmtp.Text.Trim(),
                MotDePasseSmtp = txtMotDePasseSmtp.Text,

                ExpediteurFrom = txtExpediteurFrom.Text.Trim(),
                DestinataireTo = txtDestinataireTo.Text.Trim(),
                CopieCc = txtCopieCc.Text.Trim(),
                CopieCacheeBcc = txtCopieCacheeBcc.Text.Trim(),

                Objet = txtObjet.Text.Trim(),
                CorpsMessage = txtCorpsMessage.Text,

                ActiverEnvoiEmail = chkActiverEnvoiEmail.Checked,
                JoindreFichierLog = chkJoindreFichierLog.Checked
            };
        }

        public void DemanderEnregistrement()
        {
            EnregistrementDemande?.Invoke(this, EventArgs.Empty);
        }

        private void BtnTesterEmail_Click(object? sender, EventArgs e)
        {
            TestEmailDemande?.Invoke(this, EventArgs.Empty);
        }

        private void BtnAfficherMotDePasse_Click(object? sender, EventArgs e)
        {
            if (txtMotDePasseSmtp.PasswordChar == '●')
            {
                txtMotDePasseSmtp.PasswordChar = '\0';
                btnAfficherMotDePasse.IconChar = IconChar.EyeSlash;
            }
            else
            {
                txtMotDePasseSmtp.PasswordChar = '●';
                btnAfficherMotDePasse.IconChar = IconChar.Eye;
            }
        }
    }
}