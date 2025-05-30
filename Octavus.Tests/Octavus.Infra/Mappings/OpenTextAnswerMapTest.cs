using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Octavus.Core.Domain.Entities;
using Octavus.Infra.Persistence.Mappings;
using System.Linq;

namespace Octavus.Tests.Mappings
{
    public class OpenTextAnswerMapTests
    {
        private class TestDbContext : DbContext
        {
            public DbSet<OpenTextAnswer> OpenTextAnswers => Set<OpenTextAnswer>();
            public DbSet<Question> Questions => Set<Question>();
            public DbSet<User> Users => Set<User>();

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.ApplyConfiguration(new OpenTextAnswerMap());
            }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                optionsBuilder.UseInMemoryDatabase("OpenTextAnswerTestDb");
            }
        }

        [Test]
        public void Configure_SetsTableNameAndPrimaryKey()
        {
            using var context = new TestDbContext();
            var entity = context.Model.FindEntityType(typeof(OpenTextAnswer));
            Assert.IsNotNull(entity);
            Assert.AreEqual("OpenTextAnswers", entity.GetTableName());

            var key = entity.FindPrimaryKey();
            Assert.IsNotNull(key);
            Assert.AreEqual("Id", key.Properties.First().Name);
        }

        [Test]
        public void Configure_Properties_AreConfiguredCorrectly()
        {
            using var context = new TestDbContext();
            var entity = context.Model.FindEntityType(typeof(OpenTextAnswer));
            var props = entity.GetProperties().ToDictionary(p => p.Name, p => p);

            Assert.That(props.ContainsKey("QuestionId"));
            Assert.IsTrue(props["QuestionId"].IsNullable == false);

            Assert.That(props.ContainsKey("StudentId"));
            Assert.IsTrue(props["StudentId"].IsNullable == false);

            Assert.That(props.ContainsKey("ResponseText"));
            Assert.IsTrue(props["ResponseText"].IsNullable == false);
            Assert.AreEqual(2000, props["ResponseText"].GetMaxLength());

            Assert.That(props.ContainsKey("SubmittedAt"));
            Assert.IsTrue(props["SubmittedAt"].IsNullable == false);
        }

        [Test]
        public void Configure_HasCorrectForeignKeys()
        {
            using var context = new TestDbContext();
            var entity = context.Model.FindEntityType(typeof(OpenTextAnswer));

            var fkQuestion = entity.GetForeignKeys().FirstOrDefault(fk => fk.PrincipalEntityType.ClrType == typeof(Question));
            Assert.IsNotNull(fkQuestion);
            Assert.AreEqual("QuestionId", fkQuestion.Properties.First().Name);

            var fkUser = entity.GetForeignKeys().FirstOrDefault(fk => fk.PrincipalEntityType.ClrType == typeof(User));
            Assert.IsNotNull(fkUser);
            Assert.AreEqual("StudentId", fkUser.Properties.First().Name);
        }
    }
}
