using SaimDataCopy.DataProviders.BasesCopier;
using SaimDataCopy.Models.BasesCopier;

namespace SaimDataCopy.Services.BasesCopier
{
    // Service de la page Bases à copier.
    // Il contient la logique métier : chargement, sélection, validation, sauvegarde.
    public class BasesCopierService : IBasesCopierService
    {
        // Le Service utilise le DataProvider pour charger ou enregistrer les données.
        private readonly IBasesCopierDataProvider _basesCopierDataProvider;

        // Constructeur simple.
        // Il crée directement le DataProvider.
        public BasesCopierService()
        {
            _basesCopierDataProvider = BasesCopierDataProviderFactory.Creer() ;
        }

        // Constructeur utile plus tard pour les tests ou pour changer la source de données.
        public BasesCopierService(IBasesCopierDataProvider basesCopierDataProvider)
        {
            _basesCopierDataProvider = basesCopierDataProvider;
        }

        public List<BaseCopieModel> ChargerBases()
        {
            // On charge les bases avec le dernier état sauvegardé.
            // Si aucune sauvegarde n'existe, le DataProvider retourne les bases cochées par défaut.
            return _basesCopierDataProvider.ChargerBasesSauvegardees();
        }

        public List<string> ObtenirModesCopie()
        {
            // Les choix affichés dans la ComboBox du tableau.
            return new List<string>
            {
                "Écraser",
                "Mise à jour"
            };
        }

        public List<BaseCopieModel> CocherToutesBases(List<BaseCopieModel> bases)
        {
            foreach (BaseCopieModel baseCopie in bases)
            {
                baseCopie.Inclure = true;

                // Si une base était décochée, on remet un statut normal.
                if (baseCopie.Statut == "Non sélectionnée")
                {
                    baseCopie.Statut = "Prête";
                }
            }

            return bases;
        }

        public List<BaseCopieModel> DecocherBases(
            List<BaseCopieModel> bases,
            List<string> nomsBasesSelectionnees)
        {
            foreach (BaseCopieModel baseCopie in bases)
            {
                bool doitEtreDecochee = nomsBasesSelectionnees
                    .Any(nom =>
                        nom.Equals(
                            baseCopie.NomBase,
                            StringComparison.OrdinalIgnoreCase
                        )
                    );

                if (doitEtreDecochee)
                {
                    baseCopie.Inclure = false;
                    baseCopie.Statut = "Non sélectionnée";
                }
            }

            return bases;
        }

        public List<string> ValiderBases(List<BaseCopieModel> bases)
        {
            List<string> erreurs = new List<string>();

            List<BaseCopieModel> basesIncluses = bases
                .Where(b => b.Inclure)
                .ToList();

            if (bases.Count == 0)
            {
                erreurs.Add("Aucune base n'a été trouvée sur le serveur source.");
            }
            else if (basesIncluses.Count == 0)
            {
                erreurs.Add("Vous devez cocher au moins une base à copier.");
            }

            foreach (BaseCopieModel baseCopie in basesIncluses)
            {
                if (string.IsNullOrWhiteSpace(baseCopie.NomBase))
                {
                    erreurs.Add("Le nom d'une base ne peut pas être vide.");
                }

                if (baseCopie.OrdreTraitement <= 0)
                {
                    erreurs.Add($"L'ordre de traitement de {baseCopie.NomBase} doit être supérieur à 0.");
                }

                // L'encadreur préfère switch quand il y a plusieurs choix métier.
                switch (NormaliserModeCopie(baseCopie.ModeCopie))
                {
                    case "Écraser":
                    case "Mise à jour":
                        break;

                    default:
                        erreurs.Add($"Le mode de copie de {baseCopie.NomBase} est invalide.");
                        break;
                }
            }

            var nomsDoublons = bases
                .Where(b => !string.IsNullOrWhiteSpace(b.NomBase))
                .GroupBy(b => b.NomBase.Trim(), StringComparer.OrdinalIgnoreCase)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key);

            foreach (string nom in nomsDoublons)
            {
                erreurs.Add($"Le nom de base {nom} est utilisé plusieurs fois.");
            }

            var ordresDoublons = basesIncluses
                .GroupBy(b => b.OrdreTraitement)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key);

            foreach (int ordre in ordresDoublons)
            {
                erreurs.Add($"L'ordre de traitement {ordre} est utilisé plusieurs fois.");
            }

            return erreurs;
        }

        public bool EnregistrerBases(List<BaseCopieModel> bases)
        {
            // Avant d'enregistrer, on remet les statuts propres.
            List<BaseCopieModel> basesPreparees = PreparerBasesPourEnregistrement(bases);

            List<string> erreurs = ValiderBases(basesPreparees);

            if (erreurs.Count > 0)
            {
                return false;
            }

            // Le Service ne sauvegarde pas directement.
            // Il demande au DataProvider de faire l'enregistrement.
            _basesCopierDataProvider.EnregistrerBases(basesPreparees);

            return true;
        }

        public void AppliquerModeCopieGlobal(string modeCopieGlobal)
        {
            // Cette méthode sera appelée quand la page Configuration est enregistrée.
            _basesCopierDataProvider.AppliquerModeCopieGlobal(
                NormaliserModeCopie(modeCopieGlobal)
            );
        }

        private List<BaseCopieModel> PreparerBasesPourEnregistrement(List<BaseCopieModel> bases)
        {
            foreach (BaseCopieModel baseCopie in bases)
            {
                baseCopie.NomBase = baseCopie.NomBase.Trim();
                baseCopie.ModeCopie = NormaliserModeCopie(baseCopie.ModeCopie);
                baseCopie.NomModifiable = false;
                baseCopie.ExisteSurServeurSource = true;

                if (!baseCopie.Inclure)
                {
                    baseCopie.Statut = "Non sélectionnée";
                }
                else if (baseCopie.Statut == "Non sélectionnée")
                {
                    baseCopie.Statut = "Prête";
                }
            }

            return bases;
        }

        private string NormaliserModeCopie(string modeCopie)
        {
            // Dans ton projet, Configuration utilise actuellement "Mettre à jour",
            // alors que Bases à copier utilise "Mise à jour".
            // On normalise pour éviter les erreurs de validation.
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