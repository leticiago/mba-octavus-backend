using Microsoft.AspNetCore.Mvc;
using Octavus.Core.Application.DTO;
using Octavus.Core.Application.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Xml;

[ApiController]
[Route("api/[controller]")]
public class ActivityController : ControllerBase
{
    private readonly IActivityService _activityService;
    private readonly IQuestionService _questionService;
    private readonly IDragAndDropActivityService _dragAndDropService;

    public ActivityController(
        IActivityService activityService,
        IQuestionService questionService,
        IDragAndDropActivityService dragAndDropService)
    {
        _activityService = activityService;
        _questionService = questionService;
        _dragAndDropService = dragAndDropService;
    }

    // ─── ACTIVITY ─────────────────────────────────────────────

    [HttpPost("activity")]
    public async Task<IActionResult> CreateActivity([FromBody] CreateActivityDto dto)
    {
        var result = await _activityService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetActivityById), new { id = result.Id }, result);
    }

    [HttpGet("activity")]
    public async Task<IActionResult> GetAllActivities()
    {
        var result = await _activityService.GetAllAsync();
        return Ok(result);
    }

    [HttpGet("activity/{id}")]
    public async Task<IActionResult> GetActivityById(Guid id)
    {
        var result = await _activityService.GetByIdAsync(id);
        return Ok(result);
    }

    [HttpGet("activity/professor/{professorId}")]
    public async Task<IActionResult> GetActivitiesByProfessor(Guid professorId)
    {
        var result = await _activityService.GetByProfessorIdAsync(professorId);
        return Ok(result);
    }

    [HttpPut("activity/{id}")]
    public async Task<IActionResult> UpdateActivity(Guid id, [FromBody] CreateActivityDto dto)
    {
        await _activityService.UpdateAsync(id, dto);
        return NoContent();
    }

    [HttpDelete("activity/{id}")]
    public async Task<IActionResult> DeleteActivity(Guid id)
    {
        await _activityService.DeleteAsync(id);
        return NoContent();
    }

    // ─── QUESTIONS AND ANSWERS ────────────────────────────────

    [HttpPost("questionandanswer")]
    public async Task<IActionResult> CreateQuestions([FromBody] CreateQuestionBatchDto dto)
    {
        await _questionService.AddQuestionsBatchAsync(dto);
        return StatusCode(201);
    }

    [HttpGet("questionandanswer")]
    public async Task<ActionResult<List<QuestionDto>>> GetAllQuestions()
    {
        var questions = await _questionService.GetAllAsync();
        return Ok(questions);
    }

    [HttpGet("questionandanswer/byactivity/{activityId}")]
    public async Task<IActionResult> GetQuestionsByActivity(Guid activityId)
    {
        var result = await _questionService.GetByIdAsync(activityId);
        return Ok(result);
    }

    [HttpPut("questionandanswer/{id}")]
    public async Task<IActionResult> UpdateQuestion(Guid id, [FromBody] CreateQuestionDto dto)
    {
        await _questionService.UpdateAsync(id, dto);
        return NoContent();
    }

    [HttpDelete("questionandanswer/{id}")]
    public async Task<IActionResult> DeleteQuestion(Guid id)
    {
        await _questionService.DeleteAsync(id);
        return NoContent();
    }

    // ─── DRAG AND DROP ─────────────────────────────────────────

    [HttpPost("draganddrop")]
    public async Task<IActionResult> CreateDragAndDrop([FromBody] DragAndDropActivityDto dto)
    {
        var result = await _dragAndDropService.CreateAsync(dto.ActivityId, dto.OriginalSequence);
        return CreatedAtAction(nameof(GetDragAndDropById), new { id = result.ActivityId }, result);
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
