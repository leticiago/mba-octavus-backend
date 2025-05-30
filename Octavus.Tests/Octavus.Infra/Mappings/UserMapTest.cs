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
            Assert.That(entity.GetTableName(), Is.EqualTo("Users"));

            var key = entity.FindPrimaryKey();
            Assert.IsNotNull(key);
            Assert.That(key.Properties.Single().Name, Is.EqualTo("Id"));
        }

        [Test]
        public void Configure_ShouldSetPropertyConstraints()
        {
            using var context = new TestDbContext();

            var entity = context.Model.FindEntityType(typeof(User));

            var emailProp = entity.FindProperty(nameof(User.Email));
            Assert.IsNotNull(emailProp);
            Assert.IsFalse(emailProp.IsNullable);
            Assert.That(emailProp.GetMaxLength(), Is.EqualTo(150));

            var passwordProp = entity.FindProperty(nameof(User.Password));
            Assert.IsNotNull(passwordProp);
            Assert.IsFalse(passwordProp.IsNullable);
            Assert.That(passwordProp.GetMaxLength(), Is.EqualTo(100));

            var nameProp = entity.FindProperty(nameof(User.Name));
            Assert.IsNotNull(nameProp);
            Assert.IsFalse(nameProp.IsNullable);
            Assert.That(nameProp.GetMaxLength(), Is.EqualTo(100));

            var contactProp = entity.FindProperty(nameof(User.Contact));
            Assert.IsNotNull(contactProp);
            Assert.IsFalse(contactProp.IsNullable);
            Assert.That(contactProp.GetMaxLength(), Is.EqualTo(50));

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
