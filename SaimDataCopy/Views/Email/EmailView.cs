using FontAwesome.Sharp;
using SaimDataCopy.Models.Email;
using SaimDataCopy.Styles;

namespace SaimDataCopy.Views.Email
{
    /// <summary>
    /// Vue graphique pour la page Paramètres Email.
    /// Cette classe contient seulement l'interface WinForms.
    /// </summary>
    public class EmailView : UserControl, IEmailView
    {
        public event EventHandler? EnregistrementDemande;
        public event EventHandler? TestEmailDemande;

        private readonly Panel panelContenu = new Panel();

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

        public EmailView()
        {
            InitialiserInterface();
        }

        private void InitialiserInterface()
        {
            EmailFormStyle.AppliquerPage(this);

            panelContenu.Dock = DockStyle.Top;
            panelContenu.Height = 1170;
            panelContenu.Padding = new Padding(25, 25, 25, 30);
            panelContenu.BackColor = Color.White;

            Controls.Add(panelContenu);

            AjouterTitre();
            AjouterBarreInfo();
            AjouterSectionSmtp();
            AjouterSectionDestinataires();
            AjouterSectionMessage();
            AjouterOptions();
            AjouterPlaceholders();
        }

        private void AjouterTitre()
        {
            Label lblTitre = new Label();
            lblTitre.Text = "Paramètres de notification e-mail";
            lblTitre.Location = new Point(25, 30);

            EmailFormStyle.AppliquerTitre(lblTitre);

            panelContenu.Controls.Add(lblTitre);
        }

        private void AjouterBarreInfo()
        {
            Panel panelInfo = new Panel();
            panelInfo.Location = new Point(25, 78);
            panelInfo.Size = new Size(975, 48);

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
            lblInfo.Location = new Point(50, 15);

            EmailFormStyle.AppliquerTexteInfo(lblInfo);

            panelInfo.Controls.Add(iconeInfo);
            panelInfo.Controls.Add(lblInfo);

            panelContenu.Controls.Add(panelInfo);
        }

        private void AjouterSectionSmtp()
        {
            AjouterTitreSection("Serveur SMTP", 25, 157);

            AjouterChampTexte("Serveur SMTP", txtServeurSmtp, 25, 200, 310, true);
            AjouterChampTexte("Port", txtPort, 360, 200, 310, false);
            AjouterComboSecurite(695, 200, 305);

            AjouterChampTexte("Identifiant SMTP", txtIdentifiantSmtp, 25, 292, 475, false);
            AjouterChampMotDePasse("Mot de passe SMTP", 525, 292, 475);
        }

        private void AjouterSectionDestinataires()
        {
            AjouterTitreSection("Destinataires", 25, 395);

            AjouterChampTexte("Expéditeur from", txtExpediteurFrom, 25, 438, 475, false);
            AjouterChampTexte("Destinataire To", txtDestinataireTo, 525, 438, 475, true);

            AjouterChampTexte("Copie CC", txtCopieCc, 25, 530, 475, false);
            AjouterChampTexte("Copie cachée BCC", txtCopieCacheeBcc, 525, 530, 475, false);
        }

        private void AjouterSectionMessage()
        {
            AjouterTitreSection("Contenu du message", 25, 625);

            AjouterChampTexte("Objet", txtObjet, 25, 668, 975, false);

            Label lblCorps = CreerLabel("Corps du message");
            lblCorps.Location = new Point(25, 760);
            panelContenu.Controls.Add(lblCorps);

            txtCorpsMessage.Location = new Point(25, 787);
            txtCorpsMessage.Size = new Size(975, 150);

            EmailFormStyle.AppliquerTextBoxMultiligne(txtCorpsMessage);

            panelContenu.Controls.Add(txtCorpsMessage);
        }

        private void AjouterOptions()
        {
            chkActiverEnvoiEmail.Text = "Activer l'envoi des e-mails";
            chkActiverEnvoiEmail.Location = new Point(25, 955);

            EmailFormStyle.AppliquerCheckBox(chkActiverEnvoiEmail);

            chkJoindreFichierLog.Text = "Joindre le fichier de log";
            chkJoindreFichierLog.Location = new Point(25, 985);

            EmailFormStyle.AppliquerCheckBox(chkJoindreFichierLog);

            Panel panelAlerte = new Panel();
            panelAlerte.Location = new Point(25, 1030);
            panelAlerte.Size = new Size(975, 48);

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
            lblAlerte.Location = new Point(45, 15);
            lblAlerte.AutoSize = true;
            lblAlerte.Font = new Font("Segoe UI", 9, FontStyle.Regular);
            lblAlerte.ForeColor = Color.FromArgb(20, 20, 20);
            lblAlerte.BackColor = Color.FromArgb(255, 245, 225);

            panelAlerte.Controls.Add(iconeAlerte);
            panelAlerte.Controls.Add(lblAlerte);

            btnTesterEmail.Text = "  Envoyer un e-mail de test";
            btnTesterEmail.Location = new Point(25, 1095);
            btnTesterEmail.Size = new Size(245, 38);
            btnTesterEmail.TextAlign = ContentAlignment.MiddleCenter;
            btnTesterEmail.Click += BtnTesterEmail_Click;

            EmailFormStyle.AppliquerBoutonTest(btnTesterEmail);

            panelContenu.Controls.Add(chkActiverEnvoiEmail);
            panelContenu.Controls.Add(chkJoindreFichierLog);
            panelContenu.Controls.Add(panelAlerte);
            panelContenu.Controls.Add(btnTesterEmail);
        }

