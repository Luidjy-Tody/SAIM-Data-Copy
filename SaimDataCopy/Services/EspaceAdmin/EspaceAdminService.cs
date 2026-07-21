using SaimDataCopy.DataProviders.EspaceAdmin;
using SaimDataCopy.Models.EspaceAdmin;
using SaimDataCopy.Models.Authentification;
using SaimDataCopy.Helpers;
using SaimDataCopy.Services.Authentification;
using System.Net.Mail;

namespace SaimDataCopy.Services.EspaceAdmin
{
    /// <summary>
    /// Service métier de l'Espace Admin.
    /// </summary>
    public class EspaceAdminService
    {
        private readonly EspaceAdminDataProvider espaceAdminDataProvider;
        private readonly SessionUtilisateurService sessionUtilisateurService = SessionUtilisateurService.Instance;

        public EspaceAdminService(
            EspaceAdminDataProvider espaceAdminDataProvider)
        {
            this.espaceAdminDataProvider = espaceAdminDataProvider;
        }

        public async Task<StatistiquesUtilisateursModel>
            RecupererStatistiquesUtilisateursAsync()
        {
            return await espaceAdminDataProvider.RecupererStatistiquesUtilisateursAsync();
        }

        public async Task<List<UtilisateurModel>> RecupererUtilisateursAsync(string recherche, string statut, string etat)
        {
            return await espaceAdminDataProvider.RecupererUtilisateursAsync(recherche, statut, etat);
        }

        public async Task<UtilisateurModel?> RecupererUtilisateurParIdAsync(int idUtilisateur)
        {
            return await espaceAdminDataProvider.RecupererUtilisateurParIdAsync(idUtilisateur);
        }

        public async Task<ResultatOperationUtilisateurModel> EnregistrerUtilisateurAsync(
            int? idUtilisateur,
            string nomComplet,
            string identifiant,
            string email,
            string motDePasse,
            string confirmationMotDePasse,
            string statut,
            bool estActif)
        {
            nomComplet = nomComplet.Trim();
            identifiant = identifiant.Trim();
            email = email.Trim().ToLowerInvariant();
            statut = NormaliserStatut(statut);

            if (string.IsNullOrWhiteSpace(nomComplet) ||
                string.IsNullOrWhiteSpace(identifiant) ||
                string.IsNullOrWhiteSpace(email))
            {
                return CreerResultat(false, "Veuillez remplir tous les champs obligatoires.");
            }

            if (!EstEmailValide(email))
            {
                return CreerResultat(false, "Adresse email invalide.");
            }

            bool creation = !idUtilisateur.HasValue;

            if (creation && string.IsNullOrWhiteSpace(motDePasse))
            {
                return CreerResultat(false, "Le mot de passe est obligatoire lors de la création.");
            }

            if (!string.IsNullOrWhiteSpace(motDePasse) && motDePasse != confirmationMotDePasse)
            {
                return CreerResultat(false, "Les mots de passe ne correspondent pas.");
            }

            bool identifiantExiste = await espaceAdminDataProvider.IdentifiantExisteAsync(identifiant, idUtilisateur);

            if (identifiantExiste)
            {
                return CreerResultat(false, "Cet identifiant existe déjà.");
            }

            bool emailExiste = await espaceAdminDataProvider.EmailExisteAsync(email, idUtilisateur);

            if (emailExiste)
            {
                return CreerResultat(false, "Cet email existe déjà.");
            }

            if (creation)
            {
                UtilisateurModel nouvelUtilisateur = new UtilisateurModel
                {
                    NomComplet = nomComplet,
                    Identifiant = identifiant,
                    Email = email,
                    MotDePasseHash = SecuriteMotDePasseHelper.HasherMotDePasse(motDePasse),
                    DateCreation = DateTime.Now,
                    DerniereConnexion = null,
                    EstActif = estActif,
                    Statut = statut
                };

                await espaceAdminDataProvider.AjouterUtilisateurAsync(nouvelUtilisateur);

                await AjouterLogAdministrationAsync(
                    "Création utilisateur",
                    $"Compte \"{nouvelUtilisateur.Identifiant}\" créé avec le statut {nouvelUtilisateur.Statut}."
                );

                return CreerResultat(true, "Utilisateur créé avec succès.");
            }

            UtilisateurModel? utilisateur = await espaceAdminDataProvider.RecupererUtilisateurParIdAsync(idUtilisateur!.Value);

            if (utilisateur == null)
            {
                return CreerResultat(false, "Utilisateur introuvable.");
            }

            UtilisateurModel? utilisateurConnecte = sessionUtilisateurService.UtilisateurConnecte;

            bool estCompteConnecte = utilisateurConnecte != null && utilisateurConnecte.Id == utilisateur.Id;

            bool retraitStatutAdmin =
                utilisateur.Statut.Equals("Admin", StringComparison.OrdinalIgnoreCase) &&
                statut.Equals("User", StringComparison.OrdinalIgnoreCase);

            bool desactivationCompte = utilisateur.EstActif && !estActif;

            if (estCompteConnecte && retraitStatutAdmin)
            {
                return CreerResultat(false, "Vous ne pouvez pas retirer votre propre statut administrateur.");
            }

            if (estCompteConnecte && desactivationCompte)
            {
                return CreerResultat(false, "Vous ne pouvez pas désactiver votre propre compte.");
            }

            bool retraitAccesAdministrateur =
                utilisateur.Statut.Equals("Admin", StringComparison.OrdinalIgnoreCase) &&
                (
                    statut.Equals("User", StringComparison.OrdinalIgnoreCase) ||
                    !estActif
                );

            if (retraitAccesAdministrateur && await EstDernierAdministrateurActifAsync(utilisateur))
            {
                return CreerResultat(false, "Il doit rester au moins un administrateur actif.");
            }

            // Seulement après toutes les vérifications, on applique les modifications.
            string ancienIdentifiant = utilisateur.Identifiant;
            string ancienStatut = utilisateur.Statut;
            bool ancienEtat = utilisateur.EstActif;

            utilisateur.NomComplet = nomComplet;
            utilisateur.Identifiant = identifiant;
            utilisateur.Email = email;
            utilisateur.Statut = statut;
            utilisateur.EstActif = estActif;

            if (!string.IsNullOrWhiteSpace(motDePasse))
            {
                utilisateur.MotDePasseHash = SecuriteMotDePasseHelper.HasherMotDePasse(motDePasse);
            }

            await espaceAdminDataProvider.ModifierUtilisateurAsync(utilisateur);

            string details = ConstruireDetailsModification(
                ancienIdentifiant,
                utilisateur.Identifiant,
                ancienStatut,
                utilisateur.Statut,
                ancienEtat,
                utilisateur.EstActif,
                !string.IsNullOrWhiteSpace(motDePasse)
            );

            await AjouterLogAdministrationAsync("Modification utilisateur", details);

            return CreerResultat(true, "Utilisateur modifié avec succès.");
        }

