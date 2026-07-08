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

        public event EventHandler? InscriptionDemandee;
        public event EventHandler? RetourConnexionDemande;

        public string NomComplet => txtNomComplet.Text.Trim();
        public string Identifiant => txtIdentifiant.Text.Trim();
        public string Email => txtEmail.Text.Trim();
        public string MotDePasse => txtMotDePasse.Texte;
        public string ConfirmationMotDePasse => txtConfirmation.Texte;

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

            messageControl = new AuthMessageControl
            {
                Location = new Point(70, 386),
                Size = new Size(400, 25)
            };

            Button btnInscription = new Button
            {
                Text = "S'inscrire",
                Location = new Point(70, 421),
                Size = new Size(400, 48)
            };
            InscriptionStyle.AppliquerBouton(btnInscription);
            btnInscription.Click += (s, e) => InscriptionDemandee?.Invoke(this, EventArgs.Empty);

            LinkLabel lienConnexion = new LinkLabel
            {
                Text = "Déjà un compte ? Se connecter",
                Location = new Point(70, 485),
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
            Controls.Add(messageControl);
            Controls.Add(btnInscription);
            Controls.Add(lienConnexion);
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