using Newtonsoft.Json;
using SaimDataCopy.DataProviders.BasesCopier;
using SaimDataCopy.Models.BasesCopier;
using SaimDataCopy.Models.Execution;

namespace SaimDataCopy.DataProviders.Execution
{
    // DataProvider de la page Exécution.
    // Il s'occupe seulement de charger et enregistrer les données.
    public class ExecutionDataProvider : IExecutionDataProvider
    {
        private readonly IBasesCopierDataProvider _basesCopierDataProvider;
        private readonly string _cheminFichier;

        public ExecutionDataProvider()
            : this(new BasesCopierDataProvider())
        {
        }

        public ExecutionDataProvider(IBasesCopierDataProvider basesCopierDataProvider)
        {
            _basesCopierDataProvider = basesCopierDataProvider;

            // Le fichier JSON sera stocké dans le dossier Data.
            string dossierData = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");

            if (!Directory.Exists(dossierData))
            {
                Directory.CreateDirectory(dossierData);
            }

            _cheminFichier = Path.Combine(dossierData, "execution_derniere.json");
        }

        public List<BaseCopieModel> ChargerBasesSelectionnees()
        {
            // On récupère les bases sauvegardées dans la page Bases à copier.
            // Cela respecte le dernier état enregistré par l'utilisateur.
            return _basesCopierDataProvider
                .ChargerBasesSauvegardees()
                .Where(b => b.Inclure)
                .OrderBy(b => b.OrdreTraitement)
                .ToList();
        }

        public ExecutionTableauBordModel ChargerDernierTableauBord()
        {
            ExecutionSauvegardeModel sauvegarde = ChargerSauvegarde();

            int nombreBasesSelectionnees = ChargerBasesSelectionnees().Count;

            if (sauvegarde.TableauBord == null)
            {
                return new ExecutionTableauBordModel
                {
                    NombreBasesSelectionnees = nombreBasesSelectionnees,
                    NombreLignesCopiees = 0,
                    DureeDerniereExecution = "-"
                };
            }

            // On garde le nombre de bases sélectionnées actuel.
            // Les lignes copiées et la durée viennent de la dernière exécution.
            sauvegarde.TableauBord.NombreBasesSelectionnees = nombreBasesSelectionnees;

            return sauvegarde.TableauBord;
        }

        public List<ExecutionResultatBaseModel> ChargerDerniersResultats()
        {
            ExecutionSauvegardeModel sauvegarde = ChargerSauvegarde();

            return sauvegarde.Resultats ?? new List<ExecutionResultatBaseModel>();
        }

        public void EnregistrerDerniereExecution(
            ExecutionTableauBordModel tableauBord,
            List<ExecutionResultatBaseModel> resultats)
        {
            ExecutionSauvegardeModel sauvegarde = new ExecutionSauvegardeModel
            {
                TableauBord = tableauBord,
                Resultats = resultats
            };

            string contenuJson = JsonConvert.SerializeObject(sauvegarde, Formatting.Indented);

            File.WriteAllText(_cheminFichier, contenuJson);
        }

        private ExecutionSauvegardeModel ChargerSauvegarde()
        {
            if (!File.Exists(_cheminFichier))
            {
                return new ExecutionSauvegardeModel();
            }

            string contenuJson = File.ReadAllText(_cheminFichier);

            if (string.IsNullOrWhiteSpace(contenuJson))
            {
                return new ExecutionSauvegardeModel();
            }

            ExecutionSauvegardeModel? sauvegarde =
                JsonConvert.DeserializeObject<ExecutionSauvegardeModel>(contenuJson);

            return sauvegarde ?? new ExecutionSauvegardeModel();
        }

        // Modèle privé utilisé seulement pour sauvegarder le JSON.
        private class ExecutionSauvegardeModel
        {
            public ExecutionTableauBordModel? TableauBord { get; set; }

            public List<ExecutionResultatBaseModel> Resultats { get; set; } =
                new List<ExecutionResultatBaseModel>();
        }
    }
}