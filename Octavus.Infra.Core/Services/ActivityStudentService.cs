using Octavus.Core.Application.DTO;
using Octavus.Core.Application.Repositories;
using Octavus.Core.Application.Services;
using Octavus.Core.Domain.Entities;
using Octavus.Core.Domain.Enums;
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

        public ActivityStudentService(IActivityStudentRepository repository)
        {
            _repository = repository;
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

    }

}
