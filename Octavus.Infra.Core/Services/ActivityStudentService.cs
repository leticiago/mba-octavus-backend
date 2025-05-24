using Octavus.Core.Application.DTO;
using Octavus.Core.Application.Repositories;
using Octavus.Core.Application.Services;
using Octavus.Core.Domain.Entities;
using Octavus.Core.Domain.Enums;
using Octavus.Infra.Persistence.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Octavus.Infra.Core.Services
{
    public class ActivityStudentService : IActivityStudentService
    {
        private readonly IActivityStudentRepository _repository;
        private readonly IActivityRepository _activityRepository;

        public ActivityStudentService(IActivityStudentRepository repository, IActivityRepository activityRepository)
        {
            _repository = repository;
            _activityRepository = activityRepository;
        }

        public async Task AssignActivityToStudentAsync(AssignActivityDto dto)
        {
            var exists = await _repository.ExistsAsync(dto.StudentId, dto.ActivityId);
            if (exists)
                throw new Exception("Essa atividade já foi atribuída a este aluno.");

            var entity = new ActivityStudent
            {
                Id = Guid.NewGuid(),
                StudentId = dto.StudentId,
                ActivityId = dto.ActivityId,
                Status = ActivityStatus.Pending,
                IsCorrected = false,
                Score = 0
            };

            await _repository.AddAsync(entity);
        }

        public async Task EvaluateActivityAsync(EvaluateActivityDto dto)
        {
            var activityStudent = await _repository.GetByStudentAndActivityAsync(dto.StudentId, dto.ActivityId);

            if (activityStudent == null)
                throw new Exception("Atividade atribuída não encontrada.");

            activityStudent.Score = dto.Score;
            activityStudent.Comment = dto.Comment;
            activityStudent.IsCorrected = true; 
            activityStudent.CorrectionDate = DateTime.Now;

            await _repository.UpdateAsync(activityStudent);
        }

        public async Task<List<PendingActivityReviewDto>> GetPendingReviewsAsync(Guid professorId)
        {
            return await _repository.GetPendingReviewsByProfessorAsync(professorId);
        }

        public async Task<List<ActivityStudentDto>> GetActivitiesForStudentAsync(Guid studentId)
        {
            var activities = await _repository.GetActivitiesByStudentAsync(studentId);

            return activities.Select(a => new ActivityStudentDto
            {
                ActivityId = a.ActivityId,
                Title = a.Activity.Name,
                Description = a.Activity.Description,
                Status = a.Status,
                Score = a.Score,
                Comment = a.Comment,
                IsCorrected = a.IsCorrected,
                CorrectionDate = a.CorrectionDate
            }).ToList();
        }

        public async Task<StudentMetricsDto> GetMetricsByStudentAsync(Guid studentId)
        {
            var submissions = await _repository.GetActivitiesByStudentAsync(studentId);

            if (!submissions.Any())
                return new StudentMetricsDto
                {
                    TotalActivitiesDone = 0,
                    AverageScore = 0,
                    AverageScoreByActivityType = new()
                };

            var activityIds = submissions.Select(s => s.ActivityId).Distinct();
            var activities = await _activityRepository.GetAllByIds(activityIds.ToList());

            var joined = from s in submissions
                         join a in activities on s.ActivityId equals a.Id
                         where s.Score.HasValue
                         select new { a.Type, s.Score };

            var avgByType = joined
                .GroupBy(x => x.Type.ToString())
                .ToDictionary(g => g.Key, g => g.Average(x => x.Score.Value));

            return new StudentMetricsDto
            {
                TotalActivitiesDone = submissions.Count,
                AverageScore = joined.Average(x => x.Score.Value),
                AverageScoreByActivityType = avgByType
            };
        }

    }

}
