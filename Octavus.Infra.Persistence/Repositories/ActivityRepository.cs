using Microsoft.EntityFrameworkCore;
using Octavus.Core.Application.Repositories;
using Octavus.Core.Domain.Entities;

namespace Octavus.Infra.Persistence.Repositories
{
    public class ActivityRepository : RepositoryBase<Activity>, IActivityRepository
    {
        public ActivityRepository(Context context) : base(context)
        {
        }

        public async Task<IEnumerable<Activity>> GetByProfessorIdAsync(Guid professorId)
        {
            return await _dbSet
                .Where(a => a.ProfessorId == professorId)
                .ToListAsync();
        }
    }

}
