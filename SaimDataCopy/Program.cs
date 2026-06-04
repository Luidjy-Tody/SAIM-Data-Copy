using SaimDataCopy.DataProviders.Execution;
using SaimDataCopy.Models.Execution;
using SaimDataCopy.Services.Execution;
using SaimDataCopy.Views.Forms;

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

        private static bool EstModeExecutionAutomatique(string[] args)
        {
            return args.Any(argument =>
                argument.Equals("--run-copy", StringComparison.OrdinalIgnoreCase));
        }

        private static async Task LancerExecutionAutomatiqueAsync()
        {
            // Le DataProvider lit les paramètres JSON déjà sauvegardés :
            // configuration_execution.json,
            // bases_copier.json,
            // logs_parametres.json,
            // email_parametres.json.
            ExecutionDataProvider executionDataProvider = new ExecutionDataProvider();

            // Le Service contient déjà la logique de copie,
            // les logs, l'e-mail automatique et l'historique.
            ExecutionService executionService = new ExecutionService(executionDataProvider);

            // En mode automatique, il n'y a pas d'interface à mettre à jour.
            // On garde quand même un objet Progress pour respecter la méthode existante.
            Progress<ExecutionProgressionModel> progression =
                new Progress<ExecutionProgressionModel>();

            using CancellationTokenSource cancellationTokenSource =
                new CancellationTokenSource();

            await executionService.LancerCopieAsync(
                progression,
                cancellationTokenSource.Token,
                "Automatique"
            );

            Environment.ExitCode = 0;
        }
    }
}