        private async Task AjouterLogAdministrationAsync(string action, string details)
        {
            UtilisateurModel? utilisateurConnecte = sessionUtilisateurService.UtilisateurConnecte;

            LogUtilisateurModel log = new LogUtilisateurModel
            {
                UtilisateurId = utilisateurConnecte?.Id,
                NomUtilisateur = utilisateurConnecte?.Identifiant ?? "Administrateur inconnu",
                Action = action,
                Details = details,
                DateHeure = DateTime.Now
            };

            await espaceAdminDataProvider.AjouterLogAsync(log);
        }

        private static ResultatOperationUtilisateurModel CreerResultat(bool reussite, string message)
        {
            return new ResultatOperationUtilisateurModel
            {
                Reussite = reussite,
                Message = message
            };
        }
        private static string NormaliserStatut(string statut)
        {
            return statut.Equals("Admin", StringComparison.OrdinalIgnoreCase) ? "Admin" : "User";
        }
        private static bool EstEmailValide(string email)
        {
            if (string.IsNullOrWhiteSpace(email) || email.Contains(' '))
            {
                return false;
            }

            try
            {
                MailAddress adresse = new MailAddress(email);

                return adresse.Address == email && adresse.Host.Contains('.') && adresse.Host.Split('.').All(partie => !string.IsNullOrWhiteSpace(partie));
            }
            catch
            {
                return false;
            }
        }
        private static string ConstruireDetailsModification(
            string ancienIdentifiant,
            string nouvelIdentifiant,
            string ancienStatut,
            string nouveauStatut,
            bool ancienEtat,
            bool nouvelEtat,
            bool motDePasseModifie)
        {
            List<string> modifications = new List<string>();

            if (!ancienIdentifiant.Equals(nouvelIdentifiant, StringComparison.Ordinal))
            {
                modifications.Add($"identifiant : {ancienIdentifiant} → {nouvelIdentifiant}");
            }

            if (!ancienStatut.Equals(nouveauStatut, StringComparison.Ordinal))
            {
                modifications.Add($"statut : {ancienStatut} → {nouveauStatut}");
            }

            if (ancienEtat != nouvelEtat)
            {
                modifications.Add($"état : {(ancienEtat ? "Actif" : "Inactif")} → {(nouvelEtat ? "Actif" : "Inactif")}");
            }

            if (motDePasseModifie)
            {
                modifications.Add("mot de passe modifié");
            }

            if (modifications.Count == 0)
            {
                modifications.Add("informations générales mises à jour");
            }

            return $"Compte \"{nouvelIdentifiant}\" modifié : {string.Join(", ", modifications)}.";
        }


