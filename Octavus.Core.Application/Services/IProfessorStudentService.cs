using Octavus.Core.Application.DTO;
using Octavus.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Octavus.Core.Application.Services
{
    public interface IProfessorStudentService
    {
        Task LinkByEmailAsync(LinkStudentByEmailDto dto);
        Task UpdateLinkAsync(UpdateProfessorStudentDto dto);
        Task<List<StudentDto>> GetStudentsByProfessorAsync(Guid professorId);
    }

}
