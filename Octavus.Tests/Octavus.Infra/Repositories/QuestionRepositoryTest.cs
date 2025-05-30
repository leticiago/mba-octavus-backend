using NUnit.Framework;
using Octavus.Core.Domain.Entities;
using Octavus.Infra.Persistence;
using Octavus.Infra.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Octavus.Tests.Helpers;

namespace Octavus.Tests.Repositories
{
    [TestFixture]
    public class QuestionRepositoryTests
    {
        private Context _context = null!;
        private QuestionRepository _repository = null!;

        [SetUp]
        public void Setup()
        {
            _context = TestContextProvider.CreateContext();
            _repository = new QuestionRepository(_context);

            SeedData();
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        private void SeedData()
        {
            var activityId = Guid.NewGuid();

            var question1 = new Question
            {
                Id = Guid.NewGuid(),
                Title = "Pergunta 1",
                ActivityId = activityId,
                Answers = new List<Answer>
                {
                    new Answer { Id = Guid.NewGuid(), Text = "Resposta 1.1", IsCorrect = true },
                    new Answer { Id = Guid.NewGuid(), Text = "Resposta 1.2", IsCorrect = false }
                }
            };

            var question2 = new Question
            {
                Id = Guid.NewGuid(),
                Title = "Pergunta 2",
                ActivityId = activityId,
                Answers = new List<Answer>
                {
                    new Answer { Id = Guid.NewGuid(), Text = "Resposta 2.1", IsCorrect = false },
                    new Answer { Id = Guid.NewGuid(), Text = "Resposta 2.2", IsCorrect = true }
                }
            };

            _context.Set<Question>().AddRange(question1, question2);
            _context.SaveChanges();
        }

        [Test]
        public async Task GetByIdAsync_ShouldReturnQuestionWithAnswers_WhenExists()
        {
            var question = await _context.Set<Question>().Include(q => q.Answers).FirstAsync();

            var result = await _repository.GetByIdAsync(question.Id);

            Assert.IsNotNull(result);
            Assert.AreEqual(question.Id, result!.Id);
            Assert.IsNotNull(result.Answers);
            Assert.AreEqual(question.Answers.Count, result.Answers.Count);
        }

        [Test]
        public async Task GetByIdAsync_ShouldReturnNull_WhenNotExists()
        {
            var result = await _repository.GetByIdAsync(Guid.NewGuid());
            Assert.IsNull(result);
        }

        [Test]
        public async Task GetByActivityIdAsync_ShouldReturnQuestionsWithAnswers_ForActivity()
        {
            var activityId = (await _context.Set<Question>().FirstAsync()).ActivityId;

            var result = await _repository.GetByActivityIdAsync(activityId);

            Assert.IsNotNull(result);
            Assert.That(result.Count, Is.GreaterThan(0));
            Assert.That(result.All(q => q.ActivityId == activityId));
            Assert.That(result.All(q => q.Answers != null && q.Answers.Count > 0));
        }

        [Test]
        public async Task GetAllAsync_ShouldReturnAllQuestionsWithAnswers()
        {
            var result = await _repository.GetAllAsync();

            var countInDb = await _context.Set<Question>().CountAsync();

            Assert.IsNotNull(result);
            Assert.That(result.Count(), Is.EqualTo(countInDb));
            Assert.That(result.All(q => q.Answers != null && q.Answers.Count > 0));
        }
    }
}
