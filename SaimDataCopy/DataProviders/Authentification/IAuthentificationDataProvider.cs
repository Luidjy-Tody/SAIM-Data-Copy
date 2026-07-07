using SaimDataCopy.Models.Authentification;

namespace SaimDataCopy.DataProviders.Authentification
{
    public interface IAuthentificationDataProvider
    {
        Task<UtilisateurModel?> RecupererUtilisateurParIdentifiantOuEmailAsync(string identifiantOuEmail);

        Task<UtilisateurModel?> RecupererUtilisateurParEmailAsync(string email);

        Task AjouterUtilisateurAsync(UtilisateurModel utilisateur);

        Task ModifierUtilisateurAsync(UtilisateurModel utilisateur);

        Task AjouterLogAsync(LogUtilisateurModel log);
    }
}