using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Octavus.Core.Domain.Entities;
using Octavus.Infra.Persistence.Mappings;
using System.Linq;

namespace Octavus.Tests.Mappings
{
    public class InstrumentMapTests
    {
        private class TestDbContext : DbContext
        {
            public DbSet<Instrument> Instruments => Set<Instrument>();

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.ApplyConfiguration(new InstrumentMap());
            }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                optionsBuilder.UseInMemoryDatabase("InstrumentTestDb");
            }
        }

        [Test]
        public void InstrumentMap_ConfiguresModel_Correctly()
        {
            using var context = new TestDbContext();
            var entity = context.Model.FindEntityType(typeof(Instrument));

            Assert.IsNotNull(entity, "Entity mapping not found");

            Assert.That(entity.GetTableName(), Is.EqualTo("Instruments"));

            var key = entity.FindPrimaryKey();
            Assert.IsNotNull(key);
            Assert.That(key.Properties.First().Name, Is.EqualTo("Id"));

            var props = entity.GetProperties().Select(p => p.Name).ToList();
            CollectionAssert.Contains(props, "Name");
        }
    }
}
