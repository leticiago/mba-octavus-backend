using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Octavus.Core.Domain.Entities;
using Octavus.Infra.Persistence.Mappings;
using System.Linq;

namespace Octavus.Tests.Mappings
{
    public class AnswerMapTests
    {
        private class TestDbContext : DbContext
        {
            public DbSet<Answer> Answers => Set<Answer>();
            public DbSet<Question> Questions => Set<Question>();

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.ApplyConfiguration(new AnswerMap());
            }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                optionsBuilder.UseInMemoryDatabase("AnswerTestDb");
            }
        }

        [Test]
        public void AnswerMap_ConfiguresModel_Correctly()
        {
            using var context = new TestDbContext();
            var entity = context.Model.FindEntityType(typeof(Answer));

            Assert.IsNotNull(entity, "Entity mapping not found");
            Assert.That(entity.GetTableName(), Is.EqualTo("Answers"));

            var key = entity.FindPrimaryKey();
            Assert.IsNotNull(key);
            Assert.That(key.Properties.First().Name, Is.EqualTo("Id"));

            var props = entity.GetProperties().Select(p => p.Name).ToList();
            CollectionAssert.Contains(props, "Text");
            CollectionAssert.Contains(props, "IsCorrect");
            CollectionAssert.Contains(props, "QuestionId");
        }

        [Test]
        public void AnswerMap_ValidatesRelationships_Correctly()
        {
            using var context = new TestDbContext();
            var entity = context.Model.FindEntityType(typeof(Answer));

            var fk = entity.GetForeignKeys().FirstOrDefault(f => f.PrincipalEntityType.ClrType == typeof(Question));
            Assert.IsNotNull(fk);
            Assert.That(fk.Properties.First().Name, Is.EqualTo("QuestionId"));
            Assert.That(fk.DeleteBehavior, Is.EqualTo(DeleteBehavior.Cascade));
        }
    }
}
