namespace SaimDataCopy.Models.BasesCopier
{
    // Model utilisé par Entity Framework Core pour sauvegarder
    // les bases à copier dans SQL Server.
    //
    // Ce Model représente une ligne dans la table BasesCopier.
    public class BaseCopieSqlModel
    {
        public int Id { get; set; }

        // Indique si cette base doit être copiée.
        public bool Inclure { get; set; } = true;

        // Nom de la base trouvée sur le serveur source.
        public string NomBase { get; set; } = string.Empty;

        // Ordre dans lequel la base sera copiée.
        public int OrdreTraitement { get; set; }

        // Mode de copie choisi pour cette base.
        // Exemple : Écraser ou Mise à jour.
        public string ModeCopie { get; set; } = "Écraser";

        // Statut affiché dans l'application.
        // Exemple : Prête, Avertissement, Non sélectionnée.
        public string Statut { get; set; } = "Prête";

        // Date de la dernière copie réussie.
        public DateTime? DerniereCopie { get; set; }

        // Indique si la base existe encore sur le serveur source.
        public bool ExisteSurServeurSource { get; set; } = true;

        // Dans notre cas, le nom vient du serveur source,
        // donc il n'est pas modifiable dans l'interface.
        public bool NomModifiable { get; set; } = false;

        // Date de création de l'enregistrement.
        public DateTime DateCreation { get; set; } = DateTime.Now;

        // Date de dernière modification.
        public DateTime DateModification { get; set; } = DateTime.Now;
    }
}