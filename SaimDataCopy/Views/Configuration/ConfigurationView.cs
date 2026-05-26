using FontAwesome.Sharp;
using SaimDataCopy.Helpers;
using SaimDataCopy.Models.Configuration;
using System.Drawing;
using System.Windows.Forms;

namespace SaimDataCopy.Views.Configuration
{
    // View de la page Configuration.
    // Elle affiche l'interface et récupère les valeurs saisies.
    // Elle ne contient pas la logique métier.
    public class ConfigurationView : UserControl
    {
        // Champs du serveur source.
        private TextBox txtSourceNomServeur = new TextBox();
        private TextBox txtSourceChaineConnexion = new TextBox();
        private TextBox txtSourceIdentifiant = new TextBox();
        private TextBox txtSourceMotDePasse = new TextBox();
        private TextBox txtSourcePort = new TextBox();

        // Champs du serveur cible.
        private TextBox txtCibleNomServeur = new TextBox();
        private TextBox txtCibleChaineConnexion = new TextBox();
        private TextBox txtCibleIdentifiant = new TextBox();
        private TextBox txtCibleMotDePasse = new TextBox();
        private TextBox txtCiblePort = new TextBox();

        // Listes déroulantes.
        private ComboBox cmbModeCopie = new ComboBox();
        private ComboBox cmbErreur = new ComboBox();
        private ComboBox cmbTentatives = new ComboBox();

        // Événement envoyé au Controller quand l'utilisateur veut enregistrer.
        public event EventHandler? EnregistrerConfigurationDemande;

        public ConfigurationView()
        {
            BackColor = Color.White;
            AutoScroll = true;

            CreerInterface();
        }

        private void CreerInterface()
        {
            Panel panelContenu = new Panel();

            panelContenu.Dock = DockStyle.Top;
            panelContenu.Height = 850;
            panelContenu.BackColor = Color.White;

            Controls.Add(panelContenu);

            Label lblTitre = new Label();
            lblTitre.Text = "Paramètres de configuration";
            lblTitre.Location = new Point(25, 25);

            PageFormStyle.AppliquerTitre(lblTitre);
            panelContenu.Controls.Add(lblTitre);

            CreerSectionServeurSource(
                panelContenu,
                "Serveur source (production)",
                25,
                85
            );

            CreerSectionServeurCible(
                panelContenu,
                "Serveur cible (staging)",
                25,
                365
            );

            CreerSectionErreur(panelContenu, 650);
        }

        private void CreerSectionServeurSource(Panel parent, string titre, int x, int y)
        {
            Label lblSection = new Label();
            lblSection.Text = titre;
            lblSection.Location = new Point(x, y);

            PageFormStyle.AppliquerSousTitre(lblSection);
            parent.Controls.Add(lblSection);

            AjouterLabelChamp(parent, "Nom du serveur", x, y + 45, true);
            txtSourceNomServeur = AjouterTextBox(parent, "PROD-SRV-01", x, y + 70, 475);

            AjouterLabelChamp(parent, "Chaîne de connexion", x + 500, y + 45, false);
            txtSourceChaineConnexion = AjouterTextBox(parent, "Server=...", x + 500, y + 70, 475);

            AjouterLabelChamp(parent, "Identifiant", x, y + 130, false);
            txtSourceIdentifiant = AjouterTextBox(parent, "sa", x, y + 155, 475);

            AjouterLabelChamp(parent, "Mot de passe", x + 500, y + 130, false);
            txtSourceMotDePasse = AjouterPasswordBox(parent, "12345678", x + 500, y + 155, 475);

            AjouterLabelChamp(parent, "Port", x, y + 215, false);
            txtSourcePort = AjouterTextBox(parent, "1433", x, y + 240, 475);
        }

