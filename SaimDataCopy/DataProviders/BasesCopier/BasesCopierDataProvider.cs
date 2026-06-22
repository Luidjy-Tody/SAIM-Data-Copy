using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SaimDataCopy.DataAccess;
using SaimDataCopy.DataProviders.Configuration;
using SaimDataCopy.Models.BasesCopier;
using SaimDataCopy.Models.Configuration;
using SaimDataCopy.Helpers;

namespace SaimDataCopy.DataProviders.BasesCopier
{
    // DataProvider pour la page Bases à copier.
    // Il lit les bases depuis le serveur source
    // et sauvegarde les choix de l'utilisateur dans un fichier JSON.
    public class BasesCopierDataProvider : IBasesCopierDataProvider
    {
        private readonly string _cheminFichierBases;
        private readonly IConfigurationDataProvider _configurationDataProvider;

        public BasesCopierDataProvider()
            : this(new ConfigurationDataProvider())
        {
        }

        public BasesCopierDataProvider(IConfigurationDataProvider configurationDataProvider)
        {
            _configurationDataProvider = configurationDataProvider;

            // Le fichier JSON sera stocké dans le dossier Data de l'application.
            string dossierData = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");

            if (!Directory.Exists(dossierData))
            {
                Directory.CreateDirectory(dossierData);
            }

            _cheminFichierBases = Path.Combine(dossierData, "bases_copier.json");
        }

        public List<BaseCopieModel> ChargerBasesDepuisServeurSource()
        {
            // On récupère la configuration depuis configuration_execution.json.
            ConfigurationModel? configuration = _configurationDataProvider.ChargerConfiguration();

            if (configuration == null)
            {
                return new List<BaseCopieModel>();
            }

            string chaineConnexionSource = ChaineConnexionHelper.ConstruireChaineConnexionSource(configuration);

            DbContextOptions<SaimDbContext> optionsSource = new DbContextOptionsBuilder<SaimDbContext>()
                    .UseSqlServer(chaineConnexionSource)
                    .Options;

            // Ici, on crée un DbContext connecté au serveur source.
            using SaimDbContext dbContextSource = new SaimDbContext(optionsSource);

            // Cette requête lit les bases disponibles sur le serveur source.
            // On ignore les bases système, la base interne de l'application,
            // et les anciennes bases temporaires qui commencent par CIBLE_.
            List<string> nomsBases = dbContextSource.Database
                .SqlQueryRaw<string>(
                    """
                    SELECT name AS Value
                    FROM sys.databases
                    WHERE name NOT IN ('master', 'model', 'msdb', 'tempdb', 'SaimDataCopyDb')
                    AND name NOT LIKE 'CIBLE[_]%'
                    ORDER BY name
                    """
                )
                .ToList();

            List<BaseCopieModel> bases = new List<BaseCopieModel>();

            int ordre = 1;

            foreach (string nomBase in nomsBases)
            {
                bases.Add(new BaseCopieModel
                {
                    Inclure = true,
                    NomBase = nomBase,
                    OrdreTraitement = ordre,
                    ModeCopie = "Écraser",
                    Statut = "Prête",
                    DerniereCopie = null,
                    ExisteSurServeurSource = true,
                    NomModifiable = false
                });

                ordre++;
            }

            return bases;
        }

        public List<BaseCopieModel> ChargerBasesSauvegardees()
        {
            List<BaseCopieModel> basesServeur = ChargerBasesDepuisServeurSource();

            // On charge les choix sauvegardés dans bases_copier.json.
            List<BaseCopieModel> basesJson = ChargerBasesJson();

            // Si aucune sauvegarde JSON n'existe encore,
            // on retourne les bases du serveur source cochées par défaut.
            if (basesJson.Count == 0)
            {
                return basesServeur;
            }

            // On fusionne les vraies bases du serveur source
            // avec les choix sauvegardés dans le fichier JSON.
            return FusionnerBasesServeurEtSauvegarde(
                basesServeur,
                basesJson
            );
        }

        public void EnregistrerBases(List<BaseCopieModel> bases)
        {
            // Sauvegarde uniquement dans le fichier JSON.
            EnregistrerBasesJson(bases);
        }

        public void AppliquerModeCopieGlobal(string modeCopieGlobal)
        {
            List<BaseCopieModel> bases = ChargerBasesSauvegardees();

            if (bases.Count == 0)
            {
                bases = ChargerBasesDepuisServeurSource();
            }

            foreach (BaseCopieModel baseCopie in bases)
            {
                baseCopie.ModeCopie = NormaliserModeCopie(modeCopieGlobal);
            }

            EnregistrerBases(bases);
        }

