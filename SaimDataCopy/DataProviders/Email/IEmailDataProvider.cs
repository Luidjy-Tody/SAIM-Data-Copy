using SaimDataCopy.Models.Email;

namespace SaimDataCopy.DataProviders.Email
{
    /// <summary>
    /// Interface pour l'accès aux données des paramètres email.
    /// Le DataProvider s'occupe seulement de charger et enregistrer les données.
    /// </summary>
    /// 
    public interface IEmailDataProvider
    {
        /// <summary>
        /// Charge les paramètres email.
        /// </summary>
        /// 
        EmailConfigModel Charger();

        /// <summary>
        /// Enregistre les paramètres email.
        /// </summary>
        /// 
        void Enregistrer(EmailConfigModel configuration);
    }
}
