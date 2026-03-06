using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Savage.Api.DTOs;
using Savage.Api.Services;

namespace Savage.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class VendorController : ControllerBase
{
    private readonly IVendorService _vendorService;

    public VendorController(IVendorService vendorService)
    {
        _vendorService = vendorService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var vendors = await _vendorService.GetAllAsync();
        return Ok(vendors);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var vendor = await _vendorService.GetByIdAsync(id);
        if (vendor == null) return NotFound();
        return Ok(vendor);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateVendorDto dto)
    {
        var vendor = await _vendorService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = vendor.Id }, vendor);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] UpdateVendorDto dto)
    {
        var vendor = await _vendorService.UpdateAsync(id, dto);
        if (vendor == null) return NotFound();
        return Ok(vendor);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var deleted = await _vendorService.DeleteAsync(id);
        if (!deleted) return NotFound();
        return NoContent();
    }
}
