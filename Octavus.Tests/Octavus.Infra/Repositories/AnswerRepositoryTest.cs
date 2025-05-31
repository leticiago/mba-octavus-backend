using NUnit.Framework;
using Octavus.Core.Domain.Entities;
using Octavus.Infra.Persistence;
using Octavus.Infra.Persistence.Repositories;
using Octavus.Tests.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Octavus.Tests.Repositories
{
    [TestFixture]
    public class AnswerRepositoryTests
    {
        private Context _context = null!;
        private AnswerRepository _repository = null!;

        [SetUp]
        public void Setup()
        {
            _context = TestContextProvider.CreateContext();
            _repository = new AnswerRepository(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task GetCorrectAnswersAsync_ShouldReturnOnlyCorrectAnswers_ForGivenQuestionIds()
        {
            var questionId1 = Guid.NewGuid();
            var questionId2 = Guid.NewGuid();
            var questionId3 = Guid.NewGuid();

            var answers = new List<Answer>
            {
                new Answer { Id = Guid.NewGuid(), QuestionId = questionId1, Text = "A", IsCorrect = true },
                new Answer { Id = Guid.NewGuid(), QuestionId = questionId1, Text = "B", IsCorrect = false },
                new Answer { Id = Guid.NewGuid(), QuestionId = questionId2, Text = "C", IsCorrect = true },
                new Answer { Id = Guid.NewGuid(), QuestionId = questionId2, Text = "D", IsCorrect = true },
                new Answer { Id = Guid.NewGuid(), QuestionId = questionId3, Text = "E", IsCorrect = false }
            };

            await _context.AddRangeAsync(answers);
            await _context.SaveChangesAsync();

            var questionIdsToTest = new List<Guid> { questionId1, questionId2 };

            var result = await _repository.GetCorrectAnswersAsync(questionIdsToTest);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(3));

            Assert.IsTrue(result.All(a => a.IsCorrect));
            Assert.IsTrue(result.All(a => questionIdsToTest.Contains(a.QuestionId)));
        }

        [Test]
        public async Task GetCorrectAnswersAsync_ShouldReturnEmptyList_WhenNoCorrectAnswersFound()
        {
            var questionId = Guid.NewGuid();

            var answers = new List<Answer>
            {
                new Answer { Id = Guid.NewGuid(), QuestionId = questionId, Text = "A", IsCorrect = false }
            };

            await _context.AddRangeAsync(answers);
            await _context.SaveChangesAsync();

            var result = await _repository.GetCorrectAnswersAsync(new List<Guid> { questionId });

            Assert.IsNotNull(result);
            Assert.IsEmpty(result);
        }

        [Test]
        public async Task GetCorrectAnswersAsync_ShouldReturnEmptyList_WhenNoQuestionsProvided()
        {
            var result = await _repository.GetCorrectAnswersAsync(new List<Guid>());

            Assert.IsNotNull(result);
            Assert.IsEmpty(result);
        }

        [Test]
        public async Task GetCorrectAnswersAsync_ShouldReturnEmptyList_WhenMultipleQuestionsHaveNoCorrectAnswers()
        {
            var questionId1 = Guid.NewGuid();
            var questionId2 = Guid.NewGuid();

            var answers = new List<Answer>
    {
        new Answer { Id = Guid.NewGuid(), QuestionId = questionId1, Text = "A", IsCorrect = false },
        new Answer { Id = Guid.NewGuid(), QuestionId = questionId2, Text = "B", IsCorrect = false }
    };

            await _context.AddRangeAsync(answers);
            await _context.SaveChangesAsync();

            var result = await _repository.GetCorrectAnswersAsync(new List<Guid> { questionId1, questionId2 });

            Assert.IsNotNull(result);
            Assert.IsEmpty(result);
        }

        [Test]
        public async Task GetCorrectAnswersAsync_ShouldReturnEmptyList_WhenQuestionIdsHaveNoAnswers()
        {
            var nonexistentIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };

            var result = await _repository.GetCorrectAnswersAsync(nonexistentIds);

            Assert.IsNotNull(result);
            Assert.IsEmpty(result);
        }


    }
}
