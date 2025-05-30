using Moq;
using NUnit.Framework;
using Octavus.Core.Application.DTO;
using Octavus.Core.Application.Repositories;
using Octavus.Core.Application.Services;
using Octavus.Core.Domain.Entities;
using Octavus.Infra.Core.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Octavus.Tests.Services
{
    public class UserServiceTests
    {
        private Mock<IUserRepository> _userRepoMock;
        private Mock<IKeycloakService> _kcServiceMock;
        private UserService _service;

        [SetUp]
        public void Setup()
        {
            _userRepoMock = new Mock<IUserRepository>();
            _kcServiceMock = new Mock<IKeycloakService>();
            _service = new UserService(_userRepoMock.Object, _kcServiceMock.Object);
        }

        [Test]
        public async Task CreateAsync_ShouldCreateUser_WhenDataIsValid()
        {
            var dto = new CreateUserDto
            {
                Email = "user@example.com",
                Password = "Strong@123",
                Name = "John",
                Username = "johnny",
                Contact = "123456",
                InstrumentId = Guid.NewGuid(),
                ProfileId = Guid.NewGuid(),
                Roles = new List<string> { "admin" }
            };

            _kcServiceMock.Setup(k => k.CreateUserAndAssignRolesAsync(It.IsAny<KeycloakUser>())).ReturnsAsync(true);

            var result = await _service.CreateAsync(dto);

            Assert.That(result.Email, Is.EqualTo(dto.Email));
            Assert.That(result.Name, Is.EqualTo(dto.Name));
            Assert.That(result.Username, Is.EqualTo(dto.Username));

            _userRepoMock.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Once);
            _userRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [TestCase("invalid-email")]
        [TestCase("")]
        public void CreateAsync_ShouldThrow_WhenEmailIsInvalid(string invalidEmail)
        {
            var dto = new CreateUserDto
            {
                Email = invalidEmail,
                Password = "Strong@123",
                Name = "User",
                Username = "user",
                Contact = "123",
                InstrumentId = Guid.NewGuid(),
                ProfileId = Guid.NewGuid(),
                Roles = new List<string>()
            };

            var ex = Assert.ThrowsAsync<ArgumentException>(() => _service.CreateAsync(dto));
            StringAssert.Contains("e-mail", ex.Message);
        }

        [TestCase("short")]
        [TestCase("password")]
        [TestCase("NoSymbol123")]
        public void CreateAsync_ShouldThrow_WhenPasswordIsInvalid(string invalidPassword)
        {
            var dto = new CreateUserDto
            {
                Email = "user@example.com",
                Password = invalidPassword,
                Name = "User",
                Username = "user",
                Contact = "123",
                InstrumentId = Guid.NewGuid(),
                ProfileId = Guid.NewGuid(),
                Roles = new List<string>()
            };

            var ex = Assert.ThrowsAsync<ArgumentException>(() => _service.CreateAsync(dto));
            StringAssert.Contains("senha", ex.Message);
        }

        [Test]
        public async Task GetAllAsync_ShouldReturnUsers()
        {
            var users = new List<User>
            {
                new User { Id = Guid.NewGuid(), Email = "a@a.com", Name = "A", Username = "a", Contact = "111", Password = "Password" },
                new User { Id = Guid.NewGuid(), Email = "b@b.com", Name = "B", Username = "b", Contact = "222", Password = "Password" }
            };

            _userRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(users);

            var result = await _service.GetAllAsync();

            Assert.That(result.Count, Is.EqualTo(2));
        }

        [Test]
        public async Task GetByIdAsync_ShouldReturnUser_WhenFound()
        {
            var id = Guid.NewGuid();
            var user = new User { Id = id, Email = "a@a.com", Name = "A", Username = "a", Contact = "111", Password = "Password" };

            _userRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(user);

            var result = await _service.GetByIdAsync(id);

            Assert.That(result.Id, Is.EqualTo(user.Id));
        }

        [Test]
        public void GetByIdAsync_ShouldThrow_WhenNotFound()
        {
            var id = Guid.NewGuid();
            _userRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((User)null!);

            var ex = Assert.ThrowsAsync<Exception>(() => _service.GetByIdAsync(id));
            StringAssert.Contains("Usuário não encontrado", ex.Message);
        }

        [Test]
        public async Task UpdateAsync_ShouldUpdateUser_WhenFound()
        {
            var id = Guid.NewGuid();
            var existing = new User { Id = id, Email = "old@mail.com", Username = "old", Password = "Password", Name = "Name" };

            var updateDto = new CreateUserDto
            {
                Email = "new@mail.com",
                Username = "new",
                Password = "Updated@123",
                Name = "Updated",
                Contact = "9999"
            };

            _userRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(existing);

            await _service.UpdateAsync(id, updateDto);

            _userRepoMock.Verify(r => r.UpdateAsync(It.IsAny<User>()), Times.Once);
            _userRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task DeleteAsync_ShouldDeleteUser_WhenFound()
        {
            var id = Guid.NewGuid();
            var user = new User { Id = id, Password = "Password", Username = "USERNAME", Email = "email@teste.com", Name = "Name" };

            _userRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(user);

            await _service.DeleteAsync(id);

            _userRepoMock.Verify(r => r.DeleteAsync(id), Times.Once);
            _userRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public void DeleteAsync_ShouldThrow_WhenNotFound()
        {
            var id = Guid.NewGuid();
            _userRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((User)null!);

            var ex = Assert.ThrowsAsync<Exception>(() => _service.DeleteAsync(id));
            StringAssert.Contains("Usuário não encontrado", ex.Message);
        }
    }
}
