using FontAwesome.Sharp;
using SaimDataCopy.Styles.Authentification.Commun;
using SaimDataCopy.Styles.Authentification.MotDePasseOublie;
using SaimDataCopy.Views.Authentification.Components;

namespace SaimDataCopy.Views.Authentification
{
    public class MotDePasseOublieView : Form
    {
        private readonly TextBox txtEmail;
        private readonly AuthMessageControl messageControl;

        private readonly Panel fond;
        private readonly AuthHeaderControl header;
        private readonly AuthShadowPanel carte;
        private readonly AuthFooterControl footer;

        public event EventHandler? EnvoiLienDemande;
        public event EventHandler? RetourConnexionDemande;

        public string Email => txtEmail.Text.Trim();

        public MotDePasseOublieView()
        {
            Text = "SaimDataCopy - Mot de passe oublié";
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

            footer = new AuthFooterControl();

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

            header.Location = new Point((ClientSize.Width - header.Width) / 2, 85);

            carte.Location = new Point(
                (ClientSize.Width - carte.Width) / 2,
                (ClientSize.Height - carte.Height) / 2 + 35
            );

            footer.Location = new Point(
                (ClientSize.Width - footer.Width) / 2,
                ClientSize.Height - 55
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