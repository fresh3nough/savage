using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Savage.Api.DTOs;
using Savage.Api.Services;

namespace Savage.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ShipmentController : ControllerBase
{
    private readonly IShipmentService _shipmentService;

    public ShipmentController(IShipmentService shipmentService)
    {
        _shipmentService = shipmentService;
    }

    /// <summary>
    /// Lists shipments. Vendors automatically see only their own shipments.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var role = User.FindFirstValue(ClaimTypes.Role);
        var vendorId = role == "Vendor" ? User.FindFirstValue("vendorId") : null;
        var shipments = await _shipmentService.GetAllAsync(vendorId);
        return Ok(shipments);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var shipment = await _shipmentService.GetByIdAsync(id);
        if (shipment == null) return NotFound();

        var role = User.FindFirstValue(ClaimTypes.Role);
        if (role == "Vendor" && shipment.VendorId != User.FindFirstValue("vendorId"))
            return Forbid();

        return Ok(shipment);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateShipmentDto dto)
    {
        var shipment = await _shipmentService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = shipment.Id }, shipment);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(string id, [FromBody] UpdateShipmentDto dto)
    {
        var shipment = await _shipmentService.UpdateAsync(id, dto);
        if (shipment == null) return NotFound();
        return Ok(shipment);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(string id)
    {
        var deleted = await _shipmentService.DeleteAsync(id);
        if (!deleted) return NotFound();
        return NoContent();
    }
}
