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
            Assert.That(tableName, Is.EqualTo("Activities"));

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
            Assert.That(instrumentNav.DeleteBehavior, Is.EqualTo(DeleteBehavior.Restrict));

            var questionNav = context.Model.FindEntityType(typeof(Question))?
                .GetForeignKeys().FirstOrDefault(fk => fk.PrincipalEntityType.ClrType == typeof(Activity));
            Assert.IsNotNull(questionNav);
            Assert.That(questionNav.DeleteBehavior, Is.EqualTo(DeleteBehavior.Cascade));
        }

        [Test]
        public void ActivityMap_Properties_HaveCorrectConstraints()
        {
            using var context = new TestDbContext();
            var entity = context.Model.FindEntityType(typeof(Activity));
            Assert.IsNotNull(entity);

            // Id - required (não aceita nulo)
            var idProp = entity.FindProperty("Id");
            Assert.IsNotNull(idProp);
            Assert.IsFalse(idProp.IsNullable);

            // Name - required e max length 100
            var nameProp = entity.FindProperty("Name");
            Assert.IsNotNull(nameProp);
            Assert.IsFalse(nameProp.IsNullable);
            Assert.That(nameProp.GetMaxLength(), Is.EqualTo(100));

            // Description - required e max length 500
            var descProp = entity.FindProperty("Description");
            Assert.IsNotNull(descProp);
            Assert.IsFalse(descProp.IsNullable);
            Assert.That(descProp.GetMaxLength(), Is.EqualTo(500));

            // Type - required e max length 50
            var typeProp = entity.FindProperty("Type");
            Assert.IsNotNull(typeProp);
            Assert.IsFalse(typeProp.IsNullable);
            Assert.That(typeProp.GetMaxLength(), Is.EqualTo(50));

            // Date - required
            var dateProp = entity.FindProperty("Date");
            Assert.IsNotNull(dateProp);
            Assert.IsFalse(dateProp.IsNullable);

            // Level - required e max length 50
            var levelProp = entity.FindProperty("Level");
            Assert.IsNotNull(levelProp);
            Assert.IsFalse(levelProp.IsNullable);
            Assert.That(levelProp.GetMaxLength(), Is.EqualTo(50));

            // IsPublic - required
            var isPublicProp = entity.FindProperty("IsPublic");
            Assert.IsNotNull(isPublicProp);

        }
    }
}
