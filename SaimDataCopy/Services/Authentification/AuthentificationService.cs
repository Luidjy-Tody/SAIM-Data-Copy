using SaimDataCopy.DataProviders.Authentification;
using SaimDataCopy.Models.Authentification;
using SaimDataCopy.Helpers;

namespace SaimDataCopy.Services.Authentification
{
    public class AuthentificationService : IAuthentificationService
    {
        private readonly IAuthentificationDataProvider _dataProvider;

        public UtilisateurModel? UtilisateurConnecte { get; private set; }

        public AuthentificationService()
            : this(new AuthentificationDataProvider())
        {
        }

        public AuthentificationService(IAuthentificationDataProvider dataProvider)
        {
            _dataProvider = dataProvider;
        }

        public async Task<bool> ConnecterAsync(string identifiantOuEmail, string motDePasse)
        {
            if (string.IsNullOrWhiteSpace(identifiantOuEmail) || string.IsNullOrWhiteSpace(motDePasse))
            {
                return false;
            }

            identifiantOuEmail = identifiantOuEmail.Trim();

            UtilisateurModel? utilisateur =
                await _dataProvider.RecupererUtilisateurParIdentifiantOuEmailAsync(identifiantOuEmail);

            if (utilisateur == null || !utilisateur.EstActif)
            {
                return false;
            }

            bool motDePasseCorrect =
                SecuriteMotDePasseHelper.VerifierMotDePasse(motDePasse, utilisateur.MotDePasseHash);

            if (!motDePasseCorrect)
            {
                return false;
            }

            utilisateur.DerniereConnexion = DateTime.Now;
            await _dataProvider.ModifierUtilisateurAsync(utilisateur);

            UtilisateurConnecte = utilisateur;

            await AjouterLogAsync(
                utilisateur.Id,
                utilisateur.Identifiant,
                "Connexion",
                "Connexion réussie."
            );

            return true;
        }

        public async Task<bool> VerifierAuthentificationAdminAsync(string identifiantOuEmail, string motDePasse)
        {
            if (string.IsNullOrWhiteSpace(identifiantOuEmail) ||
                string.IsNullOrWhiteSpace(motDePasse))
            {
                return false;
            }

            identifiantOuEmail = identifiantOuEmail.Trim();

            UtilisateurModel? utilisateur =
                await _dataProvider.RecupererUtilisateurParIdentifiantOuEmailAsync(identifiantOuEmail);

            if (utilisateur == null || !utilisateur.EstActif)
            {
                return false;
            }

            bool estAdmin =
                utilisateur.Statut.Equals("Admin", StringComparison.OrdinalIgnoreCase);

            if (!estAdmin)
            {
                return false;
            }

            bool motDePasseCorrect =
                SecuriteMotDePasseHelper.VerifierMotDePasse(
                    motDePasse,
                    utilisateur.MotDePasseHash
                );

            return motDePasseCorrect;
        }

        public async Task<bool> InscrireAsync(
            string nomComplet,
            string identifiant,
            string email,
            string motDePasse,
            string statut)
        {
            string message = await InscrireEtRetournerMessageAsync(
                nomComplet,
                identifiant,
                email,
                motDePasse,
                statut
            );

            return message == "Compte créé avec succès. Vous pouvez vous connecter.";
        }

        public async Task<string> InscrireEtRetournerMessageAsync(
    string nomComplet,
    string identifiant,
    string email,
    string motDePasse,
    string statut)
        {
            if (string.IsNullOrWhiteSpace(nomComplet) ||
                string.IsNullOrWhiteSpace(identifiant) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(motDePasse))
            {
                return "Veuillez remplir tous les champs obligatoires.";
            }

            nomComplet = nomComplet.Trim();
            identifiant = identifiant.Trim();
            email = email.Trim();
            statut = NormaliserStatutUtilisateur(statut);

            UtilisateurModel? identifiantExistant =
                await _dataProvider.RecupererUtilisateurParIdentifiantAsync(identifiant);

            if (identifiantExistant != null)
            {
                return "Cet identifiant existe déjà.";
            }

            UtilisateurModel? emailExistant =
                await _dataProvider.RecupererUtilisateurParEmailAsync(email);

            if (emailExistant != null)
            {
                return "Cet email existe déjà.";
            }

            UtilisateurModel utilisateur = new UtilisateurModel
            {
                NomComplet = nomComplet,
                Identifiant = identifiant,
                Email = email,
                MotDePasseHash = SecuriteMotDePasseHelper.HasherMotDePasse(motDePasse),
                DateCreation = DateTime.Now,
                EstActif = true,
                Statut = statut
            };

            await _dataProvider.AjouterUtilisateurAsync(utilisateur);

            await AjouterLogAsync(
                utilisateur.Id,
                utilisateur.Identifiant,
                "Inscription",
                "Nouveau compte utilisateur créé avec le statut : " + statut + "."
            );

            return "Compte créé avec succès. Vous pouvez vous connecter.";
        }

        private static string NormaliserStatutUtilisateur(string statut)
        {
            if (string.IsNullOrWhiteSpace(statut))
            {
                return "User";
            }

            statut = statut.Trim();

            if (statut.Equals("Admin", StringComparison.OrdinalIgnoreCase))
            {
                return "Admin";
            }

            return "User";
        }

        public async Task AjouterLogAsync(
            int? utilisateurId,
            string nomUtilisateur,
            string action,
            string details)
        {
            LogUtilisateurModel log = new LogUtilisateurModel
            {
                UtilisateurId = utilisateurId,
                NomUtilisateur = nomUtilisateur,
                Action = action,
                Details = details,
                DateHeure = DateTime.Now
            };

            await _dataProvider.AjouterLogAsync(log);
        }
    }
}