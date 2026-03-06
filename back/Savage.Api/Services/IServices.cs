using Savage.Api.DTOs;
using Savage.Api.Models;

namespace Savage.Api.Services;

/// <summary>
/// Handles user authentication: login, registration, and JWT generation.
/// </summary>
public interface IAuthService
{
    Task<AuthResponse?> LoginAsync(LoginRequest request);
    Task<AuthResponse?> RegisterAsync(RegisterRequest request);
}

/// <summary>
/// CRUD operations for inventory items with filtering support.
/// </summary>
public interface IInventoryService
{
    Task<List<InventoryItemDto>> GetAllAsync(string? category = null, string? status = null, string? vendorId = null, string? sortBy = null);
    Task<InventoryItemDto?> GetByIdAsync(string id);
    Task<InventoryItemDto> CreateAsync(CreateInventoryItemDto dto);
    Task<InventoryItemDto?> UpdateAsync(string id, UpdateInventoryItemDto dto);
    Task<bool> DeleteAsync(string id);
}

/// <summary>
/// CRUD operations for vendor profiles.
/// </summary>
public interface IVendorService
{
    Task<List<VendorDto>> GetAllAsync();
    Task<VendorDto?> GetByIdAsync(string id);
    Task<VendorDto> CreateAsync(CreateVendorDto dto);
    Task<VendorDto?> UpdateAsync(string id, UpdateVendorDto dto);
    Task<bool> DeleteAsync(string id);
}

/// <summary>
/// CRUD operations for storage/distribution locations.
/// </summary>
public interface ILocationService
{
    Task<List<LocationDto>> GetAllAsync();
    Task<LocationDto?> GetByIdAsync(string id);
    Task<LocationDto> CreateAsync(CreateLocationDto dto);
    Task<LocationDto?> UpdateAsync(string id, UpdateLocationDto dto);
    Task<bool> DeleteAsync(string id);
}

/// <summary>
/// CRUD operations for shipments with vendor-scoped access support.
/// </summary>
public interface IShipmentService
{
    Task<List<ShipmentDto>> GetAllAsync(string? vendorId = null);
    Task<ShipmentDto?> GetByIdAsync(string id);
    Task<ShipmentDto> CreateAsync(CreateShipmentDto dto);
    Task<ShipmentDto?> UpdateAsync(string id, UpdateShipmentDto dto);
    Task<bool> DeleteAsync(string id);
}
