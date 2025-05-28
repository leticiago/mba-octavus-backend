using NUnit.Framework;
using Moq;
using System;
using System.Threading.Tasks;
using Octavus.Core.Domain.Entities;
using Octavus.Core.Application.Repositories;
using Octavus.Infra.Core.Services;
using Octavus.Core.Domain.Enums;

namespace Octavus.Tests.Services
{
    [TestFixture]
    public class OpenTextAnswerServiceTests
    {
        private Mock<IOpenTextAnswerRepository> _mockOpenTextAnswerRepository = null!;
        private Mock<IActivityStudentRepository> _mockActivityStudentRepository = null!;
        private Mock<IQuestionRepository> _mockQuestionRepository = null!;
        private OpenTextAnswerService _service = null!;

        [SetUp]
        public void Setup()
        {
            _mockOpenTextAnswerRepository = new Mock<IOpenTextAnswerRepository>();
            _mockActivityStudentRepository = new Mock<IActivityStudentRepository>();
            _mockQuestionRepository = new Mock<IQuestionRepository>();

            _service = new OpenTextAnswerService(
                _mockOpenTextAnswerRepository.Object,
                _mockActivityStudentRepository.Object,
                _mockQuestionRepository.Object);
        }

        [Test]
        public async Task GetByIdAsync_ShouldReturnAnswer_WhenAnswerExists()
        {
            var id = Guid.NewGuid();
            var expectedAnswer = new OpenTextAnswer { Id = id, QuestionId = Guid.NewGuid() };

            _mockOpenTextAnswerRepository
                .Setup(r => r.GetByIdAsync(id))
                .ReturnsAsync(expectedAnswer);

            var result = await _service.GetByIdAsync(id);

            Assert.IsNotNull(result);
            Assert.AreEqual(expectedAnswer.Id, result!.Id);
            _mockOpenTextAnswerRepository.Verify(r => r.GetByIdAsync(id), Times.Once);
        }

        [Test]
        public async Task CreateAsync_ShouldAddAnswerAndUpdateActivityStudent()
        {
            var answer = new OpenTextAnswer
            {
                Id = Guid.NewGuid(),
                StudentId = Guid.NewGuid(),
                QuestionId = Guid.NewGuid()
            };

            var question = new Question
            {
                Id = answer.QuestionId,
                ActivityId = Guid.NewGuid()
            };

            _mockQuestionRepository
                .Setup(r => r.GetByIdAsync(answer.QuestionId))
                .ReturnsAsync(question);

            _mockOpenTextAnswerRepository
                .Setup(r => r.AddAsync(answer))
                .Returns(Task.CompletedTask);

            _mockOpenTextAnswerRepository
                .Setup(r => r.SaveChangesAsync())
                .ReturnsAsync(true);

            _mockActivityStudentRepository
                .Setup(r => r.UpdateAsync(It.Is<ActivityStudent>(a =>
                    a.StudentId == answer.StudentId &&
                    a.ActivityId == question.ActivityId &&
                    a.Status == ActivityStatus.Done)))
                .Returns(Task.CompletedTask);

            var result = await _service.CreateAsync(answer);

            Assert.IsNotNull(result);
            Assert.AreEqual(answer.Id, result.Id);

            _mockOpenTextAnswerRepository.Verify(r => r.AddAsync(answer), Times.Once);
            _mockOpenTextAnswerRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
            _mockActivityStudentRepository.Verify(r => r.UpdateAsync(It.IsAny<ActivityStudent>()), Times.Once);
            _mockQuestionRepository.Verify(r => r.GetByIdAsync(answer.QuestionId), Times.Once);
        }
    }
}
