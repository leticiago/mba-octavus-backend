using Microsoft.EntityFrameworkCore;
using Octavus.Core.Application.Repositories;
using Octavus.Core.Domain.Entities;

namespace Octavus.Infra.Persistence.Repositories
{
    public class QuestionRepository : RepositoryBase<Question>, IQuestionRepository
    {
        public QuestionRepository(Context context) : base(context) { }

        public override async Task<Question?> GetByIdAsync(Guid id)
        {
            return await _context.Set<Question>()
                .Include(q => q.Answers)
                .FirstOrDefaultAsync(q => q.Id == id);
        }

        public async Task<List<Question>> GetByActivityIdAsync(Guid id)
        {
            return await _context.Set<Question>()
                .Include(q => q.Answers)
                .Where(q => q.ActivityId == id)
                .ToListAsync();
        }

        public override async Task<IEnumerable<Question>> GetAllAsync()
        {
            return await _context.Set<Question>()
                .Include(q => q.Answers)
                .ToListAsync();
        }
    }

}