        private void AjouterTitreSection(string texte, int x, int y)
        {
            Label label = new Label();
            label.Text = texte;
            label.Location = new Point(x, y);

            EmailFormStyle.AppliquerTitreSection(label);

            panelContenu.Controls.Add(label);
        }

        private void AjouterChampTexte(
            string libelle,
            TextBox textBox,
            int x,
            int y,
            int largeur,
            bool requis)
        {
            Label label = CreerLabel(libelle);
            label.Location = new Point(x, y);
            panelContenu.Controls.Add(label);

            if (requis)
            {
                Label badge = new Label();
                badge.Location = new Point(x + label.PreferredWidth + 8, y - 2);

                EmailFormStyle.AppliquerBadgeRequis(badge);

                panelContenu.Controls.Add(badge);
            }

            Panel panelBordure = new Panel();
            panelBordure.Location = new Point(x, y + 27);
            panelBordure.Size = new Size(largeur, 40);

            EmailFormStyle.AppliquerPanelBordure(panelBordure);

            textBox.Location = new Point(10, 9);
            textBox.Size = new Size(largeur - 20, 22);

            EmailFormStyle.AppliquerTextBoxDansBordure(textBox);

            panelBordure.Controls.Add(textBox);
            panelContenu.Controls.Add(panelBordure);
        }

        private void AjouterComboSecurite(int x, int y, int largeur)
        {
            Label label = CreerLabel("Sécurité");
            label.Location = new Point(x, y);
            panelContenu.Controls.Add(label);

            Panel panelBordure = new Panel();
            panelBordure.Location = new Point(x, y + 27);
            panelBordure.Size = new Size(largeur, 40);

            EmailFormStyle.AppliquerPanelBordure(panelBordure);

            cmbSecurite.Location = new Point(6, 7);
            cmbSecurite.Size = new Size(largeur - 12, 26);

            EmailFormStyle.AppliquerComboBoxDansBordure(cmbSecurite);

            cmbSecurite.Items.AddRange(new object[]
            {
                "TLS",
                "SSL",
                "Aucune"
            });

            cmbSecurite.SelectedIndex = 0;

            panelBordure.Controls.Add(cmbSecurite);
            panelContenu.Controls.Add(panelBordure);
        }

        private void AjouterChampMotDePasse(string libelle, int x, int y, int largeur)
        {
            Label label = CreerLabel(libelle);
            label.Location = new Point(x, y);
            panelContenu.Controls.Add(label);

            Panel panelBordure = new Panel();
            panelBordure.Location = new Point(x, y + 27);
            panelBordure.Size = new Size(largeur, 40);

            EmailFormStyle.AppliquerPanelBordure(panelBordure);

            txtMotDePasseSmtp.Location = new Point(10, 9);
            txtMotDePasseSmtp.Size = new Size(largeur - 55, 22);
            txtMotDePasseSmtp.PasswordChar = '●';

            EmailFormStyle.AppliquerTextBoxDansBordure(txtMotDePasseSmtp);

            btnAfficherMotDePasse.IconChar = IconChar.Eye;
            btnAfficherMotDePasse.IconColor = Color.FromArgb(90, 90, 90);
            btnAfficherMotDePasse.IconSize = 16;
            btnAfficherMotDePasse.Size = new Size(35, 30);
            btnAfficherMotDePasse.Location = new Point(largeur - 42, 4);
            btnAfficherMotDePasse.FlatStyle = FlatStyle.Flat;
            btnAfficherMotDePasse.FlatAppearance.BorderSize = 0;
            btnAfficherMotDePasse.BackColor = Color.White;
            btnAfficherMotDePasse.Cursor = Cursors.Hand;
            btnAfficherMotDePasse.Click += BtnAfficherMotDePasse_Click;

            panelBordure.Controls.Add(txtMotDePasseSmtp);
            panelBordure.Controls.Add(btnAfficherMotDePasse);
            panelContenu.Controls.Add(panelBordure);
        }

        private Label CreerLabel(string texte)
        {
            Label label = new Label();
            label.Text = texte;

            EmailFormStyle.AppliquerLabel(label);

            return label;
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

        public void AfficherConfiguration(EmailConfigModel configuration)
        {
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