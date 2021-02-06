using System;
using DataProtectionDatabaseKeyStorage.Business.Repositories;
using DataProtectionDatabaseKeyStorage.Data.Interfaces;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace DataProtectionDatabaseKeyStorage.Business.StartupConfiguration
{
    public static class DataProtectionKeyStartup
    {
        //This is an example of how to setup the IDbConnection using DI for SQL Server
        //services.AddTransient<IDbConnection>(con => new SqlConnection(connectionString));

        //This is an example of how to setup the IDbConnection using DI for MySQL
        //services.AddTransient<IDbConnection>(con => new MySqlConnection(connectionString));

        //Tells the data protection service to use our XmlRepository for getting its keys and setting them up
        /*Note: The default expire time for these keys is 90 days, which has not been changed in this example (but it can be).
                When a data protection service key expires, a new one is generated but the old ones are kept for backwards compatibility, 
                this ensures anything encrypted using an older key, is still automatically decryptable by the data protection services. (key rotation)*/
        public static void AddDataProtectionKeysToDb(this IServiceCollection services)
        {
            //Adds the default MS data protection services
            services.AddDataProtection();

            //Adds the repository to DI
            services.AddTransient<IDataProtectionKeyRepository, DataProtectionKeyRepository>();

            //Configures the default data protection key management options to use our implementation of the xmlRepo 
            services.AddSingleton<IConfigureOptions<KeyManagementOptions>>(sv =>
            {
                var xmlRepository = sv.GetService<IDataProtectionKeyRepository>();
                return new ConfigureOptions<KeyManagementOptions>(options =>
                {
                    options.XmlRepository = xmlRepository;

                    //This allows you to change the expire time per key
                    //Note:Default is 90 days without supplying this option
                    options.NewKeyLifetime = TimeSpan.FromDays(90);
                });
            });
        }
    }
}