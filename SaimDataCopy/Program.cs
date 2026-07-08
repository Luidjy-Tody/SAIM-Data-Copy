using SaimDataCopy.Models.Execution;
using SaimDataCopy.Services.Execution;
using SaimDataCopy.Views.Forms;
using SaimDataCopy.DataAccess;
using Microsoft.EntityFrameworkCore;
using SaimDataCopy.Controllers.Authentification;
using SaimDataCopy.Views.Authentification;

namespace SaimDataCopy
{
    internal static class Program
    {
        /// <summary>
        /// Point d'entrée principal de l'application.
        /// </summary>
        [STAThread]
        static async Task Main(string[] args)
        {
            try
            {
                // Si l'application est lancée par le Task Scheduler avec --run-copy,
                // on lance la copie automatiquement sans afficher l'interface.
                if (EstModeExecutionAutomatique(args))
                {
                    await LancerExecutionAutomatiqueAsync();
                    return;
                }

                // Sinon, on lance l'application normalement avec l'interface WinForms.
                ApplicationConfiguration.Initialize();
                await InitialiserBaseAuthentificationAsync();

                AuthentificationController authentificationController = new AuthentificationController();

                bool authentificationOk = await AfficherAuthentificationAsync(authentificationController);

                if (!authentificationOk)
                {
                    return;
                }

                Application.Run(new MainForm());
            }
            catch (Exception ex)
            {
                // En mode automatique, il ne faut pas afficher de MessageBox,
                // car le Task Scheduler ne doit pas attendre une action utilisateur.
                if (EstModeExecutionAutomatique(args))
                {
                    Environment.ExitCode = 1;
                    return;
                }

                MessageBox.Show(
                    "Erreur pendant le démarrage de l'application :"
                    + Environment.NewLine
                    + ex.Message,
                    "Erreur",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private static Task<bool> AfficherAuthentificationAsync(AuthentificationController authentificationController)
        {
            while (true)
            {
                using IdentificationView identificationView = new IdentificationView();

                identificationView.ConnexionDemandee += async (sender, e) =>
                {
                    identificationView.ViderErreur();

                    bool connexionOk = await authentificationController.ConnecterAsync(
                        identificationView.Identifiant,
                        identificationView.MotDePasse
                    );

                    if (!connexionOk)
                    {
                        identificationView.AfficherErreur("Identifiant ou mot de passe incorrect.");
                        return;
                    }

                    identificationView.DialogResult = DialogResult.OK;
                };

                identificationView.InscriptionDemandee += (sender, e) =>
                {
                    identificationView.DialogResult = DialogResult.Retry;
                };

                identificationView.MotDePasseOublieDemande += (sender, e) =>
                {
                    identificationView.DialogResult = DialogResult.Ignore;
                };

                DialogResult resultat = identificationView.ShowDialog();

                if (resultat == DialogResult.OK)
                {
                    return Task.FromResult(true);
                }

                if (resultat == DialogResult.Cancel)
                {
                    return Task.FromResult(false);
                }

                if (resultat == DialogResult.Retry)
                {
                    bool continuer = AfficherInscription(authentificationController);

                    if (!continuer)
                    {
                        return Task.FromResult(false);
                    }
                }

                if (resultat == DialogResult.Ignore)
                {
                    bool continuer = AfficherMotDePasseOublie();

                    if (!continuer)
                    {
                        return Task.FromResult(false);
                    }
                }
            }
        }

        private static bool AfficherInscription(AuthentificationController authentificationController)
        {
            using InscriptionView inscriptionView = new InscriptionView();

            inscriptionView.InscriptionDemandee += async (sender, e) =>
            {
                inscriptionView.ViderMessage();

                if (inscriptionView.MotDePasse != inscriptionView.ConfirmationMotDePasse)
                {
                    inscriptionView.AfficherErreur("Les mots de passe ne correspondent pas.");
                    return;
                }

                string messageInscription = await authentificationController.InscrireEtRetournerMessageAsync(
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
            };

            inscriptionView.RetourConnexionDemande += (sender, e) =>
            {
                inscriptionView.DialogResult = DialogResult.OK;
            };

            DialogResult resultat = inscriptionView.ShowDialog();

            return resultat == DialogResult.OK;
        }


        private static bool AfficherMotDePasseOublie()
        {
            using MotDePasseOublieView motDePasseOublieView = new MotDePasseOublieView();

            motDePasseOublieView.EnvoiLienDemande += (sender, e) =>
            {
                motDePasseOublieView.ViderMessage();

                if (string.IsNullOrWhiteSpace(motDePasseOublieView.Email))
                {
                    motDePasseOublieView.AfficherErreur("Veuillez saisir votre adresse email.");
                    return;
                }

                motDePasseOublieView.AfficherSucces("Fonctionnalité à finaliser : réinitialisation du mot de passe.");
            };

            motDePasseOublieView.RetourConnexionDemande += (sender, e) =>
            {
                motDePasseOublieView.DialogResult = DialogResult.OK;
            };

            DialogResult resultat = motDePasseOublieView.ShowDialog();

            return resultat == DialogResult.OK;
        }

        private static async Task InitialiserBaseAuthentificationAsync()
        {
            string chaineConnexion ="server=localhost;port=3306;database=saimdatacopy_auth;user=root;password=;";

            DbContextOptions<AuthentificationDbContext> options = new DbContextOptionsBuilder<AuthentificationDbContext>()
                    .UseMySql(
                        chaineConnexion,
                        ServerVersion.AutoDetect(chaineConnexion)
                    )
                    .Options;

            using AuthentificationDbContext context = new AuthentificationDbContext(options);

            // Crée la base et les tables si elles n'existent pas.
            await context.Database.EnsureCreatedAsync();
        }

        private static bool EstModeExecutionAutomatique(string[] args)
        {
            return args.Any(argument => argument.Equals("--run-copy", StringComparison.OrdinalIgnoreCase));
        }

        private static async Task LancerExecutionAutomatiqueAsync()
        {
            // Le Service choisit automatiquement SQL Server ou MySQL
            // grâce à ExecutionDataProviderFactory.
            ExecutionService executionService = new ExecutionService();

            Progress<ExecutionProgressionModel> progression = new Progress<ExecutionProgressionModel>();

            using CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            await executionService.LancerCopieAsync(
                progression,
                cancellationTokenSource.Token,
                "Automatique"
            );

            Environment.ExitCode = 0;
        }
    }
}