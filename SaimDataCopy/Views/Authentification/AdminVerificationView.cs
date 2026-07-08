using SaimDataCopy.Styles.Authentification.Identification;
using SaimDataCopy.Views.Authentification.Components;

namespace SaimDataCopy.Views.Authentification
{
    public class AdminVerificationView : UserControl
    {
        private readonly TextBox txtIdentifiantAdmin;
        private readonly AuthPasswordTextBox txtMotDePasseAdmin;
        private readonly AuthMessageControl messageControl;

        public event EventHandler? VerificationAdminDemandee;
        public event EventHandler? RetourConnexionDemande;

        public string IdentifiantAdmin => txtIdentifiantAdmin.Text.Trim();
        public string MotDePasseAdmin => txtMotDePasseAdmin.Texte;

        public AdminVerificationView()
        {
            BackColor = Color.White;
            DoubleBuffered = true;

            Label lblTitre = new Label
            {
                Text = "Vérification Admin",
                Font = IdentificationStyle.TitreCarte(),
                ForeColor = IdentificationStyle.TextePrincipal,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(70, 55),
                Size = new Size(400, 45)
            };

            Label lblDescription = new Label
            {
                Text = "Seul un administrateur peut créer un nouveau compte.",
                Font = IdentificationStyle.TextePetit(),
                ForeColor = IdentificationStyle.TexteAide,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(70, 105),
                Size = new Size(400, 45)
            };

            Label lblIdentifiant = CreerLabel("Identifiant ou email Admin", 70, 170);

            txtIdentifiantAdmin = new TextBox
            {
                Location = new Point(70, 200),
                Size = new Size(400, 42),
                PlaceholderText = "Entrez l'identifiant ou email admin"
            };
            IdentificationStyle.AppliquerTextBox(txtIdentifiantAdmin);

            Label lblMotDePasse = CreerLabel("Mot de passe Admin", 70, 265);

            txtMotDePasseAdmin = new AuthPasswordTextBox
            {
                Location = new Point(70, 295),
                Size = new Size(430, 42)
            };

            messageControl = new AuthMessageControl
            {
                Location = new Point(70, 355),
                Size = new Size(400, 25)
            };

            Button btnVerifier = new Button
            {
                Text = "Vérifier l'accès Admin",
                Location = new Point(70, 390),
                Size = new Size(400, 50)
            };
            IdentificationStyle.AppliquerBouton(btnVerifier);
            btnVerifier.Click += (s, e) => VerificationAdminDemandee?.Invoke(this, EventArgs.Empty);

            LinkLabel lienRetour = new LinkLabel
            {
                Text = "← Retour à la connexion",
                Location = new Point(70, 455),
                Size = new Size(400, 25)
            };
            IdentificationStyle.AppliquerLien(lienRetour);
            lienRetour.LinkClicked += (s, e) => RetourConnexionDemande?.Invoke(this, EventArgs.Empty);

            Controls.Add(lblTitre);
            Controls.Add(lblDescription);
            Controls.Add(lblIdentifiant);
            Controls.Add(txtIdentifiantAdmin);
            Controls.Add(lblMotDePasse);
            Controls.Add(txtMotDePasseAdmin);
            Controls.Add(messageControl);
            Controls.Add(btnVerifier);
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

            IdentificationStyle.AppliquerLabelChamp(label);

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