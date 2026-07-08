using SaimDataCopy.Models.Authentification;

namespace SaimDataCopy.DataProviders.Authentification
{
    public interface IAuthentificationDataProvider
    {
        Task<UtilisateurModel?> RecupererUtilisateurParIdentifiantAsync(string identifiant);

        Task<UtilisateurModel?> RecupererUtilisateurParIdentifiantOuEmailAsync(string identifiantOuEmail);

        Task<UtilisateurModel?> RecupererUtilisateurParEmailAsync(string email);

        Task<bool> ExisteAuMoinsUnUtilisateurAsync();

        Task AjouterUtilisateurAsync(UtilisateurModel utilisateur);

        Task ModifierUtilisateurAsync(UtilisateurModel utilisateur);

        Task AjouterLogAsync(LogUtilisateurModel log);

        Task AjouterCodeReinitialisationAsync(CodeReinitialisationMotDePasseModel code);

        Task<CodeReinitialisationMotDePasseModel?> RecupererDernierCodeValideAsync(int utilisateurId);

        Task MarquerCodesUtilisateurCommeUtilisesAsync(int utilisateurId);



    }
}