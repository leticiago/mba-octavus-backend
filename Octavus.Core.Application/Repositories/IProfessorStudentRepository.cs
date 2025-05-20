using Octavus.Core.Application.DTO;
using Octavus.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Octavus.Core.Application.Repositories
{
    public interface IProfessorStudentRepository : IRepositoryBase<ProfessorStudent>
    { 
        ProfessorStudent? GetBond(Guid studentId, Guid professorId);
        Task<List<StudentDto>> GetStudentsByProfessorAsync(Guid professorId);
    }

}
