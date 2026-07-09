using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using SaimDataCopy.Helpers;

namespace SaimDataCopy.DataAccess
{
    public class AuthentificationDbContextFactory
        : IDesignTimeDbContextFactory<AuthentificationDbContext>
    {
        public AuthentificationDbContext CreateDbContext(string[] args)
        {
            string chaineConnexion =
                AuthentificationConnexionHelper.ObtenirChaineConnexionBaseAuthentification();

            DbContextOptions<AuthentificationDbContext> options =
                new DbContextOptionsBuilder<AuthentificationDbContext>()
                    .UseMySql(
                        chaineConnexion,
                        ServerVersion.AutoDetect(chaineConnexion)
                    )
                    .Options;

            return new AuthentificationDbContext(options);
        }
    }
}