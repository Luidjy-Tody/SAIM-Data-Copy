using Microsoft.EntityFrameworkCore;
using SaimDataCopy.DataAccess;
using SaimDataCopy.Helpers;
using SaimDataCopy.Models.EspaceAdmin;
using SaimDataCopy.Models.Authentification;

namespace SaimDataCopy.DataProviders.EspaceAdmin
{
    /// <summary>
    /// DataProvider de l'Espace Admin.
    /// Il récupère les données depuis la base d'authentification.
    /// </summary>
    public class EspaceAdminDataProvider
    {
        private static AuthentificationDbContext CreerContext()
        {
            string chaineConnexion = AuthentificationConnexionHelper.ObtenirChaineConnexionBaseAuthentification();

            DbContextOptions<AuthentificationDbContext> options = new DbContextOptionsBuilder<AuthentificationDbContext>()
                    .UseMySql(
                        chaineConnexion, AuthentificationConnexionHelper.ObtenirVersionServeurMySql()
                    )
                    .Options;

            return new AuthentificationDbContext(options);
        }

        public async Task<StatistiquesUtilisateursModel> RecupererStatistiquesUtilisateursAsync()
        {
            using AuthentificationDbContext context = CreerContext();

            int totalUtilisateurs = await context.Utilisateurs.CountAsync();

            int comptesActifs = await context.Utilisateurs.CountAsync(utilisateur => utilisateur.EstActif);

            int comptesInactifs = await context.Utilisateurs.CountAsync(utilisateur => !utilisateur.EstActif);

            int administrateurs = await context.Utilisateurs.CountAsync(utilisateur => utilisateur.Statut == "Admin");

            return new StatistiquesUtilisateursModel
            {
                TotalUtilisateurs = totalUtilisateurs,
                ComptesActifs = comptesActifs,
                ComptesInactifs = comptesInactifs,
                Administrateurs = administrateurs
            };
        }

        public async Task<List<UtilisateurModel>> RecupererUtilisateursAsync(string recherche, string statut, string etat)
        {
            using AuthentificationDbContext context = CreerContext();

            IQueryable<UtilisateurModel> requete = context.Utilisateurs.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(recherche))
            {
                string valeurRecherchee = recherche.Trim().ToLower();

                requete = requete.Where(utilisateur =>
                    utilisateur.NomComplet.ToLower().Contains(valeurRecherchee) ||
                    utilisateur.Identifiant.ToLower().Contains(valeurRecherchee) ||
                    utilisateur.Email.ToLower().Contains(valeurRecherchee)
                );
            }

            if (!string.IsNullOrWhiteSpace(statut) && statut != "Tous")
            {
                requete = requete.Where(utilisateur => utilisateur.Statut == statut);
            }

            if (etat == "Actif")
            {
                requete = requete.Where(utilisateur => utilisateur.EstActif);
            }
            else if (etat == "Inactif")
            {
                requete = requete.Where(utilisateur => !utilisateur.EstActif);
            }

            return await requete
                .OrderBy(utilisateur => utilisateur.NomComplet)
                .ThenBy(utilisateur => utilisateur.Identifiant)
                .ToListAsync();
        }

        public async Task ActiverDesactiverUtilisateurAsync(int idUtilisateur, bool estActif)
        {
            using AuthentificationDbContext context = CreerContext();

            UtilisateurModel? utilisateur = await context.Utilisateurs
                .FirstOrDefaultAsync(utilisateur => utilisateur.Id == idUtilisateur);

            if (utilisateur == null)
            {
                return;
            }

            utilisateur.EstActif = estActif;

            await context.SaveChangesAsync();
        }

        public async Task SupprimerUtilisateurAsync(int idUtilisateur)
        {
            using AuthentificationDbContext context = CreerContext();

            UtilisateurModel? utilisateur = await context.Utilisateurs
                .FirstOrDefaultAsync(utilisateur => utilisateur.Id == idUtilisateur);

            if (utilisateur == null)
            {
                return;
            }

            context.Utilisateurs.Remove(utilisateur);

            await context.SaveChangesAsync();
        }

        public async Task<UtilisateurModel?> RecupererUtilisateurParIdAsync(int idUtilisateur)
        {
            using AuthentificationDbContext context = CreerContext();

            return await context.Utilisateurs
                .AsNoTracking()
                .FirstOrDefaultAsync(utilisateur => utilisateur.Id == idUtilisateur);
        }


        public async Task<bool> IdentifiantExisteAsync(string identifiant, int? idUtilisateurExclu)
        {
            using AuthentificationDbContext context = CreerContext();

            IQueryable<UtilisateurModel> requete = context.Utilisateurs.AsNoTracking();

            requete = requete.Where(utilisateur => utilisateur.Identifiant.ToLower() == identifiant.ToLower());

            if (idUtilisateurExclu.HasValue)
            {
                requete = requete.Where(utilisateur => utilisateur.Id != idUtilisateurExclu.Value);
            }

            return await requete.AnyAsync();
        }


        public async Task<bool> EmailExisteAsync(string email, int? idUtilisateurExclu)
        {
            using AuthentificationDbContext context = CreerContext();

            IQueryable<UtilisateurModel> requete = context.Utilisateurs.AsNoTracking();

            requete = requete.Where(utilisateur => utilisateur.Email.ToLower() == email.ToLower());

            if (idUtilisateurExclu.HasValue)
            {
                requete = requete.Where(utilisateur => utilisateur.Id != idUtilisateurExclu.Value);
            }

            return await requete.AnyAsync();
        }


        public async Task AjouterUtilisateurAsync(UtilisateurModel utilisateur)
        {
            using AuthentificationDbContext context = CreerContext();

            await context.Utilisateurs.AddAsync(utilisateur);
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

            await context.LogsUtilisateurs.AddAsync(log);
            await context.SaveChangesAsync();
        }

        public async Task<int> CompterAdministrateursActifsAsync()
        {
            using AuthentificationDbContext context = CreerContext();

            return await context.Utilisateurs.CountAsync(utilisateur =>
                utilisateur.EstActif &&
                utilisateur.Statut == "Admin"
            );
        }
    }


}