using SaimDataCopy.Models.BasesCopier;

namespace SaimDataCopy.DataProviders.BasesCopier
{
    // DataProvider pour la page Bases à copier.
    // Son rôle est de charger et enregistrer les données.
    // Plus tard, ici on pourra utiliser SQL Server.
    public class BasesCopierDataProvider : IBasesCopierDataProvider
    {
        public List<BaseCopieModel> ChargerBases()
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

        public void EnregistrerBases(List<BaseCopieModel> bases)
        {
            // Pour l'instant, on simule l'enregistrement.
            // Plus tard, on mettra ici l'enregistrement réel dans SQL Server.
        }
    }
}