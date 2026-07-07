using SaimDataCopy.Styles.Authentification;
using SaimDataCopy.Views.Authentification.Components;

namespace SaimDataCopy.Views.Authentification
{
    public class MotDePasseOublieView : Form
    {
        private readonly TextBox txtEmail;
        private readonly AuthMessageControl messageControl;

        public event EventHandler? EnvoiLienDemande;
        public event EventHandler? RetourConnexionDemande;

        public string Email => txtEmail.Text.Trim();

        public MotDePasseOublieView()
        {
            Text = "SaimDataCopy - Mot de passe oublié";
            Size = new Size(560, 720);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            AuthGradientPanel fond = new AuthGradientPanel { Dock = DockStyle.Fill };

            AuthHeaderControl header = new AuthHeaderControl
            {
                Location = new Point(20, 45)
            };

            AuthShadowPanel carte = new AuthShadowPanel
            {
                Size = new Size(440, 390),
                Location = new Point(60, 155)
            };

            Label lblTitre = new Label
            {
                Text = "Mot de passe oublié",
                Font = AuthentificationFormStyle.TitreCarte(),
                ForeColor = AuthentificationFormStyle.TextePrincipal,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(40, 30),
                Size = new Size(360, 45)
            };

            Label lblDescription = new Label
            {
                Text = "Saisissez votre adresse e-mail et nous vous enverrons un lien pour réinitialiser votre mot de passe.",
                Font = AuthentificationFormStyle.LabelChamp(),
                ForeColor = AuthentificationFormStyle.TexteAide,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(40, 82),
                Size = new Size(360, 55)
            };

            Label lblEmail = CreerLabel("Email", 40, 150);

            txtEmail = new TextBox
            {
                Location = new Point(40, 178),
                Size = new Size(360, 36)
            };
            AuthTextBoxStyle.Appliquer(txtEmail);

            messageControl = new AuthMessageControl
            {
                Location = new Point(40, 225)
            };

            Button btnEnvoyer = new Button
            {
                Text = "Envoyer le lien de réinitialisation",
                Location = new Point(40, 260),
                Size = new Size(360, 48)
            };
            AuthButtonStyle.Appliquer(btnEnvoyer);
            btnEnvoyer.Click += (s, e) => EnvoiLienDemande?.Invoke(this, EventArgs.Empty);

            LinkLabel lienRetour = new LinkLabel
            {
                Text = "← Retour à la connexion",
                Location = new Point(40, 330),
                Size = new Size(360, 25)
            };
            AuthLabelStyle.AppliquerLien(lienRetour);
            lienRetour.LinkClicked += (s, e) => RetourConnexionDemande?.Invoke(this, EventArgs.Empty);

            AuthFooterControl footer = new AuthFooterControl
            {
                Location = new Point(20, 610)
            };

            carte.Controls.Add(lblTitre);
            carte.Controls.Add(lblDescription);
            carte.Controls.Add(lblEmail);
            carte.Controls.Add(txtEmail);
            carte.Controls.Add(messageControl);
            carte.Controls.Add(btnEnvoyer);
            carte.Controls.Add(lienRetour);

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