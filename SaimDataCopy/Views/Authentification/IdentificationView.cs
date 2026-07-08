using FontAwesome.Sharp;
using SaimDataCopy.Styles.Authentification.Commun;
using SaimDataCopy.Styles.Authentification.Identification;
using SaimDataCopy.Views.Authentification.Components;

namespace SaimDataCopy.Views.Authentification
{
    public class IdentificationView : Form
    {
        private readonly TextBox txtIdentifiant;
        private readonly AuthPasswordTextBox txtMotDePasse;
        private readonly AuthMessageControl messageControl;

        private readonly Panel fond;
        private readonly AuthHeaderControl header;
        private readonly AuthShadowPanel carte;
        private readonly AuthFooterControl footer;

        public event EventHandler? ConnexionDemandee;
        public event EventHandler? MotDePasseOublieDemande;
        public event EventHandler? InscriptionDemandee;

        public string Identifiant => txtIdentifiant.Text.Trim();
        public string MotDePasse => txtMotDePasse.Texte;

        public IdentificationView()
        {
            Text = "SaimDataCopy - Identification";
            Size = new Size(1280, 720);
            MinimumSize = new Size(1100, 650);
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.None;
            MaximizeBox = true;
            MinimizeBox = true;

            fond = new Panel
            {
                Dock = DockStyle.Fill
            };
            fond.Paint += Fond_Paint;

            header = new AuthHeaderControl();

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

            footer = new AuthFooterControl();

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

            header.Location = new Point((ClientSize.Width - header.Width) / 2, 70);

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