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
    public class OpenTextAnswerRepositoryTests
    {
        private Context _context = null!;
        private OpenTextAnswerRepository _repository = null!;

        [SetUp]
        public void Setup()
        {
            _context = TestContextProvider.CreateContext();
            _repository = new OpenTextAnswerRepository(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task AddAsync_ShouldAddOpenTextAnswer()
        {
            var answer = new OpenTextAnswer
            {
                Id = Guid.NewGuid(),
                ResponseText = "Resposta de teste"
            };

            await _repository.AddAsync(answer);

            var all = await _repository.GetAllAsync();
            Assert.That(all, Has.One.Items);
        }

        [Test]
        public async Task GetByIdAsync_ShouldReturnOpenTextAnswer_WhenExists()
        {
            var answer = new OpenTextAnswer
            {
                Id = Guid.NewGuid(),
                ResponseText = "Resposta para consulta"
            };

            await _repository.AddAsync(answer);

            var found = await _repository.GetByIdAsync(answer.Id);
            Assert.IsNotNull(found);
            Assert.That(found!.ResponseText, Is.EqualTo(answer.ResponseText));
        }

        [Test]
        public async Task GetByIdAsync_ShouldReturnNull_WhenNotFound()
        {
            var found = await _repository.GetByIdAsync(Guid.NewGuid());
            Assert.IsNull(found);
        }

        [Test]
        public async Task UpdateAsync_ShouldUpdateOpenTextAnswer()
        {
            var answer = new OpenTextAnswer
            {
                Id = Guid.NewGuid(),
                ResponseText = "Texto original"
            };

            await _repository.AddAsync(answer);

            answer.ResponseText = "Texto atualizado";
            await _repository.UpdateAsync(answer);

            var updated = await _repository.GetByIdAsync(answer.Id);
            Assert.That(updated!.ResponseText, Is.EqualTo("Texto atualizado"));
        }

        [Test]
        public async Task DeleteAsync_ShouldRemoveOpenTextAnswer_WhenExists()
        {
            var answer = new OpenTextAnswer
            {
                Id = Guid.NewGuid(),
                ResponseText = "Texto para deletar"
            };

            await _repository.AddAsync(answer);

            await _repository.DeleteAsync(answer.Id);

            var all = await _repository.GetAllAsync();
            Assert.That(all, Is.Empty);
        }
    }
}
