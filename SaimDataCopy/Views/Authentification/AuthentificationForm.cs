using FontAwesome.Sharp;
using SaimDataCopy.Controllers.Authentification;
using SaimDataCopy.Styles.Authentification.Commun;
using SaimDataCopy.Views.Authentification.Components;

namespace SaimDataCopy.Views.Authentification
{
    public class AuthentificationForm : Form
    {
        private readonly AuthentificationController _authentificationController;

        private readonly Panel fond;

        private readonly AuthHeaderControl header;
        private readonly AuthFooterControl footer;

        private readonly AuthShadowPanel cartePrincipale;
        private readonly Panel panelCarteContenu;

        private readonly IdentificationView identificationView;
        private readonly InscriptionView inscriptionView;
        private readonly MotDePasseOublieView motDePasseOublieView;

        private readonly IconButton btnReduire;
        private readonly IconButton btnAgrandir;
        private readonly IconButton btnFermer;

        private UserControl? pageActuelleControle;

        public bool AuthentificationReussie { get; private set; }

        public AuthentificationForm(AuthentificationController authentificationController)
        {
            _authentificationController = authentificationController;

            Text = "SaimDataCopy - Authentification";
            Size = new Size(1280, 720);
            MinimumSize = new Size(1100, 720);
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.None;
            MaximizeBox = true;
            MinimizeBox = true;

            fond = new PanelDoubleBuffer
            {
                Dock = DockStyle.Fill
            };
            fond.Paint += Fond_Paint;

            header = new AuthHeaderControl();
            footer = new AuthFooterControl();

            cartePrincipale = new AuthShadowPanel
            {
                Size = new Size(540, 530),
                Padding = new Padding(1)
            };

            panelCarteContenu = new PanelDoubleBuffer
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White
            };

            identificationView = new IdentificationView
            {
                Dock = DockStyle.Fill
            };

            inscriptionView = new InscriptionView
            {
                Dock = DockStyle.Fill
            };

            motDePasseOublieView = new MotDePasseOublieView
            {
                Dock = DockStyle.Fill
            };

            panelCarteContenu.Controls.Add(identificationView);
            panelCarteContenu.Controls.Add(inscriptionView);
            panelCarteContenu.Controls.Add(motDePasseOublieView);

            cartePrincipale.Controls.Add(panelCarteContenu);

            btnReduire = CreerBoutonFenetre(IconChar.Minus);
            btnAgrandir = CreerBoutonFenetre(IconChar.WindowMaximize);
            btnFermer = CreerBoutonFenetre(IconChar.Xmark);

            AuthWindowButtonStyle.Appliquer(btnReduire);
            AuthWindowButtonStyle.Appliquer(btnAgrandir);
            AuthWindowButtonStyle.AppliquerBoutonFermer(btnFermer);

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

            fond.Controls.Add(header);
            fond.Controls.Add(cartePrincipale);
            fond.Controls.Add(footer);
            fond.Controls.Add(btnReduire);
            fond.Controls.Add(btnAgrandir);
            fond.Controls.Add(btnFermer);

            Controls.Add(fond);

            BrancherEvenements();

            AfficherPageIdentification();

            Resize += (s, e) =>
            {
                PositionnerElementsFixes();
                fond.Invalidate();
            };

            Shown += (s, e) => PositionnerElementsFixes();
        }

        private void Fond_Paint(object? sender, PaintEventArgs e)
        {
            AuthBackgroundStyle.DessinerFond(e.Graphics, fond.ClientRectangle);
        }

        private void BrancherEvenements()
        {
            identificationView.ConnexionDemandee += IdentificationView_ConnexionDemandee;

            identificationView.InscriptionDemandee += (sender, e) =>
            {
                AfficherPageInscription();
            };

            identificationView.MotDePasseOublieDemande += (sender, e) =>
            {
                AfficherPageMotDePasseOublie();
            };

            inscriptionView.InscriptionDemandee += InscriptionView_InscriptionDemandee;

            inscriptionView.RetourConnexionDemande += (sender, e) =>
            {
                AfficherPageIdentification();
            };

            motDePasseOublieView.EnvoiLienDemande += MotDePasseOublieView_EnvoiLienDemande;

            motDePasseOublieView.RetourConnexionDemande += (sender, e) =>
            {
                AfficherPageIdentification();
            };
        }

