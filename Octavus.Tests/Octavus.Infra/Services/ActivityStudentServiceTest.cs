using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Octavus.Core.Application.DTO;
using Octavus.Core.Application.Repositories;
using Octavus.Core.Domain.Entities;
using Octavus.Core.Domain.Enums;
using Octavus.Infra.Core.Services;

namespace Octavus.Tests.Services
{
    public class ActivityStudentServiceTests
    {
        private Mock<IActivityStudentRepository> _mockStudentRepo;
        private Mock<IActivityRepository> _mockActivityRepo;
        private ActivityStudentService _service;

        [SetUp]
        public void Setup()
        {
            _mockStudentRepo = new Mock<IActivityStudentRepository>();
            _mockActivityRepo = new Mock<IActivityRepository>();
            _service = new ActivityStudentService(_mockStudentRepo.Object, _mockActivityRepo.Object);
        }

        [Test]
        public async Task AssignActivityToStudentAsync_ShouldAssign_WhenNotExists()
        {
            var dto = new AssignActivityDto { StudentId = Guid.NewGuid(), ActivityId = Guid.NewGuid() };
            _mockStudentRepo.Setup(r => r.ExistsAsync(dto.StudentId, dto.ActivityId)).ReturnsAsync(false);

            await _service.AssignActivityToStudentAsync(dto);

            _mockStudentRepo.Verify(r => r.AddAsync(It.Is<ActivityStudent>(a =>
                a.StudentId == dto.StudentId &&
                a.ActivityId == dto.ActivityId &&
                a.Status == ActivityStatus.Pending &&
                !a.IsCorrected &&
                a.Score == 0
            )), Times.Once);
        }

        [Test]
        public void AssignActivityToStudentAsync_ShouldThrow_WhenAlreadyExists()
        {
            var dto = new AssignActivityDto { StudentId = Guid.NewGuid(), ActivityId = Guid.NewGuid() };
            _mockStudentRepo.Setup(r => r.ExistsAsync(dto.StudentId, dto.ActivityId)).ReturnsAsync(true);

            Assert.ThrowsAsync<Exception>(() => _service.AssignActivityToStudentAsync(dto));
        }

        [Test]
        public async Task EvaluateActivityAsync_ShouldUpdateCorrectly()
        {
            var dto = new EvaluateActivityDto
            {
                StudentId = Guid.NewGuid(),
                ActivityId = Guid.NewGuid(),
                Score = 9,
                Comment = "Boa execução"
            };
            var entity = new ActivityStudent { StudentId = dto.StudentId, ActivityId = dto.ActivityId };
            _mockStudentRepo.Setup(r => r.GetByStudentAndActivityAsync(dto.StudentId, dto.ActivityId)).ReturnsAsync(entity);

            await _service.EvaluateActivityAsync(dto);

            _mockStudentRepo.Verify(r => r.UpdateAsync(It.Is<ActivityStudent>(a =>
                a.Score == dto.Score &&
                a.Comment == dto.Comment &&
                a.IsCorrected &&
                a.CorrectionDate.HasValue
            )), Times.Once);
        }

        [Test]
        public void EvaluateActivityAsync_ShouldThrow_WhenNotFound()
        {
            var dto = new EvaluateActivityDto { StudentId = Guid.NewGuid(), ActivityId = Guid.NewGuid() };
            _mockStudentRepo.Setup(r => r.GetByStudentAndActivityAsync(dto.StudentId, dto.ActivityId)).ReturnsAsync((ActivityStudent)null);

            Assert.ThrowsAsync<Exception>(() => _service.EvaluateActivityAsync(dto));
        }

        [Test]
        public async Task GetMetricsByStudentAsync_ShouldReturnZero_WhenNoSubmissions()
        {
            var studentId = Guid.NewGuid();
            _mockStudentRepo.Setup(r => r.GetActivitiesByStudentAsync(studentId)).ReturnsAsync(new List<ActivityStudent>());

            var result = await _service.GetMetricsByStudentAsync(studentId);

            Assert.That(result.TotalActivitiesDone, Is.EqualTo(0));
            Assert.That(result.AverageScore, Is.EqualTo(0));
            Assert.IsEmpty(result.AverageScoreByActivityType);
        }

        [Test]
        public async Task GetPendingReviewsAsync_ShouldReturnList()
        {
            var professorId = Guid.NewGuid();
            var list = new List<PendingActivityReviewDto> { new() };
            _mockStudentRepo.Setup(r => r.GetPendingReviewsByProfessorAsync(professorId)).ReturnsAsync(list);

            var result = await _service.GetPendingReviewsAsync(professorId);

            Assert.That(result.Count, Is.EqualTo(1));
        }
    }
}
