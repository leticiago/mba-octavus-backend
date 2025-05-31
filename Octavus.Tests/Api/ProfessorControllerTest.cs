using NUnit.Framework;
using Moq;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Octavus.Controllers.v1;
using Octavus.Core.Application.Services;
using Octavus.Core.Application.DTO;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace Octavus.Tests.Controllers
{
    [TestFixture]
    public class ProfessorControllerTest
    {
        private Mock<IProfessorStudentService> _professorStudentServiceMock;
        private Mock<IActivityStudentService> _activityStudentServiceMock;
        private ProfessorController _controller;

        [SetUp]
        public void Setup()
        {
            _professorStudentServiceMock = new Mock<IProfessorStudentService>();
            _activityStudentServiceMock = new Mock<IActivityStudentService>();
            _controller = new ProfessorController(_professorStudentServiceMock.Object, _activityStudentServiceMock.Object);
        }

        [Test]
        public async Task LinkStudent_ReturnsOkWithMessage()
        {
            // Arrange
            var dto = new LinkStudentByEmailDto
            {
                StudentEmail = "student@example.com",
                ProfessorId = Guid.NewGuid()
            };
            _professorStudentServiceMock
                .Setup(s => s.LinkByEmailAsync(dto))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.LinkStudent(dto);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);

            // Serializa o resultado para verificar diretamente o JSON
            var json = System.Text.Json.JsonSerializer.Serialize(okResult.Value);
            Assert.That(json, Is.EqualTo("{\"message\":\"Aluno vinculado com sucesso!\"}"));
        }

        [Test]
        public async Task ManageStudent_ReturnsOkWithMessage()
        {
            // Arrange
            var dto = new UpdateProfessorStudentDto
            {
                StudentId = Guid.NewGuid(),
                ProfessorId = Guid.NewGuid()
            };

            _professorStudentServiceMock
                .Setup(s => s.UpdateLinkAsync(dto))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.ManageStudent(dto);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);

            // Usa reflexão para obter o valor da propriedade message
            var messageProp = okResult.Value?.GetType().GetProperty("message");
            Assert.IsNotNull(messageProp, "Propriedade 'message' não encontrada no resultado.");

            var messageValue = messageProp.GetValue(okResult.Value) as string;
            Assert.That(messageValue, Is.EqualTo("Vínculo atualizado com sucesso!"));
        }



        [Test]
        public async Task GetStudentsByProfessor_ReturnsList()
        {
            // Arrange
            var professorId = Guid.NewGuid();
            var aluno1 = new StudentDto() { Name = "Aluno 1" };
            var aluno2 = new StudentDto() { Name = "Aluno 2" };
            var expected = new List<StudentDto> { aluno1, aluno2 };
            _professorStudentServiceMock.Setup(s => s.GetStudentsByProfessorAsync(professorId)).Returns(Task.FromResult(expected));

            // Act
            var result = await _controller.GetStudentsByProfessor(professorId);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.That(okResult.Value, Is.EqualTo(expected));
        }

        [Test]
        public async Task AssignActivityToStudent_ReturnsOkWithMessage()
        {
            // Arrange
            var dto = new AssignActivityDto { ActivityId = Guid.NewGuid(), StudentId = Guid.NewGuid() };
            _activityStudentServiceMock.Setup(s => s.AssignActivityToStudentAsync(dto)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.AssignActivityToStudent(dto);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.That(okResult.Value, Is.EqualTo("Atividade atribuída com sucesso."));
        }

        [Test]
        public async Task EvaluateActivity_ReturnsOkWithMessage()
        {
            // Arrange
            var dto = new EvaluateActivityDto { StudentId = Guid.NewGuid(), ActivityId = Guid.NewGuid(), Score = 10 };
            _activityStudentServiceMock.Setup(s => s.EvaluateActivityAsync(dto)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.EvaluateActivity(dto);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.That(okResult.Value, Is.EqualTo("Atividade avaliada com sucesso."));
        }

        [Test]
        public async Task GetPendingReviews_ReturnsList()
        {
            // Arrange
            var professorId = Guid.NewGuid();
            var pendingreview = new PendingActivityReviewDto() { ActivityName = "Atividade teste" };
            var pendingReviews = new List<PendingActivityReviewDto> { pendingreview };
            _activityStudentServiceMock.Setup(s => s.GetPendingReviewsAsync(professorId)).ReturnsAsync(pendingReviews);

            // Act
            var result = await _controller.GetPendingReviews(professorId);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.That(okResult.Value, Is.EqualTo(pendingReviews));
        }

        [Test]
        public void LinkStudent_ThrowsException_Returns500()
        {
            var dto = new LinkStudentByEmailDto
            {
                StudentEmail = "student@example.com",
                ProfessorId = Guid.NewGuid()
            };

            _professorStudentServiceMock
                .Setup(s => s.LinkByEmailAsync(dto))
                .ThrowsAsync(new Exception("Erro interno"));

            Assert.ThrowsAsync<Exception>(async () => await _controller.LinkStudent(dto));
        }

        [Test]
        public void ManageStudent_HasAuthorizeAttribute()
        {
            var method = typeof(ProfessorController).GetMethod("ManageStudent");
            var attribute = method.GetCustomAttributes(typeof(AuthorizeAttribute), true);
            Assert.IsNotEmpty(attribute);
        }

        [Test]
        public async Task LinkStudent_InvalidModel_ReturnsBadRequest()
        {

            var dto = new LinkStudentByEmailDto();

            var result = await _controller.LinkStudent(dto);

            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }
    }
}
