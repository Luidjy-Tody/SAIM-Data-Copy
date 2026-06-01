using Microsoft.EntityFrameworkCore;
using SaimDataCopy.DataAccess;
using SaimDataCopy.Models.BasesCopier;
using SaimDataCopy.Models.Configuration;

namespace SaimDataCopy.DataProviders.BasesCopier
{
    // DataProvider pour la page Bases à copier.
    // il sauvegarde l'état dans SQL Server avec Entity Framework Core.
    public class BasesCopierDataProvider : IBasesCopierDataProvider
    {
        public List<BaseCopieModel> ChargerBasesDepuisServeurSource()
        {
            // On récupère d'abord la configuration sauvegardée dans SaimDataCopyDb.
            using SaimDbContext dbContextApplication = SaimDbContextFactory.CreerDbContext();

            var configuration = dbContextApplication.Configurations
                .OrderBy(c => c.Id)
                .FirstOrDefault();

            if (configuration == null)
            {
                return new List<BaseCopieModel>();
            }

            string chaineConnexionSource = ConstruireChaineConnexionSource(configuration);

            DbContextOptions<SaimDbContext> optionsSource =
                new DbContextOptionsBuilder<SaimDbContext>()
                    .UseSqlServer(chaineConnexionSource)
                    .Options;

            // Ici, on crée un DbContext connecté au serveur source.
            using SaimDbContext dbContextSource = new SaimDbContext(optionsSource);

            List<string> nomsBases = dbContextSource.Database
                .SqlQueryRaw<string>(
                    """
            SELECT name AS Value
            FROM sys.databases
            WHERE name NOT IN ('master', 'model', 'msdb', 'tempdb', 'SaimDataCopyDb')
            ORDER BY name
            """
                )
                .ToList();

            List<BaseCopieModel> bases = new List<BaseCopieModel>();

            int ordre = 1;

            foreach (string nomBase in nomsBases)
            {
                bases.Add(new BaseCopieModel
                {
                    Inclure = true,
                    NomBase = nomBase,
                    OrdreTraitement = ordre,
                    ModeCopie = "Écraser",
                    Statut = "Prête",
                    DerniereCopie = null,
                    ExisteSurServeurSource = true,
                    NomModifiable = false
                });

                ordre++;
            }

            return bases;
        }

        private string ConstruireChaineConnexionSource(ConfigurationSqlModel configuration)
        {
            // Si une chaîne complète est déjà saisie dans Configuration,
            // on l'utilise directement.
            if (!string.IsNullOrWhiteSpace(configuration.ServeurSourceChaineConnexion))
            {
                return configuration.ServeurSourceChaineConnexion;
            }

            string nomServeur = configuration.ServeurSourceNom.Trim();

            // LocalDB ne fonctionne pas avec un port.
            bool estLocalDb = nomServeur.Contains(
                "(localdb)",
                StringComparison.OrdinalIgnoreCase
            );

            string serveurAvecPort = nomServeur;

            if (!estLocalDb && configuration.ServeurSourcePort > 0)
            {
                serveurAvecPort = $"{nomServeur},{configuration.ServeurSourcePort}";
            }

            bool utiliseAuthentificationSql =
                !string.IsNullOrWhiteSpace(configuration.ServeurSourceIdentifiant);

            if (utiliseAuthentificationSql)
            {
                return
                    $"Server={serveurAvecPort};" +
                    "Database=master;" +
                    $"User Id={configuration.ServeurSourceIdentifiant};" +
                    $"Password={configuration.ServeurSourceMotDePasse};" +
                    "TrustServerCertificate=True;";
            }

            return
                $"Server={serveurAvecPort};" +
                "Database=master;" +
                "Trusted_Connection=True;" +
                "TrustServerCertificate=True;";
        }
        public List<BaseCopieModel> ChargerBasesSauvegardees()
        {
            List<BaseCopieModel> basesServeur = ChargerBasesDepuisServeurSource();

            using SaimDbContext dbContext = SaimDbContextFactory.CreerDbContext();

            List<BaseCopieSqlModel> basesSql = dbContext.BasesCopier
                .OrderBy(b => b.OrdreTraitement)
                .ToList();

            // Si aucune sauvegarde SQL n'existe encore,
            // on retourne les bases du serveur source cochées par défaut.
            if (basesSql.Count == 0)
            {
                return basesServeur;
            }

            List<BaseCopieModel> basesSauvegardees = basesSql
                .Select(ConvertirVersBaseCopieModel)
                .ToList();

            return FusionnerBasesServeurEtSauvegarde(
                basesServeur,
                basesSauvegardees
            );
        }

