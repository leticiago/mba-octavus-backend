using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Octavus.Core.Domain.Entities;
using Octavus.Infra.Persistence.Mappings;
using System;
using System.Linq;

namespace Octavus.Tests.Mappings
{
    public class ActivityMapTests
    {
        private class TestDbContext : DbContext
        {
            public DbSet<Activity> Activities => Set<Activity>();

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.ApplyConfiguration(new ActivityMap());
            }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                optionsBuilder.UseInMemoryDatabase("ActivityMapTestDb");
            }
        }

        [Test]
        public void ActivityMap_ConfiguresModel_Correctly()
        {
            using var context = new TestDbContext();
            var entity = context.Model.FindEntityType(typeof(Activity));

            Assert.IsNotNull(entity, "Entity mapping not found");

            var tableName = entity.GetTableName();
            Assert.AreEqual("Activities", tableName);

            var props = entity.GetProperties().Select(p => p.Name).ToList();
            CollectionAssert.Contains(props, "Id");
            CollectionAssert.Contains(props, "Name");
            CollectionAssert.Contains(props, "Description");
            CollectionAssert.Contains(props, "Type");
            CollectionAssert.Contains(props, "Date");
            CollectionAssert.Contains(props, "Level");
            CollectionAssert.Contains(props, "IsPublic");
            CollectionAssert.Contains(props, "InstrumentId");
            CollectionAssert.Contains(props, "ProfessorId");
        }

        [Test]
        public void ActivityMap_ValidatesRelationships_Correctly()
        {
            using var context = new TestDbContext();
            var entity = context.Model.FindEntityType(typeof(Activity));

            var instrumentNav = entity.GetForeignKeys().FirstOrDefault(fk => fk.PrincipalEntityType.ClrType.Name == "Instrument");
            Assert.IsNotNull(instrumentNav);
            Assert.AreEqual(DeleteBehavior.Restrict, instrumentNav.DeleteBehavior);

            var questionNav = context.Model.FindEntityType(typeof(Question))?
                .GetForeignKeys().FirstOrDefault(fk => fk.PrincipalEntityType.ClrType == typeof(Activity));
            Assert.IsNotNull(questionNav);
            Assert.AreEqual(DeleteBehavior.Cascade, questionNav.DeleteBehavior);
        }
    }
}
