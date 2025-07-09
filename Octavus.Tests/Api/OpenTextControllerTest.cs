using NUnit.Framework;
using Moq;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Octavus.App.Api.Controllers.v1;
using Octavus.Core.Application.DTO;
using Octavus.Core.Application.Services;
using Octavus.Core.Domain.Entities;

namespace Octavus.Tests.Controllers
{
    [TestFixture]
    public class OpenTextControllerTests
    {
        private Mock<IQuestionService> _questionServiceMock;
        private Mock<IOpenTextAnswerService> _openTextAnswerServiceMock;
        private OpenTextController _controller;

        [SetUp]
        public void Setup()
        {
            _questionServiceMock = new Mock<IQuestionService>();
            _openTextAnswerServiceMock = new Mock<IOpenTextAnswerService>();
            _controller = new OpenTextController(_questionServiceMock.Object, _openTextAnswerServiceMock.Object);
        }

        [Test]
        public async Task CreateQuestion_ReturnsStatus201()
        {
            // Arrange
            var dto = new CreateQuestionOpenTextDto
            {
                Title = "Sample question",
                ActivityId = Guid.NewGuid()
            };

            var question = new Question()
            {
                ActivityId = dto.ActivityId,
                Title = dto.Title,
            };

            _questionServiceMock.Setup(s => s.CreateAsync(It.IsAny<QuestionOpenTextDto>()))
                .Returns(Task.FromResult(question));

            // Act
            var result = await _controller.CreateQuestion(dto);

            // Assert
            var statusResult = result as StatusCodeResult;
            Assert.IsNotNull(statusResult);
            Assert.That(statusResult.StatusCode, Is.EqualTo(201));
            _questionServiceMock.Verify(s => s.CreateAsync(It.Is<QuestionOpenTextDto>(q =>
                q.Title == dto.Title && q.ActivityId == dto.ActivityId && q.Id != Guid.Empty)), Times.Once);
        }

        [Test]
        public async Task GetQuestion_ReturnsOkWithQuestion()
        {
            // Arrange
            var questionId = Guid.NewGuid();
            var questionDto = new QuestionOpenTextDto
            {
                Id = questionId,
                Title = "Question Title",
                ActivityId = Guid.NewGuid()
            };

            var question = new QuestionDto()
            {
                Id = questionDto.Id,
                Title = questionDto.Title,
            };

            var expectedList = new List<QuestionDto> { question };

            _questionServiceMock
                .Setup(s => s.GetByIdAsync(questionId))
                .ReturnsAsync(expectedList);

            // Act
            var result = await _controller.GetQuestion(questionId);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);

            var actualList = okResult.Value as List<QuestionDto>;
            Assert.IsNotNull(actualList);
            Assert.That(actualList.Count, Is.EqualTo(1));
            Assert.That(actualList[0].Id, Is.EqualTo(question.Id));
            Assert.That(actualList[0].Title, Is.EqualTo(question.Title));
        }


        [Test]
        public async Task CreateAnswer_ReturnsStatus201()
        {
            // Arrange
            var dto = new OpenTextAnswer
            {
                Id = Guid.NewGuid(),
                QuestionId = Guid.NewGuid(),
                StudentId = Guid.NewGuid()
            };

            _openTextAnswerServiceMock.Setup(s => s.CreateAsync(dto)).Returns(Task.FromResult(dto));

            // Act
            var result = await _controller.CreateAnswer(dto);

            // Assert
            var statusResult = result as StatusCodeResult;
            Assert.IsNotNull(statusResult);
            Assert.That(statusResult.StatusCode, Is.EqualTo(201));
            _openTextAnswerServiceMock.Verify(s => s.CreateAsync(dto), Times.Once);
        }

        [Test]
        public async Task GetAnswer_ReturnsOkWithAnswer()
        {
            // Arrange
            var answerId = Guid.NewGuid();
            var answer = new OpenTextAnswer
            {
                Id = answerId,
                QuestionId = Guid.NewGuid(),
                StudentId = Guid.NewGuid()
            };

            _openTextAnswerServiceMock.Setup(s => s.GetByIdAsync(answer.QuestionId, answer.StudentId)).ReturnsAsync(answer);

            // Act
            var result = await _controller.GetAnswer(answer.QuestionId, answer.StudentId);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.That(okResult.Value, Is.EqualTo(answer));
        }
    }
}
