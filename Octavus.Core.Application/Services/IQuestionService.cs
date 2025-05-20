using Octavus.Core.Application.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Octavus.Core.Application.Services
{
    public interface IQuestionService
    {
        Task<QuestionDto> CreateAsync(CreateQuestionDto dto);
        Task AddQuestionsBatchAsync(CreateQuestionBatchDto dto);
        Task<List<QuestionDto>> GetAllAsync();
        Task<List<QuestionDto>> GetByIdAsync(Guid id);
        Task UpdateAsync(Guid id, CreateQuestionDto dto);
        Task DeleteAsync(Guid id);
    }

}
