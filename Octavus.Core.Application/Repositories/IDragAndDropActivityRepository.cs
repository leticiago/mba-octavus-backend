using Octavus.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Octavus.Core.Application.Repositories
{
    public interface IDragAndDropActivityRepository : IRepositoryBase<DragAndDropActivity>
    {
        Task<DragAndDropActivity> GetByActivityIdAsync(Guid activityId);
    }

}
