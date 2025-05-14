using Octavus.Core.Application.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Octavus.Core.Application.Services
{
    public interface IDragAndDropActivityService
    {
        Task<DragAndDropActivityDto> CreateAsync(string activity);
        Task<List<DragAndDropActivityDto>> GetAllAsync();
        Task<DragAndDropActivityDto> GetByIdAsync(Guid id);
    }

}
