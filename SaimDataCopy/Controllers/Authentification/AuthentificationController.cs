using SaimDataCopy.Models.Authentification;
using SaimDataCopy.Services.Authentification;

namespace SaimDataCopy.Controllers.Authentification
{
    public class AuthentificationController
    {
        private readonly IAuthentificationService _authentificationService;
        private readonly SessionUtilisateurService _sessionUtilisateurService;

        public AuthentificationController()
            : this(new AuthentificationService(), SessionUtilisateurService.Instance)
        {
        }

        public AuthentificationController(
            IAuthentificationService authentificationService,
            SessionUtilisateurService sessionUtilisateurService)
        {
            _authentificationService = authentificationService;
            _sessionUtilisateurService = sessionUtilisateurService;
        }

        public async Task<bool> ConnecterAsync(string identifiantOuEmail, string motDePasse)
        {
            bool connexionOk = await _authentificationService.ConnecterAsync(
                identifiantOuEmail,
                motDePasse
            );

            if (!connexionOk)
            {
                return false;
            }

            UtilisateurModel? utilisateur = _authentificationService.UtilisateurConnecte;

            if (utilisateur == null)
            {
                return false;
            }

            _sessionUtilisateurService.ConnecterUtilisateur(utilisateur);

            return true;
        }

        public async Task<bool> VerifierAuthentificationAdminAsync(string identifiantOuEmail, string motDePasse)
        {
            return await _authentificationService.VerifierAuthentificationAdminAsync(
                identifiantOuEmail,
                motDePasse
            );
        }

        public async Task<bool> InscrireAsync(string nomComplet, string identifiant, string email, string motDePasse, string statut)
        {
            return await _authentificationService.InscrireAsync(
                nomComplet,
                identifiant,
                email,
                motDePasse,
                statut

            );
        }

        public async Task<string> InscrireEtRetournerMessageAsync(
            string nomComplet,
            string identifiant,
            string email,
            string motDePasse,
            string statut)
        {
            return await _authentificationService.InscrireEtRetournerMessageAsync(
                nomComplet,
                identifiant,
                email,
                motDePasse,
                statut
            );
        }

        public async Task AjouterLogAsync(string action, string details)
        {
            UtilisateurModel? utilisateur = _sessionUtilisateurService.UtilisateurConnecte;

            await _authentificationService.AjouterLogAsync(
                utilisateur?.Id,
                utilisateur?.Identifiant ?? "Utilisateur inconnu",
                action,
                details
            );
        }

        public void Deconnecter()
        {
            _sessionUtilisateurService.DeconnecterUtilisateur();
        }

        public void Deverrouiller()
        {
            _sessionUtilisateurService.Deverrouiller();
        }
    }
}