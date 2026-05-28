using SaimDataCopy.Models.Historique;

namespace SaimDataCopy.DataProviders.Historique
{
    // DataProvider de la page Historique.
    // Pour le moment, on utilise des données de test.
    // Plus tard, cette classe pourra lire les données avec Entity Framework Core.
    public class HistoriqueDataProvider : IHistoriqueDataProvider
    {
        private readonly string dossierLogs;

        public HistoriqueDataProvider()
        {
            // Dossier local pour les logs de test.
            dossierLogs = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "historique_logs");

            // On crée le dossier s'il n'existe pas.
            Directory.CreateDirectory(dossierLogs);
        }

        public List<HistoriqueExecutionModel> ChargerExecutions()
        {
            // On prépare quelques fichiers logs de test pour le bouton "Ouvrir le fichier".
            string logSucces = CreerFichierLogTest(
                "2025-05-14.log",
                "Copie terminée avec succès.\nDB_Ventes copiée.\nDB_RH copiée.\nDB_Comptabilite copiée."
            );

            string logAvertissement = CreerFichierLogTest(
                "2025-05-13.log",
                "Copie terminée avec avertissement.\nDB_Ventes copiée.\nDB_RH copiée avec un avertissement.\nDB_Comptabilite copiée."
            );

            string logEchec = CreerFichierLogTest(
                "2025-05-12.log",
                "Copie en échec.\nDB_Ventes copiée.\nDB_RH non copiée.\nErreur pendant le traitement."
            );

            return new List<HistoriqueExecutionModel>
            {
                new HistoriqueExecutionModel
                {
                    Id = 1,
                    DateHeureLancement = new DateTime(2025, 5, 14, 2, 10, 0),
                    Origine = "Automatique",
                    BasesTraitees = new List<string>
                    {
                        "DB_Ventes",
                        "DB_RH",
                        "DB_Comptabilite"
                    },
                    LignesAvant = 1968,
                    LignesApres = 1972,
                    DureeSecondes = 12,
                    Statut = "Succès",
                    EmailEnvoye = true,
                    DateHeureEmail = new DateTime(2025, 5, 14, 2, 12, 0),
                    CheminFichierLog = logSucces,
                    Etapes = new List<HistoriqueEtapeExecutionModel>
                    {
                        new HistoriqueEtapeExecutionModel
                        {
                            NomBase = "DB_Ventes",
                            LignesAvant = 1238,
                            LignesApres = 1240,
                            Statut = "Succès",
                            Message = "Copie terminée correctement."
                        },
                        new HistoriqueEtapeExecutionModel
                        {
                            NomBase = "DB_RH",
                            LignesAvant = 420,
                            LignesApres = 422,
                            Statut = "Succès",
                            Message = "Copie terminée correctement."
                        },
                        new HistoriqueEtapeExecutionModel
                        {
                            NomBase = "DB_Comptabilite",
                            LignesAvant = 310,
                            LignesApres = 310,
                            Statut = "Succès",
                            Message = "Aucune nouvelle ligne à copier."
                        }
                    }
                },

                new HistoriqueExecutionModel
                {
                    Id = 2,
                    DateHeureLancement = new DateTime(2025, 5, 13, 2, 10, 0),
                    Origine = "Automatique",
                    BasesTraitees = new List<string>
                    {
                        "DB_Ventes",
                        "DB_RH",
                        "DB_Comptabilite"
                    },
                    LignesAvant = 1900,
                    LignesApres = 1968,
                    DureeSecondes = 15,
                    Statut = "Avertissement",
                    EmailEnvoye = true,
                    DateHeureEmail = new DateTime(2025, 5, 13, 2, 12, 0),
                    CheminFichierLog = logAvertissement,
                    Etapes = new List<HistoriqueEtapeExecutionModel>
                    {
                        new HistoriqueEtapeExecutionModel
                        {
                            NomBase = "DB_Ventes",
                            LignesAvant = 1200,
                            LignesApres = 1238,
                            Statut = "Succès",
                            Message = "Copie terminée correctement."
                        },
                        new HistoriqueEtapeExecutionModel
                        {
                            NomBase = "DB_RH",
                            LignesAvant = 400,
                            LignesApres = 420,
                            Statut = "Avertissement",
                            Message = "Certaines lignes ont été ignorées."
                        },
                        new HistoriqueEtapeExecutionModel
                        {
                            NomBase = "DB_Comptabilite",
                            LignesAvant = 300,
                            LignesApres = 310,
                            Statut = "Succès",
                            Message = "Copie terminée correctement."
                        }
                    }
                },

                new HistoriqueExecutionModel
                {
                    Id = 3,
                    DateHeureLancement = new DateTime(2025, 5, 12, 14, 30, 0),
                    Origine = "Manuel",
                    BasesTraitees = new List<string>
                    {
                        "DB_Ventes",
                        "DB_RH"
                    },
                    LignesAvant = 1850,
                    LignesApres = 1900,
                    DureeSecondes = 8,
                    Statut = "Échec",
                    EmailEnvoye = false,
                    DateHeureEmail = null,
                    CheminFichierLog = logEchec,
                    Etapes = new List<HistoriqueEtapeExecutionModel>
                    {
                        new HistoriqueEtapeExecutionModel
                        {
                            NomBase = "DB_Ventes",
                            LignesAvant = 1150,
                            LignesApres = 1200,
                            Statut = "Succès",
                            Message = "Copie terminée correctement."
                        },
                        new HistoriqueEtapeExecutionModel
                        {
                            NomBase = "DB_RH",
                            LignesAvant = 400,
                            LignesApres = 400,
                            Statut = "Échec",
                            Message = "Erreur pendant la copie de la base."
                        }
                    }
                }
            };
        }

        private string CreerFichierLogTest(string nomFichier, string contenu)
        {
            string chemin = Path.Combine(dossierLogs, nomFichier);

            // On écrit le fichier seulement s'il n'existe pas encore.
            if (!File.Exists(chemin))
            {
                File.WriteAllText(chemin, contenu);
            }

            return chemin;
        }
    }
}