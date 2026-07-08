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
        private readonly AuthShadowPanel carte;

        public event EventHandler? InscriptionDemandee;
        public event EventHandler? RetourConnexionDemande;

        public string NomComplet => txtNomComplet.Text.Trim();
        public string Identifiant => txtIdentifiant.Text.Trim();
        public string Email => txtEmail.Text.Trim();
        public string MotDePasse => txtMotDePasse.Texte;
        public string ConfirmationMotDePasse => txtConfirmation.Texte;

        public InscriptionView()
        {
            Dock = DockStyle.Fill;
            BackColor = Color.Transparent;
            DoubleBuffered = true;

            carte = new AuthShadowPanel
            {
                Size = new Size(540, 640)
            };

            Label lblTitre = new Label
            {
                Text = "Créer un compte",
                Font = InscriptionStyle.TitreCarte(),
                ForeColor = InscriptionStyle.TextePrincipal,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(50, 25),
                Size = new Size(400, 45)
            };

            Label lblNom = CreerLabel("Nom complet", 50, 75);
            txtNomComplet = CreerTextBox(50, 105, "Entrez votre nom complet");

            Label lblIdentifiant = CreerLabel("Nom d'utilisateur", 50, 150);
            txtIdentifiant = CreerTextBox(50, 180, "Entrez votre nom d'utilisateur");

            Label lblEmail = CreerLabel("Email", 50, 225);
            txtEmail = CreerTextBox(50, 255, "Entrez votre adresse email");

            Label lblMotDePasse = CreerLabel("Mot de passe", 50, 300);
            txtMotDePasse = new AuthPasswordTextBox
            {
                Location = new Point(50, 330),
                Size = new Size(430, 42)
            };

            Label lblConfirmation = CreerLabel("Confirmer le mot de passe", 50, 375);
            txtConfirmation = new AuthPasswordTextBox
            {
                Location = new Point(50, 405),
                Size = new Size(430, 42)
            };

            messageControl = new AuthMessageControl
            {
                Location = new Point(50, 455),
                Size = new Size(400, 25)
            };

            Button btnInscription = new Button
            {
                Text = "S'inscrire",
                Location = new Point(50, 490),
                Size = new Size(400, 50)
            };
            InscriptionStyle.AppliquerBouton(btnInscription);
            btnInscription.Click += (s, e) => InscriptionDemandee?.Invoke(this, EventArgs.Empty);

            LinkLabel lienConnexion = new LinkLabel
            {
                Text = "Déjà un compte ? Se connecter",
                Location = new Point(50, 555),
                Size = new Size(400, 25)
            };
            InscriptionStyle.AppliquerLien(lienConnexion);
            lienConnexion.LinkClicked += (s, e) => RetourConnexionDemande?.Invoke(this, EventArgs.Empty);

            carte.Controls.Add(lblTitre);
            carte.Controls.Add(lblNom);
            carte.Controls.Add(txtNomComplet);
            carte.Controls.Add(lblIdentifiant);
            carte.Controls.Add(txtIdentifiant);
            carte.Controls.Add(lblEmail);
            carte.Controls.Add(txtEmail);
            carte.Controls.Add(lblMotDePasse);
            carte.Controls.Add(txtMotDePasse);
            carte.Controls.Add(lblConfirmation);
            carte.Controls.Add(txtConfirmation);
            carte.Controls.Add(messageControl);
            carte.Controls.Add(btnInscription);
            carte.Controls.Add(lienConnexion);

            Controls.Add(carte);

            Resize += (s, e) => CentrerCarte();
            VisibleChanged += (s, e) => CentrerCarte();
        }

        private void CentrerCarte()
        {
            if (ClientSize.Width <= 0 || ClientSize.Height <= 0)
            {
                return;
            }

            carte.Location = new Point( (ClientSize.Width - carte.Width) / 2, Math.Max(0, (ClientSize.Height - carte.Height) / 2));
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
    }
}