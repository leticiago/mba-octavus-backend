using global::Octavus.Core.Application.DTO;
using global::Octavus.Core.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Octavus.Infra.Core.Services;
using System;
using System.Threading.Tasks;

namespace Octavus.Controllers.v1
{

    [ApiController]
    [Route("api/[controller]")]
    public class ProfessorController : ControllerBase
    {
        private readonly IProfessorStudentService _professorStudentService;
        private readonly IActivityStudentService _activityStudentService;

        public ProfessorController(IProfessorStudentService professorStudentService, IActivityStudentService activityStudentService)
        {
            _professorStudentService = professorStudentService;
            _activityStudentService = activityStudentService;
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
        [Authorize(Roles = "Professor")]
        public async Task<IActionResult> ManageStudent([FromBody] UpdateProfessorStudentDto dto)
        {
            await _professorStudentService.UpdateLinkAsync(dto);
            return Ok(new { message = "Vínculo atualizado com sucesso!" });
        }

        [HttpGet("{professorId}/students")]
        [Authorize(Roles = "Professor")]
        public async Task<IActionResult> GetStudentsByProfessor(Guid professorId)
        {
            var result = await _professorStudentService.GetStudentsByProfessorAsync(professorId);
            return Ok(result);
        }

        [HttpPost("assign-activity")]
        [Authorize(Roles = "Professor")]
        public async Task<IActionResult> AssignActivityToStudent([FromBody] AssignActivityDto dto)
        {
            await _activityStudentService.AssignActivityToStudentAsync(dto);
            return Ok("Atividade atribuída com sucesso.");
        }

        [HttpPut("evaluate-activity")]
        [Authorize(Roles = "Professor")]
        public async Task<IActionResult> EvaluateActivity([FromBody] EvaluateActivityDto dto)
        {
            await _activityStudentService.EvaluateActivityAsync(dto);
            return Ok("Atividade avaliada com sucesso.");
        }

        [HttpGet("{professorId}/pending-reviews")]
        [Authorize(Roles = "Professor")]
        public async Task<IActionResult> GetPendingReviews(Guid professorId)
        {
            var result = await _activityStudentService.GetPendingReviewsAsync(professorId);
            return Ok(result);
        }

    }
}
