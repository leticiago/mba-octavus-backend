using Octavus.Core.Application.DTO;
using Octavus.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Octavus.Core.Application.Repositories
{
    public interface IActivityStudentRepository : IRepositoryBase<ActivityStudent>
    {
        Task<bool> ExistsAsync(Guid studentId, Guid activityId);
        Task<ActivityStudent?> GetByStudentAndActivityAsync(Guid studentId, Guid activityId);
        Task<List<PendingActivityReviewDto>> GetPendingReviewsByProfessorAsync(Guid professorId);
    }

}
