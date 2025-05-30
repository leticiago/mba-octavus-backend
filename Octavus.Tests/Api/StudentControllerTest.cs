using NUnit.Framework;
using Moq;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Octavus.Controllers.v1;
using Octavus.Core.Application.Services;
using Octavus.Core.Application.DTO;
using System.Collections.Generic;

namespace Octavus.Tests.Controllers
{
    [TestFixture]
    public class StudentControllerTest
    {
        private Mock<IActivityStudentService> _activityStudentServiceMock;
        private Mock<IStudentService> _studentServiceMock;
        private StudentController _controller;

        [SetUp]
        public void Setup()
        {
            _activityStudentServiceMock = new Mock<IActivityStudentService>();
            _studentServiceMock = new Mock<IStudentService>();
            _controller = new StudentController(_activityStudentServiceMock.Object, _studentServiceMock.Object);
        }

        [Test]
        public async Task GetActivities_ReturnsActivitiesForStudent()
        {
            // Arrange
            var studentId = Guid.NewGuid();
            var activityStudent = new ActivityStudentDto() { Title = "Tarefa teste" };
            var expectedActivities = new List<ActivityStudentDto> { activityStudent };
            _activityStudentServiceMock.Setup(s => s.GetActivitiesForStudentAsync(studentId)).Returns(Task.FromResult(expectedActivities));

            // Act
            var result = await _controller.GetActivities(studentId);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.That(okResult.Value, Is.EqualTo(expectedActivities));
        }

        [Test]
        public async Task SubmitAnswers_ReturnsScoreAndMessage()
        {
            // Arrange
            var dto = new SubmitAnswersDto
            {
                StudentId = Guid.NewGuid(),
                Answers = new List<AnswerSubmissionDto>()
            };
            var expectedScore = 8;
            _studentServiceMock
                .Setup(s => s.SubmitAnswersAsync(dto))
                .ReturnsAsync(expectedScore);

            // Act
            var result = await _controller.SubmitAnswers(dto);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);

            var value = okResult.Value;
            var messageProp = value?.GetType().GetProperty("message");
            var scoreProp = value?.GetType().GetProperty("score");

            Assert.IsNotNull(messageProp, "Propriedade 'message' não encontrada no resultado.");
            Assert.IsNotNull(scoreProp, "Propriedade 'score' não encontrada no resultado.");

            var messageValue = messageProp.GetValue(value) as string;
            var scoreValue = (int)scoreProp.GetValue(value);

            Assert.That(messageValue, Is.EqualTo("Respostas enviadas e avaliadas com sucesso."));
            Assert.That(scoreValue, Is.EqualTo(expectedScore));
        }


        [Test]
        public async Task GetCompletedActivities_ReturnsList()
        {
            // Arrange
            var studentId = Guid.NewGuid();
            var completedActivity = new StudentCompletedActivityDto() { Title = "Atividade completa teste" };
            var completedActivities = new List<StudentCompletedActivityDto> { completedActivity };
            _studentServiceMock.Setup(s => s.GetStudentCompletedActivitiesAsync(studentId)).ReturnsAsync(completedActivities);

            // Act
            var result = await _controller.GetCompletedActivities(studentId);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.That(okResult.Value, Is.EqualTo(completedActivities));
        }

        [Test]
        public async Task SubmitDragAndDrop_ReturnsEvaluation()
        {
            // Arrange
            var dto = new DragAndDropSubmissionDto { StudentId = Guid.NewGuid(), ActivityId = Guid.NewGuid(), Answer = "A;B;C" };
            var evaluationResult = new ActivityScoreResultDto() { Score = 10 };
            _studentServiceMock.Setup(s => s.GradeDragAndDropAsync(dto)).Returns(Task.FromResult(evaluationResult));

            // Act
            var result = await _controller.SubmitDragAndDrop(dto);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.That(okResult.Value, Is.EqualTo(evaluationResult));
        }

        [Test]
        public async Task GetMetricsByStudent_ReturnsMetrics()
        {
            // Arrange
            var studentId = Guid.NewGuid();
            var metrics = new StudentMetricsDto() { AverageScore = 10, TotalActivitiesDone = 3 };
            _activityStudentServiceMock.Setup(s => s.GetMetricsByStudentAsync(studentId)).Returns(Task.FromResult(metrics));

            // Act
            var result = await _controller.GetMetricsByStudent(studentId);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.That(okResult.Value, Is.EqualTo(metrics));
        }
    }
}
