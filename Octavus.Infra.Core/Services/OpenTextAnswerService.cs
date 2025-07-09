using Octavus.Core.Application.Repositories;
using Octavus.Core.Application.Services;
using Octavus.Core.Domain.Entities;
using Octavus.Infra.Persistence.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Octavus.Infra.Core.Services
{
    public class OpenTextAnswerService : IOpenTextAnswerService
    {
        private readonly IOpenTextAnswerRepository _openTextAnswerRepository;
        private readonly IActivityStudentRepository _activityStudentRepository;
        private readonly IQuestionRepository _questionRepository;

        public OpenTextAnswerService(IOpenTextAnswerRepository openTextAnswerRepository, IActivityStudentRepository activityStudentRepository, IQuestionRepository questionRepository)
        {
            _openTextAnswerRepository = openTextAnswerRepository;
            _activityStudentRepository = activityStudentRepository;
            _questionRepository = questionRepository;
        }

        public async Task<OpenTextAnswer?> GetByIdAsync(Guid activityId, Guid studentId)
        {
            return await _openTextAnswerRepository.GetAnswerByActivity(activityId, studentId);
        }

        public async Task<OpenTextAnswer> CreateAsync(OpenTextAnswer answer)
        {
            await _openTextAnswerRepository.AddAsync(answer);
            await _openTextAnswerRepository.SaveChangesAsync();

            var question = await _questionRepository.GetByIdAsync(answer.QuestionId);
            var activity = new ActivityStudent
            {
                StudentId = answer.StudentId,
                ActivityId = question.ActivityId,
                Status = Octavus.Core.Domain.Enums.ActivityStatus.Done,
                Score = 0
            };

            await _activityStudentRepository.UpdateAsync(activity);
            return answer;
        }

    }
}