        private async Task<bool> EstDernierAdministrateurActifAsync(UtilisateurModel utilisateur)
        {
            if (!utilisateur.EstActif || !utilisateur.Statut.Equals("Admin", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            int nombreAdministrateursActifs = await espaceAdminDataProvider.CompterAdministrateursActifsAsync();

            return nombreAdministrateursActifs <= 1;
        }

        public async Task<ResultatOperationUtilisateurModel> ActiverDesactiverUtilisateurAsync(int idUtilisateur)
        {
            UtilisateurModel? utilisateur = await espaceAdminDataProvider.RecupererUtilisateurParIdAsync(idUtilisateur);

            if (utilisateur == null)
            {
                return CreerResultat(false, "Utilisateur introuvable.");
            }

            UtilisateurModel? utilisateurConnecte = sessionUtilisateurService.UtilisateurConnecte;

            if (utilisateurConnecte != null && utilisateurConnecte.Id == utilisateur.Id)
            {
                return CreerResultat(false, "Vous ne pouvez pas modifier l'état de votre propre compte.");
            }

            if (utilisateur.EstActif &&
                utilisateur.Statut.Equals("Admin", StringComparison.OrdinalIgnoreCase) &&
                await EstDernierAdministrateurActifAsync(utilisateur))
            {
                return CreerResultat(false, "Il doit rester au moins un administrateur actif.");
            }

            bool nouvelEtat = !utilisateur.EstActif;

            await espaceAdminDataProvider.ActiverDesactiverUtilisateurAsync(utilisateur.Id, nouvelEtat);

            await AjouterLogAdministrationAsync(
                nouvelEtat ? "Activation utilisateur" : "Désactivation utilisateur",
                $"Compte \"{utilisateur.Identifiant}\" {(nouvelEtat ? "activé" : "désactivé")}."
            );

            return CreerResultat(
                true,
                nouvelEtat
                    ? "Utilisateur activé avec succès."
                    : "Utilisateur désactivé avec succès."
            );
        }

        public async Task<ResultatOperationUtilisateurModel> SupprimerUtilisateurAsync(int idUtilisateur)
        {
            UtilisateurModel? utilisateur = await espaceAdminDataProvider.RecupererUtilisateurParIdAsync(idUtilisateur);

            if (utilisateur == null)
            {
                return CreerResultat(false, "Utilisateur introuvable.");
            }

            UtilisateurModel? utilisateurConnecte = sessionUtilisateurService.UtilisateurConnecte;

            if (utilisateurConnecte != null && utilisateurConnecte.Id == utilisateur.Id)
            {
                return CreerResultat(false, "Vous ne pouvez pas supprimer votre propre compte.");
            }

            if (utilisateur.EstActif &&
                utilisateur.Statut.Equals("Admin", StringComparison.OrdinalIgnoreCase) &&
                await EstDernierAdministrateurActifAsync(utilisateur))
            {
                return CreerResultat(false, "Il doit rester au moins un administrateur actif.");
            }

            string identifiantUtilisateurSupprime = utilisateur.Identifiant;
            string statutUtilisateurSupprime = utilisateur.Statut;
            bool etatUtilisateurSupprime = utilisateur.EstActif;

            await espaceAdminDataProvider.SupprimerUtilisateurAsync(utilisateur.Id);

            await AjouterLogAdministrationAsync(
                "Suppression utilisateur",
                $"Compte \"{identifiantUtilisateurSupprime}\" supprimé. Statut : {statutUtilisateurSupprime}. État avant suppression : {(etatUtilisateurSupprime ? "Actif" : "Inactif")}."
            );

            return CreerResultat(true, "Utilisateur supprimé avec succès.");
        }
    }
}