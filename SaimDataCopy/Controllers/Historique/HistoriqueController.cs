using SaimDataCopy.Models.Historique;
using SaimDataCopy.Services.Historique;
using SaimDataCopy.Views.Historique;
using System.Diagnostics;
using System.Windows.Forms;

namespace SaimDataCopy.Controllers.Historique
{
    // Controller de la page Historique.
    // Il coordonne les actions entre la View et le Service.
    public class HistoriqueController
    {
        private readonly IHistoriqueView historiqueView;
        private readonly IHistoriqueService historiqueService;

        public HistoriqueController(
            IHistoriqueView historiqueView,
            IHistoriqueService historiqueService
        )
        {
            this.historiqueView = historiqueView;
            this.historiqueService = historiqueService;

            // Quand l'utilisateur clique sur Rechercher.
            this.historiqueView.RechercheDemandee += RechercherExecutions;

            // Quand l'utilisateur clique sur Voir détail.
            this.historiqueView.DetailDemande += AfficherDetailExecution;

            // Quand l'utilisateur clique sur Ouvrir le fichier log.
            this.historiqueView.OuvrirLogDemande += OuvrirFichierLog;

            // Chargement initial de la page.
            ChargerPage();
        }

        // Charge l'historique au démarrage de la page.
        public void ChargerPage()
        {
            RechercherExecutions(this, EventArgs.Empty);
        }

        private void RechercherExecutions(object? sender, EventArgs e)
        {
            DateTime? dateFiltre = historiqueView.ObtenirDateFiltre();
            string origineFiltre = historiqueView.ObtenirOrigineFiltre();
            string statutFiltre = historiqueView.ObtenirStatutFiltre();

            List<HistoriqueExecutionModel> executions =
                historiqueService.RechercherExecutions(
                    dateFiltre,
                    origineFiltre,
                    statutFiltre
                );

            historiqueView.AfficherExecutions(executions);
        }

        private void AfficherDetailExecution(int idExecution)
        {
            HistoriqueExecutionModel? execution =
                historiqueService.ObtenirExecutionParId(idExecution);

            if (execution == null)
            {
                historiqueView.AfficherMessage(
                    "Impossible de trouver le détail de cette exécution.",
                    "Historique",
                    MessageBoxIcon.Warning
                );

                return;
            }

            historiqueView.AfficherDetail(execution);
        }

        private void OuvrirFichierLog(int idExecution)
        {
            bool fichierExiste = historiqueService.FichierLogExiste(idExecution);

            if (!fichierExiste)
            {
                historiqueView.AfficherMessage(
                    "Le fichier de log est introuvable.",
                    "Fichier log",
                    MessageBoxIcon.Warning
                );

                return;
            }

            string cheminFichierLog = historiqueService.ObtenirCheminFichierLog(idExecution);

            try
            {
                // UseShellExecute = true permet d'ouvrir le fichier
                // avec l'application par défaut de Windows.
                ProcessStartInfo processStartInfo = new ProcessStartInfo
                {
                    FileName = cheminFichierLog,
                    UseShellExecute = true
                };

                Process.Start(processStartInfo);
            }
            catch
            {
                historiqueView.AfficherMessage(
                    "Impossible d'ouvrir le fichier de log.",
                    "Fichier log",
                    MessageBoxIcon.Error
                );
            }
        }
    }
}