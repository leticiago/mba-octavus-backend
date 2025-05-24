using Octavus.Core.Application.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Octavus.Core.Application.Services
{
    public interface IStudentService
    {
        Task<int> SubmitAnswersAsync(SubmitAnswersDto dto);
        Task<List<StudentCompletedActivityDto>> GetStudentCompletedActivitiesAsync(Guid studentId);
        Task<ActivityScoreResultDto> GradeDragAndDropAsync(DragAndDropSubmissionDto dto);
    }
}
