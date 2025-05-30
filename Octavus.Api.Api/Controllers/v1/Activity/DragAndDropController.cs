using Microsoft.AspNetCore.Mvc;
using Octavus.Core.Application.DTO;
using Octavus.Core.Application.Services;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Authorization;

namespace Octavus.App.Api.Controllers.v1
{
    [ApiController]
    [Route("api/[controller]")]
    public class DragAndDropController : ControllerBase
    {
        private readonly IDragAndDropActivityService _dragAndDropService;

        public DragAndDropController(IDragAndDropActivityService dragAndDropService)
        {
            _dragAndDropService = dragAndDropService;
        }

        [HttpPost]
        [Authorize(Roles = "Professor, Colaborador")]
        public async Task<IActionResult> Create([FromBody] CreateDragAndDropActivityDto dto)
        {
            var result = await _dragAndDropService.CreateAsync(dto.ActivityId, dto.OriginalSequence);
            return CreatedAtAction(nameof(GetById), new { id = result.ActivityId }, result);
        }

        [HttpGet]
        [Authorize(Roles = "Professor")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _dragAndDropService.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Professor, Colaborador, Aluno")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _dragAndDropService.GetByIdAsync(id);
            return Ok(result);
        }
    }

}
