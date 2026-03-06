using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Savage.Api.DTOs;
using Savage.Api.Services;

namespace Savage.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class InventoryController : ControllerBase
{
    private readonly IInventoryService _inventoryService;

    public InventoryController(IInventoryService inventoryService)
    {
        _inventoryService = inventoryService;
    }

    /// <summary>
    /// Lists inventory items. Vendors automatically see only their own items.
    /// Supports optional category, status, and sortBy query params.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? category,
        [FromQuery] string? status,
        [FromQuery] string? vendorId,
        [FromQuery] string? sortBy)
    {
        // Vendors can only see items assigned to their vendor account
        var role = User.FindFirstValue(ClaimTypes.Role);
        if (role == "Vendor")
            vendorId = User.FindFirstValue("vendorId");

        var items = await _inventoryService.GetAllAsync(category, status, vendorId, sortBy);
        return Ok(items);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var item = await _inventoryService.GetByIdAsync(id);
        if (item == null) return NotFound();

        // Vendors can only view their own items
        var role = User.FindFirstValue(ClaimTypes.Role);
        if (role == "Vendor" && item.VendorId != User.FindFirstValue("vendorId"))
            return Forbid();

        return Ok(item);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateInventoryItemDto dto)
    {
        var item = await _inventoryService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(string id, [FromBody] UpdateInventoryItemDto dto)
    {
        var item = await _inventoryService.UpdateAsync(id, dto);
        if (item == null) return NotFound();
        return Ok(item);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(string id)
    {
        var deleted = await _inventoryService.DeleteAsync(id);
        if (!deleted) return NotFound();
        return NoContent();
    }
}
