using SaimDataCopy.Models.Authentification;

namespace SaimDataCopy.Services.Authentification
{
    public class SessionUtilisateurService
    {
        public static SessionUtilisateurService Instance { get; } = new SessionUtilisateurService();

        private readonly TimeSpan _dureeInactivite = TimeSpan.FromMinutes(5);

        private SessionUtilisateurService()
        {

        }

        public UtilisateurModel? UtilisateurConnecte { get; private set; }

        public DateTime DerniereActivite { get; private set; } = DateTime.Now;

        public bool EstVerrouille { get; private set; }

        public void ConnecterUtilisateur(UtilisateurModel utilisateur)
        {
            UtilisateurConnecte = utilisateur;
            EstVerrouille = false;
            ActualiserActivite();
        }

        public void DeconnecterUtilisateur()
        {
            UtilisateurConnecte = null;
            EstVerrouille = false;
            ActualiserActivite();
        }

        public void ActualiserActivite()
        {
            DerniereActivite = DateTime.Now;
        }

        public bool DoitVerrouiller()
        {
            if (UtilisateurConnecte == null)
            {
                return false;
            }

            if (EstVerrouille)
            {
                return false;
            }

            return DateTime.Now - DerniereActivite >= _dureeInactivite;
        }

        public void Verrouiller()
        {
            if (UtilisateurConnecte == null)
            {
                return;
            }

            EstVerrouille = true;
        }

        public void Deverrouiller()
        {
            if (UtilisateurConnecte == null)
            {
                return;
            }

            EstVerrouille = false;
            ActualiserActivite();
        }
    }
}