using NUnit.Framework;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Octavus.App.Api.Controllers.v1;
using Octavus.Core.Application.DTO;
using Octavus.Core.Application.Services;

namespace Octavus.Tests.Controllers
{
    [TestFixture]
    public class UsersControllerTests
    {
        private Mock<IUserService> _userServiceMock;
        private UsersController _controller;

        [SetUp]
        public void Setup()
        {
            _userServiceMock = new Mock<IUserService>();
            _controller = new UsersController(_userServiceMock.Object);
        }

        [Test]
        public async Task Create_ReturnsCreatedResult_WithUser()
        {
            // Arrange
            var dto = new CreateUserDto
            {
                Email = "teste@email.com",
                Name = "Usuário Teste"
            };

            var createdUser = new UserDto
            {
                Id = Guid.NewGuid(),
                Email = dto.Email,
                Name = dto.Name
            };

            _userServiceMock.Setup(s => s.CreateAsync(dto)).ReturnsAsync(createdUser);

            // Act
            var result = await _controller.Create(dto);

            // Assert
            var createdResult = result as CreatedAtActionResult;
            Assert.IsNotNull(createdResult);
            Assert.That(createdResult.ActionName, Is.EqualTo(nameof(_controller.GetById)));
            Assert.That(((UserDto)createdResult.Value).Id, Is.EqualTo(createdUser.Id));
        }

        [Test]
        public async Task GetAll_ReturnsAllUsers()
        {
            // Arrange
            var users = new List<UserDto>
            {
                new UserDto { Id = Guid.NewGuid(), Name = "User 1", Email = "user1@email.com" },
                new UserDto { Id = Guid.NewGuid(), Name = "User 2", Email = "user2@email.com" }
            };

            _userServiceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(users);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.That(okResult.Value, Is.EqualTo(users));
        }

        [Test]
        public async Task GetById_ReturnsUser()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new UserDto { Id = userId, Name = "User X", Email = "x@email.com" };

            _userServiceMock.Setup(s => s.GetByIdAsync(userId)).ReturnsAsync(user);

            // Act
            var result = await _controller.GetById(userId);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.That(okResult.Value, Is.EqualTo(user));
        }

        [Test]
        public async Task Update_ReturnsNoContent()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var dto = new CreateUserDto { Name = "Atualizado", Email = "atualizado@email.com" };

            _userServiceMock.Setup(s => s.UpdateAsync(userId, dto)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Update(userId, dto);

            // Assert
            Assert.IsInstanceOf<NoContentResult>(result);
        }

        [Test]
        public async Task Delete_ReturnsNoContent()
        {
            // Arrange
            var userId = Guid.NewGuid();

            _userServiceMock.Setup(s => s.DeleteAsync(userId)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Delete(userId);

            // Assert
            Assert.IsInstanceOf<NoContentResult>(result);
        }
    }
}
