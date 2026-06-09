using SaimDataCopy.DataProviders.BasesCopier;
using SaimDataCopy.DataProviders.Configuration;
using SaimDataCopy.Models.Configuration;

namespace SaimDataCopy.DataProviders.Execution
{
    // Factory qui choisit le bon DataProvider d'exécution
    // selon le type du serveur source.
    public static class ExecutionDataProviderFactory
    {
        public static IExecutionDataProvider Creer()
        {
            IConfigurationDataProvider configurationDataProvider = new ConfigurationDataProvider();

            ConfigurationModel? configuration = configurationDataProvider.ChargerConfiguration();


            string typeServeurSource =
                configuration?.ServeurSource.TypeServeur ?? "SQL Server";

            IBasesCopierDataProvider basesCopierDataProvider =
                BasesCopierDataProviderFactory.Creer();

            return typeServeurSource switch
            {
                "MySQL" => new MySqlExecutionDataProvider(basesCopierDataProvider, configurationDataProvider),

                "SQL Server" => new ExecutionDataProvider(basesCopierDataProvider, configurationDataProvider),

                _ => new ExecutionDataProvider(basesCopierDataProvider, configurationDataProvider)
            };
        }
    }
}
