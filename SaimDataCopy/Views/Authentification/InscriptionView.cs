using SaimDataCopy.Styles.Authentification;
using SaimDataCopy.Views.Authentification.Components;

namespace SaimDataCopy.Views.Authentification
{
    public class InscriptionView : Form
    {
        private readonly TextBox txtNomComplet;
        private readonly TextBox txtEmail;
        private readonly AuthPasswordTextBox txtMotDePasse;
        private readonly AuthPasswordTextBox txtConfirmation;
        private readonly AuthMessageControl messageControl;

        public event EventHandler? InscriptionDemandee;
        public event EventHandler? RetourConnexionDemande;

        public string NomComplet => txtNomComplet.Text.Trim();
        public string Email => txtEmail.Text.Trim();
        public string Identifiant => txtEmail.Text.Trim();
        public string MotDePasse => txtMotDePasse.Texte;
        public string ConfirmationMotDePasse => txtConfirmation.Texte;

        public InscriptionView()
        {
            Text = "SaimDataCopy - Inscription";
            Size = new Size(560, 760);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            AuthGradientPanel fond = new AuthGradientPanel { Dock = DockStyle.Fill };

            AuthHeaderControl header = new AuthHeaderControl
            {
                Location = new Point(20, 30)
            };

            AuthShadowPanel carte = new AuthShadowPanel
            {
                Size = new Size(440, 500),
                Location = new Point(60, 125)
            };

            Label lblTitre = new Label
            {
                Text = "Créer un compte",
                Font = AuthentificationFormStyle.TitreCarte(),
                ForeColor = AuthentificationFormStyle.TextePrincipal,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(40, 25),
                Size = new Size(360, 45)
            };

            Label lblNom = CreerLabel("Nom complet", 40, 85);
            txtNomComplet = CreerTextBox(40, 113);

            Label lblEmail = CreerLabel("Identifiant / Email", 40, 165);
            txtEmail = CreerTextBox(40, 193);

            Label lblMotDePasse = CreerLabel("Mot de passe", 40, 245);
            txtMotDePasse = new AuthPasswordTextBox
            {
                Location = new Point(40, 273),
                Size = new Size(360, 38)
            };

            Label lblConfirmation = CreerLabel("Confirmer le mot de passe", 40, 325);
            txtConfirmation = new AuthPasswordTextBox
            {
                Location = new Point(40, 353),
                Size = new Size(360, 38)
            };

            messageControl = new AuthMessageControl
            {
                Location = new Point(40, 398)
            };

            Button btnInscription = new Button
            {
                Text = "S'inscrire",
                Location = new Point(40, 430),
                Size = new Size(360, 48)
            };
            AuthButtonStyle.Appliquer(btnInscription);
            btnInscription.Click += (s, e) => InscriptionDemandee?.Invoke(this, EventArgs.Empty);

            LinkLabel lienConnexion = new LinkLabel
            {
                Text = "Déjà un compte ? Se connecter",
                Location = new Point(40, 480),
                Size = new Size(360, 25)
            };
            AuthLabelStyle.AppliquerLien(lienConnexion);
            lienConnexion.LinkClicked += (s, e) => RetourConnexionDemande?.Invoke(this, EventArgs.Empty);

            AuthFooterControl footer = new AuthFooterControl
            {
                Location = new Point(20, 665)
            };

            carte.Controls.Add(lblTitre);
            carte.Controls.Add(lblNom);
            carte.Controls.Add(txtNomComplet);
            carte.Controls.Add(lblEmail);
            carte.Controls.Add(txtEmail);
            carte.Controls.Add(lblMotDePasse);
            carte.Controls.Add(txtMotDePasse);
            carte.Controls.Add(lblConfirmation);
            carte.Controls.Add(txtConfirmation);
            carte.Controls.Add(messageControl);
            carte.Controls.Add(btnInscription);
            carte.Controls.Add(lienConnexion);

            fond.Controls.Add(header);
            fond.Controls.Add(carte);
            fond.Controls.Add(footer);

            Controls.Add(fond);
        }

        private static Label CreerLabel(string texte, int x, int y)
        {
            Label label = new Label
            {
                Text = texte,
                Location = new Point(x, y),
                Size = new Size(360, 24)
            };

            AuthLabelStyle.AppliquerLabelChamp(label);

            return label;
        }

        private static TextBox CreerTextBox(int x, int y)
        {
            TextBox textBox = new TextBox
            {
                Location = new Point(x, y),
                Size = new Size(360, 36)
            };

            AuthTextBoxStyle.Appliquer(textBox);

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
    }
}