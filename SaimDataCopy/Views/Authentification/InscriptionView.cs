using FontAwesome.Sharp;
using SaimDataCopy.Styles.Authentification.Commun;
using SaimDataCopy.Styles.Authentification.Inscription;
using SaimDataCopy.Views.Authentification.Components;

namespace SaimDataCopy.Views.Authentification
{
    public class InscriptionView : Form
    {
        private readonly TextBox txtNomComplet;
        private readonly TextBox txtIdentifiant;
        private readonly TextBox txtEmail;
        private readonly AuthPasswordTextBox txtMotDePasse;
        private readonly AuthPasswordTextBox txtConfirmation;
        private readonly AuthMessageControl messageControl;

        private readonly Panel fond;
        private readonly AuthHeaderControl header;
        private readonly AuthShadowPanel carte;
        private readonly AuthFooterControl footer;

        public event EventHandler? InscriptionDemandee;
        public event EventHandler? RetourConnexionDemande;

        public string NomComplet => txtNomComplet.Text.Trim();
        public string Identifiant => txtIdentifiant.Text.Trim();
        public string Email => txtEmail.Text.Trim();
        public string MotDePasse => txtMotDePasse.Texte;
        public string ConfirmationMotDePasse => txtConfirmation.Texte;

        public InscriptionView()
        {
            Text = "SaimDataCopy - Inscription";
            Size = new Size(1280, 720);
            MinimumSize = new Size(1100, 650);
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.None;
            MaximizeBox = true;
            MinimizeBox = true;

            fond = new Panel { Dock = DockStyle.Fill };
            fond.Paint += Fond_Paint;

            header = new AuthHeaderControl();

            carte = new AuthShadowPanel
            {
                Size = new Size(500, 615)
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

            footer = new AuthFooterControl();

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

            fond.Controls.Add(header);
            fond.Controls.Add(carte);
            fond.Controls.Add(footer);

            Controls.Add(fond);

            AjouterBoutonsFenetre();

            Resize += (s, e) => CentrerElements();
            Shown += (s, e) => CentrerElements();
        }

        private void Fond_Paint(object? sender, PaintEventArgs e)
        {
            AuthBackgroundStyle.DessinerFond(e.Graphics, fond.ClientRectangle);
        }

        private void CentrerElements()
        {
            if (ClientSize.Width <= 0 || ClientSize.Height <= 0)
            {
                return;
            }

            header.Location = new Point((ClientSize.Width - header.Width) / 2, 25);

            carte.Location = new Point(
                (ClientSize.Width - carte.Width) / 2,
                (ClientSize.Height - carte.Height) / 2 + 55
            );

            footer.Location = new Point(
                (ClientSize.Width - footer.Width) / 2,
                ClientSize.Height - 45
            );
        }

        private void AjouterBoutonsFenetre()
        {
            IconButton btnReduire = CreerBoutonFenetre(IconChar.Minus);
            IconButton btnAgrandir = CreerBoutonFenetre(IconChar.WindowMaximize);
            IconButton btnFermer = CreerBoutonFenetre(IconChar.Xmark);

            btnReduire.Location = new Point(ClientSize.Width - 170, 14);
            btnAgrandir.Location = new Point(ClientSize.Width - 115, 14);
            btnFermer.Location = new Point(ClientSize.Width - 60, 14);

            btnReduire.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnAgrandir.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnFermer.Anchor = AnchorStyles.Top | AnchorStyles.Right;

            btnReduire.Click += (s, e) => WindowState = FormWindowState.Minimized;

            btnAgrandir.Click += (s, e) =>
            {
                WindowState = WindowState == FormWindowState.Maximized
                    ? FormWindowState.Normal
                    : FormWindowState.Maximized;
            };

            btnFermer.Click += (s, e) =>
            {
                DialogResult = DialogResult.Cancel;
                Close();
            };

            AuthWindowButtonStyle.Appliquer(btnReduire);
            AuthWindowButtonStyle.Appliquer(btnAgrandir);
            AuthWindowButtonStyle.AppliquerBoutonFermer(btnFermer);

            Controls.Add(btnReduire);
            Controls.Add(btnAgrandir);
            Controls.Add(btnFermer);

            btnReduire.BringToFront();
            btnAgrandir.BringToFront();
            btnFermer.BringToFront();
        }

        private static IconButton CreerBoutonFenetre(IconChar icone)
        {
            return new IconButton
            {
                IconChar = icone
            };
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