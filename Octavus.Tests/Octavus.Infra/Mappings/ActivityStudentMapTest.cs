using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Octavus.Core.Domain.Entities;
using Octavus.Infra.Persistence.Mappings;
using System.Linq;

namespace Octavus.Tests.Mappings
{
    public class ActivityStudentMapTests
    {
        private class TestDbContext : DbContext
        {
            public DbSet<ActivityStudent> ActivityStudents => Set<ActivityStudent>();

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.ApplyConfiguration(new ActivityStudentMap());
            }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                optionsBuilder.UseInMemoryDatabase("ActivityStudentTestDb");
            }
        }

        [Test]
        public void ActivityStudentMap_ConfiguresModel_Correctly()
        {
            using var context = new TestDbContext();
            var entity = context.Model.FindEntityType(typeof(ActivityStudent));

            Assert.IsNotNull(entity, "Entity mapping not found");

            Assert.AreEqual("ActivityStudents", entity.GetTableName());

            var keyProps = entity.FindPrimaryKey()?.Properties.Select(p => p.Name).ToList();
            CollectionAssert.AreEquivalent(new[] { "StudentId", "ActivityId" }, keyProps);

            var props = entity.GetProperties().Select(p => p.Name).ToList();
            CollectionAssert.Contains(props, "Score");
            CollectionAssert.Contains(props, "Comment");
        }

        [Test]
        public void ActivityStudentMap_ValidatesRelationships_Correctly()
        {
            using var context = new TestDbContext();
            var entity = context.Model.FindEntityType(typeof(ActivityStudent));

            var fkUser = entity.GetForeignKeys().FirstOrDefault(fk => fk.Properties.Any(p => p.Name == "StudentId"));
            Assert.IsNotNull(fkUser);
            Assert.AreEqual(typeof(User), fkUser.PrincipalEntityType.ClrType);

            var fkActivity = entity.GetForeignKeys().FirstOrDefault(fk => fk.Properties.Any(p => p.Name == "ActivityId"));
            Assert.IsNotNull(fkActivity);
            Assert.AreEqual(typeof(Activity), fkActivity.PrincipalEntityType.ClrType);
            Assert.AreEqual(DeleteBehavior.Cascade, fkActivity.DeleteBehavior);
        }
    }
}
