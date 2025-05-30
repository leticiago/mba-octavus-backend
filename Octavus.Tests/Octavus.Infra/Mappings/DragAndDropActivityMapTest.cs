using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Octavus.Core.Domain.Entities;
using Octavus.Infra.Persistence.Mappings;
using System.Linq;

namespace Octavus.Tests.Mappings
{
    public class DragAndDropActivityMapTests
    {
        private class TestDbContext : DbContext
        {
            public DbSet<DragAndDropActivity> DragAndDropActivities => Set<DragAndDropActivity>();
            public DbSet<Activity> Activities => Set<Activity>();

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.ApplyConfiguration(new DragAndDropActivityMap());
            }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                optionsBuilder.UseInMemoryDatabase("DragAndDropActivityTestDb");
            }
        }

        [Test]
        public void DragAndDropActivityMap_ConfiguresModel_Correctly()
        {
            using var context = new TestDbContext();
            var entity = context.Model.FindEntityType(typeof(DragAndDropActivity));

            Assert.IsNotNull(entity, "Entity mapping not found");

            Assert.That(entity.GetTableName(), Is.EqualTo("DragAndDropActivities"));

            var key = entity.FindPrimaryKey();
            Assert.IsNotNull(key);
            Assert.That(key.Properties.First().Name, Is.EqualTo("Id"));

            var props = entity.GetProperties().Select(p => p.Name).ToList();
            CollectionAssert.Contains(props, "ActivityId");
            CollectionAssert.Contains(props, "Text");
        }

        [Test]
        public void DragAndDropActivityMap_ValidatesRelationshipWithActivity()
        {
            using var context = new TestDbContext();
            var entity = context.Model.FindEntityType(typeof(DragAndDropActivity));

            var fk = entity.GetForeignKeys().FirstOrDefault(f => f.PrincipalEntityType.ClrType == typeof(Activity));
            Assert.IsNotNull(fk, "Foreign key to Activity not found");
            Assert.That(fk.Properties.First().Name, Is.EqualTo("ActivityId"));
            Assert.That(fk.DeleteBehavior, Is.EqualTo(DeleteBehavior.Cascade));
        }
    }
}
