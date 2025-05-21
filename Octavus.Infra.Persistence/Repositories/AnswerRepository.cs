using Microsoft.EntityFrameworkCore;
using Octavus.Core.Application.Repositories;
using Octavus.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Octavus.Infra.Persistence.Repositories
{
    public class AnswerRepository : RepositoryBase<Answer>, IAnswerRepository
    {
        public AnswerRepository(Context context) : base(context)
        {
        }

        public async Task<List<Answer>> GetCorrectAnswersAsync(IEnumerable<Guid> questionIds)
        {
           return await _context.Set<Answer>()
                .Where(a => questionIds.Contains(a.QuestionId) && a.IsCorrect)
                .ToListAsync();
        }
    }
}
