using Octavus.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Octavus.Core.Application.Services
{
    public interface IInstrumentService
    {
        Task<IEnumerable<Instrument>> GetAllAsync();
        Task<Instrument?> GetByIdAsync(Guid id);
        Task<Instrument> CreateAsync(Instrument dto);
        Task<bool> UpdateAsync(Instrument dto);
        Task<bool> DeleteAsync(Guid id);
    }

}
