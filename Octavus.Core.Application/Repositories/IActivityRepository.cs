using Octavus.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Octavus.Core.Application.Repositories
{
    public interface IActivityRepository : IRepositoryBase<Activity>
    {
        Task<IEnumerable<Activity>> GetByProfessorIdAsync(Guid professorId, Guid? instrumentId);
        Task<List<Activity>> GetPublicActivitiesAsync();
        Task<List<Activity>> GetAllByIds(List<Guid> activities);
    }

}
