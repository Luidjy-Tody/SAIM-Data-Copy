using SaimDataCopy.Models.Email;

namespace SaimDataCopy.Services.Email
{
    /// <summary>
    /// Interface du service pour les paramètres email.
    /// Le service contient la logique métier : validation, sauvegarde, test d'envoi.
    /// </summary>
    public interface IEmailService
    {
        /// <summary>
        /// Charge les paramètres email.
        /// </summary>
        EmailConfigModel Charger();

        /// <summary>
        /// Enregistre les paramètres email après validation.
        /// Retourne true si tout est correct.
        /// </summary>
        bool Enregistrer(EmailConfigModel configuration, out string message);

        /// <summary>
        /// Envoie un e-mail de test avec les paramètres actuels.
        /// Retourne true si l'envoi fonctionne.
        /// </summary>
        bool EnvoyerEmailTest(EmailConfigModel configuration, out string message);

    }
}
