using SaimDataCopy.Models.Execution;
using SaimDataCopy.Services.Execution;
using SaimDataCopy.Views.Execution;

namespace SaimDataCopy.Controllers.Execution
{
    // Controller de la page Exécution.
    // Il coordonne les clics de l'utilisateur entre la View et le Service.
    public class ExecutionController
    {
        private readonly IExecutionView _view;
        private readonly IExecutionService _service;

        private CancellationTokenSource? _cancellationTokenSource;

        // Permet d'éviter de lancer deux actions en même temps.
        private bool _traitementEnCours;

        public ExecutionController(IExecutionView view, IExecutionService service)
        {
            _view = view;
            _service = service;

            // Connexion des événements de la View.
            _view.TesterConnexionDemandee += View_TesterConnexionDemandee;
            _view.LancerCopieDemandee += View_LancerCopieDemandee;
            _view.AnnulerCopieDemandee += View_AnnulerCopieDemandee;

            ChargerDonneesInitiales();
        }

        public bool EstTraitementEnCours()
        {
            return _traitementEnCours;
        }

        public void AnnulerTraitementEnCoursDepuisMainForm()
        {
            if (!_traitementEnCours || _cancellationTokenSource == null)
            {
                return;
            }

            _cancellationTokenSource.Cancel();
        }
        private void ChargerDonneesInitiales()
        {
            ExecutionTableauBordModel tableauBord =
                _service.ChargerTableauBordInitial();

            List<ExecutionResultatBaseModel> resultats =
                _service.ChargerDerniersResultats();

            _view.AfficherTableauBord(tableauBord);
            _view.AfficherResultats(resultats);

            // Au démarrage, la progression est cachée.
            _view.AfficherZoneProgression(false);

            _view.ActiverBoutonTesterConnexion(true);
            _view.ActiverBoutonLancer(true);
            _view.ActiverBoutonAnnuler(false);
        }

        private async void View_TesterConnexionDemandee(object? sender, EventArgs e)
        {
            if (_traitementEnCours)
            {
                return;
            }

            _traitementEnCours = true;
            _cancellationTokenSource = new CancellationTokenSource();

            try
            {
                _view.ViderJournal();

                // On affiche la progression seulement quand l'utilisateur lance une action.
                _view.AfficherZoneProgression(true);
                _view.AfficherProgression(0, "Test de connexion en cours...");

                _view.ActiverBoutonTesterConnexion(false);
                _view.ActiverBoutonLancer(false);
                _view.ActiverBoutonAnnuler(true);

                Progress<ExecutionProgressionModel> progression =
                    new Progress<ExecutionProgressionModel>(AfficherProgression);

                bool connexionOk = await _service.TesterConnexionAsync(
                    progression,
                    _cancellationTokenSource.Token
                );

                if (connexionOk)
                {
                    // - la lecture des tables d'une base source
                    // - le comptage des lignes de chaque table
                    //TesterLectureTablesBaseSource();

                    

                    _view.AfficherMessage(
                        "Connexion réussie. Vous pouvez lancer la copie.",
                        "Test de connexion"
                    );
                }
                else
                {
                    _view.AfficherMessage(
                        "Connexion non validée. Vérifiez les bases sélectionnées ou la configuration.",
                        "Test de connexion"
                    );
                }
            }
            catch (OperationCanceledException)
            {
                _view.AjouterLog(new ExecutionLogModel
                {
                    Heure = DateTime.Now.ToString("HH:mm:ss"),
                    Message = "Test de connexion annulé par l'utilisateur.",
                    Type = "Avertissement"
                });

                _view.AfficherProgression(0, "Test de connexion annulé.");
            }
            catch (Exception ex)
            {
                _view.AjouterLog(new ExecutionLogModel
                {
                    Heure = DateTime.Now.ToString("HH:mm:ss"),
                    Message = $"Erreur pendant le test de connexion : {ex.Message}",
                    Type = "Erreur"
                });

                _view.AfficherMessage(
                    "Une erreur est survenue pendant le test de connexion.",
                    "Erreur"
                );
            }
            finally
            {
                _traitementEnCours = false;

                _view.ActiverBoutonTesterConnexion(true);
                _view.ActiverBoutonLancer(true);
                _view.ActiverBoutonAnnuler(false);

                _cancellationTokenSource?.Dispose();
                _cancellationTokenSource = null;
            }
        }

