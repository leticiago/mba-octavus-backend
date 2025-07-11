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
    public class OpenTextAnswerRepository : RepositoryBase<OpenTextAnswer>, IOpenTextAnswerRepository
    {
        public OpenTextAnswerRepository(Context context) : base(context) { }

        public async Task<OpenTextAnswer> GetAnswerByActivity(Guid activityId, Guid studentId)
        {
            var question = await _context.Set<Question>().Where(x => x.ActivityId == activityId).Select(x => x.Id).FirstOrDefaultAsync();
            return await _context.Set<OpenTextAnswer>()
                 .Where(x => x.QuestionId == question && x.StudentId == studentId).FirstOrDefaultAsync();

        }

        public async Task<List<Question>> GetByActivityIdAsync(Guid id)
        {
            return await _context.Set<Question>()
                .Include(q => q.Answers)
                .Where(q => q.ActivityId == id)
                .ToListAsync();
        }
    }
}
