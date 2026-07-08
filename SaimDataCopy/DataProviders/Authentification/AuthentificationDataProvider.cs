using Microsoft.EntityFrameworkCore;
using SaimDataCopy.DataAccess;
using SaimDataCopy.Models.Authentification;

namespace SaimDataCopy.DataProviders.Authentification
{
    public class AuthentificationDataProvider : IAuthentificationDataProvider
    {
        private static AuthentificationDbContext CreerContext()
        {
            string chaineConnexion = "server=localhost;port=3306;database=saimdatacopy_auth;user=root;password=;";

            DbContextOptions<AuthentificationDbContext> options = new DbContextOptionsBuilder<AuthentificationDbContext>()
                .UseMySql(
                    chaineConnexion,
                    ServerVersion.AutoDetect(chaineConnexion)
                )
                .Options;

            return new AuthentificationDbContext(options);
        }

        public async Task<UtilisateurModel?> RecupererUtilisateurParIdentifiantAsync(string identifiant)
        {
            using AuthentificationDbContext context = CreerContext();

            return await context.Utilisateurs
                .FirstOrDefaultAsync(u => u.Identifiant == identifiant);
        }

        public async Task<UtilisateurModel?> RecupererUtilisateurParIdentifiantOuEmailAsync(string identifiantOuEmail)
        {
            if (string.IsNullOrWhiteSpace(identifiantOuEmail))
            {
                return null;
            }

            string valeurRecherchee = identifiantOuEmail.Trim().ToLower();

            using AuthentificationDbContext context = CreerContext();

            return await context.Utilisateurs
                .FirstOrDefaultAsync(utilisateur =>
                    utilisateur.Identifiant.ToLower() == valeurRecherchee ||
                    utilisateur.Email.ToLower() == valeurRecherchee
                );
        }

        public async Task<bool> ExisteAuMoinsUnUtilisateurAsync()
        {
            using AuthentificationDbContext context = CreerContext();

            return await context.Utilisateurs.AnyAsync();
        }

        public async Task<UtilisateurModel?> RecupererUtilisateurParEmailAsync(string email)
        {
            using AuthentificationDbContext context = CreerContext();

            return await context.Utilisateurs
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task AjouterUtilisateurAsync(UtilisateurModel utilisateur)
        {
            using AuthentificationDbContext context = CreerContext();

            context.Utilisateurs.Add(utilisateur);
            await context.SaveChangesAsync();
        }

        public async Task ModifierUtilisateurAsync(UtilisateurModel utilisateur)
        {
            using AuthentificationDbContext context = CreerContext();

            context.Utilisateurs.Update(utilisateur);
            await context.SaveChangesAsync();
        }

        public async Task AjouterLogAsync(LogUtilisateurModel log)
        {
            using AuthentificationDbContext context = CreerContext();

            context.LogsUtilisateurs.Add(log);
            await context.SaveChangesAsync();
        }
    }
}