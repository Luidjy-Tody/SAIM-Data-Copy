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
        //private readonly AuthFooterControl footer;

        private readonly AuthShadowPanel cartePrincipale;
        private readonly Panel panelCarteContenu;

        private readonly IdentificationView identificationView;
        private readonly InscriptionView inscriptionView;
        private readonly MotDePasseOublieView motDePasseOublieView;

        private readonly IconButton btnReduire;
        private readonly IconButton btnAgrandir;
        private readonly IconButton btnFermer;
        private bool creationPremierCompteEnCours;
        private readonly AdminVerificationView adminVerificationView;
        private bool adminAutoriseInscription;
        private UserControl? pageActuelleControle;

        public bool AuthentificationReussie { get; private set; }

        public AuthentificationForm(AuthentificationController authentificationController)
        {
            _authentificationController = authentificationController;

            Text = "SaimDataCopy - Authentification";
            Size = new Size(1280, 780);
            MinimumSize = new Size(1100, 760);
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
            //footer = new AuthFooterControl();

            cartePrincipale = new AuthShadowPanel
            {
                Size = new Size(540, 620),
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

            adminVerificationView = new AdminVerificationView
            {
                Dock = DockStyle.Fill
            };

            panelCarteContenu.Controls.Add(identificationView);
            panelCarteContenu.Controls.Add(adminVerificationView);
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
            //fond.Controls.Add(footer);
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

            identificationView.InscriptionDemandee += async (sender, e) =>
            {
                bool utilisateurExiste =
                    await _authentificationController.ExisteAuMoinsUnUtilisateurAsync();

                if (!utilisateurExiste)
                {
                    creationPremierCompteEnCours = true;
                    adminAutoriseInscription = true;

                    inscriptionView.ViderMessage();
                    inscriptionView.DefinirStatut("Admin");
                    inscriptionView.BloquerChoixStatut();

                    AfficherPageInscription();
                    return;
                }

                creationPremierCompteEnCours = false;
                adminAutoriseInscription = false;

                inscriptionView.AutoriserChoixStatut();

                adminVerificationView.ViderMessage();
                AfficherPageVerificationAdmin();
            };

            adminVerificationView.VerificationAdminDemandee += AdminVerificationView_VerificationAdminDemandee;

            adminVerificationView.RetourConnexionDemande += (sender, e) =>
            {
                adminAutoriseInscription = false;
                AfficherPageIdentification();
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


        private void AfficherPageVerificationAdmin()
        {
            AfficherPage(adminVerificationView);
        }

        private async void AdminVerificationView_VerificationAdminDemandee(
    object? sender,
    EventArgs e)
        {
            adminVerificationView.ViderMessage();

            bool adminOk = await _authentificationController.VerifierAuthentificationAdminAsync(
                adminVerificationView.IdentifiantAdmin,
                adminVerificationView.MotDePasseAdmin
            );

            if (!adminOk)
            {
                adminAutoriseInscription = false;
                adminVerificationView.AfficherErreur("Accès refusé. Un compte Admin valide est requis.");
                return;
            }

            adminAutoriseInscription = true;

            inscriptionView.ViderMessage();
            AfficherPageInscription();
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

            if (!adminAutoriseInscription)
            {
                inscriptionView.AfficherErreur("Accès refusé. Veuillez d'abord valider un compte Admin.");
                AfficherPageVerificationAdmin();
                return;
            }

            if (inscriptionView.MotDePasse != inscriptionView.ConfirmationMotDePasse)
            {
                inscriptionView.AfficherErreur("Les mots de passe ne correspondent pas.");
                return;
            }

            string statutCompte = creationPremierCompteEnCours? "Admin" : inscriptionView.Statut;

            string messageInscription = await _authentificationController.InscrireEtRetournerMessageAsync(
                inscriptionView.NomComplet,
                inscriptionView.Identifiant,
                inscriptionView.Email,
                inscriptionView.MotDePasse,
                statutCompte
            );

            if (messageInscription != "Compte créé avec succès. Vous pouvez vous connecter.")
            {
                inscriptionView.AfficherErreur(messageInscription);
                return;
            }

            inscriptionView.AfficherSucces(messageInscription);
            creationPremierCompteEnCours = false;
            inscriptionView.AutoriserChoixStatut();
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

            // Header fixe.
            header.Location = new Point(
                (ClientSize.Width - header.Width) / 2,
                22
            );

            // Zone disponible entre le header et le bas de la fenêtre.
            int topDisponible = header.Bottom + 25;
            int bottomDisponible = ClientSize.Height - 35;
            int hauteurDisponible = bottomDisponible - topDisponible;

            int xCarte = (ClientSize.Width - cartePrincipale.Width) / 2;
            int yCarte = topDisponible + Math.Max(0, (hauteurDisponible - cartePrincipale.Height) / 2);

            cartePrincipale.Location = new Point(xCarte, yCarte);

            // Boutons fenêtre fixes.
            btnReduire.Location = new Point(ClientSize.Width - 170, 14);
            btnAgrandir.Location = new Point(ClientSize.Width - 115, 14);
            btnFermer.Location = new Point(ClientSize.Width - 60, 14);

            header.BringToFront();
            cartePrincipale.BringToFront();

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