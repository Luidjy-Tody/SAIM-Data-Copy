using SaimDataCopy.Models.Authentification;
using SaimDataCopy.Models.EspaceAdmin;
using SaimDataCopy.Services.EspaceAdmin;
using SaimDataCopy.Views.EspaceAdmin;
using System.Windows.Forms;

namespace SaimDataCopy.Controllers.EspaceAdmin
{
    public class EspaceAdminController
    {
        private readonly EspaceAdminView espaceAdminView;
        private readonly EspaceAdminService espaceAdminService;

        public EspaceAdminController(EspaceAdminView espaceAdminView, EspaceAdminService espaceAdminService)
        {
            this.espaceAdminView = espaceAdminView;
            this.espaceAdminService = espaceAdminService;

            this.espaceAdminView.ActualisationDemandee += EspaceAdminView_ActualisationDemandee;
            this.espaceAdminView.RechercheUtilisateursDemandee += EspaceAdminView_RechercheUtilisateursDemandee;

            this.espaceAdminView.NouvelUtilisateurDemande += EspaceAdminView_NouvelUtilisateurDemande;
            this.espaceAdminView.ModificationUtilisateurDemandee += EspaceAdminView_ModificationUtilisateurDemandee;
            this.espaceAdminView.EnregistrementUtilisateurDemande += EspaceAdminView_EnregistrementUtilisateurDemande;
            this.espaceAdminView.AnnulationFormulaireDemandee += EspaceAdminView_AnnulationFormulaireDemandee;

            ChargerPage();
        }

        public async void ChargerPage()
        {
            await ChargerStatistiquesAsync();
            await ChargerUtilisateursAsync();
        }

        private async Task ChargerStatistiquesAsync()
        {
            try
            {
                espaceAdminView.AfficherChargementStatistiques();

                StatistiquesUtilisateursModel statistiques = await espaceAdminService.RecupererStatistiquesUtilisateursAsync();

                espaceAdminView.AfficherStatistiques(statistiques);
            }
            catch (Exception ex)
            {
                espaceAdminView.AfficherErreurStatistiques("Impossible de charger les statistiques des utilisateurs.");

                espaceAdminView.AfficherMessage(
                    "Erreur pendant le chargement des statistiques :" + Environment.NewLine + ex.Message,
                    "Espace Admin",
                    MessageBoxIcon.Error
                );
            }
        }

        private async Task ChargerUtilisateursAsync()
        {
            try
            {
                espaceAdminView.AfficherChargementUtilisateurs();

                string recherche = espaceAdminView.ObtenirRechercheUtilisateur();
                string statut = espaceAdminView.ObtenirStatutUtilisateurFiltre();
                string etat = espaceAdminView.ObtenirEtatUtilisateurFiltre();

                List<UtilisateurModel> utilisateurs = await espaceAdminService.RecupererUtilisateursAsync(recherche, statut, etat);

                espaceAdminView.AfficherUtilisateurs(utilisateurs);
            }
            catch (Exception ex)
            {
                espaceAdminView.AfficherErreurUtilisateurs();

                espaceAdminView.AfficherMessage(
                    "Erreur pendant le chargement des utilisateurs :" + Environment.NewLine + ex.Message,
                    "Espace Admin",
                    MessageBoxIcon.Error
                );
            }
        }

        private void EspaceAdminView_ActualisationDemandee(object? sender, EventArgs e)
        {
            ChargerPage();
        }

        private async void EspaceAdminView_RechercheUtilisateursDemandee(object? sender, EventArgs e)
        {
            await ChargerUtilisateursAsync();
        }

        private void EspaceAdminView_NouvelUtilisateurDemande(object? sender, EventArgs e)
        {
            espaceAdminView.AfficherFormulaireCreation();
        }
        private async void EspaceAdminView_ModificationUtilisateurDemandee(int idUtilisateur)
        {
            try
            {
                UtilisateurModel? utilisateur = await espaceAdminService.RecupererUtilisateurParIdAsync(idUtilisateur);

                if (utilisateur == null)
                {
                    espaceAdminView.AfficherMessage(
                        "Utilisateur introuvable.",
                        "Espace Admin",
                        MessageBoxIcon.Warning
                    );

                    return;
                }

                espaceAdminView.AfficherFormulaireModification(utilisateur);
            }
            catch (Exception ex)
            {
                espaceAdminView.AfficherMessage(
                    "Erreur pendant le chargement de l'utilisateur :" + Environment.NewLine + ex.Message,
                    "Espace Admin",
                    MessageBoxIcon.Error
                );
            }
        }
        private async void EspaceAdminView_EnregistrementUtilisateurDemande(object? sender, EventArgs e)
        {
            try
            {
                espaceAdminView.DefinirFormulaireEnChargement(true);

                ResultatOperationUtilisateurModel resultat = await espaceAdminService.EnregistrerUtilisateurAsync(
                    espaceAdminView.ObtenirIdUtilisateurSelectionne(),
                    espaceAdminView.ObtenirNomComplet(),
                    espaceAdminView.ObtenirIdentifiant(),
                    espaceAdminView.ObtenirEmail(),
                    espaceAdminView.ObtenirMotDePasse(),
                    espaceAdminView.ObtenirConfirmationMotDePasse(),
                    espaceAdminView.ObtenirStatutFormulaire(),
                    espaceAdminView.ObtenirEtatActifFormulaire()
                );

                if (!resultat.Reussite)
                {
                    espaceAdminView.AfficherMessageFormulaire(resultat.Message, false);
                    return;
                }

                espaceAdminView.MasquerFormulaireUtilisateur();
                espaceAdminView.AfficherToastSucces(resultat.Message);

                await ChargerStatistiquesAsync();
                await ChargerUtilisateursAsync();
            }
            catch (Exception ex)
            {
                espaceAdminView.AfficherMessageFormulaire(
                    "Erreur pendant l'enregistrement : " + ex.Message,
                    false
                );
            }
            finally
            {
                espaceAdminView.DefinirFormulaireEnChargement(false);
            }
        }
        private void EspaceAdminView_AnnulationFormulaireDemandee(object? sender, EventArgs e)
        {
            espaceAdminView.MasquerFormulaireUtilisateur();
        }
    }
}