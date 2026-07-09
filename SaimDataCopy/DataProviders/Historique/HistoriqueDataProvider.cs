using Newtonsoft.Json;
using SaimDataCopy.Helpers;
using SaimDataCopy.Models.Historique;

namespace SaimDataCopy.DataProviders.Historique
{
    // DataProvider de la page Historique.
    // Il sauvegarde et charge les exécutions depuis un fichier JSON.
    public class HistoriqueDataProvider : IHistoriqueDataProvider
    {
        private readonly string _cheminFichierHistorique;

        public HistoriqueDataProvider()
        {
            string dossierData = CheminApplicationHelper.ObtenirDossierData();

            _cheminFichierHistorique = Path.Combine(dossierData, "historique_executions.json");
        }

        public List<HistoriqueExecutionModel> ChargerExecutions()
        {
            if (!File.Exists(_cheminFichierHistorique))
            {
                return new List<HistoriqueExecutionModel>();
            }

            string contenuJson = File.ReadAllText(_cheminFichierHistorique);

            if (string.IsNullOrWhiteSpace(contenuJson))
            {
                return new List<HistoriqueExecutionModel>();
            }

            try
            {
                List<HistoriqueExecutionModel>? executions =
                    JsonConvert.DeserializeObject<List<HistoriqueExecutionModel>>(contenuJson);

                if (executions == null)
                {
                    return new List<HistoriqueExecutionModel>();
                }

                return executions
                    .OrderByDescending(execution => execution.DateHeureLancement)
                    .ToList();
            }
            catch
            {
                // Si le JSON est cassé, on retourne une liste vide
                // pour éviter de bloquer l'application.
                return new List<HistoriqueExecutionModel>();
            }
        }

        public void EnregistrerExecution(HistoriqueExecutionModel execution)
        {
            List<HistoriqueExecutionModel> executions = ChargerExecutions();

            execution.Id = GenererNouvelId(executions);

            executions.Add(execution);

            List<HistoriqueExecutionModel> executionsTriees = executions
                .OrderByDescending(e => e.DateHeureLancement)
                .ToList();

            string contenuJson = JsonConvert.SerializeObject(
                executionsTriees,
                Formatting.Indented
            );

            File.WriteAllText(_cheminFichierHistorique, contenuJson);
        }

        private int GenererNouvelId(List<HistoriqueExecutionModel> executions)
        {
            if (executions.Count == 0)
            {
                return 1;
            }

            return executions.Max(execution => execution.Id) + 1;
        }
    }
}