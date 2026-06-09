using MySqlConnector;
using Newtonsoft.Json;
using SaimDataCopy.DataProviders.Configuration;
using SaimDataCopy.Helpers;
using SaimDataCopy.Models.BasesCopier;
using SaimDataCopy.Models.Configuration;

namespace SaimDataCopy.DataProviders.BasesCopier
{
    // DataProvider MySQL pour la page Bases à copier.
    // Il lit les bases MySQL depuis le serveur source.
    public class MySqlBasesCopierDataProvider : IBasesCopierDataProvider
    {
        private readonly string _cheminFichierBases;
        private readonly IConfigurationDataProvider _configurationDataProvider;

        public MySqlBasesCopierDataProvider()
            : this(new ConfigurationDataProvider())
        {
        }

        public MySqlBasesCopierDataProvider(IConfigurationDataProvider configurationDataProvider)
        {
            _configurationDataProvider = configurationDataProvider;

            string dossierData = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");

            if (!Directory.Exists(dossierData))
            {
                Directory.CreateDirectory(dossierData);
            }

            _cheminFichierBases = Path.Combine(dossierData, "bases_copier.json");
        }

        public List<BaseCopieModel> ChargerBasesDepuisServeurSource()
        {
            ConfigurationModel? configuration = _configurationDataProvider.ChargerConfiguration();

            if (configuration == null)
            {
                return new List<BaseCopieModel>();
            }

            string chaineConnexionSource =
                ChaineConnexionHelper.ConstruireChaineConnexionSource(configuration);

            List<BaseCopieModel> bases = new List<BaseCopieModel>();

            using MySqlConnection connexion = new MySqlConnection(chaineConnexionSource);
            connexion.Open();

            using MySqlCommand commande = connexion.CreateCommand();

            // Cette requête lit les bases MySQL disponibles.
            // On ignore les bases système de MySQL.
            commande.CommandText =
                """
                SELECT SCHEMA_NAME
                FROM INFORMATION_SCHEMA.SCHEMATA
                WHERE SCHEMA_NAME NOT IN (
                    'information_schema',
                    'mysql',
                    'performance_schema',
                    'sys'
                )
                AND SCHEMA_NAME NOT LIKE '%_cible%'
                ORDER BY SCHEMA_NAME;
                """;

            using MySqlDataReader lecteur = commande.ExecuteReader();

            int ordre = 1;

            while (lecteur.Read())
            {
                string nomBase = lecteur.GetString("SCHEMA_NAME");

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
            List<BaseCopieModel> basesJson = ChargerBasesJson();

            if (basesJson.Count == 0)
            {
                return basesServeur;
            }

            return FusionnerBasesServeurEtSauvegarde(basesServeur, basesJson);
        }

        public void EnregistrerBases(List<BaseCopieModel> bases)
        {
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
                    resultat.Add(baseServeur);
                    continue;
                }

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