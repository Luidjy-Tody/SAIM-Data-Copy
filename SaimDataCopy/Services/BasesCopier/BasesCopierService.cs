using SaimDataCopy.DataProviders.BasesCopier;
using SaimDataCopy.Models.BasesCopier;

namespace SaimDataCopy.Services.BasesCopier
{
    // Service de la page Bases à copier.
    // Il contient la logique métier : validation, ajout, suppression, etc.
    public class BasesCopierService : IBasesCopierService
    {
        // Le Service utilise le DataProvider pour charger ou enregistrer les données.
        private readonly IBasesCopierDataProvider _basesCopierDataProvider;

        // Constructeur simple.
        // Il crée directement le DataProvider.
        public BasesCopierService()
        {
            _basesCopierDataProvider = new BasesCopierDataProvider();
        }

        // Constructeur utile plus tard pour les tests ou pour changer la source de données.
        public BasesCopierService(IBasesCopierDataProvider basesCopierDataProvider)
        {
            _basesCopierDataProvider = basesCopierDataProvider;
        }

        public List<BaseCopieModel> ChargerBasesDemo()
        {
            // Le Service ne crée plus les données lui-même.
            // Il demande au DataProvider de charger les bases.
            return _basesCopierDataProvider.ChargerBases();
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

        public BaseCopieModel CreerNouvelleBase(List<BaseCopieModel> basesExistantes, string nomBase)
        {
            int prochainOrdre = basesExistantes.Count == 0
                ? 1
                : basesExistantes.Max(b => b.OrdreTraitement) + 1;

            return new BaseCopieModel
            {
                Inclure = false,
                NomBase = nomBase.Trim(),
                OrdreTraitement = prochainOrdre,
                ModeCopie = "Écraser",
                Statut = "Non sélectionnée",
                DerniereCopie = null
            };
        }

        public List<BaseCopieModel> SupprimerBases(
            List<BaseCopieModel> bases,
            List<string> nomsBasesSelectionnees)
        {
            return bases
                .Where(b => !nomsBasesSelectionnees.Contains(b.NomBase))
                .ToList();
        }

        public List<string> ValiderBases(List<BaseCopieModel> bases)
        {
            List<string> erreurs = new List<string>();

            List<BaseCopieModel> basesIncluses = bases
                .Where(b => b.Inclure)
                .ToList();

            if (bases.Count == 0)
            {
                erreurs.Add("Vous devez ajouter une base puis la cocher pour enregistrer.");
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
                switch (baseCopie.ModeCopie)
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
            List<string> erreurs = ValiderBases(bases);

            if (erreurs.Count > 0)
            {
                return false;
            }

            // Le Service ne sauvegarde pas directement.
            // Il demande au DataProvider de le faire.
            _basesCopierDataProvider.EnregistrerBases(bases);

            return true;
        }
    }
}