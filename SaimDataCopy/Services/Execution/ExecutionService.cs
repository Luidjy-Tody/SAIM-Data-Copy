using SaimDataCopy.DataProviders.Execution;
using SaimDataCopy.Models.BasesCopier;
using SaimDataCopy.Models.Execution;
using System.Diagnostics;

namespace SaimDataCopy.Services.Execution
{
    // Service de la page Exécution.
    // Il contient la logique métier de l'exécution.
    public class ExecutionService : IExecutionService
    {
        private readonly IExecutionDataProvider _executionDataProvider;

        public ExecutionService(IExecutionDataProvider executionDataProvider)
        {
            _executionDataProvider = executionDataProvider;
        }

        public ExecutionTableauBordModel ChargerTableauBordInitial()
        {
            return _executionDataProvider.ChargerDernierTableauBord();
        }

        public List<ExecutionResultatBaseModel> ChargerDerniersResultats()
        {
            return _executionDataProvider.ChargerDerniersResultats();
        }

        public async Task<bool> TesterConnexionAsync(
            IProgress<ExecutionProgressionModel> progression,
            CancellationToken cancellationToken)
        {
            List<BaseCopieModel> basesSelectionnees =
                _executionDataProvider.ChargerBasesSelectionnees();

            progression.Report(new ExecutionProgressionModel
            {
                Pourcentage = 0,
                MessageProgression = "Test de connexion en cours...",
                Log = CreerLog("Test de connexion lancé par l'utilisateur.", "Info")
            });

            await Task.Delay(500, cancellationToken);

            if (basesSelectionnees.Count == 0)
            {
                progression.Report(new ExecutionProgressionModel
                {
                    Pourcentage = 0,
                    MessageProgression = "Aucune base sélectionnée.",
                    Log = CreerLog("Aucune base sélectionnée pour tester la connexion.", "Avertissement"),
                    TableauBord = new ExecutionTableauBordModel
                    {
                        NombreBasesSelectionnees = 0,
                        NombreLignesCopiees = 0,
                        DureeDerniereExecution = "-"
                    }
                });

                return false;
            }

            progression.Report(new ExecutionProgressionModel
            {
                Pourcentage = 35,
                MessageProgression = "Vérification du serveur source...",
                Log = CreerLog("Connexion serveur source : OK", "Succes")
            });

            await Task.Delay(500, cancellationToken);

            progression.Report(new ExecutionProgressionModel
            {
                Pourcentage = 70,
                MessageProgression = "Vérification du serveur cible...",
                Log = CreerLog("Connexion serveur cible : OK", "Succes")
            });

            await Task.Delay(500, cancellationToken);

            progression.Report(new ExecutionProgressionModel
            {
                Pourcentage = 100,
                MessageProgression = "Test de connexion réussi.",
                Log = CreerLog(
                    $"Test terminé : {basesSelectionnees.Count} base(s) prête(s) pour la copie.",
                    "Succes"),
                TableauBord = new ExecutionTableauBordModel
                {
                    NombreBasesSelectionnees = basesSelectionnees.Count,
                    NombreLignesCopiees = ChargerTableauBordInitial().NombreLignesCopiees,
                    DureeDerniereExecution = ChargerTableauBordInitial().DureeDerniereExecution
                }
            });

            return true;
        }

