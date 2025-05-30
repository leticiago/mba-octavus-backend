using NUnit.Framework;
using Moq;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Octavus.App.Api.Controllers.v1;
using Octavus.Core.Application.DTO;
using Octavus.Core.Application.Services;

namespace Octavus.Tests.Controllers
{
    [TestFixture]
    public class QuestionControllerTests
    {
        private Mock<IQuestionService> _questionServiceMock;
        private QuestionController _controller;

        [SetUp]
        public void Setup()
        {
            _questionServiceMock = new Mock<IQuestionService>();
            _controller = new QuestionController(_questionServiceMock.Object);
        }

        [Test]
        public async Task CreateBatch_ReturnsStatus201()
        {
            // Arrange
            var dto = new CreateQuestionBatchDto();
            _questionServiceMock.Setup(s => s.AddQuestionsBatchAsync(dto)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.CreateBatch(dto);

            // Assert
            var statusResult = result as StatusCodeResult;
            Assert.IsNotNull(statusResult);
            Assert.AreEqual(201, statusResult.StatusCode);
            _questionServiceMock.Verify(s => s.AddQuestionsBatchAsync(dto), Times.Once);
        }

        [Test]
        public async Task GetAll_ReturnsOkWithList()
        {
            // Arrange
            var questions = new List<QuestionDto> { new QuestionDto(), new QuestionDto() };
            _questionServiceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(questions);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(questions, okResult.Value);
        }

        [Test]
        public async Task GetByActivity_ReturnsOkWithResult()
        {
            // Arrange
            var activityId = Guid.NewGuid();
            var expectedQuestion = new QuestionDto();
            var expectedList = new List<QuestionDto> { expectedQuestion };

            _questionServiceMock
                .Setup(s => s.GetByIdAsync(activityId))
                .ReturnsAsync(expectedList);

            // Act
            var result = await _controller.GetByActivity(activityId);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);

            var actualList = okResult.Value as List<QuestionDto>;
            Assert.IsNotNull(actualList);
            Assert.AreEqual(1, actualList.Count);
            Assert.AreEqual(expectedQuestion, actualList[0]);
        }


        [Test]
        public async Task Update_ReturnsNoContent()
        {
            // Arrange
            var id = Guid.NewGuid();
            var dto = new CreateQuestionDto();
            _questionServiceMock.Setup(s => s.UpdateAsync(id, dto)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Update(id, dto);

            // Assert
            Assert.IsInstanceOf<NoContentResult>(result);
            _questionServiceMock.Verify(s => s.UpdateAsync(id, dto), Times.Once);
        }

        [Test]
        public async Task Delete_ReturnsNoContent()
        {
            // Arrange
            var id = Guid.NewGuid();
            _questionServiceMock.Setup(s => s.DeleteAsync(id)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Delete(id);

            // Assert
            Assert.IsInstanceOf<NoContentResult>(result);
            _questionServiceMock.Verify(s => s.DeleteAsync(id), Times.Once);
        }
    }
}
