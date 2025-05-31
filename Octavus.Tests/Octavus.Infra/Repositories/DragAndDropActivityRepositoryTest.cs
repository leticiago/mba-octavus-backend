using NUnit.Framework;
using Octavus.Core.Domain.Entities;
using Octavus.Infra.Persistence;
using Octavus.Infra.Persistence.Repositories;
using Octavus.Tests.Helpers;
using System;
using System.Threading.Tasks;

namespace Octavus.Tests.Repositories
{
    [TestFixture]
    public class DragAndDropActivityRepositoryTests
    {
        private Context _context = null!;
        private DragAndDropActivityRepository _repository = null!;

        [SetUp]
        public void Setup()
        {
            _context = TestContextProvider.CreateContext();
            _repository = new DragAndDropActivityRepository(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task GetByActivityIdAsync_ShouldReturnEntity_WhenExists()
        {
            var activityId = Guid.NewGuid();

            var entity = new DragAndDropActivity
            {
                Id = Guid.NewGuid(),
                ActivityId = activityId,
                Text = "Option1;Option2;Option3"
            };

            await _context.AddAsync(entity);
            await _context.SaveChangesAsync();

            var result = await _repository.GetByActivityIdAsync(activityId);

            Assert.IsNotNull(result);
            Assert.That(result.ActivityId, Is.EqualTo(activityId));
            Assert.That(result.Text, Is.EqualTo(entity.Text));
        }

        [Test]
        public async Task GetByActivityIdAsync_ShouldReturnNull_WhenNotExists()
        {
            var result = await _repository.GetByActivityIdAsync(Guid.NewGuid());

            Assert.IsNull(result);
        }
        [Test]
        public async Task GetByActivityIdAsync_ShouldReturnCorrectEntity_WhenMultipleEntitiesExist()
        {
            var activityId1 = Guid.NewGuid();
            var activityId2 = Guid.NewGuid();

            var entity1 = new DragAndDropActivity
            {
                Id = Guid.NewGuid(),
                ActivityId = activityId1,
                Text = "Option1;Option2"
            };

            var entity2 = new DragAndDropActivity
            {
                Id = Guid.NewGuid(),
                ActivityId = activityId2,
                Text = "Other1;Other2"
            };

            await _context.AddRangeAsync(entity1, entity2);
            await _context.SaveChangesAsync();

            var result = await _repository.GetByActivityIdAsync(activityId2);

            Assert.IsNotNull(result);
            Assert.That(result.ActivityId, Is.EqualTo(activityId2));
            Assert.That(result.Text, Is.EqualTo("Other1;Other2"));
        }

    }
}
