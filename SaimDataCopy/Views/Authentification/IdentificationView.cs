using SaimDataCopy.Styles.Authentification.Identification;
using SaimDataCopy.Views.Authentification.Components;

namespace SaimDataCopy.Views.Authentification
{
    public class IdentificationView : UserControl
    {
        private readonly TextBox txtIdentifiant;
        private readonly AuthPasswordTextBox txtMotDePasse;
        private readonly AuthMessageControl messageControl;
        private readonly AuthShadowPanel carte;

        public event EventHandler? ConnexionDemandee;
        public event EventHandler? MotDePasseOublieDemande;
        public event EventHandler? InscriptionDemandee;

        public string Identifiant => txtIdentifiant.Text.Trim();
        public string MotDePasse => txtMotDePasse.Texte;

        public IdentificationView()
        {
            Dock = DockStyle.Fill;
            BackColor = Color.Transparent;
            DoubleBuffered = true;

            carte = new AuthShadowPanel
            {
                Size = new Size(500, 455)
            };

            Label lblTitre = new Label
            {
                Text = "Identification",
                Font = IdentificationStyle.TitreCarte(),
                ForeColor = IdentificationStyle.TextePrincipal,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(50, 35),
                Size = new Size(400, 45)
            };

            Label lblIdentifiant = CreerLabel("Identifiant", 50, 100);

            txtIdentifiant = new TextBox
            {
                Location = new Point(50, 130),
                Size = new Size(400, 42),
                PlaceholderText = "Entrez votre nom utilisateur ou email"
            };
            IdentificationStyle.AppliquerTextBox(txtIdentifiant);

            Label lblMotDePasse = CreerLabel("Mot de passe", 50, 195);

            txtMotDePasse = new AuthPasswordTextBox
            {
                Location = new Point(50, 225),
                Size = new Size(430, 42)
            };

            LinkLabel lienMotDePasse = new LinkLabel
            {
                Text = "Mot de passe oublié ?",
                Location = new Point(260, 275),
                Size = new Size(190, 25)
            };
            IdentificationStyle.AppliquerLien(lienMotDePasse);
            lienMotDePasse.LinkClicked += (s, e) => MotDePasseOublieDemande?.Invoke(this, EventArgs.Empty);

            messageControl = new AuthMessageControl
            {
                Location = new Point(50, 310),
                Size = new Size(400, 25)
            };

            Button btnConnexion = new Button
            {
                Text = "S'identifier",
                Location = new Point(50, 345),
                Size = new Size(400, 50)
            };
            IdentificationStyle.AppliquerBouton(btnConnexion);
            btnConnexion.Click += (s, e) => ConnexionDemandee?.Invoke(this, EventArgs.Empty);

            LinkLabel lienInscription = new LinkLabel
            {
                Text = "Pas encore de compte ? S'inscrire",
                Location = new Point(50, 405),
                Size = new Size(400, 25)
            };
            IdentificationStyle.AppliquerLien(lienInscription);
            lienInscription.LinkClicked += (s, e) => InscriptionDemandee?.Invoke(this, EventArgs.Empty);

            carte.Controls.Add(lblTitre);
            carte.Controls.Add(lblIdentifiant);
            carte.Controls.Add(txtIdentifiant);
            carte.Controls.Add(lblMotDePasse);
            carte.Controls.Add(txtMotDePasse);
            carte.Controls.Add(lienMotDePasse);
            carte.Controls.Add(messageControl);
            carte.Controls.Add(btnConnexion);
            carte.Controls.Add(lienInscription);

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

            carte.Location = new Point(
                (ClientSize.Width - carte.Width) / 2,
                Math.Max(0, (ClientSize.Height - carte.Height) / 2)
            );
        }

        private static Label CreerLabel(string texte, int x, int y)
        {
            Label label = new Label
            {
                Text = texte,
                Location = new Point(x, y),
                Size = new Size(400, 24)
            };

            IdentificationStyle.AppliquerLabelChamp(label);

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