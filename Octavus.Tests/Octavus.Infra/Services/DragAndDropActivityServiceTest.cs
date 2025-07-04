using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Octavus.Core.Application.DTO;
using Octavus.Core.Application.Repositories;
using Octavus.Core.Domain.Entities;
using Octavus.Infra.Core.Services;

namespace Octavus.Tests.Services
{
    public class DragAndDropActivityServiceTests
    {
        private Mock<IDragAndDropActivityRepository> _mockRepo;
        private DragAndDropActivityService _service;

        [SetUp]
        public void Setup()
        {
            _mockRepo = new Mock<IDragAndDropActivityRepository>();
            _service = new DragAndDropActivityService(_mockRepo.Object);
        }

        [Test]
        public async Task CreateAsync_ShouldCreateAndReturnDto()
        {
            var activityId = Guid.NewGuid();
            var sequence = "C;D;E;F";

            _mockRepo.Setup(r => r.AddAsync(It.IsAny<DragAndDropActivity>())).Returns(Task.CompletedTask);
            _mockRepo.Setup(r => r.SaveChangesAsync()).Returns(Task.FromResult(true));

            var result = await _service.CreateAsync(activityId, sequence);

            Assert.That(result.OriginalSequence, Is.EqualTo(sequence));
            Assert.That(result.ShuffledOptions.Count, Is.EqualTo(4));
            Assert.That(string.Join(";", result.ShuffledOptions), Is.Not.EqualTo(sequence));
        }

        [Test]
        public async Task GetAllAsync_ShouldReturnAllActivities()
        {
            var list = new List<DragAndDropActivity>
            {
                new() { Id = Guid.NewGuid(), ActivityId = Guid.NewGuid(), Text = "A;B;C" },
                new() { Id = Guid.NewGuid(), ActivityId = Guid.NewGuid(), Text = "X;Y;Z" }
            };
            _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(list);

            var result = await _service.GetAllAsync();

            Assert.That(result.Count, Is.EqualTo(2));
            Assert.IsTrue(result.All(r => r.ShuffledOptions.Count > 1));
        }

        [Test]
        public async Task GetByIdAsync_ShouldReturnCorrectActivity()
        {
            var id = Guid.NewGuid();
            var entity = new DragAndDropActivity { Id = id, ActivityId = id, Text = "1;2;3" };
            _mockRepo.Setup(r => r.GetByActivityIdAsync(id)).ReturnsAsync(entity);

            var result = await _service.GetByIdAsync(id);

            Assert.That(result.OriginalSequence, Is.EqualTo("1;2;3"));
            Assert.That(result.ShuffledOptions.Count, Is.EqualTo(3));
        }

        [Test]
        public void GetByIdAsync_ShouldThrow_WhenNotFound()
        {
            _mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((DragAndDropActivity)null);

            Assert.ThrowsAsync<Exception>(() => _service.GetByIdAsync(Guid.NewGuid()));
        }

        [Test]
        public async Task CreateAsync_ShouldCallSaveChangesAsync()
        {
            var activityId = Guid.NewGuid();
            var sequence = "A;B;C";

            _mockRepo.Setup(r => r.AddAsync(It.IsAny<DragAndDropActivity>())).Returns(Task.CompletedTask);
            _mockRepo.Setup(r => r.SaveChangesAsync()).Returns(Task.FromResult(true));

            await _service.CreateAsync(activityId, sequence);

            _mockRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task CreateAsync_ShouldGenerateNonEmptyId()
        {
            DragAndDropActivity capturedEntity = null;
            _mockRepo.Setup(r => r.AddAsync(It.IsAny<DragAndDropActivity>()))
                .Callback<DragAndDropActivity>(a => capturedEntity = a)
                .Returns(Task.CompletedTask);
            _mockRepo.Setup(r => r.SaveChangesAsync()).Returns(Task.FromResult(true));

            var result = await _service.CreateAsync(Guid.NewGuid(), "A;B;C");

            Assert.IsNotNull(capturedEntity);
            Assert.That(capturedEntity.Id, Is.Not.EqualTo(Guid.Empty));
        }

        [Test]
        public async Task GetAllAsync_ShouldReturnEmptyList_WhenNoActivities()
        {
            _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<DragAndDropActivity>());

            var result = await _service.GetAllAsync();

            Assert.IsEmpty(result);
        }

        [Test]
        public async Task Shuffle_ShouldHandleEmptyOrSingleItemSequences()
        {
            // Empty sequence
            var resultEmpty = await _service.CreateAsync(Guid.NewGuid(), "");
            Assert.That(resultEmpty.ShuffledOptions, Is.EqualTo(new List<string>()));

            // Single item
            var resultSingle = await _service.CreateAsync(Guid.NewGuid(), "A");
            Assert.That(resultSingle.ShuffledOptions.Count, Is.EqualTo(1));
            Assert.That(resultSingle.ShuffledOptions[0], Is.EqualTo("A"));
        }
    }
}
