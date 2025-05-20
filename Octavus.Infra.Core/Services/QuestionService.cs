using Octavus.Core.Application.DTO;
using Octavus.Core.Application.Repositories;
using Octavus.Core.Application.Services;
using Octavus.Core.Domain.Entities;

namespace Octavus.Infra.Core.Services
{
    public class QuestionService : IQuestionService
    {
        private readonly IQuestionRepository _questionRepository;

        public QuestionService(IQuestionRepository questionRepository)
        {
            _questionRepository = questionRepository;
        }

        public async Task<QuestionDto> CreateAsync(CreateQuestionDto dto)
        {
            var question = new Question
            {
                Id = Guid.NewGuid(),
                Title = dto.Title,
                Answers = dto.Answers.Select(a => new Answer
                {
                    Id = Guid.NewGuid(),
                    QuestionId = Guid.Empty,
                    Text = a.Text,
                    IsCorrect = a.IsCorrect
                }).ToList()
            };

            await _questionRepository.AddAsync(question);
            return MapToDto(question);
        }

        public async Task<List<QuestionDto>> GetAllAsync()
        {
            var questions = await _questionRepository.GetAllAsync();
            return questions.Select(MapToDto).ToList();
        }

        public async Task<List<QuestionDto>?> GetByIdAsync(Guid id)
        {
            var response = new List<QuestionDto>(); 
            var questions = await _questionRepository.GetByActivityIdAsync(id) ?? throw new Exception("Atividade não encontrada.");
            
            foreach (var question in questions)
            {
                response.Add(MapToDto(question));
            }

            return response;
        }

        public async Task UpdateAsync(Guid id, CreateQuestionDto dto)
        {
            var question = await _questionRepository.GetByIdAsync(id) ?? throw new Exception("Pergunta não encontrada.");

            question.Title = dto.Title;
            question.Answers = dto.Answers.Select(a => new Answer
            {
                Id = Guid.NewGuid(),
                QuestionId = id,
                Text = a.Text,
                IsCorrect = a.IsCorrect
            }).ToList();

            await _questionRepository.UpdateAsync(question);
        }

        public async Task DeleteAsync(Guid id)
        {
            await _questionRepository.DeleteAsync(id);
        }

        private static QuestionDto MapToDto(Question question)
        {
            var rng = new Random();
            return new QuestionDto
            {
                Id = question.Id,
                Title = question.Title,
                Answers = question.Answers
                    .OrderBy(a => rng.Next()) 
                    .Select(a => new AnswerDto
                    {
                        Id = a.Id,
                        Text = a.Text,
                        IsCorrect = a.IsCorrect
                    }).ToList()
            };
        }
        public async Task AddQuestionsBatchAsync(CreateQuestionBatchDto dto)
        {
            foreach (var questionDto in dto.Questions)
            {
                var question = new Question
                {
                    Id = Guid.NewGuid(),
                    Title = questionDto.Title,
                    ActivityId = dto.ActivityId,
                    Answers = questionDto.Answers.Select(a => new Answer
                    {
                        Id = Guid.NewGuid(),
                        Text = a.Text,
                        IsCorrect = a.IsCorrect
                    }).ToList()
                };

                await _questionRepository.AddAsync(question);
            }
        }

    }

}
