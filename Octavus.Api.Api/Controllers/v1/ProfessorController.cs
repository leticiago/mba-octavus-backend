using global::Octavus.Core.Application.DTO;
using global::Octavus.Core.Application.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Octavus.Controllers.v1
{

    [ApiController]
    [Route("api/[controller]")]
    public class ProfessorController : ControllerBase
    {
        private readonly IProfessorStudentService _professorStudentService;

        public ProfessorController(IProfessorStudentService professorStudentService)
        {
            _professorStudentService = professorStudentService;
        }

        /// <summary>
        /// Vincula um aluno a um professor pelo e-mail do aluno.
        /// </summary>
        [HttpPost("link-student")]
        public async Task<IActionResult> LinkStudent([FromBody] LinkStudentByEmailDto dto)
        {
            await _professorStudentService.LinkByEmailAsync(dto);
            return Ok(new { message = "Aluno vinculado com sucesso!" });
        }

        [HttpPut("manage-student")]
        public async Task<IActionResult> ManageStudent([FromBody] UpdateProfessorStudentDto dto)
        {
            await _professorStudentService.UpdateLinkAsync(dto);
            return Ok(new { message = "Vínculo atualizado com sucesso!" });
        }

        [HttpGet("{professorId}/students")]
        public async Task<IActionResult> GetStudentsByProfessor(Guid professorId)
        {
            var result = await _professorStudentService.GetStudentsByProfessorAsync(professorId);
            return Ok(result);
        }
    }
}
