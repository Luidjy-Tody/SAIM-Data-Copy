namespace SaimDataCopy.Models.Email
{
    /// <summary>
    /// Modèle qui représente les paramètres e-mail de l'application.
    /// Ces données seront utilisées par la View, le Service et le DataProvider.
    /// </summary>
    public class EmailConfigModel
    {
        // -----------------------------
        // Serveur SMTP
        // -----------------------------

        public string ServeurSmtp { get; set; } = "";

        public int Port { get; set; } = 587;

        public string Securite { get; set; } = "TLS";

        public string IdentifiantSmtp { get; set; } = "";

        public string MotDePasseSmtp { get; set; } = "";


        // -----------------------------
        // Destinataires
        // -----------------------------

        public string ExpediteurFrom { get; set; } = "";

        public string DestinataireTo { get; set; } = "";

        public string CopieCc { get; set; } = "";

        public string CopieCacheeBcc { get; set; } = "";


        // -----------------------------
        // Contenu du message
        // -----------------------------

        public string Objet { get; set; } = "[SAIM] Copie réussie — {date}";

        public string CorpsMessage { get; set; } =
@"Bonjour,

La copie automatique des bases de données s’est terminée avec succès le {date} à {heure}.

Bases traitées : {liste_bases}
Durée totale : {duree}

Cordialement,
SAIM LTD";


        // -----------------------------
        // Options
        // -----------------------------

        public bool JoindreFichierLog { get; set; } = false;
    }
}