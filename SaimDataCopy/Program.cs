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

                ApplicationConfiguration.Initialize();

                await InitialiserBaseAuthentificationAsync();

                AuthentificationController authentificationController =
                    new AuthentificationController();

                using AuthentificationForm authentificationForm =
                    new AuthentificationForm(authentificationController);

                while (true)
                {
                    bool authentificationOk =
                        await AfficherAuthentificationAsync(authentificationForm);

                    if (!authentificationOk)
                    {
                        return;
                    }

                    using MainForm mainForm = new MainForm();

                    Application.Run(mainForm);

                    // Quand MainForm se ferme avec X, on revient ici.
                    // MainForm a déjà déconnecté l'utilisateur.
                    authentificationForm.PreparerRetourApresDeconnexionDepuisMainForm();
                }
            }
            catch (Exception ex)
            {
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
            AuthentificationForm authentificationForm)
        {
            authentificationForm.PreparerAffichageAuthentification();

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
                        AuthentificationConnexionHelper.ObtenirVersionServeurMySql()
                    )
                    .Options;

            using AuthentificationDbContext context =
                new AuthentificationDbContext(options);

            await context.Database.MigrateAsync();

            await GarantirPresenceAdminAsync(context);
        }

        private static async Task CreerBaseAuthentificationSiInexistanteAsync(
            string chaineConnexionServeur,
            string nomBaseAuthentification)
        {
            using MySqlConnection connexion = new MySqlConnection(chaineConnexionServeur);

            await connexion.OpenAsync();

            using MySqlCommand commande = connexion.CreateCommand();

            commande.CommandText =
                "CREATE DATABASE IF NOT EXISTS `" + nomBaseAuthentification + "` " +
                "CHARACTER SET latin1 COLLATE latin1_swedish_ci;";

            await commande.ExecuteNonQueryAsync();
        }

        private static async Task GarantirPresenceAdminAsync(
            AuthentificationDbContext context)
        {
            bool adminExiste = await context.Utilisateurs
                .AnyAsync(utilisateur => utilisateur.Statut.Equals("Admin"));

            if (adminExiste)
            {
                return;
            }

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