using Microsoft.EntityFrameworkCore;

namespace SaimDataCopy.DataAccess
{
    // DbContext utilisé pour les accès EF Core.
    // Ici, il sert surtout à exécuter des requêtes SQL sur le serveur source,
    // par exemple pour lire la liste des bases disponibles.
    public class SaimDbContext : DbContext
    {
        public SaimDbContext(DbContextOptions<SaimDbContext> options)
            : base(options)
        {
        }
    }
}