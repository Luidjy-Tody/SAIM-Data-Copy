using SaimDataCopy.Styles.Authentification;
using SaimDataCopy.Views.Authentification.Components;

namespace SaimDataCopy.Views.Authentification
{
    public class IdentificationView : Form
    {
        private readonly TextBox txtIdentifiant;
        private readonly AuthPasswordTextBox txtMotDePasse;
        private readonly AuthMessageControl messageControl;

        public event EventHandler? ConnexionDemandee;
        public event EventHandler? MotDePasseOublieDemande;
        public event EventHandler? InscriptionDemandee;

        public string Identifiant => txtIdentifiant.Text.Trim();
        public string MotDePasse => txtMotDePasse.Texte;

        public IdentificationView()
        {
            Text = "SaimDataCopy - Identification";
            Size = new Size(560, 720);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            AuthGradientPanel fond = new AuthGradientPanel
            {
                Dock = DockStyle.Fill
            };

            AuthHeaderControl header = new AuthHeaderControl
            {
                Location = new Point(20, 45)
            };

            AuthShadowPanel carte = new AuthShadowPanel
            {
                Size = new Size(440, 410),
                Location = new Point(60, 155)
            };

            Label lblTitre = new Label
            {
                Text = "Identification",
                Font = AuthentificationFormStyle.TitreCarte(),
                ForeColor = AuthentificationFormStyle.TextePrincipal,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(40, 30),
                Size = new Size(360, 45)
            };

            Label lblIdentifiant = CreerLabel("Identifiant", 40, 95);

            txtIdentifiant = new TextBox
            {
                Location = new Point(40, 123),
                Size = new Size(360, 36)
            };
            AuthTextBoxStyle.Appliquer(txtIdentifiant);

            Label lblMotDePasse = CreerLabel("Mot de passe", 40, 180);

            txtMotDePasse = new AuthPasswordTextBox
            {
                Location = new Point(40, 208),
                Size = new Size(360, 38)
            };

            LinkLabel lienMotDePasse = new LinkLabel
            {
                Text = "Mot de passe oublié ?",
                Location = new Point(220, 252),
                Size = new Size(180, 25)
            };
            AuthLabelStyle.AppliquerLien(lienMotDePasse);
            lienMotDePasse.LinkClicked += (s, e) => MotDePasseOublieDemande?.Invoke(this, EventArgs.Empty);

            messageControl = new AuthMessageControl
            {
                Location = new Point(40, 282)
            };

            Button btnConnexion = new Button
            {
                Text = "S'identifier",
                Location = new Point(40, 315),
                Size = new Size(360, 48)
            };
            AuthButtonStyle.Appliquer(btnConnexion);
            btnConnexion.Click += (s, e) => ConnexionDemandee?.Invoke(this, EventArgs.Empty);

            LinkLabel lienInscription = new LinkLabel
            {
                Text = "Pas encore de compte ? S'inscrire",
                Location = new Point(40, 370),
                Size = new Size(360, 25)
            };
            AuthLabelStyle.AppliquerLien(lienInscription);
            lienInscription.LinkClicked += (s, e) => InscriptionDemandee?.Invoke(this, EventArgs.Empty);

            AuthFooterControl footer = new AuthFooterControl
            {
                Location = new Point(20, 610)
            };

            carte.Controls.Add(lblTitre);
            carte.Controls.Add(lblIdentifiant);
            carte.Controls.Add(txtIdentifiant);
            carte.Controls.Add(lblMotDePasse);
            carte.Controls.Add(txtMotDePasse);
            carte.Controls.Add(lienMotDePasse);
            carte.Controls.Add(messageControl);
            carte.Controls.Add(btnConnexion);
            carte.Controls.Add(lienInscription);

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

        public void ViderErreur()
        {
            messageControl.Effacer();
        }
    }
}