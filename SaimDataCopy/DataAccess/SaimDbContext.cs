using Microsoft.EntityFrameworkCore;
using SaimDataCopy.Models.BasesCopier;
using SaimDataCopy.Models.Configuration;

namespace SaimDataCopy.DataAccess
{
    // DbContext principal de l'application.
    // Il représente la connexion entre l'application WinForms
    // et la base de données SQL Server.
    public class SaimDbContext : DbContext
    {
        public SaimDbContext(DbContextOptions<SaimDbContext> options)
            : base(options)
        {
        }

        // Table pour sauvegarder la configuration générale.
        public DbSet<ConfigurationSqlModel> Configurations { get; set; }

        // Table pour sauvegarder les bases à copier.
        public DbSet<BaseCopieSqlModel> BasesCopier { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuration de la table Configurations.
            modelBuilder.Entity<ConfigurationSqlModel>(entity =>
            {
                entity.ToTable("Configurations");

                entity.HasKey(c => c.Id);

                entity.Property(c => c.ServeurSourceNom)
                    .HasMaxLength(200)
                    .IsRequired();

                entity.Property(c => c.ServeurSourceChaineConnexion)
                    .HasMaxLength(500);

                entity.Property(c => c.ServeurSourceIdentifiant)
                    .HasMaxLength(100);

                entity.Property(c => c.ServeurSourceMotDePasse)
                    .HasMaxLength(200);

                entity.Property(c => c.ServeurCibleNom)
                    .HasMaxLength(200)
                    .IsRequired();

                entity.Property(c => c.ServeurCibleChaineConnexion)
                    .HasMaxLength(500);

                entity.Property(c => c.ServeurCibleIdentifiant)
                    .HasMaxLength(100);

                entity.Property(c => c.ServeurCibleMotDePasse)
                    .HasMaxLength(200);

                entity.Property(c => c.ModeCopie)
                    .HasMaxLength(50)
                    .IsRequired();

                entity.Property(c => c.ComportementErreur)
                    .HasMaxLength(100)
                    .IsRequired();
            });

            // Configuration de la table BasesCopier.
            modelBuilder.Entity<BaseCopieSqlModel>(entity =>
            {
                entity.ToTable("BasesCopier");

                entity.HasKey(b => b.Id);

                entity.Property(b => b.NomBase)
                    .HasMaxLength(200)
                    .IsRequired();

                entity.Property(b => b.ModeCopie)
                    .HasMaxLength(50)
                    .IsRequired();

                entity.Property(b => b.Statut)
                    .HasMaxLength(100)
                    .IsRequired();

                // Une base ne doit pas être enregistrée deux fois.
                entity.HasIndex(b => b.NomBase)
                    .IsUnique();
            });
        }
    }
}