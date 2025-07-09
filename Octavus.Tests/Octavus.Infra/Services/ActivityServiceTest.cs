using Moq;
using NUnit.Framework;
using Octavus.Core.Application.DTO;
using Octavus.Core.Application.Repositories;
using Octavus.Core.Application.Services;
using Octavus.Core.Domain.Entities;
using Octavus.Core.Domain.Enums;
using Octavus.Infra.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Octavus.Tests.Services
{
    public class ActivityServiceTests
    {
        private Mock<IActivityRepository> _repositoryMock;
        private IActivityService _service;

        [SetUp]
        public void SetUp()
        {
            _repositoryMock = new Mock<IActivityRepository>();
            _service = new ActivityService(_repositoryMock.Object);
        }

        [Test]
        public async Task CreateAsync_ShouldReturnDto()
        {
            var dto = new CreateActivityDto
            {
                Name = "Nome",
                Description = "Desc",
                Date = DateTime.UtcNow,
                InstrumentId = Guid.NewGuid(),
                Level = Level.Intermediate,
                ProfessorId = Guid.NewGuid(),
                IsPublic = true,
                Type = ActivityType.QuestionAndAnswer
            };

            var result = await _service.CreateAsync(dto);

            _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Activity>()), Times.Once);
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Name, Is.EqualTo(dto.Name));
        }

        [Test]
        public async Task GetAllAsync_ShouldReturnMappedDtos()
        {
            var activities = new List<Activity>
            {
                new Activity
                {
                    Id = Guid.NewGuid(),
                    Name = "Atividade 1",
                    Description = "Desc",
                    Date = DateTime.UtcNow,
                    InstrumentId = Guid.NewGuid(),
                    Level = "Beginner",
                    ProfessorId = Guid.NewGuid(),
                    IsPublic = true,
                    Type = ActivityType.QuestionAndAnswer.ToString(),
                }
            };

            _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(activities);

            var result = await _service.GetAllAsync();

            Assert.That(result.Count(), Is.EqualTo(1));
            Assert.That(result.First().Name, Is.EqualTo("Atividade 1"));
        }

        [Test]
        public async Task GetByIdAsync_ShouldReturnMappedDto()
        {
            var id = Guid.NewGuid();
            var entity = new Activity
            {
                Id = id,
                Name = "Teste",
                Description = "Desc",
                Date = DateTime.UtcNow,
                InstrumentId = Guid.NewGuid(),
                Level = "Beginner",
                ProfessorId = Guid.NewGuid(),
                IsPublic = false,
                Type = ActivityType.QuestionAndAnswer.ToString(),
            };

            _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(entity);

            var result = await _service.GetByIdAsync(id);

            Assert.That(result.Id, Is.EqualTo(id));
            Assert.That(result.Name, Is.EqualTo("Teste"));
        }

        [Test]
        public async Task GetByProfessorIdAsync_ShouldReturnDtos()
        {
            var professorId = Guid.NewGuid();
            var activities = new List<Activity>
            {
                new Activity
                {
                    Id = Guid.NewGuid(),
                    Name = "Aula",
                    Description = "Desc",
                    Date = DateTime.UtcNow,
                    InstrumentId = Guid.NewGuid(),
                    Level = "Beginner",
                    ProfessorId = professorId,
                    IsPublic = true,
                    Type = ActivityType.QuestionAndAnswer.ToString(),
                }
            };

            _repositoryMock.Setup(r => r.GetByProfessorIdAsync(professorId, null)).ReturnsAsync(activities);

            var result = await _service.GetByProfessorIdAsync(professorId, null);

            Assert.That(result.Count(), Is.EqualTo(1));
            Assert.That(result.First().ProfessorId, Is.EqualTo(professorId));
        }

        [Test]
        public async Task UpdateAsync_WithValidId_ShouldUpdate()
        {
            var id = Guid.NewGuid();
            var existing = new Activity { Id = id };
            var dto = new CreateActivityDto
            {
                Name = "Nova Atividade",
                Description = "Nova Desc",
                Date = DateTime.UtcNow,
                InstrumentId = Guid.NewGuid(),
                Level = Level.Advanced,
                ProfessorId = Guid.NewGuid(),
                IsPublic = false,
                Type = ActivityType.DragAndDrop
            };

            _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(existing);

            await _service.UpdateAsync(id, dto);

            _repositoryMock.Verify(r => r.UpdateAsync(It.Is<Activity>(a => a.Name == "Nova Atividade")), Times.Once);
        }

        [Test]
        public void UpdateAsync_WithInvalidId_ShouldThrow()
        {
            _repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Activity)null);

            var dto = new CreateActivityDto();
            var ex = Assert.ThrowsAsync<Exception>(async () =>
                await _service.UpdateAsync(Guid.NewGuid(), dto));

            Assert.That(ex.Message, Is.EqualTo("Atividade não encontrada"));
        }

        [Test]
        public async Task DeleteAsync_WithValidId_ShouldDelete()
        {
            var id = Guid.NewGuid();
            var activity = new Activity { Id = id };

            _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(activity);

            await _service.DeleteAsync(id);

            _repositoryMock.Verify(r => r.DeleteAsync(id), Times.Once);
        }

        [Test]
        public void DeleteAsync_WithInvalidId_ShouldThrow()
        {
            _repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Activity)null);

            var ex = Assert.ThrowsAsync<Exception>(async () =>
                await _service.DeleteAsync(Guid.NewGuid()));

            Assert.That(ex.Message, Is.EqualTo("Atividade não encontrada"));
        }

        [Test]
        public async Task GetPublicActivitiesAsync_ShouldReturnDtos()
        {
            var list = new List<Activity>
            {
                new Activity { Id = Guid.NewGuid(), Name = "Pública", Description = "Livre", Type = ActivityType.DragAndDrop.ToString(), Level = Level.Beginner.ToString() }
            };

            _repositoryMock.Setup(r => r.GetPublicActivitiesAsync()).ReturnsAsync(list);

            var result = await _service.GetPublicActivitiesAsync();

            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0].Name, Is.EqualTo("Pública"));
        }

        [Test]
        public async Task GetAllAsync_WithEmptyList_ShouldReturnEmpty()
        {
            _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Activity>());

            var result = await _service.GetAllAsync();

            Assert.IsNotNull(result);
            Assert.That(result.Count(), Is.EqualTo(0));
        }

        [Test]
        public async Task GetByProfessorIdAsync_WithEmptyList_ShouldReturnEmpty()
        {
            var professorId = Guid.NewGuid();
            _repositoryMock.Setup(r => r.GetByProfessorIdAsync(professorId, null)).ReturnsAsync(new List<Activity>());

            var result = await _service.GetByProfessorIdAsync(professorId, null);

            Assert.IsNotNull(result);
            Assert.That(result.Count(), Is.EqualTo(0));
        }

        [Test]
        public async Task UpdateAsync_ShouldUpdateAllFields()
        {
            var id = Guid.NewGuid();
            var existing = new Activity
            {
                Id = id,
                Name = "Old Name",
                Description = "Old Desc",
                Date = DateTime.UtcNow.AddDays(-1),
                InstrumentId = Guid.NewGuid(),
                Level = Level.Beginner.ToString(),
                ProfessorId = Guid.NewGuid(),
                IsPublic = true,
                Type = ActivityType.QuestionAndAnswer.ToString()
            };

            var dto = new CreateActivityDto
            {
                Name = "New Name",
                Description = "New Desc",
                Date = DateTime.UtcNow,
                InstrumentId = Guid.NewGuid(),
                Level = Level.Advanced,
                ProfessorId = Guid.NewGuid(),
                IsPublic = false,
                Type = ActivityType.DragAndDrop
            };

            _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(existing);

            await _service.UpdateAsync(id, dto);

            _repositoryMock.Verify(r => r.UpdateAsync(It.Is<Activity>(a =>
                a.Name == existing.Name &&
                a.Description == existing.Description &&
                a.Date == existing.Date &&
                a.InstrumentId == existing.InstrumentId &&
                a.Level == existing.Level.ToString() &&
                a.ProfessorId == existing.ProfessorId &&
                a.IsPublic == existing.IsPublic &&
                a.Type == existing.Type.ToString()
            )), Times.Once);
        }

        [Test]
        public void UpdateAsync_WithNullDto_ShouldThrowExceptionAtividadeNaoEncontrada()
        {
            var id = Guid.NewGuid();

            _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Activity)null);

            var ex = Assert.ThrowsAsync<Exception>(async () =>
                await _service.UpdateAsync(id, null));

            Assert.That(ex.Message, Is.EqualTo("Atividade não encontrada"));
        }


        [Test]
        public async Task GetPublicActivitiesAsync_ShouldMapAllFieldsCorrectly()
        {
            var id = Guid.NewGuid();
            var activities = new List<Activity>
            {
                new Activity { Id = id, Name = "Public Activity", Description = "Description", IsPublic = true, Type = ActivityType.QuestionAndAnswer.ToString(), Level = Level.Beginner.ToString() }
            };

            _repositoryMock.Setup(r => r.GetPublicActivitiesAsync()).ReturnsAsync(activities);

            var result = await _service.GetPublicActivitiesAsync();

            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0].Id, Is.EqualTo(id));
            Assert.That(result[0].Name, Is.EqualTo("Public Activity"));
            Assert.That(result[0].Description, Is.EqualTo("Description"));
        }

    }
}
