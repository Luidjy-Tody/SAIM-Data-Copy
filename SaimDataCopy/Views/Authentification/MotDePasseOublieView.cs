using SaimDataCopy.Styles.Authentification.MotDePasseOublie;
using SaimDataCopy.Views.Authentification.Components;

namespace SaimDataCopy.Views.Authentification
{
    public class MotDePasseOublieView : UserControl
    {
        private readonly TextBox txtEmail;
        private readonly AuthMessageControl messageControl;

        public event EventHandler? EnvoiLienDemande;
        public event EventHandler? RetourConnexionDemande;

        public string Email => txtEmail.Text.Trim();

        public MotDePasseOublieView()
        {
            BackColor = Color.White;
            DoubleBuffered = true;

            Label lblTitre = new Label
            {
                Text = "Mot de passe oublié",
                Font = MotDePasseOublieStyle.TitreCarte(),
                ForeColor = MotDePasseOublieStyle.TextePrincipal,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(70, 80),
                Size = new Size(400, 45)
            };

            Label lblDescription = new Label
            {
                Text = "Saisissez votre adresse e-mail pour recevoir un lien de réinitialisation.",
                Font = MotDePasseOublieStyle.TextePetit(),
                ForeColor = MotDePasseOublieStyle.TexteAide,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(70, 135),
                Size = new Size(400, 50)
            };

            Label lblEmail = CreerLabel("Email ou identifiant", 70, 195);

            txtEmail = new TextBox
            {
                Location = new Point(70, 230),
                Size = new Size(400, 42),
                PlaceholderText = "Entrez votre email ou identifiant"
            };
            MotDePasseOublieStyle.AppliquerTextBox(txtEmail);

            messageControl = new AuthMessageControl
            {
                Location = new Point(70, 290),
                Size = new Size(400, 25)
            };

            Button btnEnvoyer = new Button
            {
                Text = "Envoyer le lien de réinitialisation",
                Location = new Point(70, 325),
                Size = new Size(400, 50)
            };
            MotDePasseOublieStyle.AppliquerBouton(btnEnvoyer);
            btnEnvoyer.Click += (s, e) => EnvoiLienDemande?.Invoke(this, EventArgs.Empty);

            LinkLabel lienRetour = new LinkLabel
            {
                Text = "← Retour à la connexion",
                Location = new Point(70, 395),
                Size = new Size(400, 25)
            };
            MotDePasseOublieStyle.AppliquerLien(lienRetour);
            lienRetour.LinkClicked += (s, e) => RetourConnexionDemande?.Invoke(this, EventArgs.Empty);

            Controls.Add(lblTitre);
            Controls.Add(lblDescription);
            Controls.Add(lblEmail);
            Controls.Add(txtEmail);
            Controls.Add(messageControl);
            Controls.Add(btnEnvoyer);
            Controls.Add(lienRetour);
        }

        private static Label CreerLabel(string texte, int x, int y)
        {
            Label label = new Label
            {
                Text = texte,
                Location = new Point(x, y),
                Size = new Size(400, 24)
            };

            MotDePasseOublieStyle.AppliquerLabelChamp(label);

            return label;
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