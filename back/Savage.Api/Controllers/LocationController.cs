using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Savage.Api.DTOs;
using Savage.Api.Services;

namespace Savage.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class LocationController : ControllerBase
{
    private readonly ILocationService _locationService;

    public LocationController(ILocationService locationService)
    {
        _locationService = locationService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var locations = await _locationService.GetAllAsync();
        return Ok(locations);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var location = await _locationService.GetByIdAsync(id);
        if (location == null) return NotFound();
        return Ok(location);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateLocationDto dto)
    {
        var location = await _locationService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = location.Id }, location);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] UpdateLocationDto dto)
    {
        var location = await _locationService.UpdateAsync(id, dto);
        if (location == null) return NotFound();
        return Ok(location);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var deleted = await _locationService.DeleteAsync(id);
        if (!deleted) return NotFound();
        return NoContent();
    }
}
