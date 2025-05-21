using Microsoft.EntityFrameworkCore;
using Octavus.Core.Application.DTO;
using Octavus.Core.Application.Repositories;
using Octavus.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Octavus.Infra.Persistence.Repositories
{
    public class ActivityStudentRepository : RepositoryBase<ActivityStudent>, IActivityStudentRepository
    {
        public ActivityStudentRepository(Context context) : base(context) { }

        public async Task<bool> ExistsAsync(Guid studentId, Guid activityId)
        {
            return await _context.Set<ActivityStudent>()
                .AnyAsync(a => a.StudentId == studentId && a.ActivityId == activityId);
        }

        public async Task<ActivityStudent?> GetByStudentAndActivityAsync(Guid studentId, Guid activityId)
        {
            return await _context.Set<ActivityStudent>()
                .FirstOrDefaultAsync(x => x.StudentId == studentId && x.ActivityId == activityId);
        }
        public async Task<List<PendingActivityReviewDto>> GetPendingReviewsByProfessorAsync(Guid professorId)
        {
            var query = from activityStudent in _context.Set<ActivityStudent>()
                        join activity in _context.Set<Activity>() on activityStudent.ActivityId equals activity.Id
                        join student in _context.Set<User>() on activityStudent.StudentId equals student.Id
                        join link in _context.Set<ProfessorStudent>() on student.Id equals link.StudentId
                        where link.ProfessorId == professorId
                              && link.Active
                              && activityStudent.IsCorrected == false
                        select new PendingActivityReviewDto
                        {
                            StudentId = student.Id,
                            StudentName = student.Name,
                            ActivityId = activity.Id,
                            ActivityName = activity.Name
                        };

            return await query.ToListAsync();
        }

        public async Task<List<ActivityStudent>> GetActivitiesByStudentAsync(Guid studentId)
        {
            return await _context.Set<ActivityStudent>()
                .Include(a => a.Activity)
                .Where(a => a.StudentId == studentId)
                .ToListAsync();
        }

        public async Task<ActivityStudent?> GetActivityStudentAsync(Guid activityId, Guid studentId)
        {
            return await _context.Set<ActivityStudent>()
                .FirstOrDefaultAsync(x => x.ActivityId == activityId && x.StudentId == studentId);
        }

        public async Task<List<StudentCompletedActivityDto>> GetCompletedActivitiesByStudentAsync(Guid studentId)
        {
            return await _context.Set<ActivityStudent>()
                .Where(a => a.StudentId == studentId && a.IsCorrected)
                .OrderByDescending(a => a.CorrectionDate)
                .Select(a => new StudentCompletedActivityDto
                {
                    ActivityId = a.ActivityId,
                    Title = a.Activity.Name,
                    Type = a.Activity.Type.ToString(),
                    Score = a.Score,
                    CorrectionDate = a.CorrectionDate ?? DateTime.MinValue
                })
                .ToListAsync();
        }
    }

}
