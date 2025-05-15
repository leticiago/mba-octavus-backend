using Octavus.Core.Application.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Octavus.Core.Application.Services
{
    public interface IActivityService
    {
        Task<ActivityDto> CreateAsync(CreateActivityDto dto);
        Task<IEnumerable<ActivityDto>> GetAllAsync();
        Task<ActivityDto> GetByIdAsync(Guid id);
        Task<IEnumerable<ActivityDto>> GetByProfessorIdAsync(Guid professorId);
        Task UpdateAsync(Guid id, CreateActivityDto dto);
        Task DeleteAsync(Guid id);
    }

}
