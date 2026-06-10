using SaimDataCopy.DataProviders.Email;
using SaimDataCopy.DataProviders.Execution;
using SaimDataCopy.DataProviders.Historique;
using SaimDataCopy.DataProviders.Logs;
using SaimDataCopy.Helpers;
using SaimDataCopy.Models.BasesCopier;
using SaimDataCopy.Models.Execution;
using SaimDataCopy.Models.Historique;
using SaimDataCopy.Services.Email;
using SaimDataCopy.Services.Logs;
using System.Diagnostics;
using SaimDataCopy.DataProviders.Configuration;
using SaimDataCopy.Models.Configuration;
using System.Configuration;
using SaimDataCopy.DataProviders.BasesCopier;
namespace SaimDataCopy.Services.Execution
{
    // Service de la page Exécution.
    // Il contient la logique métier de l'exécution.
    public class ExecutionService : IExecutionService
    {
        private readonly IExecutionDataProvider _executionDataProvider;
        private readonly IJournalisationService _journalisationService;
        private readonly IEmailService _emailService;
        private readonly IHistoriqueDataProvider _historiqueDataProvider;
        private readonly IConfigurationDataProvider _configurationDataProvider;

        public ExecutionService()

            : this(ExecutionDataProviderFactory.Creer())
        {
        }

        public ExecutionService(IExecutionDataProvider executionDataProvider)
            : this(
                executionDataProvider,
                new JournalisationService(new LogsDataProvider()),
                new EmailService(new EmailDataProvider()))
        {
        }

        public ExecutionService(
            IExecutionDataProvider executionDataProvider,
            IJournalisationService journalisationService)
            : this(
                executionDataProvider,
                journalisationService,
                new EmailService(new EmailDataProvider()))
        {
        }

        public ExecutionService(
            IExecutionDataProvider executionDataProvider,
            IJournalisationService journalisationService,
            IEmailService emailService)
            : this(
                executionDataProvider,
                journalisationService,
                emailService,
                new HistoriqueDataProvider())
        {
        }

        public ExecutionService(
    IExecutionDataProvider executionDataProvider,
    IJournalisationService journalisationService,
    IEmailService emailService,
    IHistoriqueDataProvider historiqueDataProvider)
    : this(
        executionDataProvider,
        journalisationService,
        emailService,
        historiqueDataProvider,
        new ConfigurationDataProvider())
        {
        }

        public ExecutionService(
            IExecutionDataProvider executionDataProvider,
            IJournalisationService journalisationService,
            IEmailService emailService,
            IHistoriqueDataProvider historiqueDataProvider,
            IConfigurationDataProvider configurationDataProvider)
        {
            _executionDataProvider = executionDataProvider;
            _journalisationService = journalisationService;
            _emailService = emailService;
            _historiqueDataProvider = historiqueDataProvider;
            _configurationDataProvider = configurationDataProvider;
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
            if (string.IsNullOrWhiteSpace(nomBase))
            {
                return false;
            }

            return _executionDataProvider.VerifierOuCreerBaseCible(nomBase);
        }

        public bool VerifierOuCreerTableCible(string nomBase, string nomTable)
        {
            return _executionDataProvider.VerifierOuCreerTableCible(nomBase, nomTable);
        }

