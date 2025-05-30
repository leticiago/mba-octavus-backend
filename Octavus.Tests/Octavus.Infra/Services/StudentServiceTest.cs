using Moq;
using NUnit.Framework;
using Octavus.Core.Application.DTO;
using Octavus.Core.Application.Repositories;
using Octavus.Core.Domain.Entities;
using Octavus.Core.Domain.Enums;
using Octavus.Infra.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Octavus.Tests.Services
{
    [TestFixture]
    public class StudentServiceTests
    {
        private Mock<IAnswerRepository> _answerRepo = null!;
        private Mock<IActivityStudentRepository> _activityStudentRepo = null!;
        private Mock<IDragAndDropActivityRepository> _dragRepo = null!;
        private StudentService _service = null!;

        [SetUp]
        public void Setup()
        {
            _answerRepo = new Mock<IAnswerRepository>();
            _activityStudentRepo = new Mock<IActivityStudentRepository>();
            _dragRepo = new Mock<IDragAndDropActivityRepository>();
            _service = new StudentService(_answerRepo.Object, _activityStudentRepo.Object, _dragRepo.Object);
        }

        [Test]
        public async Task SubmitAnswersAsync_ShouldCalculateScoreAndUpdateActivity()
        {
            var questionId = Guid.NewGuid();
            var answerId = Guid.NewGuid();

            var dto = new SubmitAnswersDto
            {
                ActivityId = Guid.NewGuid(),
                StudentId = Guid.NewGuid(),
                Answers = new List<AnswerSubmissionDto>
                {
                    new AnswerSubmissionDto
                    {
                        QuestionId = questionId,
                        SelectedAnswerId = answerId
                    }
                }
            };

            var correctAnswers = new List<Answer>
            {
                new Answer { Id = answerId, QuestionId = questionId, IsCorrect = true }
            };

            var activityStudent = new ActivityStudent
            {
                ActivityId = dto.ActivityId,
                StudentId = dto.StudentId
            };

            _answerRepo.Setup(r => r.GetCorrectAnswersAsync(It.IsAny<List<Guid>>()))
                       .ReturnsAsync(correctAnswers);
            _activityStudentRepo.Setup(r => r.GetActivityStudentAsync(dto.ActivityId, dto.StudentId))
                                .ReturnsAsync(activityStudent);

            var score = await _service.SubmitAnswersAsync(dto);

            Assert.That(score, Is.EqualTo(100));
            Assert.IsTrue(activityStudent.IsCorrected);
            Assert.That(activityStudent.Status, Is.EqualTo(ActivityStatus.Done));

            _activityStudentRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public void SubmitAnswersAsync_ShouldThrow_WhenActivityStudentNotFound()
        {
            var dto = new SubmitAnswersDto
            {
                ActivityId = Guid.NewGuid(),
                StudentId = Guid.NewGuid(),
                Answers = new List<AnswerSubmissionDto>
                {
                    new AnswerSubmissionDto
                    {
                        QuestionId = Guid.NewGuid(),
                        SelectedAnswerId = Guid.NewGuid()
                    }
                }
            };

            _answerRepo.Setup(r => r.GetCorrectAnswersAsync(It.IsAny<List<Guid>>()))
                       .ReturnsAsync(new List<Answer>());

            _activityStudentRepo.Setup(r => r.GetActivityStudentAsync(dto.ActivityId, dto.StudentId))
                                .ReturnsAsync((ActivityStudent?)null);

            var ex = Assert.ThrowsAsync<Exception>(() => _service.SubmitAnswersAsync(dto));
            Assert.That(ex!.Message, Is.EqualTo("Atividade não atribuída ao aluno."));
        }

        [Test]
        public async Task GetStudentCompletedActivitiesAsync_ShouldReturnList()
        {
            var studentId = Guid.NewGuid();
            var completed = new List<StudentCompletedActivityDto>
            {
                new StudentCompletedActivityDto { ActivityId = Guid.NewGuid(), Score = 85 }
            };

            _activityStudentRepo.Setup(r => r.GetCompletedActivitiesByStudentAsync(studentId))
                                .ReturnsAsync(completed);

            var result = await _service.GetStudentCompletedActivitiesAsync(studentId);

            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0].Score, Is.EqualTo(85));
        }

        [Test]
        public async Task GradeDragAndDropAsync_ShouldReturnCorrectScoreAndUpdate()
        {
            var dto = new DragAndDropSubmissionDto
            {
                ActivityId = Guid.NewGuid(),
                StudentId = Guid.NewGuid(),
                Answer = "A;B;C"
            };

            var activity = new DragAndDropActivity
            {
                ActivityId = dto.ActivityId,
                Text = "A;B;C"
            };

            var activityStudent = new ActivityStudent
            {
                ActivityId = dto.ActivityId,
                StudentId = dto.StudentId
            };

            _dragRepo.Setup(r => r.GetByActivityIdAsync(dto.ActivityId)).ReturnsAsync(activity);
            _activityStudentRepo.Setup(r => r.GetActivityStudentAsync(dto.ActivityId, dto.StudentId))
                                .ReturnsAsync(activityStudent);

            var result = await _service.GradeDragAndDropAsync(dto);

            Assert.That(result.Score, Is.EqualTo(100));
            Assert.That(result.Total, Is.EqualTo(3));
            Assert.That(result.Correct, Is.EqualTo(3));
            Assert.IsTrue(activityStudent.IsCorrected);

            _activityStudentRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task GradeDragAndDropAsync_ShouldCreateActivityStudentIfNotExists()
        {
            var dto = new DragAndDropSubmissionDto
            {
                ActivityId = Guid.NewGuid(),
                StudentId = Guid.NewGuid(),
                Answer = "X;Y;Z"
            };

            var activity = new DragAndDropActivity
            {
                ActivityId = dto.ActivityId,
                Text = "X;Y;Z"
            };

            _dragRepo.Setup(r => r.GetByActivityIdAsync(dto.ActivityId)).ReturnsAsync(activity);
            _activityStudentRepo.Setup(r => r.GetActivityStudentAsync(dto.ActivityId, dto.StudentId))
                                .ReturnsAsync((ActivityStudent?)null);

            ActivityStudent? createdStudent = null;
            _activityStudentRepo.Setup(r => r.AddAsync(It.IsAny<ActivityStudent>()))
                .Callback<ActivityStudent>(a => createdStudent = a)
                .Returns(Task.CompletedTask);

            await _service.GradeDragAndDropAsync(dto);

            Assert.IsNotNull(createdStudent);
            Assert.That(createdStudent.ActivityId, Is.EqualTo(dto.ActivityId));
            Assert.That(createdStudent.StudentId, Is.EqualTo(dto.StudentId));

            _activityStudentRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public void GradeDragAndDropAsync_ShouldThrow_WhenActivityNotFound()
        {
            var dto = new DragAndDropSubmissionDto
            {
                ActivityId = Guid.NewGuid(),
                StudentId = Guid.NewGuid(),
                Answer = "A;B;C"
            };

            _dragRepo.Setup(r => r.GetByActivityIdAsync(dto.ActivityId)).ReturnsAsync((DragAndDropActivity?)null);

            var ex = Assert.ThrowsAsync<Exception>(() => _service.GradeDragAndDropAsync(dto));
            Assert.That(ex!.Message, Is.EqualTo("Atividade não encontrada"));
        }
    }
}
