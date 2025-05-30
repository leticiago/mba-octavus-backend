using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Octavus.Core.Domain.Entities;
using Octavus.Infra.Persistence.Mappings;
using System.Linq;

namespace Octavus.Tests.Mappings
{
    public class ProfileMapTests
    {
        private class TestDbContext : DbContext
        {
            public DbSet<Profile> Profiles => Set<Profile>();

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.ApplyConfiguration(new ProfileMap());
            }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                optionsBuilder.UseInMemoryDatabase("ProfileTestDb");
            }
        }

        [Test]
        public void Configure_ShouldSetTableNameAndPrimaryKey()
        {
            using var context = new TestDbContext();

            var entity = context.Model.FindEntityType(typeof(Profile));

            Assert.IsNotNull(entity);
            Assert.AreEqual("Profiles", entity.GetTableName());

            var key = entity.FindPrimaryKey();
            Assert.IsNotNull(key);
            Assert.AreEqual("Id", key.Properties.Single().Name);
        }

        [Test]
        public void Configure_ShouldSetNamePropertyConstraints()
        {
            using var context = new TestDbContext();

            var entity = context.Model.FindEntityType(typeof(Profile));
            var nameProperty = entity.FindProperty(nameof(Profile.Name));

            Assert.IsNotNull(nameProperty);
            Assert.IsFalse(nameProperty.IsNullable);
            Assert.AreEqual(100, nameProperty.GetMaxLength());
        }
    }
}
