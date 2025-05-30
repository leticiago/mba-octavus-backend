using Microsoft.AspNetCore.Mvc;
using Octavus.Core.Application.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Octavus.Core.Domain.Entities;
using Microsoft.AspNetCore.Authorization;

namespace Octavus.App.Api.Controllers.v1
{
    [ApiController]
    [Route("api/v{version:apiVersion}/instruments")]
    [ApiVersion("1.0")]
    public class InstrumentController : ControllerBase
    {
        private readonly IInstrumentService _instrumentService;

        public InstrumentController(IInstrumentService instrumentService)
        {
            _instrumentService = instrumentService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Instrument>>> GetAll()
        {
            var instruments = await _instrumentService.GetAllAsync();
            return Ok(instruments);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<Instrument>> GetById(Guid id)
        {
            var instrument = await _instrumentService.GetByIdAsync(id);
            if (instrument == null) return NotFound();

            return Ok(instrument);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Instrument>> Create(Instrument instrument)
        {
            Instrument dto = new Instrument()
            {
                Id = Guid.NewGuid(),
                Name = instrument.Name,
            };
            var created = await _instrumentService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(Guid id, Instrument dto)
        {
            if (id != dto.Id) return BadRequest("ID mismatch");

            var result = await _instrumentService.UpdateAsync(dto);
            return result ? NoContent() : NotFound();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _instrumentService.DeleteAsync(id);
            return result ? NoContent() : NotFound();
        }
    }

}
