using SaimDataCopy.Styles.Authentification.MotDePasseOublie;
using SaimDataCopy.Views.Authentification.Components;

namespace SaimDataCopy.Views.Authentification
{
    public class MotDePasseOublieView : UserControl
    {
        private readonly TextBox txtEmail;
        private readonly AuthMessageControl messageControl;
        private readonly AuthShadowPanel carte;

        public event EventHandler? EnvoiLienDemande;
        public event EventHandler? RetourConnexionDemande;

        public string Email => txtEmail.Text.Trim();

        public MotDePasseOublieView()
        {
            Dock = DockStyle.Fill;
            BackColor = Color.Transparent;
            DoubleBuffered = true;

            carte = new AuthShadowPanel
            {
                Size = new Size(500, 415)
            };

            Label lblTitre = new Label
            {
                Text = "Mot de passe oublié",
                Font = MotDePasseOublieStyle.TitreCarte(),
                ForeColor = MotDePasseOublieStyle.TextePrincipal,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(50, 35),
                Size = new Size(400, 45)
            };

            Label lblDescription = new Label
            {
                Text = "Saisissez votre adresse e-mail pour recevoir un lien de réinitialisation.",
                Font = MotDePasseOublieStyle.TextePetit(),
                ForeColor = MotDePasseOublieStyle.TexteAide,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(50, 85),
                Size = new Size(400, 50)
            };

            Label lblEmail = CreerLabel("Email", 50, 150);

            txtEmail = new TextBox
            {
                Location = new Point(50, 180),
                Size = new Size(400, 42),
                PlaceholderText = "Entrez votre adresse email"
            };
            MotDePasseOublieStyle.AppliquerTextBox(txtEmail);

            messageControl = new AuthMessageControl
            {
                Location = new Point(50, 235),
                Size = new Size(400, 25)
            };

            Button btnEnvoyer = new Button
            {
                Text = "Envoyer le lien de réinitialisation",
                Location = new Point(50, 270),
                Size = new Size(400, 50)
            };
            MotDePasseOublieStyle.AppliquerBouton(btnEnvoyer);
            btnEnvoyer.Click += (s, e) => EnvoiLienDemande?.Invoke(this, EventArgs.Empty);

            LinkLabel lienRetour = new LinkLabel
            {
                Text = "← Retour à la connexion",
                Location = new Point(50, 340),
                Size = new Size(400, 25)
            };
            MotDePasseOublieStyle.AppliquerLien(lienRetour);
            lienRetour.LinkClicked += (s, e) => RetourConnexionDemande?.Invoke(this, EventArgs.Empty);

            carte.Controls.Add(lblTitre);
            carte.Controls.Add(lblDescription);
            carte.Controls.Add(lblEmail);
            carte.Controls.Add(txtEmail);
            carte.Controls.Add(messageControl);
            carte.Controls.Add(btnEnvoyer);
            carte.Controls.Add(lienRetour);

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

            carte.Location = new Point((ClientSize.Width - carte.Width) / 2, Math.Max(0, (ClientSize.Height - carte.Height) / 2));
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