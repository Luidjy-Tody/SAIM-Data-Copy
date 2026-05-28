using SaimDataCopy.DataProviders.Historique;
using SaimDataCopy.Models.Historique;

namespace SaimDataCopy.Services.Historique
{
    // Service de la page Historique.
    // Il contient la logique métier : filtres, détail et vérification du fichier log.
    public class HistoriqueService : IHistoriqueService
    {
        private readonly IHistoriqueDataProvider historiqueDataProvider;

        // Liste gardée en mémoire pour éviter de recharger à chaque clic.
        private List<HistoriqueExecutionModel> executions = new List<HistoriqueExecutionModel>();

        public HistoriqueService(IHistoriqueDataProvider historiqueDataProvider)
        {
            this.historiqueDataProvider = historiqueDataProvider;
            executions = this.historiqueDataProvider.ChargerExecutions();
        }

        public List<HistoriqueExecutionModel> RechercherExecutions(
            DateTime? dateFiltre,
            string origineFiltre,
            string statutFiltre
        )
        {
            // On recharge les données pour avoir une liste à jour.
            executions = historiqueDataProvider.ChargerExecutions();

            IEnumerable<HistoriqueExecutionModel> resultat = executions;

            // Filtre par date.
            if (dateFiltre != null)
            {
                resultat = resultat.Where(execution =>
                    execution.DateHeureLancement.Date == dateFiltre.Value.Date
                );
            }

            // Filtre par origine : Tous, Manuel, Automatique.
            if (!string.IsNullOrWhiteSpace(origineFiltre) && origineFiltre != "Tous")
            {
                resultat = resultat.Where(execution =>
                    execution.Origine.Equals(origineFiltre, StringComparison.OrdinalIgnoreCase)
                );
            }

            // Filtre par statut : Tous, Succès, Avertissement, Échec.
            if (!string.IsNullOrWhiteSpace(statutFiltre) && statutFiltre != "Tous")
            {
                resultat = resultat.Where(execution =>
                    execution.Statut.Equals(statutFiltre, StringComparison.OrdinalIgnoreCase)
                );
            }

            // On affiche les plus récentes en premier.
            return resultat
                .OrderByDescending(execution => execution.DateHeureLancement)
                .ToList();
        }

        public HistoriqueExecutionModel? ObtenirExecutionParId(int idExecution)
        {
            return executions.FirstOrDefault(execution => execution.Id == idExecution);
        }

        public bool FichierLogExiste(int idExecution)
        {
            HistoriqueExecutionModel? execution = ObtenirExecutionParId(idExecution);

            if (execution == null)
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(execution.CheminFichierLog))
            {
                return false;
            }

            return File.Exists(execution.CheminFichierLog);
        }

        public string ObtenirCheminFichierLog(int idExecution)
        {
            HistoriqueExecutionModel? execution = ObtenirExecutionParId(idExecution);

            if (execution == null)
            {
                return string.Empty;
            }

            return execution.CheminFichierLog;
        }
    }
}