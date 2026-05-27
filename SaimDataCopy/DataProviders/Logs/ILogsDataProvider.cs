using SaimDataCopy.Models.Logs;

namespace SaimDataCopy.DataProviders.Logs
{
    /// <summary>
    /// Interface pour l'accès aux données des paramètres logs.
    /// Ici, on prépare l'enregistrement et le chargement de la configuration.
    /// </summary>
    
    public interface ILogsDataProvider
    {
        /// <summary>
        /// Charge la configuration des logs.
        /// </summary>
        LogConfigModel ChargerConfiguration();

        /// <summary>
        /// Enregistre la configuration des logs.
        /// </summary>
        void EnregistrerConfiguration(LogConfigModel configuration);
    }
}
