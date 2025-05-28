using NUnit.Framework;
using Moq;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Octavus.Core.Application.DTO;
using Octavus.Core.Application.Services;
using Octavus.Core.Domain.Entities;

[TestFixture]
public class ActivityControllerTests
{
    private Mock<IActivityService> _activityServiceMock;
    private ActivityController _controller;

    [SetUp]
    public void Setup()
    {
        _activityServiceMock = new Mock<IActivityService>();
        _controller = new ActivityController(_activityServiceMock.Object);
    }

    [Test]
    public async Task Create_ReturnsCreatedAtAction_WithCreatedActivity()
    {
        // Arrange
        var dto = new CreateActivityDto();
        var createdActivity = new ActivityDto { Id = Guid.NewGuid() };
        _activityServiceMock.Setup(s => s.CreateAsync(dto)).Returns(Task.FromResult(createdActivity));

        // Act
        var result = await _controller.Create(dto);

        // Assert
        var createdResult = result as CreatedAtActionResult;
        Assert.IsNotNull(createdResult);
        Assert.AreEqual(nameof(ActivityController.GetById), createdResult.ActionName);
        Assert.AreEqual(createdActivity.Id, createdResult.RouteValues["id"]);
        Assert.AreEqual(createdActivity, createdResult.Value);
    }

    [Test]
    public async Task GetAll_ReturnsOk_WithListOfActivities()
    {
        // Arrange
        var activities = new List<ActivityDto> { new ActivityDto(), new ActivityDto() };
        _activityServiceMock.Setup(s => s.GetAllAsync()).Returns(Task.FromResult(activities.AsEnumerable()));

        // Act
        var result = await _controller.GetAll();

        // Assert
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(activities, okResult.Value);
    }

    [Test]
    public async Task GetById_ReturnsOk_WithActivity()
    {
        // Arrange
        var id = Guid.NewGuid();
        var activity = new ActivityDto { Id = id };
        _activityServiceMock.Setup(s => s.GetByIdAsync(id)).Returns(Task.FromResult(activity));

        // Act
        var result = await _controller.GetById(id);

        // Assert
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(activity, okResult.Value);
    }

    [Test]
    public async Task GetByProfessor_ReturnsOk_WithListOfActivities()
    {
        // Arrange
        var professorId = Guid.NewGuid();
        var activities = new List<ActivityDto> { new ActivityDto(), new ActivityDto() };
        _activityServiceMock.Setup(s => s.GetByProfessorIdAsync(professorId)).Returns(Task.FromResult(activities.AsEnumerable()));

        // Act
        var result = await _controller.GetByProfessor(professorId);

        // Assert
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(activities, okResult.Value);
    }

    [Test]
    public async Task Update_ReturnsNoContent()
    {
        // Arrange
        var id = Guid.NewGuid();
        var dto = new CreateActivityDto();

        _activityServiceMock.Setup(s => s.UpdateAsync(id, dto)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Update(id, dto);

        // Assert
        Assert.IsInstanceOf<NoContentResult>(result);
    }

    [Test]
    public async Task Delete_ReturnsNoContent()
    {
        // Arrange
        var id = Guid.NewGuid();
        _activityServiceMock.Setup(s => s.DeleteAsync(id)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Delete(id);

        // Assert
        Assert.IsInstanceOf<NoContentResult>(result);
    }

    [Test]
    public async Task GetPublic_ReturnsOk_WithListOfPublicActivities()
    {
        // Arrange
        var activities = new List<ActivityDto> { new ActivityDto(), new ActivityDto() };
        _activityServiceMock.Setup(s => s.GetPublicActivitiesAsync()).Returns(Task.FromResult(activities));

        // Act
        var result = await _controller.GetPublic();

        // Assert
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(activities, okResult.Value);
    }
}
