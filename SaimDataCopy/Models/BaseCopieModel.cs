
namespace SaimDataCopy.Models
{
    public class BaseCopieModel
    {
        // Indique si cette base doit être copiée ou non
        public bool Inclure { get; set; }

        // Nom de la base de donnees
        public string NomBase { get; set; } = string.Empty;

        //Ordre de traitement pendant la copie
        public int OrdreTraitement { get; set; }

        //Mode choisi pour la copie : Ecraser ou Mise a jour
        public string ModeCopie { get; set; } = "Écraser";

        //Statut affiche dans le tableau : Prete, Avertissement, Non selectionnee 
        public string Statut { get; set; } = "Prête";

        //Date de la derniere copie.
        //Peut etre null si la base n'a jamais ete copie
        public DateTime? DerniereCopie { get; set; }

    }
}



