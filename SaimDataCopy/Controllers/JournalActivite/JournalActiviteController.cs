using SaimDataCopy.Models.JournalActivite;
using SaimDataCopy.Services.JournalActivite;
using SaimDataCopy.Views.JournalActivite;
using System.Windows.Forms;

namespace SaimDataCopy.Controllers.JournalActivite
{
    /// <summary>
    /// Controller de la page Journal d'activité.
    /// Il relie la View au Service.
    /// </summary>
    public class JournalActiviteController
    {
        private readonly JournalActiviteView journalActiviteView;
        private readonly JournalActiviteService journalActiviteService;

        public JournalActiviteController(
            JournalActiviteView journalActiviteView,
            JournalActiviteService journalActiviteService)
        {
            this.journalActiviteView = journalActiviteView;
            this.journalActiviteService = journalActiviteService;

            this.journalActiviteView.RechercheDemandee += JournalActiviteView_RechercheDemandee;
            this.journalActiviteView.ActualisationDemandee += JournalActiviteView_ActualisationDemandee;
            this.journalActiviteView.ReinitialisationDemandee += JournalActiviteView_ReinitialisationDemandee;

            _ = ChargerJournalAsync();
        }

        private async void JournalActiviteView_RechercheDemandee(
            object? sender,
            EventArgs e)
        {
            await ChargerJournalAsync();
        }

        private async void JournalActiviteView_ActualisationDemandee(
            object? sender,
            EventArgs e)
        {
            await ChargerJournalAsync();
        }

        private async void JournalActiviteView_ReinitialisationDemandee(
            object? sender,
            EventArgs e)
        {
            await ChargerJournalAsync();
        }

        private async Task ChargerJournalAsync()
        {
            try
            {
                journalActiviteView.AfficherChargement();

                string recherche = journalActiviteView.ObtenirRecherche();
                string action = journalActiviteView.ObtenirActionSelectionnee();

                List<JournalActiviteModel> logs =
                    await journalActiviteService.RecupererJournalActiviteAsync(
                        recherche,
                        action
                    );

                journalActiviteView.AfficherJournal(logs);
            }
            catch (UnauthorizedAccessException exception)
            {
                journalActiviteView.AfficherErreur();

                journalActiviteView.AfficherMessage(
                    exception.Message,
                    "Accès refusé",
                    MessageBoxIcon.Warning
                );
            }
            catch (Exception exception)
            {
                journalActiviteView.AfficherErreur();

                journalActiviteView.AfficherMessage(
                    "Impossible de charger le journal d'activité.\n\n" +
                    exception.Message,
                    "Journal d'activité",
                    MessageBoxIcon.Error
                );
            }
        }
    }
}