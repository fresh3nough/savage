using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Savage.Api.Models;

/// <summary>
/// Represents an application user with role-based access (Admin or Vendor).
/// </summary>
public class User
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;

    [BsonElement("username")]
    public string Username { get; set; } = null!;

    [BsonElement("email")]
    public string Email { get; set; } = null!;

    [BsonElement("passwordHash")]
    public string PasswordHash { get; set; } = null!;

    [BsonElement("role")]
    public string Role { get; set; } = "Vendor";

    [BsonElement("vendorId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? VendorId { get; set; }

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Represents an inventory item tracked in the system.
/// </summary>
public class InventoryItem
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;

    [BsonElement("sku")]
    public string Sku { get; set; } = null!;

    [BsonElement("title")]
    public string Title { get; set; } = null!;

    [BsonElement("description")]
    public string Description { get; set; } = string.Empty;

    [BsonElement("category")]
    public string Category { get; set; } = string.Empty;

    [BsonElement("quantity")]
    public int Quantity { get; set; }

    [BsonElement("status")]
    public string Status { get; set; } = "Available";

    [BsonElement("vendorId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? VendorId { get; set; }

    [BsonElement("intakeDate")]
    public DateTime IntakeDate { get; set; } = DateTime.UtcNow;

    [BsonElement("locationId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? LocationId { get; set; }

    [BsonElement("shipmentStatus")]
    public string ShipmentStatus { get; set; } = "None";

    [BsonElement("notes")]
    public string Notes { get; set; } = string.Empty;
}

/// <summary>
/// Represents an external vendor that supplies or receives inventory.
/// </summary>
public class Vendor
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;

    [BsonElement("name")]
    public string Name { get; set; } = null!;

    [BsonElement("contactEmail")]
    public string ContactEmail { get; set; } = null!;

    [BsonElement("contactPhone")]
    public string ContactPhone { get; set; } = string.Empty;

    [BsonElement("address")]
    public string Address { get; set; } = string.Empty;

    [BsonElement("status")]
    public string Status { get; set; } = "Active";

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Represents a physical storage or distribution location.
/// </summary>
public class Location
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;

    [BsonElement("name")]
    public string Name { get; set; } = null!;

    [BsonElement("address")]
    public string Address { get; set; } = null!;

    [BsonElement("type")]
    public string Type { get; set; } = "Warehouse";

    [BsonElement("capacity")]
    public int Capacity { get; set; }

    [BsonElement("currentOccupancy")]
    public int CurrentOccupancy { get; set; }
}

/// <summary>
/// Represents a shipment containing one or more inventory items.
/// </summary>
public class Shipment
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;

    [BsonElement("inventoryItemIds")]
    public List<string> InventoryItemIds { get; set; } = new();

    [BsonElement("vendorId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? VendorId { get; set; }

    [BsonElement("origin")]
    public string Origin { get; set; } = null!;

    [BsonElement("destination")]
    public string Destination { get; set; } = null!;

    [BsonElement("status")]
    public string Status { get; set; } = "Pending";

    [BsonElement("trackingNumber")]
    public string TrackingNumber { get; set; } = string.Empty;

    [BsonElement("shippedDate")]
    public DateTime? ShippedDate { get; set; }

    [BsonElement("estimatedArrival")]
    public DateTime? EstimatedArrival { get; set; }

    [BsonElement("notes")]
    public string Notes { get; set; } = string.Empty;
}

/// <summary>
/// MongoDB connection settings loaded from appsettings.json.
/// </summary>
public class MongoDbSettings
{
    public string ConnectionString { get; set; } = null!;
    public string DatabaseName { get; set; } = null!;
}
