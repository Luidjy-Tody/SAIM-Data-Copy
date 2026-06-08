namespace SaimDataCopy.Views.Commun
{
    /// <summary>
    /// Interface commune pour les pages qui possèdent des paramètres enregistrables.
    /// Exemple : Configuration, Paramètres Email, Paramètres Logs.
    /// </summary>
    public interface IPageEnregistrable
    {
        /// <summary>
        /// Indique si l'utilisateur a modifié des champs sans enregistrer.
        /// </summary>
        bool ADesModificationsNonEnregistrees { get; }

        /// <summary>
        /// Appelé après un enregistrement réussi.
        /// Cela remet l'état de la page à "aucune modification".
        /// </summary>
        void MarquerCommeEnregistre();

        /// <summary>
        /// Appelé si l'utilisateur choisit "Non".
        /// Cela permet de revenir à la dernière version enregistrée.
        /// </summary>
        void AnnulerModificationsNonEnregistrees();
    }
}