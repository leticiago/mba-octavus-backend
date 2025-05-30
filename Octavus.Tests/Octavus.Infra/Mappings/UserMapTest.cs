using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Octavus.Core.Domain.Entities;
using Octavus.Infra.Persistence.Mappings;
using System.Linq;

namespace Octavus.Tests.Mappings
{
    public class UserMapTests
    {
        private class TestDbContext : DbContext
        {
            public DbSet<User> Users => Set<User>();
            public DbSet<Profile> Profiles => Set<Profile>();
            public DbSet<Instrument> Instruments => Set<Instrument>();

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.ApplyConfiguration(new UserMap());
                modelBuilder.Entity<Profile>().HasKey(p => p.Id);
                modelBuilder.Entity<Instrument>().HasKey(i => i.Id);
            }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                optionsBuilder.UseInMemoryDatabase("UserMapTestDb");
            }
        }

        [Test]
        public void Configure_ShouldSetTableNameAndPrimaryKey()
        {
            using var context = new TestDbContext();

            var entity = context.Model.FindEntityType(typeof(User));
            Assert.IsNotNull(entity);
            Assert.AreEqual("Users", entity.GetTableName());

            var key = entity.FindPrimaryKey();
            Assert.IsNotNull(key);
            Assert.AreEqual("Id", key.Properties.Single().Name);
        }

        [Test]
        public void Configure_ShouldSetPropertyConstraints()
        {
            using var context = new TestDbContext();

            var entity = context.Model.FindEntityType(typeof(User));

            var emailProp = entity.FindProperty(nameof(User.Email));
            Assert.IsNotNull(emailProp);
            Assert.IsFalse(emailProp.IsNullable);
            Assert.AreEqual(150, emailProp.GetMaxLength());

            var passwordProp = entity.FindProperty(nameof(User.Password));
            Assert.IsNotNull(passwordProp);
            Assert.IsFalse(passwordProp.IsNullable);
            Assert.AreEqual(100, passwordProp.GetMaxLength());

            var nameProp = entity.FindProperty(nameof(User.Name));
            Assert.IsNotNull(nameProp);
            Assert.IsFalse(nameProp.IsNullable);
            Assert.AreEqual(100, nameProp.GetMaxLength());

            var contactProp = entity.FindProperty(nameof(User.Contact));
            Assert.IsNotNull(contactProp);
            Assert.IsFalse(contactProp.IsNullable);
            Assert.AreEqual(50, contactProp.GetMaxLength());

            var instrumentIdProp = entity.FindProperty(nameof(User.InstrumentId));
            Assert.IsNotNull(instrumentIdProp);
            Assert.IsFalse(instrumentIdProp.IsNullable);

            var profileIdProp = entity.FindProperty(nameof(User.ProfileId));
            Assert.IsNotNull(profileIdProp);
            Assert.IsFalse(profileIdProp.IsNullable);
        }

        [Test]
        public void Configure_ShouldHaveForeignKeysToProfileAndInstrument()
        {
            using var context = new TestDbContext();

            var entity = context.Model.FindEntityType(typeof(User));
            var fks = entity.GetForeignKeys().ToList();

            Assert.IsTrue(fks.Any(fk => fk.PrincipalEntityType.ClrType == typeof(Profile) &&
                                       fk.Properties.Any(p => p.Name == "ProfileId")));

            Assert.IsTrue(fks.Any(fk => fk.PrincipalEntityType.ClrType == typeof(Instrument) &&
                                       fk.Properties.Any(p => p.Name == "InstrumentId")));
        }
    }
}