        private void CreerSectionServeurCible(Panel parent, string titre, int x, int y)
        {
            Label lblSection = new Label();
            lblSection.Text = titre;
            lblSection.Location = new Point(x, y);

            PageFormStyle.AppliquerSousTitre(lblSection);
            parent.Controls.Add(lblSection);

            AjouterLabelChamp(parent, "Nom du serveur", x, y + 45, true);
            txtCibleNomServeur = AjouterTextBox(parent, "STAGING-SRV-01", x, y + 70, 475);

            AjouterLabelChamp(parent, "Chaîne de connexion", x + 500, y + 45, false);
            txtCibleChaineConnexion = AjouterTextBox(parent, "Server=...", x + 500, y + 70, 475);

            AjouterLabelChamp(parent, "Identifiant", x, y + 130, false);
            txtCibleIdentifiant = AjouterTextBox(parent, "sa", x, y + 155, 475);

            AjouterLabelChamp(parent, "Mot de passe", x + 500, y + 130, false);
            txtCibleMotDePasse = AjouterPasswordBox(parent, "12345678", x + 500, y + 155, 475);

            AjouterLabelChamp(parent, "Port", x, y + 215, false);
            txtCiblePort = AjouterTextBox(parent, "1433", x, y + 240, 475);

            AjouterLabelChamp(parent, "Mode de copie", x + 500, y + 215, false);

            cmbModeCopie = new ComboBox();
            cmbModeCopie.Location = new Point(x + 500, y + 240);
            cmbModeCopie.Width = 475;

            cmbModeCopie.Items.Add("Écraser");
            cmbModeCopie.Items.Add("Mettre à jour");
            cmbModeCopie.SelectedIndex = 0;

            PageFormStyle.AppliquerComboBox(cmbModeCopie);
            parent.Controls.Add(cmbModeCopie);
        }

        private void CreerSectionErreur(Panel parent, int y)
        {
            Label lblSection = new Label();
            lblSection.Text = "Comportement en cas d'erreur";
            lblSection.Location = new Point(25, y);

            PageFormStyle.AppliquerSousTitre(lblSection);
            parent.Controls.Add(lblSection);

            AjouterLabelChamp(parent, "Si une base échoue", 25, y + 45, false);

            cmbErreur = new ComboBox();
            cmbErreur.Location = new Point(25, y + 70);
            cmbErreur.Width = 475;

            cmbErreur.Items.Add("Continuer avec les autres");
            cmbErreur.Items.Add("Arrêter tous les traitements");
            cmbErreur.SelectedIndex = 0;

            PageFormStyle.AppliquerComboBox(cmbErreur);
            parent.Controls.Add(cmbErreur);

            AjouterLabelChamp(parent, "Tentatives de reprise", 525, y + 45, false);

            cmbTentatives = new ComboBox();
            cmbTentatives.Location = new Point(525, y + 70);
            cmbTentatives.Width = 475;

            cmbTentatives.Items.Add("1 tentative");
            cmbTentatives.Items.Add("2 tentatives");
            cmbTentatives.Items.Add("3 tentatives");
            cmbTentatives.SelectedIndex = 0;

            PageFormStyle.AppliquerComboBox(cmbTentatives);
            parent.Controls.Add(cmbTentatives);

            cmbErreur.SelectedIndexChanged += (sender, e) =>
            {
                bool arreterTraitements = cmbErreur.SelectedItem?.ToString() == "Arrêter tous les traitements";

                cmbTentatives.Enabled = !arreterTraitements;
                cmbTentatives.Cursor = arreterTraitements ? Cursors.No : Cursors.Default;
            };
        }

        private void AjouterLabelChamp(Panel parent, string texte, int x, int y, bool requis)
        {
            Label label = new Label();

            label.Text = texte;
            label.Location = new Point(x, y);

            PageFormStyle.AppliquerLabelChamp(label);
            parent.Controls.Add(label);

            if (requis)
            {
                Label badge = PageFormStyle.CreerBadgeRequis();

                badge.Location = new Point(x + label.Width + 10, y - 2);

                parent.Controls.Add(badge);
            }
        }

        private TextBox AjouterTextBox(Panel parent, string textePlaceholder, int x, int y, int largeur)
        {
            TextBox textBox = new TextBox();

            textBox.Location = new Point(x, y);
            textBox.Width = largeur;

            // Le texte est seulement un exemple pour guider l'utilisateur.
            textBox.PlaceholderText = textePlaceholder;
            textBox.Text = "";

            PageFormStyle.AppliquerTextBox(textBox);

            parent.Controls.Add(textBox);

            return textBox;
        }

