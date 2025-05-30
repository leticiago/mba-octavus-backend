using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Octavus.Core.Domain.Entities;
using Octavus.Infra.Persistence.Mappings;
using System.Linq;

namespace Octavus.Tests.Mappings
{
    public class ProfessorStudentMapTests
    {
        private class TestDbContext : DbContext
        {
            public DbSet<ProfessorStudent> ProfessorStudents => Set<ProfessorStudent>();
            public DbSet<User> Users => Set<User>();

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.ApplyConfiguration(new ProfessorStudentMap());
            }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                optionsBuilder.UseInMemoryDatabase("ProfessorStudentTestDb");
            }
        }

        [Test]
        public void Configure_SetsTableNameAndCompositeKey()
        {
            using var context = new TestDbContext();
            var entity = context.Model.FindEntityType(typeof(ProfessorStudent));
            Assert.IsNotNull(entity);
            Assert.That(entity.GetTableName(), Is.EqualTo("ProfessorStudents"));

            var key = entity.FindPrimaryKey();
            Assert.IsNotNull(key);
            var keyProps = key.Properties.Select(p => p.Name).ToList();
            CollectionAssert.AreEquivalent(new[] { "StudentId", "ProfessorId" }, keyProps);
        }

        [Test]
        public void Configure_Properties_AreConfiguredCorrectly()
        {
            using var context = new TestDbContext();
            var entity = context.Model.FindEntityType(typeof(ProfessorStudent));
            var props = entity.GetProperties().ToDictionary(p => p.Name, p => p);

            Assert.That(props.ContainsKey("StudentId"));
            Assert.IsFalse(props["StudentId"].IsNullable);

            Assert.That(props.ContainsKey("ProfessorId"));
            Assert.IsFalse(props["ProfessorId"].IsNullable);
            Assert.That(props["ProfessorId"].GetMaxLength(), Is.EqualTo(100));

            Assert.That(props.ContainsKey("Date"));
            Assert.IsFalse(props["Date"].IsNullable);

            Assert.That(props.ContainsKey("Active"));
            Assert.IsFalse(props["Active"].IsNullable);

            Assert.That(props.ContainsKey("InstrumentId"));
            Assert.IsFalse(props["InstrumentId"].IsNullable);
        }

        [Test]
        public void Configure_HasForeignKeyToUser_OnStudentId()
        {
            using var context = new TestDbContext();
            var entity = context.Model.FindEntityType(typeof(ProfessorStudent));

            var fk = entity.GetForeignKeys().FirstOrDefault(f => f.Properties.Any(p => p.Name == "StudentId"));
            Assert.IsNotNull(fk);
            Assert.That(fk.PrincipalEntityType.ClrType, Is.EqualTo(typeof(User)));
            Assert.That(fk.Properties.First().Name, Is.EqualTo("StudentId"));
        }
    }
}
