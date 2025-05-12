using Octavus.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Octavus.Core.Application.Services
{
    public interface IProfileService
    {
        Task<IEnumerable<Profile>> GetAllAsync();
        Task<Profile> GetByIdAsync(Guid id);
        Task<Profile> CreateAsync(Profile dto);
        Task<bool> UpdateAsync(Profile dto);
        Task<bool> DeleteAsync(Guid id);
    }

}
