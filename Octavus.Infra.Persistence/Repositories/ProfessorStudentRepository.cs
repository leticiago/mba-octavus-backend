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
    public class ProfessorStudentRepository : RepositoryBase<ProfessorStudent>, IProfessorStudentRepository
    {
        public ProfessorStudentRepository(Context context) : base(context)
        {
        }

        public ProfessorStudent? GetBond(Guid studentId, Guid professorId)
        {
            var response = _context.Set<ProfessorStudent>().Where(x => x.ProfessorId == professorId && x.StudentId == studentId).FirstOrDefault();
            if (response != null)
                return response;

            return null;
        }

        public async Task<List<StudentDto>> GetStudentsByProfessorAsync(Guid professorId)
        {
            return await (
                from ps in _context.Set<ProfessorStudent>()
                join u in _context.Set<User>() on ps.StudentId equals u.Id
                join i in _context.Set<Instrument>() on ps.InstrumentId equals i.Id
                where ps.ProfessorId == professorId && ps.Active
                select new StudentDto
                {
                    Id = u.Id,
                    Name = u.Name,
                    Email = u.Email,
                    Instrument = i.Name,
                }
                ).ToListAsync();
        }
    }

}
