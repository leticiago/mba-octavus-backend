using NUnit.Framework;
using Octavus.Core.Domain.Entities;
using Octavus.Core.Application.DTO;
using Octavus.Infra.Persistence.Repositories;
using Octavus.Tests.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Octavus.Core.Domain.Enums;
using Octavus.Infra.Persistence;

namespace Octavus.Tests.Repositories
{
    [TestFixture]
    public class ActivityStudentRepositoryTests
    {
        private Context _context = null!;
        private ActivityStudentRepository _repository = null!;

        [SetUp]
        public void Setup()
        {
            _context = TestContextProvider.CreateContext();
            _repository = new ActivityStudentRepository(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task ExistsAsync_ShouldReturnTrue_WhenExists()
        {
            var studentId = Guid.NewGuid();
            var activityId = Guid.NewGuid();

            var entity = new ActivityStudent
            {
                Id = Guid.NewGuid(),
                StudentId = studentId,
                ActivityId = activityId,
                Score = 10,
            };

            await _context.AddAsync(entity);
            await _context.SaveChangesAsync();

            var exists = await _repository.ExistsAsync(studentId, activityId);
            Assert.IsTrue(exists);
        }

        [Test]
        public async Task ExistsAsync_ShouldReturnFalse_WhenNotExists()
        {
            var exists = await _repository.ExistsAsync(Guid.NewGuid(), Guid.NewGuid());
            Assert.IsFalse(exists);
        }

        [Test]
        public async Task GetByStudentAndActivityAsync_ShouldReturnEntity_WhenExists()
        {
            var studentId = Guid.NewGuid();
            var activityId = Guid.NewGuid();

            var entity = new ActivityStudent
            {
                Id = Guid.NewGuid(),
                StudentId = studentId,
                ActivityId = activityId,
                Score = 10,
            };

            await _context.AddAsync(entity);
            await _context.SaveChangesAsync();

            var result = await _repository.GetByStudentAndActivityAsync(studentId, activityId);
            Assert.IsNotNull(result);
            Assert.That(result!.StudentId, Is.EqualTo(studentId));
            Assert.That(result.ActivityId, Is.EqualTo(activityId));
        }

        [Test]
        public async Task GetByStudentAndActivityAsync_ShouldReturnNull_WhenNotExists()
        {
            var result = await _repository.GetByStudentAndActivityAsync(Guid.NewGuid(), Guid.NewGuid());
            Assert.IsNull(result);
        }

        [Test]
        public async Task GetPendingReviewsByProfessorAsync_ShouldReturnCorrectDtos()
        {
            var professorId = Guid.NewGuid();
            var studentId = Guid.NewGuid();
            var activityId = Guid.NewGuid();

            // Criar dados para professor-student link ativo
            var professorStudent = new ProfessorStudent
            {
                Id = Guid.NewGuid(),
                ProfessorId = professorId,
                StudentId = studentId,
                Active = true
            };

            var activity = new Activity
            {
                Id = activityId,
                Name = "Atividade 1",
                ProfessorId = professorId,
                IsPublic = false,
                Description = "Description",
                Level = Level.Advanced.ToString(),
                Type = ActivityType.OpenText.ToString(),
            };

            var student = new User
            {
                Id = studentId,
                Name = "Aluno 1",
                Email = "Email",
                Password = "password",
                Username = "username"
            };

            var activityStudent = new ActivityStudent
            {
                Id = Guid.NewGuid(),
                StudentId = studentId,
                ActivityId = activityId,
                IsCorrected = false,
                Score = 10
            };

            await _context.AddRangeAsync(professorStudent, activity, student, activityStudent);
            await _context.SaveChangesAsync();

            var results = await _repository.GetPendingReviewsByProfessorAsync(professorId);

            Assert.That(results, Has.Exactly(1).Items);
            var dto = results.First();
            Assert.That(dto.StudentId, Is.EqualTo(studentId));
            Assert.That(dto.StudentName, Is.EqualTo("Aluno 1"));
            Assert.That(dto.ActivityId, Is.EqualTo(activityId));
            Assert.That(dto.ActivityName, Is.EqualTo("Atividade 1"));
        }

        [Test]
        public async Task GetActivitiesByStudentAsync_ShouldReturnActivitiesIncludingActivityEntity()
        {
            var studentId = Guid.NewGuid();
            var activityId = Guid.NewGuid();

            var activity = new Activity
            {
                Id = activityId,
                Name = "Atividade 1",
                Description = "description",
                Level = Level.Advanced.ToString(),
                Type = ActivityType.DragAndDrop.ToString()
            };

            var activityStudent = new ActivityStudent
            {
                Id = Guid.NewGuid(),
                StudentId = studentId,
                ActivityId = activityId,
                Activity = activity,
                Score = 10,
            };

            await _context.AddAsync(activityStudent);
            await _context.SaveChangesAsync();

            var results = await _repository.GetActivitiesByStudentAsync(studentId);

            Assert.That(results, Has.Exactly(1).Items);
            Assert.IsNotNull(results.First().Activity);
            Assert.That(results.First().Activity.Name, Is.EqualTo("Atividade 1"));
        }

        [Test]
        public async Task GetActivityStudentAsync_ShouldReturnEntity_WhenExists()
        {
            var studentId = Guid.NewGuid();
            var activityId = Guid.NewGuid();

            var entity = new ActivityStudent
            {
                Id = Guid.NewGuid(),
                StudentId = studentId,
                ActivityId = activityId,
                Score = 10,
            };

            await _context.AddAsync(entity);
            await _context.SaveChangesAsync();

            var result = await _repository.GetActivityStudentAsync(activityId, studentId);
            Assert.IsNotNull(result);
            Assert.That(result!.StudentId, Is.EqualTo(studentId));
            Assert.That(result.ActivityId, Is.EqualTo(activityId));
        }

        [Test]
        public async Task GetActivityStudentAsync_ShouldReturnNull_WhenNotExists()
        {
            var result = await _repository.GetActivityStudentAsync(Guid.NewGuid(), Guid.NewGuid());
            Assert.IsNull(result);
        }

        [Test]
        public async Task GetCompletedActivitiesByStudentAsync_ShouldReturnCorrectDtos()
        {
            var studentId = Guid.NewGuid();

            var activity1 = new Activity { Id = Guid.NewGuid(), Name = "Atividade 1", Type = ActivityType.OpenText.ToString(), Description = "description", Level = Level.Intermediate.ToString() };
            var activity2 = new Activity { Id = Guid.NewGuid(), Name = "Atividade 2", Type = ActivityType.QuestionAndAnswer.ToString(), Description = "description", Level = Level.Intermediate.ToString() };

            var completed1 = new ActivityStudent
            {
                Id = Guid.NewGuid(),
                StudentId = studentId,
                ActivityId = activity1.Id,
                Activity = activity1,
                IsCorrected = true,
                Score = 90,
                CorrectionDate = DateTime.UtcNow.AddDays(-1)
            };

            var completed2 = new ActivityStudent
            {
                Id = Guid.NewGuid(),
                StudentId = studentId,
                ActivityId = activity2.Id,
                Activity = activity2,
                IsCorrected = true,
                Score = 80,
                CorrectionDate = DateTime.UtcNow
            };

            var notCompleted = new ActivityStudent
            {
                Id = Guid.NewGuid(),
                StudentId = studentId,
                ActivityId = Guid.NewGuid(),
                IsCorrected = false,
                Score = 10,
            };

            await _context.AddRangeAsync(completed1, completed2, notCompleted);
            await _context.SaveChangesAsync();

            var results = await _repository.GetCompletedActivitiesByStudentAsync(studentId);

            Assert.That(results, Has.Exactly(2).Items);
            Assert.That(results[0].CorrectionDate >= results[1].CorrectionDate);

            Assert.IsTrue(results.All(dto => dto.Title == "Atividade 1" || dto.Title == "Atividade 2"));
        }
    }
}
