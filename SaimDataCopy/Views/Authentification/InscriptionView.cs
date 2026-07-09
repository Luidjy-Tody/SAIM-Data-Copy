using SaimDataCopy.Styles.Authentification.Inscription;
using SaimDataCopy.Views.Authentification.Components;

namespace SaimDataCopy.Views.Authentification
{
    public class InscriptionView : UserControl
    {
        private readonly TextBox txtNomComplet;
        private readonly TextBox txtIdentifiant;
        private readonly TextBox txtEmail;
        private readonly AuthPasswordTextBox txtMotDePasse;
        private readonly AuthPasswordTextBox txtConfirmation;
        private readonly AuthMessageControl messageControl;
        private readonly ComboBox cboStatut;

        public event EventHandler? InscriptionDemandee;
        public event EventHandler? RetourConnexionDemande;

        public string NomComplet => txtNomComplet.Text.Trim();
        public string Identifiant => txtIdentifiant.Text.Trim();
        public string Email => txtEmail.Text.Trim();
        public string MotDePasse => txtMotDePasse.Texte;
        public string ConfirmationMotDePasse => txtConfirmation.Texte;
        public string Statut => cboStatut.SelectedItem?.ToString() ?? "User";

        public InscriptionView()
        {
            BackColor = Color.White;
            DoubleBuffered = true;

            Label lblTitre = new Label
            {
                Text = "Créer un compte",
                Font = InscriptionStyle.TitreCarte(),
                ForeColor = InscriptionStyle.TextePrincipal,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(70, 18),
                Size = new Size(400, 45)
            };

            Label lblNom = CreerLabel("Nom complet", 70, 64);
            txtNomComplet = CreerTextBox(70, 88, "Entrez votre nom complet");

            Label lblIdentifiant = CreerLabel("Nom d'utilisateur", 70, 122);
            txtIdentifiant = CreerTextBox(70, 146, "Entrez votre nom d'utilisateur");

            Label lblEmail = CreerLabel("Email", 70, 180);
            txtEmail = CreerTextBox(70, 204, "Entrez votre adresse email");

            Label lblMotDePasse = CreerLabel("Mot de passe", 70, 238);
            txtMotDePasse = new AuthPasswordTextBox
            {
                Location = new Point(70, 262),
                Size = new Size(430, 42)
            };

            Label lblConfirmation = CreerLabel("Confirmer le mot de passe", 70, 312);
            txtConfirmation = new AuthPasswordTextBox
            {
                Location = new Point(70, 336),
                Size = new Size(430, 42)
            };

            Label lblStatut = CreerLabel("Statut", 70, 386);

            cboStatut = new ComboBox
            {
                Location = new Point(70, 410),
                Size = new Size(400, 42),
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            cboStatut.Items.Add("User");
            cboStatut.Items.Add("Admin");
            cboStatut.SelectedItem = "User";

            messageControl = new AuthMessageControl
            {
                Location = new Point(70, 462),
                Size = new Size(400, 25)
            };

            Button btnInscription = new Button
            {
                Text = "S'inscrire",
                Location = new Point(70, 500),
                Size = new Size(400, 48)
            };
            InscriptionStyle.AppliquerBouton(btnInscription);
            btnInscription.Click += (s, e) => InscriptionDemandee?.Invoke(this, EventArgs.Empty);

            LinkLabel lienConnexion = new LinkLabel
            {
                Text = "Déjà un compte ? Se connecter",
                Location = new Point(70, 565),
                Size = new Size(400, 25)
            };
            InscriptionStyle.AppliquerLien(lienConnexion);
            lienConnexion.LinkClicked += (s, e) => RetourConnexionDemande?.Invoke(this, EventArgs.Empty);

            Controls.Add(lblTitre);
            Controls.Add(lblNom);
            Controls.Add(txtNomComplet);
            Controls.Add(lblIdentifiant);
            Controls.Add(txtIdentifiant);
            Controls.Add(lblEmail);
            Controls.Add(txtEmail);
            Controls.Add(lblMotDePasse);
            Controls.Add(txtMotDePasse);
            Controls.Add(lblConfirmation);
            Controls.Add(txtConfirmation);
            Controls.Add(lblStatut);
            Controls.Add(cboStatut);
            Controls.Add(messageControl);
            Controls.Add(btnInscription);
            Controls.Add(lienConnexion);
            ConfigurerNavigationClavier();
        }

        public void DefinirStatut(string statut)
        {
            if (statut == "Admin")
            {
                cboStatut.SelectedItem = "Admin";
                return;
            }

            cboStatut.SelectedItem = "User";
        }

        public void BloquerChoixStatut()
        {
            cboStatut.Enabled = false;
        }

        public void AutoriserChoixStatut()
        {
            cboStatut.Enabled = true;
        }
        private static Label CreerLabel(string texte, int x, int y)
        {
            Label label = new Label
            {
                Text = texte,
                Location = new Point(x, y),
                Size = new Size(400, 24)
            };

            InscriptionStyle.AppliquerLabelChamp(label);

            return label;
        }

        private static TextBox CreerTextBox(int x, int y, string placeholder)
        {
            TextBox textBox = new TextBox
            {
                Location = new Point(x, y),
                Size = new Size(400, 42),
                PlaceholderText = placeholder
            };

            InscriptionStyle.AppliquerTextBox(textBox);

            return textBox;
        }

        public void AfficherErreur(string message)
        {
            messageControl.AfficherErreur(message);
        }

        public void AfficherSucces(string message)
        {
            messageControl.AfficherSucces(message);
        }

        public void ViderMessage()
        {
            messageControl.Effacer();
        }

        private void ConfigurerNavigationClavier()
        {
            txtNomComplet.KeyDown += TxtNomComplet_KeyDown;
            txtIdentifiant.KeyDown += TxtIdentifiant_KeyDown;
            txtEmail.KeyDown += TxtEmail_KeyDown;
            txtMotDePasse.TextBox.KeyDown += TxtMotDePasse_KeyDown;
            txtConfirmation.TextBox.KeyDown += TxtConfirmation_KeyDown;
            cboStatut.KeyDown += CboStatut_KeyDown;
        }

        private void TxtNomComplet_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
            {
                return;
            }

            e.SuppressKeyPress = true;

            if (string.IsNullOrWhiteSpace(txtNomComplet.Text))
            {
                AfficherErreur("Veuillez remplir le nom complet.");
                txtNomComplet.Focus();
                return;
            }

            ViderMessage();
            txtIdentifiant.Focus();
        }

