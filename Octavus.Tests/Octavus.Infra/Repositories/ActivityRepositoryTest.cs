using Castle.Core.Logging;
using NUnit.Framework;
using Octavus.Core.Domain.Entities;
using Octavus.Core.Domain.Enums;
using Octavus.Infra.Persistence;
using Octavus.Infra.Persistence.Repositories;
using Octavus.Tests.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Octavus.Tests.Repositories
{
    [TestFixture]
    public class ActivityRepositoryTests
    {
        private Context _context = null!;
        private ActivityRepository _repository = null!;

        [SetUp]
        public void Setup()
        {
            _context = TestContextProvider.CreateContext();
            _repository = new ActivityRepository(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task GetByProfessorIdAsync_ShouldReturnActivitiesForGivenProfessor()
        {
            var professorId = Guid.NewGuid();

            var activities = new List<Activity>
            {
                new Activity { Id = Guid.NewGuid(), ProfessorId = professorId, IsPublic = false, Description = "description", Name = "name", Level = Level.Intermediate.ToString(), Type = ActivityType.OpenText.ToString() },
                new Activity { Id = Guid.NewGuid(), ProfessorId = professorId, IsPublic = true,Description = "description", Name = "name", Level = Level.Intermediate.ToString(), Type = ActivityType.OpenText.ToString() },
                new Activity { Id = Guid.NewGuid(), ProfessorId = Guid.NewGuid(), IsPublic = true, Description = "description", Name = "name", Level = Level.Intermediate.ToString(), Type = ActivityType.OpenText.ToString() }
            };

            await _context.AddRangeAsync(activities);
            await _context.SaveChangesAsync();

            var result = await _repository.GetByProfessorIdAsync(professorId);

            Assert.That(result, Has.Exactly(2).Items);
            Assert.IsTrue(result.All(a => a.ProfessorId == professorId));
        }

        [Test]
        public async Task GetPublicActivitiesAsync_ShouldReturnOnlyPublicActivities()
        {
            var activities = new List<Activity>
            {
                new Activity { Id = Guid.NewGuid(), IsPublic = true, Description = "description", Name = "name", Level = Level.Intermediate.ToString(), Type = ActivityType.OpenText.ToString()},
                new Activity { Id = Guid.NewGuid(), IsPublic = false, Description = "description", Name = "name", Level = Level.Intermediate.ToString(), Type = ActivityType.OpenText.ToString()},
                new Activity { Id = Guid.NewGuid(), IsPublic = true, Description = "description", Name = "name", Level = Level.Intermediate.ToString(), Type = ActivityType.OpenText.ToString()}
            };

            await _context.AddRangeAsync(activities);
            await _context.SaveChangesAsync();

            var result = await _repository.GetPublicActivitiesAsync();

            Assert.That(result, Has.Exactly(2).Items);
            Assert.IsTrue(result.All(a => a.IsPublic));
        }

        [Test]
        public async Task GetAllByIds_ShouldReturnActivitiesMatchingGivenIds()
        {
            var existingActivities = new List<Activity>
            {
                new Activity { Id = Guid.NewGuid(), Description = "description", Name = "name", Level = Level.Intermediate.ToString(), Type = ActivityType.OpenText.ToString() },
                new Activity { Id = Guid.NewGuid(), Description = "description", Name = "name", Level = Level.Intermediate.ToString(), Type = ActivityType.OpenText.ToString() },
                new Activity {Id = Guid.NewGuid(), Description = "description", Name = "name", Level = Level.Intermediate.ToString(), Type = ActivityType.OpenText.ToString()}
            };

            await _context.AddRangeAsync(existingActivities);
            await _context.SaveChangesAsync();

            var searchIds = new List<Guid> { existingActivities[0].Id, existingActivities[2].Id, Guid.NewGuid() };

            var result = await _repository.GetAllByIds(searchIds);

            Assert.That(result.Count, Is.EqualTo(2));
            Assert.IsTrue(result.All(a => searchIds.Contains(a.Id)));
        }

        [Test]
        public async Task AddAsync_ShouldAddActivity()
        {
            var activity = new Activity { Id = Guid.NewGuid(), ProfessorId = Guid.NewGuid(), IsPublic = true, Description = "description", Name = "name", Level = Level.Intermediate.ToString(), Type = ActivityType.OpenText.ToString() };
            await _repository.AddAsync(activity);

            var saved = await _repository.GetByIdAsync(activity.Id);
            Assert.IsNotNull(saved);
            Assert.That(saved!.Id, Is.EqualTo(activity.Id));
        }

        [Test]
        public async Task GetAllByIds_ShouldReturnEmptyList_WhenInputListIsEmpty()
        {
            var result = await _repository.GetAllByIds(new List<Guid>());

            Assert.That(result, Is.Empty);
        }

        [Test]
        public async Task GetByProfessorIdAsync_ShouldReturnEmpty_WhenNoMatch()
        {
            var result = await _repository.GetByProfessorIdAsync(Guid.NewGuid());

            Assert.That(result, Is.Empty);
        }

        [Test]
        public async Task GetPublicActivitiesAsync_ShouldReturnEmpty_WhenNoPublicActivities()
        {
            var activities = new List<Activity>
            {
                new Activity { Id = Guid.NewGuid(), IsPublic = false, Description = "description", Name = "name", Level = Level.Intermediate.ToString(), Type = ActivityType.OpenText.ToString()}
            };

            await _context.AddRangeAsync(activities);
            await _context.SaveChangesAsync();

            var result = await _repository.GetPublicActivitiesAsync();

            Assert.That(result, Is.Empty);
        }

    }
}