        public void EnregistrerBases(List<BaseCopieModel> bases)
        {
            using SaimDbContext dbContext = SaimDbContextFactory.CreerDbContext();

            // Liste des bases actuellement visibles dans la page.
            // Ce sont les bases réellement trouvées sur le serveur source.
            List<string> nomsBasesActuelles = bases
                .Select(b => b.NomBase.Trim())
                .ToList();

            // On charge les anciennes bases déjà enregistrées dans SQL.
            List<BaseCopieSqlModel> basesSqlExistantes = dbContext.BasesCopier
                .ToList();

            // On supprime les bases qui étaient sauvegardées avant,
            // mais qui n'existent plus dans la liste actuelle.
            List<BaseCopieSqlModel> basesASupprimer = basesSqlExistantes
                .Where(b => !nomsBasesActuelles.Any(nom =>
                    nom.Equals(b.NomBase, StringComparison.OrdinalIgnoreCase)))
                .ToList();

            if (basesASupprimer.Count > 0)
            {
                dbContext.BasesCopier.RemoveRange(basesASupprimer);
            }

            foreach (BaseCopieModel baseCopie in bases)
            {
                string nomBase = baseCopie.NomBase.Trim();

                BaseCopieSqlModel? baseSql = dbContext.BasesCopier
                    .FirstOrDefault(b => b.NomBase == nomBase);

                if (baseSql == null)
                {
                    baseSql = new BaseCopieSqlModel
                    {
                        NomBase = nomBase,
                        DateCreation = DateTime.Now
                    };

                    dbContext.BasesCopier.Add(baseSql);
                }

                RemplirBaseCopieSql(baseSql, baseCopie);
            }

            dbContext.SaveChanges();
        }
        public void AppliquerModeCopieGlobal(string modeCopieGlobal)
        {
            using SaimDbContext dbContext = SaimDbContextFactory.CreerDbContext();

            List<BaseCopieSqlModel> basesSql = dbContext.BasesCopier
                .OrderBy(b => b.OrdreTraitement)
                .ToList();

            // Si rien n'est encore sauvegardé,
            // on crée d'abord les bases depuis la liste simulée.
            if (basesSql.Count == 0)
            {
                List<BaseCopieModel> basesServeur = ChargerBasesDepuisServeurSource();

                foreach (BaseCopieModel baseCopie in basesServeur)
                {
                    BaseCopieSqlModel nouvelleBaseSql = new BaseCopieSqlModel
                    {
                        NomBase = baseCopie.NomBase,
                        DateCreation = DateTime.Now
                    };

                    RemplirBaseCopieSql(nouvelleBaseSql, baseCopie);
                    dbContext.BasesCopier.Add(nouvelleBaseSql);
                    basesSql.Add(nouvelleBaseSql);
                }
            }

            foreach (BaseCopieSqlModel baseSql in basesSql)
            {
                baseSql.ModeCopie = NormaliserModeCopie(modeCopieGlobal);
                baseSql.DateModification = DateTime.Now;
            }

            dbContext.SaveChanges();
        }

        private void RemplirBaseCopieSql(
            BaseCopieSqlModel baseSql,
            BaseCopieModel baseCopie)
        {
            baseSql.Inclure = baseCopie.Inclure;
            baseSql.NomBase = baseCopie.NomBase.Trim();
            baseSql.OrdreTraitement = baseCopie.OrdreTraitement;
            baseSql.ModeCopie = NormaliserModeCopie(baseCopie.ModeCopie);
            baseSql.Statut = baseCopie.Statut;
            baseSql.DerniereCopie = baseCopie.DerniereCopie;
            baseSql.ExisteSurServeurSource = baseCopie.ExisteSurServeurSource;
            baseSql.NomModifiable = false;
            baseSql.DateModification = DateTime.Now;
        }

        private BaseCopieModel ConvertirVersBaseCopieModel(BaseCopieSqlModel baseSql)
        {
            return new BaseCopieModel
            {
                Inclure = baseSql.Inclure,
                NomBase = baseSql.NomBase,
                OrdreTraitement = baseSql.OrdreTraitement,
                ModeCopie = NormaliserModeCopie(baseSql.ModeCopie),
                Statut = baseSql.Statut,
                DerniereCopie = baseSql.DerniereCopie,
                ExisteSurServeurSource = baseSql.ExisteSurServeurSource,
                NomModifiable = false
            };
        }

        private List<BaseCopieModel> FusionnerBasesServeurEtSauvegarde(
            List<BaseCopieModel> basesServeur,
            List<BaseCopieModel> basesSauvegardees)
        {
            List<BaseCopieModel> resultat = new List<BaseCopieModel>();

            foreach (BaseCopieModel baseServeur in basesServeur)
            {
                BaseCopieModel? baseSauvegardee = basesSauvegardees
                    .FirstOrDefault(b =>
                        b.NomBase.Equals(
                            baseServeur.NomBase,
                            StringComparison.OrdinalIgnoreCase
                        )
                    );

                if (baseSauvegardee == null)
                {
                    // Nouvelle base trouvée sur le serveur source.
                    // Par défaut, elle est cochée.
                    resultat.Add(baseServeur);
                    continue;
                }

                // La base existe déjà dans SQL.
                // On garde les choix de l'utilisateur.
                resultat.Add(new BaseCopieModel
                {
                    Inclure = baseSauvegardee.Inclure,
                    NomBase = baseServeur.NomBase,
                    OrdreTraitement = baseSauvegardee.OrdreTraitement,
                    ModeCopie = NormaliserModeCopie(baseSauvegardee.ModeCopie),
                    Statut = baseSauvegardee.Inclure
                        ? baseServeur.Statut
                        : "Non sélectionnée",
                    DerniereCopie = baseSauvegardee.DerniereCopie,
                    ExisteSurServeurSource = true,
                    NomModifiable = false
                });
            }

            return resultat
                .OrderBy(b => b.OrdreTraitement)
                .ToList();
        }

        private string NormaliserModeCopie(string modeCopie)
        {
            return modeCopie switch
            {
                "Mettre à jour" => "Mise à jour",
                "Mise à jour" => "Mise à jour",
                "Écraser" => "Écraser",
                _ => "Écraser"
            };
        }
    }
}