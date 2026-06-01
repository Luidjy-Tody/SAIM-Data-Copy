using Microsoft.EntityFrameworkCore;
namespace SaimDataCopy.DataAccess
{
    // Classe qui prépare la base de données au démarrage.
    // Elle vérifie si la base existe déjà.
    // Si elle n'existe pas, EF Core la crée avec les tables.
    public static class DatabaseInitializer
    {
        public static void InitialiserBaseDeDonnees() 
        {
            using SaimDbContext dbContext = SaimDbContextFactory.CreerDbContext();
            // Pour le début du projet, EnsureCreated est simple :
            // - crée la base si elle n'existe pas
            // - crée les tables Configurations et BasesCopier
            //
            // Plus tard, si ton encadreur demande les migrations EF,
            // on pourra remplacer par dbContext.Database.Migrate().

            dbContext.Database.EnsureCreated();

        }
    }
}
