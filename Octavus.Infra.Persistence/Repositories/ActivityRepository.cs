using Microsoft.EntityFrameworkCore;
using Octavus.Core.Application.Repositories;
using Octavus.Core.Domain.Entities;
using Octavus.Core.Domain.Enums;

namespace Octavus.Infra.Persistence.Repositories
{
    public class ActivityRepository : RepositoryBase<Activity>, IActivityRepository
    {
        public ActivityRepository(Context context) : base(context)
        {
        }

        public async Task<IEnumerable<Activity>> GetByProfessorIdAsync(Guid professorId, Guid? instrumentId)
        {
            return await _dbSet
                .Where(a => a.ProfessorId == professorId &&
                            (instrumentId == null || a.InstrumentId == instrumentId))
                .ToListAsync();
        }

        public async Task<List<Activity>> GetPublicActivitiesAsync()
        {
            return await _context.Set<Activity>()
                .Where(a => a.IsPublic && a.Type != ActivityType.OpenText.ToString())
                .ToListAsync();
        }

        public async Task<List<Activity>> GetAllByIds(List<Guid> activities)
        {
            return await _context.Set<Activity>()
                .Where(x => activities.Contains(x.Id))
                .ToListAsync();
        }
    }

}
