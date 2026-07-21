using SaimDataCopy.DataProviders.JournalActivite;
using SaimDataCopy.Models.Authentification;
using SaimDataCopy.Models.JournalActivite;
using SaimDataCopy.Services.Authentification;

namespace SaimDataCopy.Services.JournalActivite
{
    /// <summary>
    /// Service métier du Journal d'activité.
    /// L'accès est réservé aux administrateurs actifs.
    /// </summary>
    public class JournalActiviteService
    {
        private readonly JournalActiviteDataProvider
            journalActiviteDataProvider;

        private readonly SessionUtilisateurService
            sessionUtilisateurService =
                SessionUtilisateurService.Instance;

        public JournalActiviteService(
            JournalActiviteDataProvider journalActiviteDataProvider)
        {
            this.journalActiviteDataProvider =
                journalActiviteDataProvider;
        }

        public async Task<List<JournalActiviteModel>>
            RecupererJournalActiviteAsync(
                string recherche,
                string action)
        {
            VerifierAccesAdministrateur();

            recherche = recherche.Trim();
            action = action.Trim();

            return await journalActiviteDataProvider
                .RecupererJournalActiviteAsync(
                    recherche,
                    action
                );
        }

        public async Task<List<string>>
            RecupererTypesActionsAsync()
        {
            VerifierAccesAdministrateur();

            return await journalActiviteDataProvider
                .RecupererTypesActionsAsync();
        }

        public async Task<int> CompterLogsAsync()
        {
            VerifierAccesAdministrateur();

            return await journalActiviteDataProvider
                .CompterLogsAsync();
        }

        private void VerifierAccesAdministrateur()
        {
            UtilisateurModel? utilisateurConnecte =
                sessionUtilisateurService.UtilisateurConnecte;

            bool estAdministrateurActif =
                utilisateurConnecte != null &&
                utilisateurConnecte.EstActif &&
                utilisateurConnecte.Statut.Equals(
                    "Admin",
                    StringComparison.OrdinalIgnoreCase
                );

            if (!estAdministrateurActif)
            {
                throw new UnauthorizedAccessException(
                    "Le Journal d'activité est réservé aux administrateurs actifs."
                );
            }
        }
    }
}