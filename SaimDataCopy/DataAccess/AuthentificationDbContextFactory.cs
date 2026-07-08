using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace SaimDataCopy.DataAccess
{
    public class AuthentificationDbContextFactory
        : IDesignTimeDbContextFactory<AuthentificationDbContext>
    {
        public AuthentificationDbContext CreateDbContext(string[] args)
        {
            string chaineConnexion ="server=localhost;port=3306;database=saimdatacopy_auth;user=root;password=;";

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