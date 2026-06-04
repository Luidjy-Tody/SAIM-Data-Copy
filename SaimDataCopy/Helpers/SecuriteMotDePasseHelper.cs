using System.Security.Cryptography;
using System.Text;

namespace SaimDataCopy.Helpers
{
    /// <summary>
    /// Helper pour protéger les mots de passe stockés dans les fichiers JSON.
    /// Ici on utilise DPAPI Windows.
    /// </summary>
    public static class SecuriteMotDePasseHelper
    {
        private const string PrefixeChiffrement = "DPAPI:";

        public static string Chiffrer(string motDePasse)
        {
            if (string.IsNullOrWhiteSpace(motDePasse))
            {
                return string.Empty;
            }

            // Si le mot de passe est déjà chiffré, on ne le chiffre pas une deuxième fois.
            if (motDePasse.StartsWith(PrefixeChiffrement))
            {
                return motDePasse;
            }

            byte[] donneesClaires = Encoding.UTF8.GetBytes(motDePasse);

            byte[] donneesProtegees = ProtectedData.Protect(
                donneesClaires,
                null,
                DataProtectionScope.CurrentUser
            );

            return PrefixeChiffrement + Convert.ToBase64String(donneesProtegees);
        }

        public static string Dechiffrer(string motDePasseChiffre)
        {
            if (string.IsNullOrWhiteSpace(motDePasseChiffre))
            {
                return string.Empty;
            }

            // Si l'ancien JSON contient encore un mot de passe en clair,
            // on le retourne tel quel pour éviter de casser l'application.
            if (!motDePasseChiffre.StartsWith(PrefixeChiffrement))
            {
                return motDePasseChiffre;
            }

            string valeurBase64 = motDePasseChiffre.Replace(PrefixeChiffrement, "");

            byte[] donneesProtegees = Convert.FromBase64String(valeurBase64);

            byte[] donneesClaires = ProtectedData.Unprotect(
                donneesProtegees,
                null,
                DataProtectionScope.CurrentUser
            );

            return Encoding.UTF8.GetString(donneesClaires);
        }
    }
}