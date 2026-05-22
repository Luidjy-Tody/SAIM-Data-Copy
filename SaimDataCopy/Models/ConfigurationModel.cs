namespace SaimDataCopy.Models
{
    // Model principal pour la page Configuration.
    // Il regroupe les paramètres du serveur source, du serveur cible,
    // et le comportement en cas d'erreur.
    public class ConfigurationModel
    {
        public ServeurConfigModel ServeurSource { get; set; } = new ServeurConfigModel();

        public ServeurConfigModel ServeurCible { get; set; } = new ServeurConfigModel();

        public string ModeCopie { get; set; } = string.Empty;

        public string ComportementErreur { get; set; } = string.Empty;

        public int TentativesReprise { get; set; }
    }
}