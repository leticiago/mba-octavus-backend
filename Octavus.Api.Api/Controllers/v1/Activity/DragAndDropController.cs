using Microsoft.AspNetCore.Mvc;
using Octavus.Core.Application.DTO;
using Octavus.Core.Application.Services;
using System.Threading.Tasks;
using System;

namespace Octavus.App.Api.Controllers.v1.Activity
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
        public async Task<IActionResult> Create([FromBody] CreateDragAndDropActivityDto dto)
        {
            var result = await _dragAndDropService.CreateAsync(dto.ActivityId, dto.OriginalSequence);
            return CreatedAtAction(nameof(GetById), new { id = result.ActivityId }, result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _dragAndDropService.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _dragAndDropService.GetByIdAsync(id);
            return Ok(result);
        }
    }

}
