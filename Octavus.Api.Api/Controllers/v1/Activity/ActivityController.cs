using Microsoft.AspNetCore.Mvc;
using Octavus.Core.Application.DTO;
using Octavus.Core.Application.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Xml;
using Octavus.Core.Domain.Entities;
using Microsoft.AspNetCore.Authorization;

namespace Octavus.App.Api.Controllers.v1
{
    [ApiController]
    [Route("api/[controller]")]
    public class ActivityController : ControllerBase
    {
        private readonly IActivityService _activityService;

        public ActivityController(IActivityService activityService)
        {
            _activityService = activityService;
        }

        [HttpPost]
        [Authorize(Roles = "Professor, Colaborador")]
        public async Task<IActionResult> Create([FromBody] CreateActivityDto dto)
        {
            var result = await _activityService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpGet]
        [Authorize(Roles = "Professor")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _activityService.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Professor, Colaborador, Aluno")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _activityService.GetByIdAsync(id);
            return Ok(result);
        }

        [HttpGet("professor/{professorId}")]
        [Authorize(Roles = "Professor")]
        public async Task<IActionResult> GetByProfessor(Guid professorId)
        {
            var result = await _activityService.GetByProfessorIdAsync(professorId);
            return Ok(result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Professor, Colaborador")]
        public async Task<IActionResult> Update(Guid id, [FromBody] CreateActivityDto dto)
        {
            await _activityService.UpdateAsync(id, dto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Professor, Colaborador")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _activityService.DeleteAsync(id);
            return NoContent();
        }

        [HttpGet("public")]
        [Authorize(Roles = "Professor, Colaborador, Aluno")]
        public async Task<IActionResult> GetPublic()
        {
            var result = await _activityService.GetPublicActivitiesAsync();
            return Ok(result);
        }
    }
}