using Microsoft.AspNetCore.Mvc;
using Octavus.Core.Application.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Octavus.Core.Domain.Entities;

namespace Octavus.App.Api.Controllers.v1
{
    [ApiController]
    [Route("api/v{version:apiVersion}/profiles")]
    [ApiVersion("1.0")]
    public class ProfileController : ControllerBase
    {
        private readonly IProfileService _profileService;
     
        public ProfileController(IProfileService profileService)
        {
            _profileService = profileService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Profile>>> GetAll()
        {
            var profiles = await _profileService.GetAllAsync();
            return Ok(profiles);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Profile>> GetById(Guid id)
        {
            var profile = await _profileService.GetByIdAsync(id);
            if (profile == null) return NotFound();

            return Ok(profile);
        }

        [HttpPost]
        public async Task<ActionResult<Profile>> Create(Profile dto)
        {
            var created = await _profileService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, Profile dto)
        {
            if (id != dto.Id) return BadRequest("ID mismatch");

            var result = await _profileService.UpdateAsync(dto);
            return result ? NoContent() : NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _profileService.DeleteAsync(id);
            return result ? NoContent() : NotFound();
        }
    }

}
