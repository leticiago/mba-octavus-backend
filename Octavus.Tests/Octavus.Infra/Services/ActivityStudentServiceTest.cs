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

        [Test]
        public async Task GetActivitiesForStudentAsync_ShouldReturnMappedList()
        {
            var studentId = Guid.NewGuid();
            var activitiesDomain = new List<ActivityStudent>
    {
        new ActivityStudent
        {
            ActivityId = Guid.NewGuid(),
            Status = ActivityStatus.Pending,
            Score = 8,
            Comment = "Comentário",
            IsCorrected = true,
            CorrectionDate = DateTime.UtcNow,
            Activity = new Activity
            {
                Name = "Atividade 1",
                Description = "Descrição 1"
            }
        },
        new ActivityStudent
        {
            ActivityId = Guid.NewGuid(),
            Status = ActivityStatus.Done,
            Score = 10,
            Comment = "Ótimo",
            IsCorrected = true,
            CorrectionDate = DateTime.UtcNow,
            Activity = new Activity
            {
                Name = "Atividade 2",
                Description = "Descrição 2"
            }
        }
    };

            _mockStudentRepo.Setup(r => r.GetActivitiesByStudentAsync(studentId)).ReturnsAsync(activitiesDomain);

            var result = await _service.GetActivitiesForStudentAsync(studentId);

            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result[0].ActivityId, Is.EqualTo(activitiesDomain[0].ActivityId));
            Assert.That(result[0].Title, Is.EqualTo("Atividade 1"));
            Assert.That(result[0].Description, Is.EqualTo("Descrição 1"));
            Assert.That(result[1].Title, Is.EqualTo("Atividade 2"));
        }

        [Test]
        public async Task GetMetricsByStudentAsync_ShouldCalculateCorrectAverages()
        {
            var studentId = Guid.NewGuid();
            var activityId1 = Guid.NewGuid();
            var activityId2 = Guid.NewGuid();

            var submissions = new List<ActivityStudent>
            {
                new ActivityStudent { ActivityId = activityId1, Score = 8 },
                new ActivityStudent { ActivityId = activityId1, Score = 6 },
                new ActivityStudent { ActivityId = activityId2, Score = 10 }
            };

            var activities = new List<Activity>
            {
                new Activity { Id = activityId1, Type = ActivityType.QuestionAndAnswer.ToString() },
                new Activity { Id = activityId2, Type = ActivityType.OpenText.ToString() }
            };

            _mockStudentRepo.Setup(r => r.GetActivitiesByStudentAsync(studentId)).ReturnsAsync(submissions);
            _mockActivityRepo.Setup(r => r.GetAllByIds(It.IsAny<List<Guid>>())).ReturnsAsync(activities);

            var result = await _service.GetMetricsByStudentAsync(studentId);

            Assert.That(result.TotalActivitiesDone, Is.EqualTo(submissions.Count));

            Assert.That(result.AverageScore, Is.EqualTo(8));

            Assert.That(result.AverageScoreByActivityType[ActivityType.QuestionAndAnswer.ToString()], Is.EqualTo(7)); 
            Assert.That(result.AverageScoreByActivityType[ActivityType.OpenText.ToString()], Is.EqualTo(10));
        }

    }
}
