using NUnit.Framework;
using Octavus.Infra.Persistence;
using Octavus.Infra.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using System.Linq;
using Octavus.Tests.Helpers;
using System.Collections.Generic;
using Octavus.Core.Domain.Entities;

namespace Octavus.Tests.Repositories
{
    [TestFixture]
    public class RepositoryBaseTests
    {
        private Context _context = null!;
        private RepositoryBase<Profile> _repository = null!;

        [SetUp]
        public void Setup()
        {
            _context = TestContextProvider.CreateContext();
            _repository = new RepositoryBase<Profile>(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task AddAsync_ShouldAddEntity()
        {
            var entity = new Profile { Id = Guid.NewGuid(), Name = "Test" };

            await _repository.AddAsync(entity);

            var all = await _repository.GetAllAsync();
            Assert.That(all.Count(), Is.EqualTo(1));
            Assert.That(all.First().Name, Is.EqualTo("Test"));
        }

        [Test]
        public async Task GetByIdAsync_ShouldReturnEntity_WhenExists()
        {
            var entity = new Profile { Id = Guid.NewGuid(), Name = "Test" };
            await _repository.AddAsync(entity);

            var result = await _repository.GetByIdAsync(entity.Id);

            Assert.IsNotNull(result);
            Assert.That(result!.Id, Is.EqualTo(entity.Id));
        }

        [Test]
        public async Task GetByIdAsync_ShouldReturnNull_WhenNotExists()
        {
            var result = await _repository.GetByIdAsync(Guid.NewGuid());
            Assert.IsNull(result);
        }

        [Test]
        public async Task GetAllAsync_ShouldReturnAllEntities()
        {
            var entities = new List<Profile>
            {
                new() { Id = Guid.NewGuid(), Name = "Test 1" },
                new() { Id = Guid.NewGuid(), Name = "Test 2" }
            };

            foreach (var e in entities)
                await _repository.AddAsync(e);

            var all = await _repository.GetAllAsync();

            Assert.That(all.Count(), Is.EqualTo(2));
        }

        [Test]
        public async Task UpdateAsync_ShouldUpdateEntity()
        {
            var entity = new Profile { Id = Guid.NewGuid(), Name = "OldName" };
            await _repository.AddAsync(entity);

            entity.Name = "NewName";
            await _repository.UpdateAsync(entity);

            var updated = await _repository.GetByIdAsync(entity.Id);
            Assert.That(updated!.Name, Is.EqualTo("NewName"));
        }

        [Test]
        public async Task DeleteAsync_ShouldRemoveEntity_WhenExists()
        {
            var entity = new Profile { Id = Guid.NewGuid(), Name = "ToDelete" };
            await _repository.AddAsync(entity);

            await _repository.DeleteAsync(entity.Id);

            var deleted = await _repository.GetByIdAsync(entity.Id);
            Assert.IsNull(deleted);
        }

        [Test]
        public async Task DeleteAsync_ShouldNotThrow_WhenEntityDoesNotExist()
        {
            Assert.DoesNotThrowAsync(async () => await _repository.DeleteAsync(Guid.NewGuid()));
        }
    }
}
