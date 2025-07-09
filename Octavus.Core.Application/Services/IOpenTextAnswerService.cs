using Octavus.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Octavus.Core.Application.Services
{
    public interface IOpenTextAnswerService
    {
        Task<OpenTextAnswer?> GetByIdAsync(Guid questionId, Guid studentId);
        Task<OpenTextAnswer> CreateAsync(OpenTextAnswer answer);
    }
}
