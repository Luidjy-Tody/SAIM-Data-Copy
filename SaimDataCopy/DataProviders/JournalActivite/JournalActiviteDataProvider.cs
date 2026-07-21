using Microsoft.EntityFrameworkCore;
using SaimDataCopy.DataAccess;
using SaimDataCopy.Helpers;
using SaimDataCopy.Models.Authentification;
using SaimDataCopy.Models.JournalActivite;

namespace SaimDataCopy.DataProviders.JournalActivite
{
    /// <summary>
    /// Récupère les actions enregistrées dans la table Log.
    /// </summary>
    public class JournalActiviteDataProvider
    {
        private static AuthentificationDbContext CreerContext()
        {
            string chaineConnexion =
                AuthentificationConnexionHelper
                    .ObtenirChaineConnexionBaseAuthentification();

            DbContextOptions<AuthentificationDbContext> options =
                new DbContextOptionsBuilder<AuthentificationDbContext>()
                    .UseMySql(
                        chaineConnexion,
                        AuthentificationConnexionHelper
                            .ObtenirVersionServeurMySql()
                    )
                    .Options;

            return new AuthentificationDbContext(options);
        }

        public async Task<List<JournalActiviteModel>>
            RecupererJournalActiviteAsync(
                string recherche,
                string action)
        {
            using AuthentificationDbContext context = CreerContext();

            IQueryable<LogUtilisateurModel> requete =
                context.LogsUtilisateurs.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(recherche))
            {
                string valeurRecherchee =
                    recherche.Trim().ToLower();

                requete = requete.Where(log =>
                    log.NomUtilisateur
                        .ToLower()
                        .Contains(valeurRecherchee) ||

                    log.Action
                        .ToLower()
                        .Contains(valeurRecherchee) ||

                    log.Details
                        .ToLower()
                        .Contains(valeurRecherchee)
                );
            }

            if (!string.IsNullOrWhiteSpace(action) &&
                action != "Toutes les actions")
            {
                requete = requete.Where(log =>
                    log.Action == action
                );
            }

            return await requete
                .OrderByDescending(log => log.DateHeure)
                .ThenByDescending(log => log.Id)
                .Select(log => new JournalActiviteModel
                {
                    Id = log.Id,
                    UtilisateurId = log.UtilisateurId,
                    NomUtilisateur = log.NomUtilisateur,
                    Action = log.Action,
                    Details = log.Details,
                    DateHeure = log.DateHeure
                })
                .ToListAsync();
        }

        public async Task<List<string>>
            RecupererTypesActionsAsync()
        {
            using AuthentificationDbContext context = CreerContext();

            return await context.LogsUtilisateurs
                .AsNoTracking()
                .Where(log =>
                    log.Action != null &&
                    log.Action != string.Empty
                )
                .Select(log => log.Action)
                .Distinct()
                .OrderBy(action => action)
                .ToListAsync();
        }

        public async Task<int> CompterLogsAsync()
        {
            using AuthentificationDbContext context = CreerContext();

            return await context.LogsUtilisateurs.CountAsync();
        }
    }
}