using Microsoft.AspNetCore.Mvc;
using Octavus.Core.Application.Services;
using System.Threading.Tasks;
using System;
using Octavus.Core.Application.DTO;
using System.Collections.Generic;

namespace Octavus.App.Api.Controllers.v1
{
    [ApiController]
    [Route("api/[controller]")]
    public class ActivityController : ControllerBase
    {
        //private readonly IActivityService _activityService;
        private readonly IQuestionService _questionService;
        private readonly IDragAndDropActivityService _dragAndDropService;

        public ActivityController(
            //IActivityService activityService,
            IQuestionService questionService,
            IDragAndDropActivityService dragAndDropService)
        {
            //_activityService = activityService;
            _questionService = questionService;
            _dragAndDropService = dragAndDropService;
        }
        
        //[HttpPost]
        //public async Task<IActionResult> CreateActivity([FromBody] CreateActivityDto dto)
        //{
        //    var result = await _activityService.CreateAsync(dto);
        //    return CreatedAtAction(nameof(GetActivityById), new { id = result.Id }, result);
        //}

        //[HttpGet]
        //public async Task<IActionResult> GetAllActivities()
        //{
        //    var result = await _activityService.GetAllAsync();
        //    return Ok(result);
        //}

        //[HttpGet("{id}")]
        //public async Task<IActionResult> GetActivityById(Guid id)
        //{
        //    var result = await _activityService.GetByIdAsync(id);
        //    return Ok(result);
        //}

        //[HttpPut("{id}")]
        //public async Task<IActionResult> UpdateActivity(Guid id, [FromBody] CreateActivityDto dto)
        //{
        //    await _activityService.UpdateAsync(id, dto);
        //    return NoContent();
        //}

        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteActivity(Guid id)
        //{
        //    await _activityService.DeleteAsync(id);
        //    return NoContent();
        //}

        [HttpPost("questionandanswer")]
        public async Task<IActionResult> CreateQuestions([FromBody] CreateQuestionBatchDto dto)
        {
            await _questionService.AddQuestionsBatchAsync(dto);
            return StatusCode(201);
        }

        [HttpGet("questionandanswer/{activityId}")]
        public async Task<IActionResult> GetQuestionsByActivity(Guid activityId)
        {
            var result = await _questionService.GetByIdAsync(activityId);
            return Ok(result);
        }

        [HttpGet("questionandanswer/all")]
        [HttpGet]
        public async Task<ActionResult<List<QuestionDto>>> GetAll()
        {
            var questions = await _questionService.GetAllAsync();
            return Ok(questions);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, CreateQuestionDto dto)
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

        [HttpPost("draganddrop")]
        public async Task<IActionResult> CreateDragAndDrop([FromBody] DragAndDropActivityDto dto)
        {
            var result = await _dragAndDropService.CreateAsync(dto.OriginalSequence);
            return CreatedAtAction(nameof(GetDragAndDropById), new { id = result.Id }, result);
        }

        [HttpGet("draganddrop")]
        public async Task<IActionResult> GetAllDragAndDrop()
        {
            var result = await _dragAndDropService.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("draganddrop/{id}")]
        public async Task<IActionResult> GetDragAndDropById(Guid id)
        {
            var result = await _dragAndDropService.GetByIdAsync(id);
            return Ok(result);
        }
    }

}
