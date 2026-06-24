using SaimDataCopy.Models.BasesCopier;
using SaimDataCopy.Models.Execution;

namespace SaimDataCopy.DataProviders.Execution
{
    // Interface du DataProvider Exécution.
    // Elle définit les opérations nécessaires pour tester la connexion,
    // préparer les bases, copier les données et sauvegarder le résultat.
    public interface IExecutionDataProvider
    {
        // Charge les bases cochées dans la configuration des bases à copier.
        List<BaseCopieModel> ChargerBasesSelectionnees();

        // Teste réellement la connexion au serveur source.
        // Cela vérifie le serveur, le port, l'identifiant et le mot de passe.
        void TesterConnexionSource();

        // Teste réellement la connexion au serveur cible.
        // Cela vérifie le serveur, le port, l'identifiant et le mot de passe.
        void TesterConnexionCible();

        // Charge les tables d'une base source.
        // Exemple SQL Server : DB_TestRH peut retourner dbo.EmployesTest.
        // Exemple MySQL : sakila peut retourner actor.
        List<string> ChargerTablesBaseSource(string nomBase);

        // Compte le nombre de lignes dans une table d'une base source.
        int CompterLignesTableSource(string nomBase, string nomTable);

        // Compte le nombre de lignes dans une table de la base cible.
        int CompterLignesTableCible(string nomBase, string nomTable);

        // Vérifie si une base existe sur le serveur cible.
        // Si elle n'existe pas, elle est créée.
        // Retourne true si la base a été créée, false si elle existait déjà.
        bool VerifierOuCreerBaseCible(string nomBase);

        // Vérifie si une table existe sur le serveur cible.
        // Si elle n'existe pas, elle est créée.
        // Retourne true si la table a été créée, false si elle existait déjà.
        bool VerifierOuCreerTableCible(string nomBase, string nomTable);

        // Copie les lignes d'une table source vers la table cible.
        int CopierLignesTableSourceVersCible(string nomBase, string nomTable, string modeCopie, CancellationToken cancellationToken);

        // Recrée les contraintes de clés étrangères après la copie.
        // Pour SQL Server, cette méthode ne fait rien pour l'instant.
        // Pour MySQL, elle ajoute les FOREIGN KEY après création des tables.
        void RecreerContraintesForeignKey(string nomBase);

        // Charge le tableau de bord de la dernière exécution.
        ExecutionTableauBordModel ChargerDernierTableauBord();

        // Charge le résumé de la dernière exécution.
        List<ExecutionResultatBaseModel> ChargerDerniersResultats();

        // Enregistre le résultat de la dernière exécution.
        void EnregistrerDerniereExecution(
            ExecutionTableauBordModel tableauBord,
            List<ExecutionResultatBaseModel> resultats);
    }
}