using SaimDataCopy.DataProviders.Configuration;
using SaimDataCopy.Models.Configuration;

namespace SaimDataCopy.DataProviders.BasesCopier
{
    // Factory qui choisit le bon DataProvider selon le type du serveur source.
    public static class BasesCopierDataProviderFactory
    {
        public static IBasesCopierDataProvider Creer()
        {
            IConfigurationDataProvider configurationDataProvider = new ConfigurationDataProvider();

            ConfigurationModel? configuration = configurationDataProvider.ChargerConfiguration();

            string typeServeurSource =
                configuration?.ServeurSource.TypeServeur ?? "SQL Server";

            return typeServeurSource switch
            {
                "MySQL" => new MySqlBasesCopierDataProvider(configurationDataProvider),
                "SQL Server" => new BasesCopierDataProvider(configurationDataProvider),
                _ => new BasesCopierDataProvider(configurationDataProvider)
            };
        }
    }
}