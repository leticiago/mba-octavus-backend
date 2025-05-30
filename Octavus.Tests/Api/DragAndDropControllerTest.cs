using Moq;
using Microsoft.AspNetCore.Mvc;
using Octavus.Core.Application.DTO;
using Octavus.Core.Application.Services;
using Octavus.App.Api.Controllers.v1;

namespace Octavus.Tests.Controllers
{
    [TestFixture]
    public class DragAndDropControllerTests
    {
        private Mock<IDragAndDropActivityService> _dragAndDropServiceMock;
        private DragAndDropController _controller;

        [SetUp]
        public void Setup()
        {
            _dragAndDropServiceMock = new Mock<IDragAndDropActivityService>();
            _controller = new DragAndDropController(_dragAndDropServiceMock.Object);
        }

        [Test]
        public async Task Create_ReturnsCreatedAtAction_WithCreatedActivity()
        {
            // Arrange
            var dto = new CreateDragAndDropActivityDto
            {
                ActivityId = Guid.NewGuid(),
                OriginalSequence = "Option1;Option2;Option3"
            };
            var createdResult = new DragAndDropActivityDto { ActivityId = dto.ActivityId, OriginalSequence = dto.OriginalSequence };
            _dragAndDropServiceMock.Setup(s => s.CreateAsync(dto.ActivityId, dto.OriginalSequence)).ReturnsAsync(createdResult);

            // Act
            var result = await _controller.Create(dto);

            // Assert
            var createdAtResult = result as CreatedAtActionResult;
            Assert.IsNotNull(createdAtResult);
            Assert.AreEqual(nameof(DragAndDropController.GetById), createdAtResult.ActionName);
            Assert.AreEqual(dto.ActivityId, createdAtResult.RouteValues["id"]);
            Assert.AreEqual(createdResult, createdAtResult.Value);
        }

        [Test]
        public async Task GetAll_ReturnsOk_WithListOfActivities()
        {
            // Arrange
            var list = new List<DragAndDropActivityDto>
            {
                new DragAndDropActivityDto { ActivityId = Guid.NewGuid(), OriginalSequence = "A;B;C" },
                new DragAndDropActivityDto { ActivityId = Guid.NewGuid(), OriginalSequence = "X;Y;Z" }
            };
            _dragAndDropServiceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(list);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(list, okResult.Value);
        }

        [Test]
        public async Task GetById_ReturnsOk_WithActivity()
        {
            // Arrange
            var id = Guid.NewGuid();
            var activity = new DragAndDropActivityDto { ActivityId = id, OriginalSequence = "1;2;3" };
            _dragAndDropServiceMock.Setup(s => s.GetByIdAsync(id)).ReturnsAsync(activity);

            // Act
            var result = await _controller.GetById(id);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(activity, okResult.Value);
        }
    }
}