        private TextBox AjouterPasswordBox(Panel parent, string textePlaceholder, int x, int y, int largeur)
        {
            Panel panelPassword = new Panel();

            panelPassword.Location = new Point(x, y);
            panelPassword.Width = largeur;
            panelPassword.Height = 38;
            panelPassword.BackColor = Color.White;
            panelPassword.BorderStyle = BorderStyle.FixedSingle;

            parent.Controls.Add(panelPassword);

            TextBox txtPassword = new TextBox();

            txtPassword.PlaceholderText = textePlaceholder;
            txtPassword.Text = "";
            txtPassword.UseSystemPasswordChar = false;
            txtPassword.BorderStyle = BorderStyle.None;
            txtPassword.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
            txtPassword.Location = new Point(10, 9);
            txtPassword.Width = largeur - 55;

            panelPassword.Controls.Add(txtPassword);

            IconButton btnVoirPassword = new IconButton();

            btnVoirPassword.IconChar = IconChar.Eye;
            btnVoirPassword.IconSize = 18;
            btnVoirPassword.IconColor = Color.FromArgb(80, 80, 80);
            btnVoirPassword.FlatStyle = FlatStyle.Flat;
            btnVoirPassword.FlatAppearance.BorderSize = 0;
            btnVoirPassword.BackColor = Color.White;
            btnVoirPassword.Width = 40;
            btnVoirPassword.Height = 34;
            btnVoirPassword.Location = new Point(largeur - 43, 1);

            bool motDePasseVisible = false;

            txtPassword.TextChanged += (sender, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtPassword.Text))
                {
                    txtPassword.UseSystemPasswordChar = false;
                }
                else
                {
                    txtPassword.UseSystemPasswordChar = !motDePasseVisible;
                }
            };

            btnVoirPassword.Click += (sender, e) =>
            {
                motDePasseVisible = !motDePasseVisible;

                if (!string.IsNullOrWhiteSpace(txtPassword.Text))
                {
                    txtPassword.UseSystemPasswordChar = !motDePasseVisible;
                }

                btnVoirPassword.IconChar = motDePasseVisible ? IconChar.EyeSlash : IconChar.Eye;
            };

            panelPassword.Controls.Add(btnVoirPassword);

            return txtPassword;
        }

        // Cette méthode transforme les champs de l'interface en Model.
        // La View récupère seulement les valeurs, puis le Controller les donne au Service.
        public ConfigurationModel RecupererConfiguration()
        {
            ConfigurationModel configuration = new ConfigurationModel();

            configuration.ServeurSource = new ServeurConfigModel
            {
                NomServeur = txtSourceNomServeur.Text.Trim(),
                ChaineConnexion = txtSourceChaineConnexion.Text.Trim(),
                Identifiant = txtSourceIdentifiant.Text.Trim(),
                MotDePasse = txtSourceMotDePasse.Text.Trim(),
                Port = ConvertirPort(txtSourcePort.Text)
            };

            configuration.ServeurCible = new ServeurConfigModel
            {
                NomServeur = txtCibleNomServeur.Text.Trim(),
                ChaineConnexion = txtCibleChaineConnexion.Text.Trim(),
                Identifiant = txtCibleIdentifiant.Text.Trim(),
                MotDePasse = txtCibleMotDePasse.Text.Trim(),
                Port = ConvertirPort(txtCiblePort.Text)
            };

            configuration.ModeCopie = cmbModeCopie.SelectedItem?.ToString() ?? string.Empty;
            configuration.ComportementErreur = cmbErreur.SelectedItem?.ToString() ?? string.Empty;
            configuration.TentativesReprise = ConvertirTentatives(cmbTentatives.SelectedItem?.ToString());

            return configuration;
        }

        // MainForm appelle cette méthode quand l'utilisateur clique
        // sur le bouton "Enregistrer les paramètres".
        public void DemanderEnregistrement()
        {
            EnregistrerConfigurationDemande?.Invoke(this, EventArgs.Empty);
        }

        public void AfficherMessageSucces(string message)
        {
            MessageBox.Show(
                message,
                "Succès",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }

        public void AfficherMessageErreur(string message)
        {
            MessageBox.Show(
                message,
                "Erreur",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            );
        }

        private int ConvertirPort(string valeur)
        {
            if (int.TryParse(valeur, out int port))
            {
                return port;
            }

            return 0;
        }

        private int ConvertirTentatives(string? valeur)
        {
            return valeur switch
            {
                "1 tentative" => 1,
                "2 tentatives" => 2,
                "3 tentatives" => 3,
                _ => 0
            };
        }
    }
}