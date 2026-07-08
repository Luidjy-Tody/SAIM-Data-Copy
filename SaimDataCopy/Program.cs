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
            using AuthentificationForm authentificationForm = new AuthentificationForm(authentificationController);

            DialogResult resultat = authentificationForm.ShowDialog();

            bool authentificationOk =
                resultat == DialogResult.OK &&
                authentificationForm.AuthentificationReussie;

            return Task.FromResult(authentificationOk);
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