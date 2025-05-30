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
    public class UserRepositoryTests
    {
        private Context _context = null!;
        private UserRepository _repository = null!;

        [SetUp]
        public void Setup()
        {
            _context = TestContextProvider.CreateContext();
            _repository = new UserRepository(_context);

            SeedData();
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        private void SeedData()
        {
            _context.Set<User>().AddRange(
                new User { Id = Guid.NewGuid(), Name = "Alice", Email = "alice@example.com", Password = "1234", Username = "alice" },
                new User { Id = Guid.NewGuid(), Name = "Bob", Email = "bob@example.com", Password = "abcd", Username = "bob" }
            );

            _context.SaveChanges();
        }

        [Test]
        public async Task GetByEmailAsync_ShouldReturnUser_WhenEmailExists()
        {
            var email = "alice@example.com";

            var user = await _repository.GetByEmailAsync(email);

            Assert.IsNotNull(user);
            Assert.That(user.Email, Is.EqualTo(email));
        }

        [Test]
        public async Task GetByEmailAsync_ShouldReturnNull_WhenEmailDoesNotExist()
        {
            var email = "nonexistent@example.com";

            var user = await _repository.GetByEmailAsync(email);

            Assert.IsNull(user);
        }

        [Test]
        public async Task AddAsync_ShouldAddUser()
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                Name = "Charlie",
                Email = "charlie@example.com",
                Password = "pass",
                Username = "charlie"
            };

            await _repository.AddAsync(user);

            var found = await _repository.GetByEmailAsync(user.Email);
            Assert.IsNotNull(found);
            Assert.That(found!.Name, Is.EqualTo(user.Name));
        }

        [Test]
        public async Task DeleteAsync_ShouldRemoveUser()
        {
            var user = await _repository.GetByEmailAsync("bob@example.com");
            Assert.IsNotNull(user);

            await _repository.DeleteAsync(user!.Id);

            var deletedUser = await _repository.GetByEmailAsync("bob@example.com");
            Assert.IsNull(deletedUser);
        }
    }
}