        private async void View_LancerCopieDemandee(object? sender, EventArgs e)
        {
            if (_traitementEnCours)
            {
                return;
            }

            _traitementEnCours = true;
            _cancellationTokenSource = new CancellationTokenSource();

            try
            {
                _view.ViderJournal();

                // On affiche la progression seulement quand la copie commence.
                _view.AfficherZoneProgression(true);
                _view.AfficherProgression(0, "Progression : 0%");

                _view.ActiverBoutonTesterConnexion(false);
                _view.ActiverBoutonLancer(false);
                _view.ActiverBoutonAnnuler(true);

                Progress<ExecutionProgressionModel> progression =
                    new Progress<ExecutionProgressionModel>(AfficherProgression);

                List<ExecutionResultatBaseModel> resultats =
                    await _service.LancerCopieAsync(
                        progression,
                        _cancellationTokenSource.Token
                    );

                _view.AfficherResultats(resultats);
            }
            catch (OperationCanceledException)
            {
                _view.AjouterLog(new ExecutionLogModel
                {
                    Heure = DateTime.Now.ToString("HH:mm:ss"),
                    Message = "Copie annulée par l'utilisateur.",
                    Type = "Avertissement"
                });

                _view.AfficherProgression(0, "Copie annulée.");
            }
            catch (Exception ex)
            {
                _view.AjouterLog(new ExecutionLogModel
                {
                    Heure = DateTime.Now.ToString("HH:mm:ss"),
                    Message = $"Erreur pendant l'exécution : {ex.Message}",
                    Type = "Erreur"
                });

                _view.AfficherMessage(
                    "Une erreur est survenue pendant l'exécution.",
                    "Erreur"
                );
            }
            finally
            {
                _traitementEnCours = false;

                _view.ActiverBoutonTesterConnexion(true);
                _view.ActiverBoutonLancer(true);
                _view.ActiverBoutonAnnuler(false);

                _cancellationTokenSource?.Dispose();
                _cancellationTokenSource = null;
            }
        }

        private void View_AnnulerCopieDemandee(object? sender, EventArgs e)
        {
            if (!_traitementEnCours || _cancellationTokenSource == null)
            {
                return;
            }

            _cancellationTokenSource.Cancel();
        }

        private void AfficherProgression(ExecutionProgressionModel progression)
        {
            _view.AfficherProgression(
                progression.Pourcentage,
                progression.MessageProgression
            );

            if (progression.Log != null)
            {
                _view.AjouterLog(progression.Log);
            }

            if (progression.TableauBord != null)
            {
                _view.AfficherTableauBord(progression.TableauBord);
            }
        }

        private void TesterLectureTablesBaseSource()
        {
            string nomBaseTest = "DB_TestRH";

            _view.AjouterLog(new ExecutionLogModel
            {
                Heure = DateTime.Now.ToString("HH:mm:ss"),
                Message = $"Lecture des tables de la base {nomBaseTest}...",
                Type = "Info"
            });

            List<string> tables = _service.ChargerTablesBaseSource(nomBaseTest);

            if (tables.Count == 0)
            {
                _view.AjouterLog(new ExecutionLogModel
                {
                    Heure = DateTime.Now.ToString("HH:mm:ss"),
                    Message = $"Aucune table trouvée dans {nomBaseTest}.",
                    Type = "Avertissement"
                });

                return;
            }

            foreach (string table in tables)
            {
                int nombreLignes =
                    _service.CompterLignesTableSource(nomBaseTest, table);

                _view.AjouterLog(new ExecutionLogModel
                {
                    Heure = DateTime.Now.ToString("HH:mm:ss"),
                    Message = $"Table trouvée : {table} - {nombreLignes} ligne(s)",
                    Type = "Succes"
                });
            }
        } 

        
    }
}