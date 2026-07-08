using SaimDataCopy.Styles.Authentification.Identification;
using SaimDataCopy.Views.Authentification.Components;

namespace SaimDataCopy.Views.Authentification
{
    public class IdentificationView : UserControl
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
            BackColor = Color.White;
            DoubleBuffered = true;

            Label lblTitre = new Label
            {
                Text = "Identification",
                Font = IdentificationStyle.TitreCarte(),
                ForeColor = IdentificationStyle.TextePrincipal,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(70, 45),
                Size = new Size(400, 45)
            };

            Label lblIdentifiant = CreerLabel("Identifiant", 70, 115);

            txtIdentifiant = new TextBox
            {
                Location = new Point(70, 145),
                Size = new Size(400, 42),
                PlaceholderText = "Entrez votre nom utilisateur ou email"
            };
            IdentificationStyle.AppliquerTextBox(txtIdentifiant);

            Label lblMotDePasse = CreerLabel("Mot de passe", 70, 210);

            txtMotDePasse = new AuthPasswordTextBox
            {
                Location = new Point(70, 240),
                Size = new Size(430, 42)
            };

            LinkLabel lienMotDePasse = new LinkLabel
            {
                Text = "Mot de passe oublié ?",
                Location = new Point(280, 292),
                Size = new Size(190, 25),
                Visible = false

            };
            IdentificationStyle.AppliquerLien(lienMotDePasse);
            lienMotDePasse.LinkClicked += (s, e) => MotDePasseOublieDemande?.Invoke(this, EventArgs.Empty);

            messageControl = new AuthMessageControl
            {
                Location = new Point(70, 325),
                Size = new Size(400, 25)
            };

            Button btnConnexion = new Button
            {
                Text = "S'identifier",
                Location = new Point(70, 360),
                Size = new Size(400, 50)
            };
            IdentificationStyle.AppliquerBouton(btnConnexion);
            btnConnexion.Click += (s, e) => ConnexionDemandee?.Invoke(this, EventArgs.Empty);

            LinkLabel lienInscription = new LinkLabel
            {
                Text = "Pas encore de compte ? S'inscrire",
                Location = new Point(70, 425),
                Size = new Size(400, 25)
            };
            IdentificationStyle.AppliquerLien(lienInscription);
            lienInscription.LinkClicked += (s, e) => InscriptionDemandee?.Invoke(this, EventArgs.Empty);

            Controls.Add(lblTitre);
            Controls.Add(lblIdentifiant);
            Controls.Add(txtIdentifiant);
            Controls.Add(lblMotDePasse);
            Controls.Add(txtMotDePasse);
            Controls.Add(lienMotDePasse);
            Controls.Add(messageControl);
            Controls.Add(btnConnexion);
            Controls.Add(lienInscription);
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