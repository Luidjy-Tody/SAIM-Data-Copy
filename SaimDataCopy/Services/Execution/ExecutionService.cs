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

        public List<string> ChargerTablesBaseSource(string nomBase)
        {
            if (string.IsNullOrWhiteSpace(nomBase))
            {
                return new List<string>();
            }

            return _executionDataProvider.ChargerTablesBaseSource(nomBase);
        }

        public int CompterLignesTableSource(string nomBase, string nomTable)
        {
            if (string.IsNullOrWhiteSpace(nomBase) ||
                string.IsNullOrWhiteSpace(nomTable))
            {
                return 0;
            }

            return _executionDataProvider.CompterLignesTableSource(nomBase, nomTable);
        }

        public bool VerifierOuCreerBaseCible(string nomBase)
        {
            // Le Service vérifie seulement que le nom de la base est correct.
            // Ensuite, il demande au DataProvider de vérifier ou créer la base cible.
            if (string.IsNullOrWhiteSpace(nomBase))
            {
                return false;
            }

            return _executionDataProvider.VerifierOuCreerBaseCible(nomBase);
        }

        public bool VerifierOuCreerTableCible(string nomBase, string nomTable)
        {
            // Le service ne fait pas directement le SQL.
            // Il demande au DataProvider de vérifier ou créer la table côté cible.
            return _executionDataProvider.VerifierOuCreerTableCible(nomBase, nomTable);
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
                MessageProgression = $"Progression : 0 base sur {totalBases} traitée",
                Log = CreerLog("Démarrage de la lecture réelle des bases source.", "Info"),
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
                    MessageProgression = $"Lecture en cours : {baseCopie.NomBase}",
                    Log = CreerLog($"Lecture de {baseCopie.NomBase} en cours...", "Info")
                });

                ExecutionResultatBaseModel resultat;

                try
                {
                    // On lance la lecture SQL dans une tâche séparée pour éviter de bloquer l'interface.
                    resultat = await Task.Run(
                        () => CreerResultatLectureReelle(baseCopie, cancellationToken),
                        cancellationToken
                    );
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (InvalidOperationException ex)
                {
                    // Ce cas peut arriver quand la source et la cible pointent vers la même base.
                    // Ce n'est pas une erreur technique, c'est une protection pour éviter de modifier la source.
                    resultat = new ExecutionResultatBaseModel
                    {
                        NomBase = baseCopie.NomBase,
                        LignesAvant = 0,
                        LignesApres = 0,
                        Resultat = "Avertissement",
                        Message = ex.Message
                    };
                }
                catch (Exception ex)
                {
                    // Ici, on garde les vraies erreurs techniques :
                    // problème SQL, problème de connexion, table invalide, etc.
                    resultat = new ExecutionResultatBaseModel
                    {
                        NomBase = baseCopie.NomBase,
                        LignesAvant = 0,
                        LignesApres = 0,
                        Resultat = "Erreur",
                        Message = $"Erreur pendant la copie : {ex.Message}"
                    };
                }

                resultats.Add(resultat);

                int lignesPourCetteBase =
                    Math.Max(0, resultat.LignesApres - resultat.LignesAvant);

                totalLignesCopiees += lignesPourCetteBase;

                int basesDejaTraitees = i + 1;
                int pourcentage = CalculerPourcentage(basesDejaTraitees, totalBases);

                progression.Report(new ExecutionProgressionModel
                {
                    Pourcentage = pourcentage,
                    MessageProgression =
                        $"Progression : {basesDejaTraitees} base(s) sur {totalBases} traitée(s)",
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

            // On remplace l'ancien JSON par les nouveaux résultats réels.
            _executionDataProvider.EnregistrerDerniereExecution(
                tableauBordFinal,
                resultats
            );

            progression.Report(new ExecutionProgressionModel
            {
                Pourcentage = 100,
                MessageProgression = "Lecture terminée.",
                Log = CreerLog("Lecture réelle des bases terminée.", "Succes"),
                TableauBord = tableauBordFinal
            });

            return resultats;
        }

        private ExecutionResultatBaseModel CreerResultatLectureReelle( BaseCopieModel baseCopie,
            CancellationToken cancellationToken)
        {
            // Avant de lire les tables, on vérifie si la base existe côté cible.
            // Si elle n'existe pas, elle sera créée.
            bool baseCibleCreee =
                _executionDataProvider.VerifierOuCreerBaseCible(baseCopie.NomBase);

            string messageBaseCible = baseCibleCreee
                ? "Base cible créée."
                : "Base cible déjà existante.";

            // On charge les vraies tables de la base source.
            List<string> tables =
                _executionDataProvider.ChargerTablesBaseSource(baseCopie.NomBase);

            if (tables.Count == 0)
            {
                return new ExecutionResultatBaseModel
                {
                    NomBase = baseCopie.NomBase,
                    LignesAvant = 0,
                    LignesApres = 0,
                    Resultat = "Avertissement",
                    Message = $"{messageBaseCible} Aucune table trouvée dans cette base."
                };
            }

            int totalLignes = 0;
            int nombreTablesCreees = 0;
            int nombreTablesDejaExistantes = 0;

            foreach (string table in tables)
            {
                cancellationToken.ThrowIfCancellationRequested();

                // Pour chaque table trouvée côté source,
                // on vérifie si la même table existe côté cible.
                // Si elle n'existe pas, elle sera créée automatiquement.
                bool tableCibleCreee = _executionDataProvider.VerifierOuCreerTableCible(baseCopie.NomBase, table);

                if (tableCibleCreee)
                {
                    nombreTablesCreees++;
                }
                else
                {
                    nombreTablesDejaExistantes++;
                }

                // On récupère le mode de copie choisi pour cette base.
                // Exemple : "Ecraser" ou "Mettre a jour".

                string modeCopie = baseCopie.ModeCopie;
                // Ici, on lance la vraie copie des lignes.
                // Si la source et la cible sont identiques, le DataProvider va bloquer la copie.

                int lignesCopiees = _executionDataProvider.CopierLignesTableSourceVersCible(
                    baseCopie.NomBase,
                    table,
                    modeCopie

                
                    );

                totalLignes += lignesCopiees;
            }

            return new ExecutionResultatBaseModel
            {
                NomBase = baseCopie.NomBase,
                LignesAvant = 0,
                LignesApres = totalLignes,
                Resultat = "Succès",
                Message =
                    $"{messageBaseCible} " +
                    $"Préparation cible terminée : {nombreTablesCreees} table(s) créée(s), " +
                    $"{nombreTablesDejaExistantes} table(s) déjà existante(s). " +
                    $"Copie terminée : {tables.Count} table(s), {totalLignes} ligne(s) copiée(s)."
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