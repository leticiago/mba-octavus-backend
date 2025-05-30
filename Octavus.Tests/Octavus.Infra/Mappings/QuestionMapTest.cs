using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Octavus.Core.Domain.Entities;
using Octavus.Infra.Persistence.Mappings;
using System.Linq;

namespace Octavus.Tests.Mappings
{
    public class QuestionMapTests
    {
        private class TestDbContext : DbContext
        {
            public DbSet<Question> Questions => Set<Question>();
            public DbSet<Activity> Activities => Set<Activity>();
            public DbSet<Answer> Answers => Set<Answer>();

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.ApplyConfiguration(new QuestionMap());
                modelBuilder.Entity<Activity>().HasKey(a => a.Id);
                modelBuilder.Entity<Answer>().HasKey(a => a.Id);
            }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                optionsBuilder.UseInMemoryDatabase("QuestionTestDb");
            }
        }

        [Test]
        public void Configure_ShouldSetTableNameAndPrimaryKey()
        {
            using var context = new TestDbContext();

            var entity = context.Model.FindEntityType(typeof(Question));
            Assert.IsNotNull(entity);
            Assert.That(entity.GetTableName(), Is.EqualTo("Questions"));

            var key = entity.FindPrimaryKey();
            Assert.IsNotNull(key);
            Assert.That(key.Properties.Single().Name, Is.EqualTo("Id"));
        }

        [Test]
        public void Configure_ShouldSetTitlePropertyConstraints()
        {
            using var context = new TestDbContext();

            var entity = context.Model.FindEntityType(typeof(Question));
            var titleProperty = entity.FindProperty(nameof(Question.Title));

            Assert.IsNotNull(titleProperty);
            Assert.IsFalse(titleProperty.IsNullable);
            Assert.That(titleProperty.GetMaxLength(), Is.EqualTo(200));
        }

        [Test]
        public void Configure_ShouldSetActivityIdAsRequiredAndRelation()
        {
            using var context = new TestDbContext();

            var entity = context.Model.FindEntityType(typeof(Question));
            var activityIdProperty = entity.FindProperty(nameof(Question.ActivityId));
            Assert.IsNotNull(activityIdProperty);
            Assert.IsFalse(activityIdProperty.IsNullable);

            var fk = entity.GetForeignKeys().SingleOrDefault(f => f.Properties.Any(p => p.Name == "ActivityId"));
            Assert.IsNotNull(fk);
            Assert.That(fk.DeleteBehavior, Is.EqualTo(DeleteBehavior.Cascade));
        }

        [Test]
        public void Configure_ShouldSetAnswersRelationWithCascadeDelete()
        {
            using var context = new TestDbContext();

            var entity = context.Model.FindEntityType(typeof(Question));
            var answersFk = entity.GetReferencingForeignKeys().SingleOrDefault(fk => fk.PrincipalEntityType.ClrType == typeof(Question));
            Assert.IsNotNull(answersFk);
            Assert.That(answersFk.DeleteBehavior, Is.EqualTo(DeleteBehavior.Cascade));
        }
    }
}
