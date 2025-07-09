using Microsoft.AspNetCore.Mvc;
using Octavus.Core.Application.DTO;
using Octavus.Core.Application.Services;
using Octavus.Core.Domain.Entities;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Authorization;

namespace Octavus.App.Api.Controllers.v1
{
    [ApiController]
    [Route("api/[controller]")]
    public class OpenTextController : ControllerBase
    {
        private readonly IQuestionService _questionService;
        private readonly IOpenTextAnswerService _openTextAnswerService;

        public OpenTextController(IQuestionService questionService, IOpenTextAnswerService openTextAnswerService)
        {
            _questionService = questionService;
            _openTextAnswerService = openTextAnswerService;
        }

        [HttpPost("question")]
        [Authorize(Roles = "Professor, Colaborador")]
        public async Task<IActionResult> CreateQuestion([FromBody] CreateQuestionOpenTextDto dto)
        {
            var question = new QuestionOpenTextDto
            {
                Title = dto.Title,
                ActivityId = dto.ActivityId,
                Id = Guid.NewGuid()
            };

            await _questionService.CreateAsync(question);
            return StatusCode(201);
        }

        [HttpGet("question/{questionId}")]
        [Authorize(Roles = "Professor, Colaborador, Aluno")]
        public async Task<IActionResult> GetQuestion(Guid questionId)
        {
            var result = await _questionService.GetByIdAsync(questionId);
            return Ok(result);
        }

        [HttpPost("answer")]
        [Authorize(Roles = "Professor, Colaborador, Aluno")]
        public async Task<IActionResult> CreateAnswer([FromBody] OpenTextAnswer dto)
        {
            await _openTextAnswerService.CreateAsync(dto);
            return StatusCode(201);
        }

        [HttpGet("activity/{activityId}/student/{studentId}")]
        [Authorize(Roles = "Professor, Aluno")]
        public async Task<IActionResult> GetAnswer(Guid activityId, Guid studentId)
        {
            var result = await _openTextAnswerService.GetByIdAsync(activityId, studentId);
            return Ok(result);
        }
    }

}
