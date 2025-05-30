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
    public class InstrumentRepositoryTests
    {
        private Context _context = null!;
        private InstrumentRepository _repository = null!;

        [SetUp]
        public void Setup()
        {
            _context = TestContextProvider.CreateContext();
            _repository = new InstrumentRepository(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task AddAsync_ShouldAddInstrument()
        {
            var instrument = new Instrument
            {
                Id = Guid.NewGuid(),
                Name = "Guitar"
            };

            await _repository.AddAsync(instrument);

            var instruments = await _repository.GetAllAsync();
            Assert.That(instruments, Has.One.Items);
        }

        [Test]
        public async Task GetByIdAsync_ShouldReturnInstrument_WhenExists()
        {
            var instrument = new Instrument
            {
                Id = Guid.NewGuid(),
                Name = "Piano"
            };

            await _repository.AddAsync(instrument);

            var found = await _repository.GetByIdAsync(instrument.Id);
            Assert.IsNotNull(found);
            Assert.That(found!.Name, Is.EqualTo(instrument.Name));
        }

        [Test]
        public async Task GetByIdAsync_ShouldReturnNull_WhenNotFound()
        {
            var found = await _repository.GetByIdAsync(Guid.NewGuid());
            Assert.IsNull(found);
        }

        [Test]
        public async Task UpdateAsync_ShouldUpdateInstrument()
        {
            var instrument = new Instrument
            {
                Id = Guid.NewGuid(),
                Name = "Violin"
            };

            await _repository.AddAsync(instrument);

            instrument.Name = "Electric Violin";
            await _repository.UpdateAsync(instrument);

            var updated = await _repository.GetByIdAsync(instrument.Id);
            Assert.That(updated!.Name, Is.EqualTo("Electric Violin"));
        }

        [Test]
        public async Task DeleteAsync_ShouldRemoveInstrument_WhenExists()
        {
            var instrument = new Instrument
            {
                Id = Guid.NewGuid(),
                Name = "Drums"
            };

            await _repository.AddAsync(instrument);

            await _repository.DeleteAsync(instrument.Id);

            var all = await _repository.GetAllAsync();
            Assert.That(all, Is.Empty);
        }
    }
}
