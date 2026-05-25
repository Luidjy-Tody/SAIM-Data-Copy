using SaimDataCopy.Models;

namespace SaimDataCopy.Services
{
    public class BasesCopierService
    {
        public List<BaseCopieModel> ChargerBasesDemo()
        {
            // Données temporaires pour afficher la page.
            // Plus tard, ces données viendront de SQL Server.
            return new List<BaseCopieModel>
            {
                new BaseCopieModel
                {
                    Inclure = true,
                    NomBase = "DB_Ventes",
                    OrdreTraitement = 1,
                    ModeCopie = "Écraser",
                    Statut = "Prête",
                    DerniereCopie = new DateTime(2025, 5, 14, 2, 0, 0)
                },

                new BaseCopieModel
                {
                    Inclure = true,
                    NomBase = "DB_RH",
                    OrdreTraitement = 2,
                    ModeCopie = "Écraser",
                    Statut = "Prête",
                    DerniereCopie = new DateTime(2025, 5, 14, 2, 3, 0)
                },

                new BaseCopieModel
                {
                    Inclure = true,
                    NomBase = "DB_Comptabilite",
                    OrdreTraitement = 3,
                    ModeCopie = "Mise à jour",
                    Statut = "Avertissement",
                    DerniereCopie = new DateTime(2025, 5, 14, 2, 6, 0)
                },

                new BaseCopieModel
                {
                    Inclure = false,
                    NomBase = "DB_Archive",
                    OrdreTraitement = 4,
                    ModeCopie = "Écraser",
                    Statut = "Non sélectionnée",
                    DerniereCopie = null
                }
            };
        }

        public List<string> ObtenirModesCopie()
        {
            // Les choix qui seront affichés dans la ComboBox du tableau.
            return new List<string>
            {
                "Écraser",
                "Mise à jour"
            };
        }

        public BaseCopieModel CreerNouvelleBase(List<BaseCopieModel> basesExistantes,string nomBase)
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
            // Pour l'instant, on simule l'enregistrement.
            // Plus tard, on appellera une classe dans DataAccess.
            return true;
        }
    }
}