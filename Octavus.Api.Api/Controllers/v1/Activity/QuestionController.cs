using Microsoft.AspNetCore.Mvc;
using Octavus.Core.Application.DTO;
using Octavus.Core.Application.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace Octavus.App.Api.Controllers.v1.Activity
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuestionController : ControllerBase
    {
        private readonly IQuestionService _questionService;

        public QuestionController(IQuestionService questionService)
        {
            _questionService = questionService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateBatch([FromBody] CreateQuestionBatchDto dto)
        {
            await _questionService.AddQuestionsBatchAsync(dto);
            return StatusCode(201);
        }

        [HttpGet]
        public async Task<ActionResult<List<QuestionDto>>> GetAll()
        {
            var questions = await _questionService.GetAllAsync();
            return Ok(questions);
        }

        [HttpGet("byactivity/{activityId}")]
        public async Task<IActionResult> GetByActivity(Guid activityId)
        {
            var result = await _questionService.GetByIdAsync(activityId);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] CreateQuestionDto dto)
        {
            await _questionService.UpdateAsync(id, dto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _questionService.DeleteAsync(id);
            return NoContent();
        }
    }

}
