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
    public class StudentService: IStudentService
    {
        private readonly IAnswerRepository _answerRepository;
        private readonly IActivityStudentRepository _activityStudentRepository;
        private readonly IDragAndDropActivityRepository _dragAndDropActivityRepository;
        public StudentService(IAnswerRepository answerRepository, IActivityStudentRepository activityStudentRepository, IDragAndDropActivityRepository dragAndDropActivityRepository)
        {
            _answerRepository = answerRepository;
            _activityStudentRepository = activityStudentRepository;
            _dragAndDropActivityRepository = dragAndDropActivityRepository;
        }

        public async Task<int> SubmitAnswersAsync(SubmitAnswersDto dto)
        {
            var questionIds = dto.Answers.Select(a => a.QuestionId).ToList();
            var correctAnswers = await _answerRepository.GetCorrectAnswersAsync(questionIds);

            int correctCount = dto.Answers.Count(a =>
                correctAnswers.Any(ca => ca.QuestionId == a.QuestionId && ca.Id == a.SelectedAnswerId));

            var score = (int)((double)correctCount / dto.Answers.Count * 100);

            var activityStudent = await _activityStudentRepository.GetActivityStudentAsync(dto.ActivityId, dto.StudentId);
            if (activityStudent == null)
                throw new Exception("Atividade não atribuída ao aluno.");

            activityStudent.Score = score;
            activityStudent.IsCorrected = true;
            activityStudent.CorrectionDate = DateTime.UtcNow;
            activityStudent.Status = ActivityStatus.Done;

            await _activityStudentRepository.SaveChangesAsync();

            return score;
        }

        public async Task<List<StudentCompletedActivityDto>> GetStudentCompletedActivitiesAsync(Guid studentId)
        {
            return await _activityStudentRepository.GetCompletedActivitiesByStudentAsync(studentId);
        }

    }
}
