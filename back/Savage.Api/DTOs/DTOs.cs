namespace Savage.Api.DTOs;

// ── Auth ──

public class LoginRequest
{
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
}

public class RegisterRequest
{
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string Role { get; set; } = "Vendor";
    public string? VendorId { get; set; }
}

public class AuthResponse
{
    public string Token { get; set; } = null!;
    public string Username { get; set; } = null!;
    public string Role { get; set; } = null!;
    public string? VendorId { get; set; }
}

// ── User ──

public class UserDto
{
    public string Id { get; set; } = null!;
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Role { get; set; } = null!;
    public string? VendorId { get; set; }
    public DateTime CreatedAt { get; set; }
}

// ── Inventory ──

public class InventoryItemDto
{
    public string Id { get; set; } = null!;
    public string Sku { get; set; } = null!;
    public string Title { get; set; } = null!;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public string Status { get; set; } = null!;
    public string? VendorId { get; set; }
    public DateTime IntakeDate { get; set; }
    public string? LocationId { get; set; }
    public string ShipmentStatus { get; set; } = null!;
    public string Notes { get; set; } = string.Empty;
}

public class CreateInventoryItemDto
{
    public string Sku { get; set; } = null!;
    public string Title { get; set; } = null!;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public string Status { get; set; } = "Available";
    public string? VendorId { get; set; }
    public string? LocationId { get; set; }
    public string ShipmentStatus { get; set; } = "None";
    public string Notes { get; set; } = string.Empty;
}

public class UpdateInventoryItemDto
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Category { get; set; }
    public int? Quantity { get; set; }
    public string? Status { get; set; }
    public string? VendorId { get; set; }
    public string? LocationId { get; set; }
    public string? ShipmentStatus { get; set; }
    public string? Notes { get; set; }
}

// ── Vendor ──

public class VendorDto
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string ContactEmail { get; set; } = null!;
    public string ContactPhone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Status { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
}

public class CreateVendorDto
{
    public string Name { get; set; } = null!;
    public string ContactEmail { get; set; } = null!;
    public string ContactPhone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
}

public class UpdateVendorDto
{
    public string? Name { get; set; }
    public string? ContactEmail { get; set; }
    public string? ContactPhone { get; set; }
    public string? Address { get; set; }
    public string? Status { get; set; }
}

// ── Location ──

public class LocationDto
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Address { get; set; } = null!;
    public string Type { get; set; } = null!;
    public int Capacity { get; set; }
    public int CurrentOccupancy { get; set; }
}

public class CreateLocationDto
{
    public string Name { get; set; } = null!;
    public string Address { get; set; } = null!;
    public string Type { get; set; } = "Warehouse";
    public int Capacity { get; set; }
}

public class UpdateLocationDto
{
    public string? Name { get; set; }
    public string? Address { get; set; }
    public string? Type { get; set; }
    public int? Capacity { get; set; }
    public int? CurrentOccupancy { get; set; }
}

// ── Shipment ──

public class ShipmentDto
{
    public string Id { get; set; } = null!;
    public List<string> InventoryItemIds { get; set; } = new();
    public string? VendorId { get; set; }
    public string Origin { get; set; } = null!;
    public string Destination { get; set; } = null!;
    public string Status { get; set; } = null!;
    public string TrackingNumber { get; set; } = string.Empty;
    public DateTime? ShippedDate { get; set; }
    public DateTime? EstimatedArrival { get; set; }
    public string Notes { get; set; } = string.Empty;
}

public class CreateShipmentDto
{
    public List<string> InventoryItemIds { get; set; } = new();
    public string? VendorId { get; set; }
    public string Origin { get; set; } = null!;
    public string Destination { get; set; } = null!;
    public string TrackingNumber { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
}

public class UpdateShipmentDto
{
    public string? Status { get; set; }
    public string? TrackingNumber { get; set; }
    public DateTime? ShippedDate { get; set; }
    public DateTime? EstimatedArrival { get; set; }
    public string? Notes { get; set; }
    public string? Destination { get; set; }
}