        private async void IdentificationView_ConnexionDemandee(object? sender, EventArgs e)
        {
            identificationView.ViderErreur();

            bool connexionOk = await _authentificationController.ConnecterAsync(
                identificationView.Identifiant,
                identificationView.MotDePasse
            );

            if (!connexionOk)
            {
                identificationView.AfficherErreur("Identifiant ou mot de passe incorrect.");
                return;
            }

            AuthentificationReussie = true;
            DialogResult = DialogResult.OK;
            Close();
        }

        private async void InscriptionView_InscriptionDemandee(object? sender, EventArgs e)
        {
            inscriptionView.ViderMessage();

            if (inscriptionView.MotDePasse != inscriptionView.ConfirmationMotDePasse)
            {
                inscriptionView.AfficherErreur("Les mots de passe ne correspondent pas.");
                return;
            }

            string messageInscription = await _authentificationController.InscrireEtRetournerMessageAsync(
                inscriptionView.NomComplet,
                inscriptionView.Identifiant,
                inscriptionView.Email,
                inscriptionView.MotDePasse
            );

            if (messageInscription != "Compte créé avec succès. Vous pouvez vous connecter.")
            {
                inscriptionView.AfficherErreur(messageInscription);
                return;
            }

            inscriptionView.AfficherSucces(messageInscription);
        }

        private void MotDePasseOublieView_EnvoiLienDemande(object? sender, EventArgs e)
        {
            motDePasseOublieView.ViderMessage();

            if (string.IsNullOrWhiteSpace(motDePasseOublieView.Email))
            {
                motDePasseOublieView.AfficherErreur("Veuillez saisir votre adresse email.");
                return;
            }

            motDePasseOublieView.AfficherSucces("Fonctionnalité à finaliser : réinitialisation du mot de passe.");
        }

        private void AfficherPageIdentification()
        {
            AfficherPage(identificationView);
        }

        private void AfficherPageInscription()
        {
            AfficherPage(inscriptionView);
        }

        private void AfficherPageMotDePasseOublie()
        {
            AfficherPage(motDePasseOublieView);
        }

        private void AfficherPage(UserControl page)
        {
            if (pageActuelleControle == page)
            {
                return;
            }

            panelCarteContenu.SuspendLayout();

            foreach (Control controle in panelCarteContenu.Controls)
            {
                controle.Visible = false;
            }

            page.Visible = true;
            page.BringToFront();
            page.Focus();

            pageActuelleControle = page;

            panelCarteContenu.ResumeLayout();

            panelCarteContenu.Invalidate();
            cartePrincipale.Invalidate();
            fond.Invalidate();
        }

        private void PositionnerElementsFixes()
        {
            if (ClientSize.Width <= 0 || ClientSize.Height <= 0)
            {
                return;
            }

            header.Location = new Point(
                (ClientSize.Width - header.Width) / 2,
                22
            );

            footer.Location = new Point(
                (ClientSize.Width - footer.Width) / 2,
                ClientSize.Height - footer.Height - 12
            );

            int topDisponible = header.Bottom + 20;
            int bottomDisponible = footer.Top - 20;
            int hauteurDisponible = bottomDisponible - topDisponible;

            int xCarte = (ClientSize.Width - cartePrincipale.Width) / 2;
            int yCarte = topDisponible + Math.Max(0, (hauteurDisponible - cartePrincipale.Height) / 2);

            cartePrincipale.Location = new Point(xCarte, yCarte);

            btnReduire.Location = new Point(ClientSize.Width - 170, 14);
            btnAgrandir.Location = new Point(ClientSize.Width - 115, 14);
            btnFermer.Location = new Point(ClientSize.Width - 60, 14);

            header.BringToFront();
            cartePrincipale.BringToFront();
            footer.BringToFront();

            btnReduire.BringToFront();
            btnAgrandir.BringToFront();
            btnFermer.BringToFront();
        }

        private static IconButton CreerBoutonFenetre(IconChar icone)
        {
            return new IconButton
            {
                IconChar = icone,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
        }

        private class PanelDoubleBuffer : Panel
        {
            public PanelDoubleBuffer()
            {
                DoubleBuffered = true;
                ResizeRedraw = true;
            }
        }
    }
}