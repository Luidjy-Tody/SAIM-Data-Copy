using Microsoft.EntityFrameworkCore;
using SaimDataCopy.DataAccess;
using SaimDataCopy.Models.Authentification;
using SaimDataCopy.Helpers;
namespace SaimDataCopy.DataProviders.Authentification
{
    public class AuthentificationDataProvider : IAuthentificationDataProvider
    {
        private static AuthentificationDbContext CreerContext()
        {
            string chaineConnexion = AuthentificationConnexionHelper.ObtenirChaineConnexionBaseAuthentification();

            DbContextOptions<AuthentificationDbContext> options = new DbContextOptionsBuilder<AuthentificationDbContext>()
                .UseMySql(
                    chaineConnexion,
                    AuthentificationConnexionHelper.ObtenirVersionServeurMySql()
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

        public async Task AjouterCodeReinitialisationAsync(CodeReinitialisationMotDePasseModel code)
        {
            using AuthentificationDbContext context = CreerContext();

            await context.CodesReinitialisationMotDePasse.AddAsync(code);

            await context.SaveChangesAsync();
        }

        public async Task<CodeReinitialisationMotDePasseModel?> RecupererDernierCodeValideAsync(int utilisateurId)
        {
            using AuthentificationDbContext context = CreerContext();

            return await context.CodesReinitialisationMotDePasse
                .Where(code =>
                    code.UtilisateurId == utilisateurId &&
                    !code.EstUtilise &&
                    code.DateExpiration > DateTime.Now)
                .OrderByDescending(code => code.DateCreation)
                .FirstOrDefaultAsync();
        }

        public async Task MarquerCodesUtilisateurCommeUtilisesAsync(int utilisateurId)
        {
            using AuthentificationDbContext context = CreerContext();

            List<CodeReinitialisationMotDePasseModel> codes =
                await context.CodesReinitialisationMotDePasse
                    .Where(code =>
                        code.UtilisateurId == utilisateurId &&
                        !code.EstUtilise)
                    .ToListAsync();

            foreach (CodeReinitialisationMotDePasseModel code in codes)
            {
                code.EstUtilise = true;
            }

            await context.SaveChangesAsync();
        }
    }
}