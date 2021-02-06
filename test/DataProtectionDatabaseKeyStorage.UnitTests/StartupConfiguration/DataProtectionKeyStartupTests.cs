using System;
using System.Data;
using DataProtectionDatabaseKeyStorage.Business.Repositories;
using DataProtectionDatabaseKeyStorage.Business.StartupConfiguration;
using DataProtectionDatabaseKeyStorage.Data.Interfaces;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace DataProtectionDatabaseKeyStorage.UnitTests.StartupConfiguration
{
    public class DataProtectionKeyStartupTests
    {
        public DataProtectionKeyStartupTests()
        {
            ServiceCollection = new ServiceCollection();
        }

        private ServiceCollection ServiceCollection { get; }

        [Fact]
        public void AddDataProtectionKeysToDb()
        {
            //Mocks the db connection
            var mockDbConnection = new Mock<IDbConnection>();
            ServiceCollection.AddTransient(d => mockDbConnection.Object);

            Assert.DoesNotContain(ServiceCollection, x => x.ServiceType == typeof(IDataProtectionKeyRepository));
            ServiceCollection.AddDataProtectionKeysToDb();
            Assert.Contains(ServiceCollection, x => x.ServiceType == typeof(IDataProtectionKeyRepository));

            IServiceProvider provider = ServiceCollection.BuildServiceProvider();

            var service = provider.GetRequiredService(typeof(IDataProtectionKeyRepository));
            Assert.Equal(typeof(DataProtectionKeyRepository), service?.GetType());

            var configurationOptions = provider.GetRequiredService(typeof(IConfigureOptions<KeyManagementOptions>));
            Assert.Equal(typeof(ConfigureOptions<KeyManagementOptions>), configurationOptions?.GetType());

            var options = provider.GetRequiredService(typeof(IOptions<KeyManagementOptions>));
            Assert.True(options is IOptions<KeyManagementOptions> keyMOptions && keyMOptions?.Value?.XmlRepository?.GetType() == typeof(DataProtectionKeyRepository));
        }
    }
}