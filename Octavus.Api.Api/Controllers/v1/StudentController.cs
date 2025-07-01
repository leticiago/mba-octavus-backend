using Microsoft.AspNetCore.Mvc;
using Octavus.Core.Application.Services;
using System.Threading.Tasks;
using System;
using Octavus.Core.Application.DTO;
using Octavus.Infra.Core.Services;
using Microsoft.AspNetCore.Authorization;

namespace Octavus.Controllers.v1
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudentController : ControllerBase
    {
        private readonly IActivityStudentService _activityStudentService;
        private readonly IStudentService _studentService;

        public StudentController(IActivityStudentService activityStudentService, IStudentService studentservice)
        {
            _activityStudentService = activityStudentService;
            _studentService = studentservice;
        }

        [HttpGet("{studentId}/activities")]
        [Authorize(Roles = "Aluno")]
        public async Task<IActionResult> GetActivities(Guid studentId)
        {
            var result = await _activityStudentService.GetActivitiesForStudentAsync(studentId);
            return Ok(result);
        }

        [HttpPost("submit/question-and-answer")]
        [Authorize(Roles = "Aluno")]
        public async Task<IActionResult> SubmitAnswers([FromBody] SubmitAnswersDto dto)
        {
            var score = await _studentService.SubmitAnswersAsync(dto);
            return Ok(new
            {
                message = "Respostas enviadas e avaliadas com sucesso.",
                score
            });
        }

        [HttpGet("{studentId}/completed-activities")]
        [Authorize(Roles = "Aluno")]
        public async Task<IActionResult> GetCompletedActivities(Guid studentId)
        {
            var result = await _studentService.GetStudentCompletedActivitiesAsync(studentId);
            return Ok(result);
        }

        [HttpPost("submit/drag-and-drop")]
        [Authorize(Roles = "Aluno")]
        public async Task<IActionResult> SubmitDragAndDrop([FromBody] DragAndDropSubmissionDto dto)
        {
            var result = await _studentService.GradeDragAndDropAsync(dto);
            return Ok(result);
        }

        [HttpGet("{studentId}/metrics")]
        [Authorize(Roles = "Aluno, Professor")]
        public async Task<IActionResult> GetMetricsByStudent(Guid studentId)
        {
            var metrics = await _activityStudentService.GetMetricsByStudentAsync(studentId);
            return Ok(metrics);
        }
    }

}
