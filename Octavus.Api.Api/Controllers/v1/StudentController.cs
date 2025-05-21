using Microsoft.AspNetCore.Mvc;
using Octavus.Core.Application.Services;
using System.Threading.Tasks;
using System;
using Octavus.Core.Application.DTO;
using Octavus.Infra.Core.Services;

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

        [HttpGet("{studentId}")]
        public async Task<IActionResult> GetActivities(Guid studentId)
        {
            var result = await _activityStudentService.GetActivitiesForStudentAsync(studentId);
            return Ok(result);
        }

        [HttpPost("submit-answers")]
        public async Task<IActionResult> SubmitAnswers([FromBody] SubmitAnswersDto dto)
        {
            var score = await _studentService.SubmitAnswersAsync(dto);
            return Ok(new
            {
                message = "Respostas enviadas e avaliadas com sucesso.",
                score
            });
        }

        [HttpGet("students/{studentId}/completed-activities")]
        public async Task<IActionResult> GetCompletedActivities(Guid studentId)
        {
            var result = await _studentService.GetStudentCompletedActivitiesAsync(studentId);
            return Ok(result);
        }
    }

}
