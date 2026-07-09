using SaimDataCopy.Models.Execution;
using SaimDataCopy.Services.Execution;
using SaimDataCopy.Views.Forms;
using SaimDataCopy.DataAccess;
using Microsoft.EntityFrameworkCore;
using SaimDataCopy.Controllers.Authentification;
using SaimDataCopy.Views.Authentification;
using SaimDataCopy.Helpers;
using MySqlConnector;

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

        private static Task<bool> AfficherAuthentificationAsync(
            AuthentificationController authentificationController)
        {
            using AuthentificationForm authentificationForm =
                new AuthentificationForm(authentificationController);

            DialogResult resultat = authentificationForm.ShowDialog();

            bool authentificationOk = resultat == DialogResult.OK && authentificationForm.AuthentificationReussie;

            return Task.FromResult(authentificationOk);
        }

        private static async Task InitialiserBaseAuthentificationAsync()
        {
            string chaineConnexionServeur = AuthentificationConnexionHelper.ObtenirChaineConnexionServeur();

            string nomBaseAuthentification = AuthentificationConnexionHelper.ObtenirNomBaseAuthentification();

            await CreerBaseAuthentificationSiInexistanteAsync(chaineConnexionServeur, nomBaseAuthentification);

            string chaineConnexionBase = AuthentificationConnexionHelper.ObtenirChaineConnexionBaseAuthentification();

            DbContextOptions<AuthentificationDbContext> options =
                new DbContextOptionsBuilder<AuthentificationDbContext>()
                    .UseMySql(
                        chaineConnexionBase,
                        ServerVersion.AutoDetect(chaineConnexionBase)
                    )
                    .Options;

            using AuthentificationDbContext context =
                new AuthentificationDbContext(options);

            // Crée automatiquement les tables User, Log, PasswordResetCode
            await context.Database.MigrateAsync();

            // Si une base contient déjà des utilisateurs mais aucun Admin,
            // le premier utilisateur devient Admin.
            await GarantirPresenceAdminAsync(context);
        }

        private static async Task CreerBaseAuthentificationSiInexistanteAsync(string chaineConnexionServeur, string nomBaseAuthentification)
        {
            using MySqlConnection connexion = new MySqlConnection(chaineConnexionServeur);

            await connexion.OpenAsync();

            using MySqlCommand commande = connexion.CreateCommand();

            // On force latin1 pour éviter les erreurs avec utf8mb4.
            commande.CommandText ="CREATE DATABASE IF NOT EXISTS `" + nomBaseAuthentification + "` " + "CHARACTER SET latin1 COLLATE latin1_swedish_ci;";

            await commande.ExecuteNonQueryAsync();
        }

        private static async Task GarantirPresenceAdminAsync(
            AuthentificationDbContext context)
        {
            // On vérifie s'il existe déjà au moins un compte Admin.
            bool adminExiste = await context.Utilisateurs
                .AnyAsync(utilisateur => utilisateur.Statut == "Admin");

            if (adminExiste)
            {
                return;
            }

            // Si aucun Admin n'existe, on récupère le premier utilisateur créé.
            var premierUtilisateur = await context.Utilisateurs
                .OrderBy(utilisateur => utilisateur.Id)
                .FirstOrDefaultAsync();

            if (premierUtilisateur == null)
            {
                return;
            }

            premierUtilisateur.Statut = "Admin";

            await context.SaveChangesAsync();
        }

        private static bool EstModeExecutionAutomatique(string[] args)
        {
            return args.Any(argument =>
                argument.Equals("--run-copy", StringComparison.OrdinalIgnoreCase));
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