using SaimDataCopy.Models.Authentification;

namespace SaimDataCopy.Services.Authentification
{
    public interface IAuthentificationService
    {
        Task<bool> ConnecterAsync(string identifiantOuEmail, string motDePasse);

        Task<bool> InscrireAsync(string nomComplet, string identifiant, string email, string motDePasse);

        Task AjouterLogAsync(int? utilisateurId, string nomUtilisateur, string action, string details);

        UtilisateurModel? UtilisateurConnecte { get; }

    }
}
