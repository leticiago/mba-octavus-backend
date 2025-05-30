using Octavus.Core.Application.DTO;
using Octavus.Core.Application.Repositories;
using Octavus.Core.Application.Services;
using Octavus.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Octavus.Infra.Core.Services
{
    public class ProfessorStudentService : IProfessorStudentService
    {
        private readonly IProfessorStudentRepository _repository;
        private readonly IUserRepository _userRepository;

        public ProfessorStudentService(
            IProfessorStudentRepository repository,
            IUserRepository userRepository)
        {
            _repository = repository;
            _userRepository = userRepository;
        }

        public async Task LinkByEmailAsync(LinkStudentByEmailDto dto)
        {
            var student = new User() { Name = "", Email = "", Password = "", Username = "" };

            if (dto.StudentEmail != null)
            {
                student = await _userRepository.GetByEmailAsync(dto.StudentEmail);
                if (student == null)
                    throw new Exception("Aluno não encontrado com o e-mail informado.");
            }
            else
            {
                student.Id = dto.StudentId;
            }

            var entity = new ProfessorStudent
            {
                Id = Guid.NewGuid(),
                StudentId = student.Id,
                ProfessorId = dto.ProfessorId,
                InstrumentId = dto.InstrumentId,
                Date = DateTime.UtcNow,
                Active = true
            };

            await _repository.AddAsync(entity);
        }

        public async Task UpdateLinkAsync(UpdateProfessorStudentDto dto)
        {
            var professorStudent = new LinkStudentByEmailDto()
            {
                ProfessorId = dto.ProfessorId,
                InstrumentId = dto.InstrumentId,
                StudentEmail = null,
                StudentId = dto.StudentId,
            };

            var link = _repository.GetBond(professorStudent.StudentId, professorStudent.ProfessorId);
            if (link == null) throw new Exception("Vínculo não encontrado.");

            if (dto.Active.HasValue)
                link.Active = dto.Active.Value;

            await _repository.UpdateAsync(link);
        }

        public async Task<List<StudentDto>> GetStudentsByProfessorAsync(Guid professorId)
        {
            var students = await _repository.GetStudentsByProfessorAsync(professorId);

            return students.Select(s => new StudentDto
            {
                Id = s.Id,
                Name = s.Name,
                Email = s.Email,
                Instrument = s.Instrument
            }).ToList();
        }
    }

}
