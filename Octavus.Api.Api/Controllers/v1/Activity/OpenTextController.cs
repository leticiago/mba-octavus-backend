using Microsoft.AspNetCore.Mvc;
using Octavus.Core.Application.DTO;
using Octavus.Core.Application.Services;
using Octavus.Core.Domain.Entities;
using System.Threading.Tasks;
using System;

namespace Octavus.App.Api.Controllers.v1.Activity
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
        public async Task<IActionResult> GetQuestion(Guid questionId)
        {
            var result = await _questionService.GetByIdAsync(questionId);
            return Ok(result);
        }

        [HttpPost("answer")]
        public async Task<IActionResult> CreateAnswer([FromBody] OpenTextAnswer dto)
        {
            await _openTextAnswerService.CreateAsync(dto);
            return StatusCode(201);
        }

        [HttpGet("answer/{answerId}")]
        public async Task<IActionResult> GetAnswer(Guid answerId)
        {
            var result = await _openTextAnswerService.GetByIdAsync(answerId);
            return Ok(result);
        }
    }

}
