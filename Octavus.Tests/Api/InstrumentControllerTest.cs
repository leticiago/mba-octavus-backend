using Moq;
using NUnit.Framework;
using Octavus.App.Api.Controllers.v1;
using Octavus.Core.Application.Services;
using Octavus.Core.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Octavus.Tests.Controllers
{
    [TestFixture]
    public class InstrumentControllerTest
    {
        private Mock<IInstrumentService> _instrumentServiceMock;
        private InstrumentController _controller;

        [SetUp]
        public void Setup()
        {
            _instrumentServiceMock = new Mock<IInstrumentService>();
            _controller = new InstrumentController(_instrumentServiceMock.Object);
        }

        [Test]
        public async Task GetAll_ReturnsAllInstruments()
        {
            // Arrange
            var instruments = new List<Instrument>
            {
                new Instrument { Id = Guid.NewGuid(), Name = "Guitar" },
                new Instrument { Id = Guid.NewGuid(), Name = "Piano" }
            };
            _instrumentServiceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(instruments);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(instruments, okResult.Value);
        }

        [Test]
        public async Task GetById_ReturnsInstrument_WhenFound()
        {
            // Arrange
            var id = Guid.NewGuid();
            var instrument = new Instrument { Id = id, Name = "Flute" };
            _instrumentServiceMock.Setup(s => s.GetByIdAsync(id)).ReturnsAsync(instrument);

            // Act
            var result = await _controller.GetById(id);

            // Assert
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(instrument, okResult.Value);
        }

        [Test]
        public async Task GetById_ReturnsNotFound_WhenNotExists()
        {
            // Arrange
            var id = Guid.NewGuid();
            _instrumentServiceMock.Setup(s => s.GetByIdAsync(id)).ReturnsAsync((Instrument)null);

            // Act
            var result = await _controller.GetById(id);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result.Result);
        }

        [Test]
        public async Task Create_ReturnsCreatedInstrument()
        {
            // Arrange
            var input = new Instrument { Name = "Drums" };
            var created = new Instrument { Id = Guid.NewGuid(), Name = "Drums" };
            _instrumentServiceMock.Setup(s => s.CreateAsync(It.IsAny<Instrument>())).ReturnsAsync(created);

            // Act
            var result = await _controller.Create(input);

            // Assert
            var createdAtResult = result.Result as CreatedAtActionResult;
            Assert.IsNotNull(createdAtResult);
            Assert.AreEqual(201, createdAtResult.StatusCode);
            Assert.AreEqual(created, createdAtResult.Value);
        }

        [Test]
        public async Task Update_ReturnsNoContent_WhenSuccess()
        {
            // Arrange
            var id = Guid.NewGuid();
            var instrument = new Instrument { Id = id, Name = "Violin" };
            _instrumentServiceMock.Setup(s => s.UpdateAsync(instrument)).ReturnsAsync(true);

            // Act
            var result = await _controller.Update(id, instrument);

            // Assert
            Assert.IsInstanceOf<NoContentResult>(result);
        }

        [Test]
        public async Task Update_ReturnsNotFound_WhenUpdateFails()
        {
            // Arrange
            var id = Guid.NewGuid();
            var instrument = new Instrument { Id = id, Name = "Violin" };
            _instrumentServiceMock.Setup(s => s.UpdateAsync(instrument)).ReturnsAsync(false);

            // Act
            var result = await _controller.Update(id, instrument);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task Update_ReturnsBadRequest_WhenIdMismatch()
        {
            // Arrange
            var id = Guid.NewGuid();
            var instrument = new Instrument { Id = Guid.NewGuid(), Name = "Saxophone" };

            // Act
            var result = await _controller.Update(id, instrument);

            // Assert
            var badRequest = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequest);
            Assert.AreEqual(400, badRequest.StatusCode);
        }

        [Test]
        public async Task Delete_ReturnsNoContent_WhenSuccess()
        {
            // Arrange
            var id = Guid.NewGuid();
            _instrumentServiceMock.Setup(s => s.DeleteAsync(id)).ReturnsAsync(true);

            // Act
            var result = await _controller.Delete(id);

            // Assert
            Assert.IsInstanceOf<NoContentResult>(result);
        }

        [Test]
        public async Task Delete_ReturnsNotFound_WhenFails()
        {
            // Arrange
            var id = Guid.NewGuid();
            _instrumentServiceMock.Setup(s => s.DeleteAsync(id)).ReturnsAsync(false);

            // Act
            var result = await _controller.Delete(id);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
        }
    }
}