        public async Task<bool> TesterConnexionAsync( IProgress<ExecutionProgressionModel> progression,
            CancellationToken cancellationToken)
        {
            progression.Report(new ExecutionProgressionModel
            {
                Pourcentage = 0,
                MessageProgression = "Test de connexion en cours...",
                Log = CreerLog("Test de connexion lancé par l'utilisateur.", "Info")
            });

            await Task.Delay(300, cancellationToken);

            try
            {
                progression.Report(new ExecutionProgressionModel
                {
                    Pourcentage = 35,
                    MessageProgression = "Vérification réelle du serveur source...",
                    Log = CreerLog("Vérification réelle du serveur source...", "Info")
                });

                await Task.Run(
                    () => _executionDataProvider.TesterConnexionSource(),
                    cancellationToken);

                progression.Report(new ExecutionProgressionModel
                {
                    Pourcentage = 60,
                    MessageProgression = "Connexion serveur source : OK",
                    Log = CreerLog("Connexion serveur source : OK", "Succes")
                });

                progression.Report(new ExecutionProgressionModel
                {
                    Pourcentage = 75,
                    MessageProgression = "Vérification réelle du serveur cible...",
                    Log = CreerLog("Vérification réelle du serveur cible...", "Info")
                });

                await Task.Run(
                    () => _executionDataProvider.TesterConnexionCible(),
                    cancellationToken);

                progression.Report(new ExecutionProgressionModel
                {
                    Pourcentage = 90,
                    MessageProgression = "Connexion serveur cible : OK",
                    Log = CreerLog("Connexion serveur cible : OK", "Succes")
                });

                List<BaseCopieModel> basesSelectionnees =
                    _executionDataProvider.ChargerBasesSelectionnees();

                ExecutionTableauBordModel tableauBordInitial = ChargerTableauBordInitial();

                progression.Report(new ExecutionProgressionModel
                {
                    Pourcentage = 100,
                    MessageProgression = "Test de connexion réussi.",
                    Log = CreerLog(
                        $"Test terminé : connexion source et cible validées. {basesSelectionnees.Count} base(s) sélectionnée(s).",
                        "Succes"),
                    TableauBord = new ExecutionTableauBordModel
                    {
                        NombreBasesSelectionnees = basesSelectionnees.Count,
                        NombreLignesCopiees = tableauBordInitial.NombreLignesCopiees,
                        DureeDerniereExecution = tableauBordInitial.DureeDerniereExecution
                    }
                });

                return true;
            }
            catch
            {
                throw;
            }
        }

