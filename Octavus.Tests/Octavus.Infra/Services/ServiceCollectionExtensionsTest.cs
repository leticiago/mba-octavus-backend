using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Octavus.Infra.Core;
using Octavus.Infra.Persistence;
using System;

namespace Octavus.Tests.Configuration
{
    public class ServiceCollectionExtensionsTests
    {
        [Test]
        public void AddPersistence_ShouldRegisterDbContext_WithSqlServerProvider()
        {
            // Arrange
            var services = new ServiceCollection();

            var inMemorySettings = new Dictionary<string, string>
            {
                { "ConnectionStrings:SqlDefault", "Server=(localdb)\\mssqllocaldb;Database=OctavusTestDb;Trusted_Connection=True;" }
            };

            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            // Act
            services.AddPersistence(configuration);
            var provider = services.BuildServiceProvider();
            var context = provider.GetService<Context>();

            // Assert
            Assert.IsNotNull(context);
            Assert.IsInstanceOf<Context>(context);
        }

        [Test]
        public void AddPersistence_ShouldThrow_WhenConnectionStringIsMissing()
        {
            // Arrange
            var services = new ServiceCollection();
            IConfiguration configuration = new ConfigurationBuilder().Build();

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() =>
            {
                services.AddPersistence(configuration);
            });

            Assert.That(ex.Message, Is.EqualTo("Connection string 'SqlDefault' is missing."));
        }


    }
}
