using Octavus.Core.Application.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Octavus.Core.Application.Services
{
    public interface IActivityStudentService
    {
        Task AssignActivityToStudentAsync(AssignActivityDto dto);
        Task EvaluateActivityAsync(EvaluateActivityDto dto);
        Task<List<PendingActivityReviewDto>> GetPendingReviewsAsync(Guid professorId);
        Task<List<ActivityStudentDto>> GetActivitiesForStudentAsync(Guid studentId);

    }

}
