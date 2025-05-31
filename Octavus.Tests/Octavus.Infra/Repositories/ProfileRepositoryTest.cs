using NUnit.Framework;
using Octavus.Core.Domain.Entities;
using Octavus.Infra.Persistence;
using Octavus.Infra.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Octavus.Tests.Helpers;

namespace Octavus.Tests.Repositories
{
    [TestFixture]
    public class ProfileRepositoryTests
    {
        private Context _context = null!;
        private ProfileRepository _repository = null!;

        [SetUp]
        public void Setup()
        {
            _context = TestContextProvider.CreateContext();
            _repository = new ProfileRepository(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task AddAsync_ShouldAddProfile()
        {
            var profile = new Profile
            {
                Id = Guid.NewGuid(),
                Name = "Perfil Teste"
            };

            await _repository.AddAsync(profile);

            var all = await _repository.GetAllAsync();
            Assert.That(all, Has.One.Items);
        }

        [Test]
        public async Task GetByIdAsync_ShouldReturnProfile_WhenExists()
        {
            var profile = new Profile
            {
                Id = Guid.NewGuid(),
                Name = "Perfil Consulta"
            };

            await _repository.AddAsync(profile);

            var found = await _repository.GetByIdAsync(profile.Id);
            Assert.IsNotNull(found);
            Assert.That(found!.Name, Is.EqualTo(profile.Name));
        }

        [Test]
        public async Task GetByIdAsync_ShouldReturnNull_WhenNotFound()
        {
            var found = await _repository.GetByIdAsync(Guid.NewGuid());
            Assert.IsNull(found);
        }

        [Test]
        public async Task UpdateAsync_ShouldUpdateProfile()
        {
            var profile = new Profile
            {
                Id = Guid.NewGuid(),
                Name = "Perfil Original"
            };

            await _repository.AddAsync(profile);

            profile.Name = "Perfil Atualizado";
            await _repository.UpdateAsync(profile);

            var updated = await _repository.GetByIdAsync(profile.Id);
            Assert.That(updated!.Name, Is.EqualTo("Perfil Atualizado"));
        }

        [Test]
        public async Task DeleteAsync_ShouldRemoveProfile_WhenExists()
        {
            var profile = new Profile
            {
                Id = Guid.NewGuid(),
                Name = "Perfil Para Deletar"
            };

            await _repository.AddAsync(profile);

            await _repository.DeleteAsync(profile.Id);

            var all = await _repository.GetAllAsync();
            Assert.That(all, Is.Empty);
        }

        [Test]
        public async Task GetAllAsync_ShouldReturnAllProfiles()
        {
            var profile1 = new Profile { Id = Guid.NewGuid(), Name = "Perfil 1" };
            var profile2 = new Profile { Id = Guid.NewGuid(), Name = "Perfil 2" };

            await _repository.AddAsync(profile1);
            await _repository.AddAsync(profile2);

            var all = await _repository.GetAllAsync();

            Assert.That(all.Count, Is.EqualTo(2));
            Assert.That(all.Any(p => p.Name == "Perfil 1"), Is.True);
            Assert.That(all.Any(p => p.Name == "Perfil 2"), Is.True);
        }

        [Test]
        public void DeleteAsync_ShouldNotThrow_WhenProfileDoesNotExist()
        {
            Assert.DoesNotThrowAsync(async () => await _repository.DeleteAsync(Guid.NewGuid()));
        }

        [Test]
        public void UpdateAsync_ShoulThrow_WhenProfileDoesNotExist()
        {
            var profile = new Profile { Id = Guid.NewGuid(), Name = "Perfil Inexistente" };
            Assert.ThrowsAsync<DbUpdateConcurrencyException>(async () => await _repository.UpdateAsync(profile));
        }

    }
}
