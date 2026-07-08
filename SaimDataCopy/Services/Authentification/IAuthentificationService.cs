using SaimDataCopy.Models.Authentification;

namespace SaimDataCopy.Services.Authentification
{
    public interface IAuthentificationService
    {
        Task<bool> ConnecterAsync(string identifiantOuEmail, string motDePasse);

        Task<bool> VerifierAuthentificationAdminAsync(string identifiantOuEmail, string motDePasse);

        Task<bool> InscrireAsync(string nomComplet, string identifiant, string email, string motDePasse, string statut);

        Task<string> InscrireEtRetournerMessageAsync(string nomComplet, string identifiant, string email, string motDePasse, string statut);

        Task AjouterLogAsync(int? utilisateurId, string nomUtilisateur, string action, string details);

        UtilisateurModel? UtilisateurConnecte { get; }

    }
}