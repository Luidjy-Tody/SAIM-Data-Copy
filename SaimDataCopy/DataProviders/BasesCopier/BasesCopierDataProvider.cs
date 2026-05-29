using Newtonsoft.Json;
using SaimDataCopy.Models.BasesCopier;

namespace SaimDataCopy.DataProviders.BasesCopier
{
    // DataProvider pour la page Bases à copier.
    // Il s'occupe seulement de charger et sauvegarder les données.
    public class BasesCopierDataProvider : IBasesCopierDataProvider
    {
        private readonly string _cheminFichier;

        public BasesCopierDataProvider()
        {
            // Le fichier JSON sera stocké dans le dossier Data.
            string dossierData = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "Data"
            );

            if (!Directory.Exists(dossierData))
            {
                Directory.CreateDirectory(dossierData);
            }

            _cheminFichier = Path.Combine(dossierData, "bases_copier_config.json");
        }

        public List<BaseCopieModel> ChargerBasesDepuisServeurSource()
        {
            // Pour l'instant, on simule les bases trouvées sur le serveur source.
            // Plus tard, cette méthode sera remplacée par une vraie lecture SQL Server / EF Core.
            return new List<BaseCopieModel>
            {
                new BaseCopieModel
                {
                    Inclure = true,
                    NomBase = "DB_Ventes",
                    OrdreTraitement = 1,
                    ModeCopie = "Écraser",
                    Statut = "Prête",
                    DerniereCopie = new DateTime(2025, 5, 14, 2, 0, 0),
                    ExisteSurServeurSource = true,
                    NomModifiable = false
                },

                new BaseCopieModel
                {
                    Inclure = true,
                    NomBase = "DB_RH",
                    OrdreTraitement = 2,
                    ModeCopie = "Écraser",
                    Statut = "Prête",
                    DerniereCopie = new DateTime(2025, 5, 14, 2, 3, 0),
                    ExisteSurServeurSource = true,
                    NomModifiable = false
                },

                new BaseCopieModel
                {
                    Inclure = true,
                    NomBase = "DB_Comptabilite",
                    OrdreTraitement = 3,
                    ModeCopie = "Mise à jour",
                    Statut = "Avertissement",
                    DerniereCopie = new DateTime(2025, 5, 14, 2, 6, 0),
                    ExisteSurServeurSource = true,
                    NomModifiable = false
                },

                new BaseCopieModel
                {
                    Inclure = true,
                    NomBase = "DB_Archive",
                    OrdreTraitement = 4,
                    ModeCopie = "Écraser",
                    Statut = "Prête",
                    DerniereCopie = null,
                    ExisteSurServeurSource = true,
                    NomModifiable = false
                }
            };
        }

        public List<BaseCopieModel> ChargerBasesSauvegardees()
        {
            List<BaseCopieModel> basesServeur = ChargerBasesDepuisServeurSource();

            // Si aucun fichier de sauvegarde n'existe encore,
            // on retourne les bases du serveur source cochées par défaut.
            if (!File.Exists(_cheminFichier))
            {
                return basesServeur;
            }

            string contenuJson = File.ReadAllText(_cheminFichier);

            if (string.IsNullOrWhiteSpace(contenuJson))
            {
                return basesServeur;
            }

            List<BaseCopieModel>? basesSauvegardees =
                JsonConvert.DeserializeObject<List<BaseCopieModel>>(contenuJson);

            if (basesSauvegardees == null || basesSauvegardees.Count == 0)
            {
                return basesServeur;
            }

            return FusionnerBasesServeurEtSauvegarde(
                basesServeur,
                basesSauvegardees
            );
        }

        public void EnregistrerBases(List<BaseCopieModel> bases)
        {
            string contenuJson = JsonConvert.SerializeObject(
                bases,
                Formatting.Indented
            );

            File.WriteAllText(_cheminFichier, contenuJson);
        }

        public void AppliquerModeCopieGlobal(string modeCopieGlobal)
        {
            List<BaseCopieModel> bases = ChargerBasesSauvegardees();

            foreach (BaseCopieModel baseCopie in bases)
            {
                baseCopie.ModeCopie = NormaliserModeCopie(modeCopieGlobal);
            }

            EnregistrerBases(bases);
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
                    // Par défaut, elle est cochée.
                    resultat.Add(baseServeur);
                    continue;
                }

                // La base existe déjà dans la sauvegarde.
                // On garde le choix de l'utilisateur : Inclure + ModeCopie + Ordre.
                resultat.Add(new BaseCopieModel
                {
                    Inclure = baseSauvegardee.Inclure,
                    NomBase = baseServeur.NomBase,
                    OrdreTraitement = baseSauvegardee.OrdreTraitement,
                    ModeCopie = NormaliserModeCopie(baseSauvegardee.ModeCopie),
                    Statut = baseSauvegardee.Inclure
                        ? baseServeur.Statut
                        : "Non sélectionnée",
                    DerniereCopie = baseServeur.DerniereCopie,
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
            // On accepte les deux textes parce que ton projet utilise actuellement
            // "Mettre à jour" dans Configuration et "Mise à jour" dans Bases à copier.
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