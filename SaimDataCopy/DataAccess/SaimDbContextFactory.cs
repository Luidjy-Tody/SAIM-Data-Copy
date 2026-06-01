using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace SaimDataCopy.DataAccess
{
    // Factory simple pour créer le DbContext.
    // Elle lit la chaîne de connexion depuis appsettings.json.
    public static class SaimDbContextFactory
    {
        public static SaimDbContext CreerDbContext()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            string? chaineConnexion = configuration
                .GetConnectionString("SaimDataCopyDb");

            if (string.IsNullOrWhiteSpace(chaineConnexion))
            {
                throw new InvalidOperationException(
                    "La chaîne de connexion 'SaimDataCopyDb' est introuvable dans appsettings.json."
                );
            }

            DbContextOptions<SaimDbContext> options =
                new DbContextOptionsBuilder<SaimDbContext>()
                    .UseSqlServer(chaineConnexion)
                    .Options;

            return new SaimDbContext(options);
        }
    }
}