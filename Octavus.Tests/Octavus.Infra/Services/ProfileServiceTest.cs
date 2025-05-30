using Moq;
using NUnit.Framework;
using Octavus.Core.Application.Repositories;
using Octavus.Core.Domain.Entities;
using Octavus.Infra.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Octavus.Tests.Services
{
    [TestFixture]
    public class ProfileServiceTests
    {
        private Mock<IProfileRepository> _mockProfileRepository = null!;
        private ProfileService _service = null!;

        [SetUp]
        public void Setup()
        {
            _mockProfileRepository = new Mock<IProfileRepository>();
            _service = new ProfileService(_mockProfileRepository.Object);
        }

        [TearDown]
        public void Teardown()
        {
            // Nada a ser descartado por enquanto.
        }

        [Test]
        public async Task GetAllAsync_ShouldReturnAllProfiles()
        {
            var profiles = new List<Profile>
            {
                new Profile { Id = Guid.NewGuid(), Name = "Aluno" },
                new Profile { Id = Guid.NewGuid(), Name = "Professor" }
            };

            _mockProfileRepository
                .Setup(r => r.GetAllAsync())
                .ReturnsAsync(profiles);

            var result = await _service.GetAllAsync();

            Assert.That(result.Count(), Is.EqualTo(2));
        }

        [Test]
        public async Task GetByIdAsync_ShouldReturnProfile_WhenFound()
        {
            var id = Guid.NewGuid();
            var profile = new Profile { Id = id, Name = "Admin" };

            _mockProfileRepository
                .Setup(r => r.GetByIdAsync(id))
                .ReturnsAsync(profile);

            var result = await _service.GetByIdAsync(id);

            Assert.NotNull(result);
            Assert.That(result!.Name, Is.EqualTo("Admin"));
        }

        [Test]
        public async Task CreateAsync_ShouldAddProfileAndReturnIt()
        {
            var profile = new Profile { Id = Guid.NewGuid(), Name = "Aluno" };

            _mockProfileRepository.Setup(r => r.AddAsync(profile))
                .Returns(Task.CompletedTask);
            _mockProfileRepository.Setup(r => r.SaveChangesAsync())
                .ReturnsAsync(true);

            var result = await _service.CreateAsync(profile);

            _mockProfileRepository.Verify(r => r.AddAsync(profile), Times.Once);
            _mockProfileRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
            Assert.That(result, Is.EqualTo(profile));
        }

        [Test]
        public async Task UpdateAsync_ShouldUpdateAndReturnTrue_WhenProfileExists()
        {
            var id = Guid.NewGuid();
            var profile = new Profile { Id = id, Name = "Novo Nome" };

            var existing = new Profile { Id = id, Name = "Antigo Nome" };

            _mockProfileRepository.Setup(r => r.GetByIdAsync(id))
                .ReturnsAsync(existing);
            _mockProfileRepository.Setup(r => r.UpdateAsync(existing))
                .Returns(Task.CompletedTask);
            _mockProfileRepository.Setup(r => r.SaveChangesAsync())
                .ReturnsAsync(true);

            var result = await _service.UpdateAsync(profile);

            Assert.IsTrue(result);
            Assert.That(existing.Name, Is.EqualTo("Novo Nome"));
        }

        [Test]
        public async Task UpdateAsync_ShouldReturnFalse_WhenProfileNotFound()
        {
            var profile = new Profile { Id = Guid.NewGuid(), Name = "Não existe" };

            _mockProfileRepository.Setup(r => r.GetByIdAsync(profile.Id))
                .ReturnsAsync((Profile?)null);

            var result = await _service.UpdateAsync(profile);

            Assert.IsFalse(result);
        }

        [Test]
        public async Task DeleteAsync_ShouldDeleteAndReturnTrue_WhenProfileExists()
        {
            var id = Guid.NewGuid();
            var profile = new Profile { Id = id, Name = "Excluir" };

            _mockProfileRepository.Setup(r => r.GetByIdAsync(id))
                .ReturnsAsync(profile);
            _mockProfileRepository.Setup(r => r.DeleteAsync(id))
                .Returns(Task.CompletedTask);
            _mockProfileRepository.Setup(r => r.SaveChangesAsync())
                .ReturnsAsync(true);

            var result = await _service.DeleteAsync(id);

            Assert.IsTrue(result);
        }

        [Test]
        public async Task DeleteAsync_ShouldReturnFalse_WhenProfileNotFound()
        {
            var id = Guid.NewGuid();

            _mockProfileRepository.Setup(r => r.GetByIdAsync(id))
                .ReturnsAsync((Profile?)null);

            var result = await _service.DeleteAsync(id);

            Assert.IsFalse(result);
        }
    }
}
