using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Octavus.Core.Domain.Entities;
using Octavus.Core.Application.Repositories;
using Octavus.Infra.Core.Services;

namespace Octavus.Tests.Services
{
    public class InstrumentServiceTests
    {
        private Mock<IInstrumentRepository> _mockRepo;
        private InstrumentService _service;

        [SetUp]
        public void Setup()
        {
            _mockRepo = new Mock<IInstrumentRepository>();
            _service = new InstrumentService(_mockRepo.Object);
        }

        [Test]
        public async Task GetAllAsync_ReturnsAllInstruments()
        {
            var instruments = new List<Instrument>
            {
                new() { Id = Guid.NewGuid(), Name = "Guitar" },
                new() { Id = Guid.NewGuid(), Name = "Piano" }
            };

            _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(instruments);

            var result = await _service.GetAllAsync();

            Assert.AreEqual(2, ((List<Instrument>)result).Count);
            Assert.AreEqual("Guitar", ((List<Instrument>)result)[0].Name);
        }

        [Test]
        public async Task GetByIdAsync_ReturnsInstrument_WhenExists()
        {
            var id = Guid.NewGuid();
            var instrument = new Instrument { Id = id, Name = "Violin" };
            _mockRepo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(instrument);

            var result = await _service.GetByIdAsync(id);

            Assert.IsNotNull(result);
            Assert.AreEqual("Violin", result.Name);
        }

        [Test]
        public async Task GetByIdAsync_ReturnsNull_WhenNotFound()
        {
            _mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Instrument?)null);

            var result = await _service.GetByIdAsync(Guid.NewGuid());

            Assert.IsNull(result);
        }

        [Test]
        public async Task CreateAsync_AddsInstrumentAndReturnsEntity()
        {
            var instrument = new Instrument { Id = Guid.NewGuid(), Name = "Flute" };

            _mockRepo.Setup(r => r.AddAsync(instrument)).Returns(Task.CompletedTask);
            _mockRepo.Setup(r => r.SaveChangesAsync()).ReturnsAsync(true);

            var result = await _service.CreateAsync(instrument);

            Assert.AreEqual(instrument.Id, result.Id);
            Assert.AreEqual("Flute", result.Name);
            _mockRepo.Verify(r => r.AddAsync(instrument), Times.Once);
            _mockRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task UpdateAsync_ReturnsFalse_WhenInstrumentDoesNotExist()
        {
            var instrument = new Instrument { Id = Guid.NewGuid(), Name = "Drums" };

            _mockRepo.Setup(r => r.GetByIdAsync(instrument.Id)).ReturnsAsync((Instrument?)null);

            var result = await _service.UpdateAsync(instrument);

            Assert.IsFalse(result);
        }

        [Test]
        public async Task UpdateAsync_UpdatesInstrumentAndReturnsTrue_WhenExists()
        {
            var instrument = new Instrument { Id = Guid.NewGuid(), Name = "Drums" };
            var existing = new Instrument { Id = instrument.Id, Name = "Old Drums" };

            _mockRepo.Setup(r => r.GetByIdAsync(instrument.Id)).ReturnsAsync(existing);
            _mockRepo.Setup(r => r.UpdateAsync(existing)).Returns(Task.CompletedTask);
            _mockRepo.Setup(r => r.SaveChangesAsync()).ReturnsAsync(true);

            var result = await _service.UpdateAsync(instrument);

            Assert.IsTrue(result);
            Assert.AreEqual("Drums", existing.Name);
            _mockRepo.Verify(r => r.UpdateAsync(existing), Times.Once);
            _mockRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task DeleteAsync_ReturnsFalse_WhenInstrumentDoesNotExist()
        {
            _mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Instrument?)null);

            var result = await _service.DeleteAsync(Guid.NewGuid());

            Assert.IsFalse(result);
        }

        [Test]
        public async Task DeleteAsync_DeletesInstrumentAndReturnsTrue_WhenExists()
        {
            var id = Guid.NewGuid();
            var instrument = new Instrument { Id = id, Name = "Cello" };

            _mockRepo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(instrument);
            _mockRepo.Setup(r => r.DeleteAsync(id)).Returns(Task.CompletedTask);
            _mockRepo.Setup(r => r.SaveChangesAsync()).ReturnsAsync(true);

            var result = await _service.DeleteAsync(id);

            Assert.IsTrue(result);
            _mockRepo.Verify(r => r.DeleteAsync(id), Times.Once);
            _mockRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
        }
    }
}
