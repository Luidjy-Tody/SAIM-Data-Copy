using SaimDataCopy.DataAccess;
using SaimDataCopy.Views.Forms;

namespace SaimDataCopy
{
    internal static class Program
    {
        /// <summary>
        /// Point d'entrée principal de l'application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            try
            {
                // Au démarrage, on vérifie si la base SQL Server existe.
                // Si elle n'existe pas, EF Core crée la base et les tables.
                DatabaseInitializer.InitialiserBaseDeDonnees();

                Application.Run(new MainForm());
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Erreur pendant l'initialisation de la base de données :"
                    + Environment.NewLine
                    + ex.Message,
                    "Erreur SQL Server",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }
    }
}