namespace SaimDataCopy.Models.BasesCopier
{
    public class BaseCopieModel
    {
        // Indique si cette base doit être copiée ou non.
        // Dans la nouvelle logique, toutes les bases sont cochées par défaut.
        public bool Inclure { get; set; } = true;

        // Nom de la base de données trouvée sur le serveur source.
        // Ce nom doit être affiché en lecture seule dans la View.
        public string NomBase { get; set; } = string.Empty;

        // Ordre de traitement pendant la copie.
        public int OrdreTraitement { get; set; }

        // Mode choisi pour cette base.
        // Exemple : Écraser, Mise à jour.
        public string ModeCopie { get; set; } = "Écraser";

        // Statut affiché dans le tableau.
        // Exemple : Prête, Avertissement, Non sélectionnée.
        public string Statut { get; set; } = "Prête";

        // Date de la dernière copie.
        // Peut être null si la base n'a jamais été copiée.
        public DateTime? DerniereCopie { get; set; }

        // Indique si la base vient vraiment du serveur source.
        // Plus tard, cela aidera à savoir si une base existe encore ou non.
        public bool ExisteSurServeurSource { get; set; } = true;

        // Indique si le nom de la base doit être modifiable dans l'interface.
        // Ici, c'est false parce que les bases viennent du serveur source.
        public bool NomModifiable { get; set; } = false;
    }
}