        public async Task<List<ExecutionResultatBaseModel>> LancerCopieAsync(
            IProgress<ExecutionProgressionModel> progression,
            CancellationToken cancellationToken,
            string origineExecution = "Manuel")
        {
            List<BaseCopieModel> basesSelectionnees =
                _executionDataProvider.ChargerBasesSelectionnees();

            List<ExecutionResultatBaseModel> resultats = new List<ExecutionResultatBaseModel>();

            // On démarre un nouveau fichier .log pour cette exécution.
            _journalisationService.DemarrerNouvelleExecution();

            // On supprime les anciens logs selon la durée de conservation.
            _journalisationService.NettoyerAnciensLogs();

            _journalisationService.EcrireInformation(
                $"Lancement {NormaliserOrigineExecution(origineExecution).ToLower()} de la copie."); _journalisationService.EcrireInformation(
                $"{basesSelectionnees.Count} base(s) sélectionnée(s) pour la copie.");

            if (basesSelectionnees.Count == 0)
            {
                _journalisationService.EcrireAvertissement(
                    "Aucune base sélectionnée pour la copie.");

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

            DateTime dateHeureLancement = DateTime.Now;

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

                _journalisationService.EcrireInformation(
                    $"Début du traitement de la base : {baseCopie.NomBase}");

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
                    _journalisationService.EcrireAvertissement(
                        "Copie annulée par l'utilisateur.");

                    throw;
                }
                catch (InvalidOperationException ex)
                {
                    // Ce cas peut arriver quand la source et la cible pointent vers la même base.
                    // Ce n'est pas une erreur technique, c'est une protection pour éviter de modifier la source.
                    _journalisationService.EcrireAvertissement(
                        $"{baseCopie.NomBase} : {ex.Message}");

                    resultat = new ExecutionResultatBaseModel
                    {
                        NomBase = baseCopie.NomBase,
                        LignesAvant = 0,
                        LignesApres = 0,
                        LignesCopiees = 0,

                        Resultat = "Avertissement",
                        Message = ex.Message
                    };
                }
                catch (Exception ex)
                {
                    // On transforme l'erreur technique SQL en message simple pour l'utilisateur.
                    string messageSimple = ObtenirMessageErreurExecution(ex, baseCopie.NomBase);

                    // Dans le fichier log, on garde aussi le détail technique avec l'exception.
                    _journalisationService.EcrireErreur(
                        $"Erreur pendant la copie de {baseCopie.NomBase}. Message utilisateur : {messageSimple}", ex);

                    resultat = new ExecutionResultatBaseModel
                    {
                        NomBase = baseCopie.NomBase,
                        LignesAvant = 0,
                        LignesApres = 0,
                        LignesCopiees = 0,

                        Resultat = "Erreur",
                        Message = messageSimple
                    };
                }

                resultats.Add(resultat);

                EcrireResultatDansFichier(baseCopie.NomBase, resultat);

                // Si la copie est réussie, on mémorise la date de dernière copie.
                if (resultat.Resultat == "Succès")
                {
                    baseCopie.DerniereCopie = DateTime.Now;
                }

                // On utilise le nombre réel de lignes copiées pendant l'exécution.
                totalLignesCopiees += resultat.LignesCopiees;

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

            // Sauvegarde des nouvelles dates de dernière copie.
            IBasesCopierDataProvider basesCopierDataProvider = BasesCopierDataProviderFactory.Creer();

            basesCopierDataProvider.EnregistrerBases(basesSelectionnees);


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

            _journalisationService.EcrireSucces(
                $"Exécution terminée. {totalBases} base(s) traitée(s), " +
                $"{totalLignesCopiees} ligne(s) copiée(s), " +
                $"durée : {FormaterDuree(chronometre.Elapsed)}.");

            bool emailEnvoye = EnvoyerEmailConfirmationSiSucces(
                resultats,
                chronometre.Elapsed
            );

            EnregistrerExecutionDansHistorique(
                dateHeureLancement,
                resultats,
                chronometre.Elapsed,
                emailEnvoye,
                origineExecution
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

        private string ObtenirMessageErreurExecution(Exception ex, string nomBase)
        {
            string message = ex.Message;

            if (message.Contains("syntax", StringComparison.OrdinalIgnoreCase))
            {
                return
                    $"Erreur de syntaxe SQL pendant le traitement de la base {nomBase}. " +
                    "Cela veut dire qu'une requête envoyée à MySQL n'est pas correcte. " +
                    $"Détail technique : {message}";
            }

            if (message.Contains("Access denied", StringComparison.OrdinalIgnoreCase))
            {
                return
                    $"Accès refusé pendant le traitement de la base {nomBase}. " +
                    "Vérifiez l'identifiant, le mot de passe ou les droits de l'utilisateur MySQL.";
            }

            if (message.Contains("Unknown database", StringComparison.OrdinalIgnoreCase))
            {
                return
                    $"Base de données introuvable pendant le traitement de {nomBase}. " +
                    "Vérifiez que la base source ou cible existe bien.";
            }

            if (message.Contains("Table", StringComparison.OrdinalIgnoreCase) &&
                message.Contains("doesn't exist", StringComparison.OrdinalIgnoreCase))
            {
                return
                    $"Table introuvable pendant le traitement de la base {nomBase}. " +
                    "Vérifiez que la table existe bien dans la base source ou cible.";
            }

            return
                $"Erreur pendant le traitement de la base {nomBase}. " +
                $"Détail technique : {message}";
        }

        private ExecutionResultatBaseModel CreerResultatLectureReelle(BaseCopieModel baseCopie,
            CancellationToken cancellationToken)
        {
            _journalisationService.EcrireInformation(
                $"Vérification de la base cible : {baseCopie.NomBase}");

            // Avant de lire les tables, on vérifie si la base existe côté cible.
            // Si elle n'existe pas, elle sera créée.
            bool baseCibleCreee =
                _executionDataProvider.VerifierOuCreerBaseCible(baseCopie.NomBase);

            string messageBaseCible = baseCibleCreee
                ? "Base cible créée."
                : "Base cible déjà existante.";

            _journalisationService.EcrireInformation(
                $"{baseCopie.NomBase} : {messageBaseCible}");

            // On charge les vraies tables de la base source.
            List<string> tables =
                _executionDataProvider.ChargerTablesBaseSource(baseCopie.NomBase);

            string modeCopie = NormaliserModeCopie(baseCopie.ModeCopie);

            _journalisationService.EcrireInformation(
                $"{baseCopie.NomBase} : mode de copie utilisé = {modeCopie}");

            if (tables.Count == 0)
            {
                _journalisationService.EcrireAvertissement(
                    $"{baseCopie.NomBase} : aucune table trouvée.");

                return new ExecutionResultatBaseModel
                {
                    NomBase = baseCopie.NomBase,
                    LignesAvant = 0,
                    LignesApres = 0,
                    LignesCopiees = 0,
                    Resultat = "Avertissement",
                    Message = $"Mode : {modeCopie}. {messageBaseCible} Aucune table trouvée dans cette base."
                };
            }

            _journalisationService.EcrireInformation(
                $"{baseCopie.NomBase} : {tables.Count} table(s) trouvée(s).");

            int totalLignesAvant = 0;
            int totalLignesApres = 0;
            int totalLignesCopiees = 0;

            int nombreTablesCreees = 0;
            int nombreTablesDejaExistantes = 0;

            foreach (string table in tables)
            {
                cancellationToken.ThrowIfCancellationRequested();

                _journalisationService.EcrireInformation(
                    $"{baseCopie.NomBase} : traitement de la table {table}.");

                bool tableCibleCreee =
                    _executionDataProvider.VerifierOuCreerTableCible(
                        baseCopie.NomBase,
                        table);

                if (tableCibleCreee)
                {
                    nombreTablesCreees++;

                    _journalisationService.EcrireInformation(
                        $"{baseCopie.NomBase}.{table} : table cible créée.");
                }
                else
                {
                    nombreTablesDejaExistantes++;

                    _journalisationService.EcrireInformation(
                        $"{baseCopie.NomBase}.{table} : table cible déjà existante.");
                }

                // Important :
                // On compte les lignes de la cible AVANT la copie.
                // En mode Écraser, cela doit être fait avant le DELETE.
                int lignesAvantTable =
                    _executionDataProvider.CompterLignesTableCible(
                        baseCopie.NomBase,
                        table);

                int lignesCopiees =
                    _executionDataProvider.CopierLignesTableSourceVersCible(
                        baseCopie.NomBase,
                        table,
                        modeCopie,
                        cancellationToken);

                // Après la copie, on recompte les lignes réellement présentes côté cible.
                int lignesApresTable =
                    _executionDataProvider.CompterLignesTableCible(
                        baseCopie.NomBase,
                        table);

                totalLignesAvant += lignesAvantTable;
                totalLignesApres += lignesApresTable;
                totalLignesCopiees += lignesCopiees;

                _journalisationService.EcrireSucces(
                    $"{baseCopie.NomBase}.{table} : {lignesCopiees} ligne(s) copiée(s). " +
                    $"Lignes avant : {lignesAvantTable}, lignes après : {lignesApresTable}.");
            }

            _journalisationService.EcrireSucces(
                $"{baseCopie.NomBase} : copie terminée avec succès. " +
                $"{tables.Count} table(s), {totalLignesCopiees} ligne(s) copiée(s). " +
                $"Lignes avant : {totalLignesAvant}, lignes après : {totalLignesApres}.");

            return new ExecutionResultatBaseModel
            {
                NomBase = baseCopie.NomBase,
                LignesAvant = totalLignesAvant,
                LignesApres = totalLignesApres,
                LignesCopiees = totalLignesCopiees,
                Resultat = "Succès",
                Message =
                    $"Mode : {modeCopie}. " +
                    $"{messageBaseCible} " +
                    $"Préparation cible terminée : {nombreTablesCreees} table(s) créée(s), " +
                    $"{nombreTablesDejaExistantes} table(s) déjà existante(s). " +
                    $"Copie terminée : {tables.Count} table(s), {totalLignesCopiees} ligne(s) copiée(s). " +
                    $"Lignes avant : {totalLignesAvant}, lignes après : {totalLignesApres}."
            };
        }

        private bool EnvoyerEmailConfirmationSiSucces(
            List<ExecutionResultatBaseModel> resultats,
            TimeSpan dureeExecution)
        {
            if (!ExecutionTermineeAvecSucces(resultats))
            {
                _journalisationService.EcrireInformation(
                    "E-mail de confirmation non envoyé car l'exécution contient une erreur ou un avertissement.");

                return false;
            }

            string listeBases = ConstruireListeBasesTraitees(resultats);
            string duree = FormaterDuree(dureeExecution);

            // On récupère le fichier log de l'exécution actuelle.
            // Ce fichier pourra être joint à l'e-mail si l'option est cochée dans Paramètres Email.
            string cheminFichierLog =
                _journalisationService.RecupererCheminFichierExecutionActuel();

            bool emailEnvoye = _emailService.EnvoyerEmailConfirmationCopie(
                listeBases,
                duree,
                cheminFichierLog,
                out string messageEmail
            );

            if (emailEnvoye)
            {
                _journalisationService.EcrireInformation(messageEmail);
            }
            else
            {
                _journalisationService.EcrireAvertissement(messageEmail);
            }

            return emailEnvoye;
        }

        private bool ExecutionTermineeAvecSucces(List<ExecutionResultatBaseModel> resultats)
        {
            if (resultats.Count == 0)
            {
                return false;
            }

            foreach (ExecutionResultatBaseModel resultat in resultats)
            {
                if (resultat.Resultat != "Succès")
                {
                    return false;
                }
            }

            return true;
        }

        private string ConstruireListeBasesTraitees(List<ExecutionResultatBaseModel> resultats)
        {
            List<string> nomsBases = new List<string>();

            foreach (ExecutionResultatBaseModel resultat in resultats)
            {
                nomsBases.Add(resultat.NomBase);
            }

            return string.Join(", ", nomsBases);
        }

        private void EnregistrerExecutionDansHistorique(
            DateTime dateHeureLancement,
            List<ExecutionResultatBaseModel> resultats,
            TimeSpan dureeExecution,
            bool emailEnvoye,
            string origineExecution)
        {
            try
            {
                ConfigurationModel? configuration =
                    _configurationDataProvider.ChargerConfiguration();

                HistoriqueExecutionModel historique = new HistoriqueExecutionModel
                {
                    DateHeureLancement = dateHeureLancement,
                    Origine = NormaliserOrigineExecution(origineExecution),
                    ServeurSource = ObtenirServeurAffichage(configuration?.ServeurSource),
                    ServeurCible = ObtenirServeurAffichage(configuration?.ServeurCible),
                    BasesTraitees = resultats
                        .Select(resultat => resultat.NomBase)
                        .ToList(),
                    LignesAvant = resultats.Sum(resultat => resultat.LignesAvant),
                    LignesApres = resultats.Sum(resultat => resultat.LignesApres),
                    DureeSecondes = (int)Math.Ceiling(dureeExecution.TotalSeconds),
                    Statut = DeterminerStatutHistorique(resultats),
                    EmailEnvoye = emailEnvoye,
                    DateHeureEmail = emailEnvoye ? DateTime.Now : null,
                    EmailRapport = emailEnvoye ? "Rapport envoyé" : "Non envoyé",
                    CheminFichierLog =
                        _journalisationService.RecupererCheminFichierExecutionActuel(),
                    Etapes = ConvertirResultatsEnEtapesHistorique(resultats)
                };

                _historiqueDataProvider.EnregistrerExecution(historique);

                _journalisationService.EcrireInformation(
                    "Historique de l'exécution enregistré.");
            }
            catch (Exception ex)
            {
                // Si l'historique échoue, on ne bloque pas toute la copie.
                _journalisationService.EcrireAvertissement(
                    $"Impossible d'enregistrer l'historique : {ex.Message}");
            }
        }

        private string DeterminerStatutHistorique(List<ExecutionResultatBaseModel> resultats)
        {
            if (resultats.Any(resultat => resultat.Resultat == "Erreur"))
            {
                return "Échec";
            }

            if (resultats.Any(resultat => resultat.Resultat == "Avertissement"))
            {
                return "Avertissement";
            }

            return "Succès";
        }

        private List<HistoriqueEtapeExecutionModel> ConvertirResultatsEnEtapesHistorique(
            List<ExecutionResultatBaseModel> resultats)
        {
            List<HistoriqueEtapeExecutionModel> etapes =
                new List<HistoriqueEtapeExecutionModel>();

            foreach (ExecutionResultatBaseModel resultat in resultats)
            {
                HistoriqueEtapeExecutionModel etape = new HistoriqueEtapeExecutionModel
                {
                    NomBase = resultat.NomBase,
                    LignesAvant = resultat.LignesAvant,
                    LignesApres = resultat.LignesApres,
                    Statut = resultat.Resultat == "Erreur" ? "Échec" : resultat.Resultat,
                    Message = resultat.Message,
                    Logs = ConstruireLogsAffichageBase(resultat)
                };

                etapes.Add(etape);
            }

            return etapes;
        }
        private List<string> ConstruireLogsAffichageBase(ExecutionResultatBaseModel resultat)
        {
            List<string> logs = new List<string>();

            logs.Add($"[{DateTime.Now:HH:mm:ss}] Traitement de la base {resultat.NomBase}");
            logs.Add($"[{DateTime.Now:HH:mm:ss}] Statut : {resultat.Resultat}");
            logs.Add($"[{DateTime.Now:HH:mm:ss}] Lignes : {resultat.LignesAvant} → {resultat.LignesApres}");

            if (!string.IsNullOrWhiteSpace(resultat.Message))
            {
                logs.Add($"[{DateTime.Now:HH:mm:ss}] {resultat.Message}");
            }

            return logs;
        }

        private string ObtenirServeurAffichage(ServeurConfigModel? serveur)
        {
            if (serveur == null)
            {
                return "Non renseigné";
            }

            if (!string.IsNullOrWhiteSpace(serveur.ChaineConnexion))
            {
                return serveur.ChaineConnexion;
            }

            if (string.IsNullOrWhiteSpace(serveur.NomServeur))
            {
                return "Non renseigné";
            }

            if (serveur.Port > 0)
            {
                return $"{serveur.NomServeur},{serveur.Port}";
            }

            return serveur.NomServeur;
        }
        private string NormaliserOrigineExecution(string origineExecution)
        {
            return origineExecution switch
            {
                "Automatique" => "Automatique",
                "Task Scheduler" => "Automatique",
                "TaskScheduler" => "Automatique",
                "Manuel" => "Manuel",
                _ => "Manuel"
            };
        }
        private string NormaliserModeCopie(string modeCopie)
        {
            return modeCopie switch
            {
                "Ecraser" => "Écraser",
                "Écraser" => "Écraser",
                "Mettre a jour" => "Mise à jour",
                "Mettre à jour" => "Mise à jour",
                "Mise à jour" => "Mise à jour",
                _ => "Écraser"
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

        private void EcrireResultatDansFichier(
            string nomBase,
            ExecutionResultatBaseModel resultat)
        {
            string message = $"{nomBase} : {resultat.Resultat} - {resultat.Message}";

            switch (resultat.Resultat)
            {
                case "Succès":
                    _journalisationService.EcrireSucces(message);
                    break;

                case "Avertissement":
                    _journalisationService.EcrireAvertissement(message);
                    break;

                case "Erreur":
                    _journalisationService.EcrireErreur(message);
                    break;

                default:
                    _journalisationService.EcrireInformation(message);
                    break;
            }
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