        public async Task<List<ExecutionResultatBaseModel>> LancerCopieAsync(
            IProgress<ExecutionProgressionModel> progression,
            CancellationToken cancellationToken)
        {
            List<BaseCopieModel> basesSelectionnees =
                _executionDataProvider.ChargerBasesSelectionnees();

            List<ExecutionResultatBaseModel> resultats = new List<ExecutionResultatBaseModel>();

            if (basesSelectionnees.Count == 0)
            {
                progression.Report(new ExecutionProgressionModel
                {
                    Pourcentage = 0,
                    MessageProgression = "Aucune base sélectionnée pour la copie.",
                    Log = CreerLog("Aucune base sélectionnée pour la copie.", "Avertissement"),
                    TableauBord = new ExecutionTableauBordModel
                    {
                        NombreBasesSelectionnees = 0,
                        NombreLignesCopiees = 0,
                        DureeDerniereExecution = "-"
                    }
                });

                return resultats;
            }

            Stopwatch chronometre = Stopwatch.StartNew();

            int totalBases = basesSelectionnees.Count;
            int totalLignesCopiees = 0;

            progression.Report(new ExecutionProgressionModel
            {
                Pourcentage = 0,
                MessageProgression = $"Progression : 0 base sur {totalBases} copiée",
                Log = CreerLog("Démarrage de la copie des bases.", "Info"),
                TableauBord = new ExecutionTableauBordModel
                {
                    NombreBasesSelectionnees = totalBases,
                    NombreLignesCopiees = 0,
                    DureeDerniereExecution = "-"
                }
            });

            for (int i = 0; i < totalBases; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                BaseCopieModel baseCopie = basesSelectionnees[i];

                progression.Report(new ExecutionProgressionModel
                {
                    Pourcentage = CalculerPourcentage(i, totalBases),
                    MessageProgression = $"Copie en cours : {baseCopie.NomBase}",
                    Log = CreerLog($"Copie de {baseCopie.NomBase} en cours...", "Info")
                });

                // Simulation temporaire.
                // Plus tard, cette partie sera remplacée par la vraie copie SQL Server / EF Core.
                await Task.Delay(900, cancellationToken);

                ExecutionResultatBaseModel resultat =
                    CreerResultatSimulation(baseCopie, i);

                resultats.Add(resultat);

                // Ici on calcule les lignes vraiment copiées.
                int lignesCopieesPourCetteBase =
                    Math.Max(0, resultat.LignesApres - resultat.LignesAvant);

                totalLignesCopiees += lignesCopieesPourCetteBase;

                int basesDejaCopiees = i + 1;
                int pourcentage = CalculerPourcentage(basesDejaCopiees, totalBases);

                progression.Report(new ExecutionProgressionModel
                {
                    Pourcentage = pourcentage,
                    MessageProgression =
                        $"Progression : {basesDejaCopiees} base(s) sur {totalBases} copiée(s)",
                    Log = CreerLog(
                        $"{baseCopie.NomBase} : {resultat.Resultat} - {resultat.Message}",
                        ObtenirTypeLogDepuisResultat(resultat.Resultat)),
                    TableauBord = new ExecutionTableauBordModel
                    {
                        NombreBasesSelectionnees = totalBases,
                        NombreLignesCopiees = totalLignesCopiees,
                        DureeDerniereExecution = FormaterDuree(chronometre.Elapsed)
                    }
                });
            }

            chronometre.Stop();

            ExecutionTableauBordModel tableauBordFinal = new ExecutionTableauBordModel
            {
                NombreBasesSelectionnees = totalBases,
                NombreLignesCopiees = totalLignesCopiees,
                DureeDerniereExecution = FormaterDuree(chronometre.Elapsed)
            };

            _executionDataProvider.EnregistrerDerniereExecution(
                tableauBordFinal,
                resultats
            );

            progression.Report(new ExecutionProgressionModel
            {
                Pourcentage = 100,
                MessageProgression = "Copie terminée.",
                Log = CreerLog("Copie terminée avec succès.", "Succes"),
                TableauBord = tableauBordFinal
            });

            return resultats;
        }

        private ExecutionResultatBaseModel CreerResultatSimulation(
            BaseCopieModel baseCopie,
            int index)
        {
            int lignesAvant = 1000 + (index * 350);
            int lignesApres = lignesAvant + 120;

            string resultat = ObtenirResultatDepuisStatut(baseCopie.Statut);
            string message = ObtenirMessageDepuisResultat(resultat, baseCopie.ModeCopie);

            return new ExecutionResultatBaseModel
            {
                NomBase = baseCopie.NomBase,
                LignesAvant = lignesAvant,
                LignesApres = lignesApres,
                Resultat = resultat,
                Message = message
            };
        }

        private string ObtenirResultatDepuisStatut(string statut)
        {
            return statut switch
            {
                "Prête" => "Succès",
                "Avertissement" => "Avertissement",
                "Non sélectionnée" => "Avertissement",
                _ => "Avertissement"
            };
        }

        private string ObtenirMessageDepuisResultat(string resultat, string modeCopie)
        {
            return resultat switch
            {
                "Succès" => $"Copie terminée en mode {modeCopie}.",
                "Avertissement" => $"Copie terminée avec avertissement en mode {modeCopie}.",
                "Erreur" => "Erreur pendant la copie.",
                _ => "Résultat inconnu."
            };
        }

        private string ObtenirTypeLogDepuisResultat(string resultat)
        {
            return resultat switch
            {
                "Succès" => "Succes",
                "Avertissement" => "Avertissement",
                "Erreur" => "Erreur",
                _ => "Info"
            };
        }

        private int CalculerPourcentage(int valeurActuelle, int valeurMax)
        {
            if (valeurMax <= 0)
            {
                return 0;
            }

            return (int)Math.Round((double)valeurActuelle / valeurMax * 100);
        }

        private ExecutionLogModel CreerLog(string message, string type)
        {
            return new ExecutionLogModel
            {
                Heure = DateTime.Now.ToString("HH:mm:ss"),
                Message = message,
                Type = type
            };
        }

        private string FormaterDuree(TimeSpan duree)
        {
            if (duree.TotalSeconds < 1)
            {
                return $"{duree.Milliseconds} ms";
            }

            if (duree.TotalMinutes < 1)
            {
                return $"{Math.Round(duree.TotalSeconds, 1)} s";
            }

            return $"{(int)duree.TotalMinutes} min {duree.Seconds} s";
        }
    }
}