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

        public DbSet<CodeReinitialisationMotDePasseModel> CodesReinitialisationMotDePasse { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Charset compatible avec MySQL 5.1.
            // On évite utf8mb4 car le serveur de test du superviseur ne le supporte pas.
            modelBuilder.HasCharSet("latin1");

            modelBuilder.Entity<UtilisateurModel>()
                .ToTable("User")
                .HasCharSet("latin1");

            modelBuilder.Entity<LogUtilisateurModel>()
                .ToTable("Log")
                .HasCharSet("latin1");

            modelBuilder.Entity<CodeReinitialisationMotDePasseModel>()
                .ToTable("PasswordResetCode")
                .HasCharSet("latin1");

            modelBuilder.Entity<UtilisateurModel>()
                .HasIndex(utilisateur => utilisateur.Identifiant)
                .IsUnique();

            modelBuilder.Entity<UtilisateurModel>()
                .HasIndex(utilisateur => utilisateur.Email)
                .IsUnique();

            modelBuilder.Entity<UtilisateurModel>()
                .Property(utilisateur => utilisateur.Statut)
                .HasMaxLength(20)
                .HasDefaultValue("User");

            modelBuilder.Entity<UtilisateurModel>()
                .Property(utilisateur => utilisateur.DateCreation)
                .HasColumnType("datetime");

            modelBuilder.Entity<UtilisateurModel>()
                .Property(utilisateur => utilisateur.DerniereConnexion)
                .HasColumnType("datetime");

            modelBuilder.Entity<LogUtilisateurModel>()
                .HasOne(log => log.Utilisateur)
                .WithMany()
                .HasForeignKey(log => log.UtilisateurId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<LogUtilisateurModel>()
                .Property(log => log.DateHeure)
                .HasColumnType("datetime");

            modelBuilder.Entity<CodeReinitialisationMotDePasseModel>()
                .HasOne(code => code.Utilisateur)
                .WithMany()
                .HasForeignKey(code => code.UtilisateurId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CodeReinitialisationMotDePasseModel>()
                .Property(code => code.DateCreation)
                .HasColumnType("datetime");

            modelBuilder.Entity<CodeReinitialisationMotDePasseModel>()
                .Property(code => code.DateExpiration)
                .HasColumnType("datetime");
        }
    }
}