using SaimDataCopy.Models.Logs;

namespace SaimDataCopy.Services.Logs
{
    /// <summary>
    /// Interface de service pour les paramètres logs.
    /// Le service contient la logique métier et les validations.
    /// </summary>
    public interface ILogsService
    {
        /// <summary>
        /// Charge la configuration des logs.
        /// </summary>
        LogConfigModel ChargerConfiguration();

        /// <summary>
        /// Enregistre la configuration après validation.
        /// </summary>
        void EnregistrerConfiguration(LogConfigModel configuration);

        /// <summary>
        /// Vérifie si la configuration est correcte.
        /// </summary>
        bool ValiderConfiguration(LogConfigModel configuration, out string messageErreur);
    }
}
