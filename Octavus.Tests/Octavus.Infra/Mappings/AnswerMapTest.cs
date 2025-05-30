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
            Assert.AreEqual("Answers", entity.GetTableName());

            var key = entity.FindPrimaryKey();
            Assert.IsNotNull(key);
            Assert.AreEqual("Id", key.Properties.First().Name);

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
            Assert.AreEqual("QuestionId", fk.Properties.First().Name);
            Assert.AreEqual(DeleteBehavior.Cascade, fk.DeleteBehavior);
        }
    }
}
