using Microsoft.EntityFrameworkCore;
using SaimDataCopy.Models.Authentification;

namespace SaimDataCopy.DataAccess
{
    public class AuthentificationDbContext : DbContext
    {
        public AuthentificationDbContext(DbContextOptions<AuthentificationDbContext> options)
            : base(options)
        {

        }

        public DbSet<UtilisateurModel> Utilisateurs { get; set; }

        public DbSet<LogUtilisateurModel> LogsUtilisateurs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UtilisateurModel>().ToTable("User");
            modelBuilder.Entity<LogUtilisateurModel>().ToTable("Log");

            modelBuilder.Entity<UtilisateurModel>()
                .HasIndex(u => u.Identifiant)
                .IsUnique();

            modelBuilder.Entity<UtilisateurModel>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<UtilisateurModel>()
                .Property(u => u.Statut)
                .HasMaxLength(20)
                .HasDefaultValue("User");

            modelBuilder.Entity<LogUtilisateurModel>()
                .HasOne(l => l.Utilisateur)
                .WithMany()
                .HasForeignKey(l => l.UtilisateurId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}