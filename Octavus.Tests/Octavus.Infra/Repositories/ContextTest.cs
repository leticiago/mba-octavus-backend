using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Octavus.Infra.Persistence;
using Octavus.Core.Domain.Entities;
using System;
using System.Threading.Tasks;

namespace Octavus.Tests.Persistence
{
    [TestFixture]
    public class ContextTests
    {
        private DbContextOptions<Context> _options = null!;

        [SetUp]
        public void Setup()
        {
            _options = new DbContextOptionsBuilder<Context>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;
        }

        [Test]
        public void Context_CanBeConfiguredWithInMemory()
        {
            var options = new DbContextOptionsBuilder<Context>()
                .UseInMemoryDatabase("TestDb")
                .Options;

            using var context = new Context(options);

            Assert.IsNotNull(context);
        }


        [Test]
        public void OnConfiguring_Should_HaveSqlServerConfigured()
        {
            var optionsBuilder = new DbContextOptionsBuilder<Context>();
            using var context = new Context();

            context.GetType()
                .GetMethod("OnConfiguring", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
                ?.Invoke(context, new object[] { optionsBuilder });

            Assert.That(optionsBuilder.Options.Extensions,
                Has.Some.Matches<object>(ext => ext.GetType().Name.Contains("SqlServer")));
        }

    }
}