        private void TxtIdentifiant_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
            {
                return;
            }

            e.SuppressKeyPress = true;

            if (string.IsNullOrWhiteSpace(txtIdentifiant.Text))
            {
                AfficherErreur("Veuillez remplir le nom d'utilisateur.");
                txtIdentifiant.Focus();
                return;
            }

            ViderMessage();
            txtEmail.Focus();
        }

        private void TxtEmail_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
            {
                return;
            }

            e.SuppressKeyPress = true;

            if (string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                AfficherErreur("Veuillez remplir l'email.");
                txtEmail.Focus();
                return;
            }

            ViderMessage();
            txtMotDePasse.TextBox.Focus();
        }

        private void TxtMotDePasse_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
            {
                return;
            }

            e.SuppressKeyPress = true;

            if (string.IsNullOrWhiteSpace(txtMotDePasse.Texte))
            {
                AfficherErreur("Veuillez remplir le mot de passe.");
                txtMotDePasse.TextBox.Focus();
                return;
            }

            ViderMessage();
            txtConfirmation.TextBox.Focus();
        }

        private void TxtConfirmation_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
            {
                return;
            }

            e.SuppressKeyPress = true;

            if (string.IsNullOrWhiteSpace(txtConfirmation.Texte))
            {
                AfficherErreur("Veuillez confirmer le mot de passe.");
                txtConfirmation.TextBox.Focus();
                return;
            }

            ViderMessage();
            cboStatut.Focus();
        }

        private void CboStatut_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
            {
                return;
            }

            e.SuppressKeyPress = true;

            DemanderInscriptionDepuisClavier();
        }

        private void DemanderInscriptionDepuisClavier()
        {
            if (string.IsNullOrWhiteSpace(txtNomComplet.Text))
            {
                AfficherErreur("Veuillez remplir le nom complet.");
                txtNomComplet.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtIdentifiant.Text))
            {
                AfficherErreur("Veuillez remplir le nom d'utilisateur.");
                txtIdentifiant.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                AfficherErreur("Veuillez remplir l'email.");
                txtEmail.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtMotDePasse.Texte))
            {
                AfficherErreur("Veuillez remplir le mot de passe.");
                txtMotDePasse.TextBox.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtConfirmation.Texte))
            {
                AfficherErreur("Veuillez confirmer le mot de passe.");
                txtConfirmation.TextBox.Focus();
                return;
            }

            if (txtMotDePasse.Texte != txtConfirmation.Texte)
            {
                AfficherErreur("Les mots de passe ne correspondent pas.");
                txtConfirmation.TextBox.Focus();
                return;
            }

            ViderMessage();
            InscriptionDemandee?.Invoke(this, EventArgs.Empty);
        }
    }
}