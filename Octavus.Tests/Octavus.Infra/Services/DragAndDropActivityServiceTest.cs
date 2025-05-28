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

            Assert.AreEqual(sequence, result.OriginalSequence);
            Assert.AreEqual(4, result.ShuffledOptions.Count);
            Assert.AreNotEqual(sequence, string.Join(";", result.ShuffledOptions));
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

            Assert.AreEqual(2, result.Count);
            Assert.IsTrue(result.All(r => r.ShuffledOptions.Count > 1));
        }

        [Test]
        public async Task GetByIdAsync_ShouldReturnCorrectActivity()
        {
            var id = Guid.NewGuid();
            var entity = new DragAndDropActivity { Id = id, ActivityId = Guid.NewGuid(), Text = "1;2;3" };
            _mockRepo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(entity);

            var result = await _service.GetByIdAsync(id);

            Assert.AreEqual("1;2;3", result.OriginalSequence);
            Assert.AreEqual(3, result.ShuffledOptions.Count);
        }

        [Test]
        public void GetByIdAsync_ShouldThrow_WhenNotFound()
        {
            _mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((DragAndDropActivity)null);

            Assert.ThrowsAsync<Exception>(() => _service.GetByIdAsync(Guid.NewGuid()));
        }
    }
}
