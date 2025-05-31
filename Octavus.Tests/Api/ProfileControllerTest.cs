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
    public class ProfileControllerTest
    {
        private Mock<IProfileService> _profileServiceMock;
        private ProfileController _controller;

        [SetUp]
        public void Setup()
        {
            _profileServiceMock = new Mock<IProfileService>();
            _controller = new ProfileController(_profileServiceMock.Object);
        }

        [Test]
        public async Task GetAll_ReturnsAllProfiles()
        {
            // Arrange
            var profiles = new List<Profile>
            {
                new Profile { Id = Guid.NewGuid(), Name = "Aluno" },
                new Profile { Id = Guid.NewGuid(), Name = "Professor" }
            };
            _profileServiceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(profiles);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.That(okResult.StatusCode, Is.EqualTo(200));
            Assert.That(okResult.Value, Is.EqualTo(profiles));
        }

        [Test]
        public async Task GetById_ReturnsProfile_WhenFound()
        {
            // Arrange
            var id = Guid.NewGuid();
            var profile = new Profile { Id = id, Name = "Admin" };
            _profileServiceMock.Setup(s => s.GetByIdAsync(id)).ReturnsAsync(profile);

            // Act
            var result = await _controller.GetById(id);

            // Assert
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.That(okResult.StatusCode, Is.EqualTo(200));
            Assert.That(okResult.Value, Is.EqualTo(profile));
        }

        [Test]
        public async Task GetById_ReturnsNotFound_WhenNotExists()
        {
            // Arrange
            var id = Guid.NewGuid();
            _profileServiceMock.Setup(s => s.GetByIdAsync(id)).ReturnsAsync((Profile)null);

            // Act
            var result = await _controller.GetById(id);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result.Result);
        }

        [Test]
        public async Task Create_ReturnsCreatedProfile()
        {
            // Arrange
            var input = "Coordenador";
            var created = new Profile { Id = Guid.NewGuid(), Name = input };

            _profileServiceMock.Setup(s => s.CreateAsync(It.IsAny<Profile>())).ReturnsAsync(created);

            // Act
            var result = await _controller.Create(input);

            // Assert
            var createdAtResult = result.Result as CreatedAtActionResult;
            Assert.IsNotNull(createdAtResult);
            Assert.That(createdAtResult.StatusCode, Is.EqualTo(201));
            Assert.That(createdAtResult.Value, Is.EqualTo(created));
        }

        [Test]
        public async Task Update_ReturnsNoContent_WhenSuccess()
        {
            // Arrange
            var id = Guid.NewGuid();
            var profile = new Profile { Id = id, Name = "Aluno" };
            _profileServiceMock.Setup(s => s.UpdateAsync(profile)).ReturnsAsync(true);

            // Act
            var result = await _controller.Update(id, profile);

            // Assert
            Assert.IsInstanceOf<NoContentResult>(result);
        }

        [Test]
        public async Task Update_ReturnsNotFound_WhenUpdateFails()
        {
            // Arrange
            var id = Guid.NewGuid();
            var profile = new Profile { Id = id, Name = "Aluno" };
            _profileServiceMock.Setup(s => s.UpdateAsync(profile)).ReturnsAsync(false);

            // Act
            var result = await _controller.Update(id, profile);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task Update_ReturnsBadRequest_WhenIdMismatch()
        {
            // Arrange
            var id = Guid.NewGuid();
            var profile = new Profile { Id = Guid.NewGuid(), Name = "Aluno" };

            // Act
            var result = await _controller.Update(id, profile);

            // Assert
            var badRequest = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequest);
            Assert.That(badRequest.StatusCode, Is.EqualTo(400));
        }

        [Test]
        public async Task Delete_ReturnsNoContent_WhenSuccess()
        {
            // Arrange
            var id = Guid.NewGuid();
            _profileServiceMock.Setup(s => s.DeleteAsync(id)).ReturnsAsync(true);

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
            _profileServiceMock.Setup(s => s.DeleteAsync(id)).ReturnsAsync(false);

            // Act
            var result = await _controller.Delete(id);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
        }
    }
}