        private void EnregistrerBasesJson(List<BaseCopieModel> bases)
        {
            List<BaseCopieModel> basesNormalisees = bases
                .Select(NormaliserBaseCopieModel)
                .OrderBy(b => b.OrdreTraitement)
                .ToList();

            string contenuJson = JsonConvert.SerializeObject(
                basesNormalisees,
                Formatting.Indented
            );

            File.WriteAllText(_cheminFichierBases, contenuJson);
        }

        private List<BaseCopieModel> ChargerBasesJson()
        {
            if (!File.Exists(_cheminFichierBases))
            {
                return new List<BaseCopieModel>();
            }

            string contenuJson = File.ReadAllText(_cheminFichierBases);

            if (string.IsNullOrWhiteSpace(contenuJson))
            {
                return new List<BaseCopieModel>();
            }

            try
            {
                List<BaseCopieModel>? bases =
                    JsonConvert.DeserializeObject<List<BaseCopieModel>>(contenuJson);

                if (bases == null)
                {
                    return new List<BaseCopieModel>();
                }

                return bases
                    .Select(NormaliserBaseCopieModel)
                    .OrderBy(b => b.OrdreTraitement)
                    .ToList();
            }
            catch (JsonException)
            {
                throw new InvalidOperationException(
                    "Le fichier bases_copier.json est invalide. Vérifiez son contenu JSON.");
            }
        }

        private BaseCopieModel NormaliserBaseCopieModel(BaseCopieModel baseCopie)
        {
            return new BaseCopieModel
            {
                Inclure = baseCopie.Inclure,
                NomBase = baseCopie.NomBase.Trim(),
                OrdreTraitement = baseCopie.OrdreTraitement,
                ModeCopie = NormaliserModeCopie(baseCopie.ModeCopie),
                Statut = baseCopie.Statut,
                DerniereCopie = baseCopie.DerniereCopie,
                ExisteSurServeurSource = baseCopie.ExisteSurServeurSource,
                NomModifiable = false
            };
        }

        private List<BaseCopieModel> FusionnerBasesServeurEtSauvegarde(
            List<BaseCopieModel> basesServeur,
            List<BaseCopieModel> basesSauvegardees)
        {
            List<BaseCopieModel> resultat = new List<BaseCopieModel>();

            foreach (BaseCopieModel baseServeur in basesServeur)
            {
                BaseCopieModel? baseSauvegardee = basesSauvegardees
                    .FirstOrDefault(b =>
                        b.NomBase.Equals(
                            baseServeur.NomBase,
                            StringComparison.OrdinalIgnoreCase
                        )
                    );

                if (baseSauvegardee == null)
                {
                    // Nouvelle base trouvée sur le serveur source.
                    // Si un fichier JSON existe déjà, on ne doit pas cocher automatiquement
                    // une nouvelle base, sinon l'application ne respecte plus la sauvegarde.
                    resultat.Add(new BaseCopieModel
                    {
                        Inclure = false,
                        NomBase = baseServeur.NomBase,
                        OrdreTraitement = baseServeur.OrdreTraitement,
                        ModeCopie = NormaliserModeCopie(baseServeur.ModeCopie),
                        Statut = "Non sélectionnée",
                        DerniereCopie = null,
                        ExisteSurServeurSource = true,
                        NomModifiable = false
                    });

                    continue;
                }

                // La base existe déjà dans le fichier JSON.
                // On garde les choix de l'utilisateur.
                resultat.Add(new BaseCopieModel
                {
                    Inclure = baseSauvegardee.Inclure,
                    NomBase = baseServeur.NomBase,
                    OrdreTraitement = baseSauvegardee.OrdreTraitement,
                    ModeCopie = NormaliserModeCopie(baseSauvegardee.ModeCopie),
                    Statut = baseSauvegardee.Inclure
                        ? baseServeur.Statut
                        : "Non sélectionnée",
                    DerniereCopie = baseSauvegardee.DerniereCopie,
                    ExisteSurServeurSource = true,
                    NomModifiable = false
                });
            }

            return resultat
                .OrderBy(b => b.OrdreTraitement)
                .ToList();
        }

        private string NormaliserModeCopie(string modeCopie)
        {
            return modeCopie switch
            {
                "Mettre à jour" => "Mise à jour",
                "Mise à jour" => "Mise à jour",
                "Écraser" => "Écraser",
                _ => "Écraser"
            };
        }
    }
}