using MongoDB.Driver;
using Savage.Api.Models;

namespace Savage.Api.Data;

/// <summary>
/// Seeds the MongoDB database with initial fake data when collections are empty.
/// Called once at application startup.
/// </summary>
public static class DatabaseSeeder
{
    public static async Task SeedAsync(IMongoDatabase database)
    {
        var vendors = database.GetCollection<Vendor>("Vendors");
        if (await vendors.CountDocumentsAsync(_ => true) > 0) return;

        // ── Vendors ──
        var vendorList = new List<Vendor>
        {
            new() { Name = "NeonTech Supplies", ContactEmail = "sales@neontech.io", ContactPhone = "555-0101", Address = "100 Circuit Blvd, Austin TX 78701", Status = "Active" },
            new() { Name = "ByteWare Logistics", ContactEmail = "ops@byteware.com", ContactPhone = "555-0202", Address = "250 Data Drive, Seattle WA 98101", Status = "Active" },
            new() { Name = "Quantum Parts Co", ContactEmail = "info@quantumparts.net", ContactPhone = "555-0303", Address = "77 Hadron Lane, Denver CO 80202", Status = "Active" },
            new() { Name = "CyberVault Industries", ContactEmail = "contact@cybervault.dev", ContactPhone = "555-0404", Address = "900 Firewall St, Portland OR 97201", Status = "Inactive" },
            new() { Name = "GridLine Distribution", ContactEmail = "hello@gridline.co", ContactPhone = "555-0505", Address = "42 Mesh Ave, Chicago IL 60601", Status = "Active" }
        };
        await vendors.InsertManyAsync(vendorList);

        // ── Locations ──
        var locations = database.GetCollection<Location>("Locations");
        var locationList = new List<Location>
        {
            new() { Name = "Warehouse Alpha", Address = "1000 Storage Rd, Dallas TX 75201", Type = "Warehouse", Capacity = 5000, CurrentOccupancy = 2340 },
            new() { Name = "Warehouse Beta", Address = "2000 Logistics Pkwy, Atlanta GA 30301", Type = "Warehouse", Capacity = 8000, CurrentOccupancy = 5120 },
            new() { Name = "Distribution Hub East", Address = "300 Freight Way, Newark NJ 07101", Type = "Distribution", Capacity = 3000, CurrentOccupancy = 1800 },
            new() { Name = "Retail Store Downtown", Address = "55 Main St, San Francisco CA 94101", Type = "Store", Capacity = 500, CurrentOccupancy = 320 },
            new() { Name = "Cold Storage Facility", Address = "400 Freeze Ln, Minneapolis MN 55401", Type = "Warehouse", Capacity = 2000, CurrentOccupancy = 890 }
        };
        await locations.InsertManyAsync(locationList);

        // ── Inventory Items ──
        var items = database.GetCollection<InventoryItem>("InventoryItems");
        var categories = new[] { "Electronics", "Hardware", "Software", "Networking", "Peripherals" };
        var statuses = new[] { "Available", "Reserved", "In Transit", "Depleted" };
        var shipStatuses = new[] { "None", "Pending", "Shipped", "Delivered" };

        var itemList = new List<InventoryItem>();
        var rng = new Random(42);
        for (int i = 1; i <= 25; i++)
        {
            var vendorIdx = rng.Next(vendorList.Count);
            var locIdx = rng.Next(locationList.Count);
            itemList.Add(new InventoryItem
            {
                Sku = $"SVG-{i:D4}",
                Title = $"Component {(char)('A' + (i % 26))}-{i:D3}",
                Description = $"Auto-generated inventory item #{i} for testing",
                Category = categories[i % categories.Length],
                Quantity = rng.Next(1, 500),
                Status = statuses[i % statuses.Length],
                VendorId = vendorList[vendorIdx].Id,
                IntakeDate = DateTime.UtcNow.AddDays(-rng.Next(1, 180)),
                LocationId = locationList[locIdx].Id,
                ShipmentStatus = shipStatuses[i % shipStatuses.Length],
                Notes = i % 3 == 0 ? "Flagged for review" : ""
            });
        }
        await items.InsertManyAsync(itemList);

        // ── Shipments ──
        var shipments = database.GetCollection<Shipment>("Shipments");
        var shipmentList = new List<Shipment>
        {
            new() { InventoryItemIds = new List<string> { itemList[0].Id, itemList[1].Id }, VendorId = vendorList[0].Id, Origin = locationList[0].Name, Destination = locationList[2].Name, Status = "InTransit", TrackingNumber = "TRK-100001", ShippedDate = DateTime.UtcNow.AddDays(-3), EstimatedArrival = DateTime.UtcNow.AddDays(2), Notes = "Priority shipment" },
            new() { InventoryItemIds = new List<string> { itemList[2].Id }, VendorId = vendorList[1].Id, Origin = locationList[1].Name, Destination = locationList[3].Name, Status = "Delivered", TrackingNumber = "TRK-100002", ShippedDate = DateTime.UtcNow.AddDays(-10), EstimatedArrival = DateTime.UtcNow.AddDays(-5) },
            new() { InventoryItemIds = new List<string> { itemList[3].Id, itemList[4].Id, itemList[5].Id }, VendorId = vendorList[2].Id, Origin = locationList[0].Name, Destination = locationList[4].Name, Status = "Pending", TrackingNumber = "TRK-100003" },
            new() { InventoryItemIds = new List<string> { itemList[6].Id }, VendorId = vendorList[0].Id, Origin = locationList[2].Name, Destination = locationList[1].Name, Status = "Cancelled", TrackingNumber = "TRK-100004", Notes = "Cancelled by vendor request" },
            new() { InventoryItemIds = new List<string> { itemList[7].Id, itemList[8].Id }, VendorId = vendorList[4].Id, Origin = locationList[3].Name, Destination = locationList[0].Name, Status = "InTransit", TrackingNumber = "TRK-100005", ShippedDate = DateTime.UtcNow.AddDays(-1), EstimatedArrival = DateTime.UtcNow.AddDays(4) }
        };
        await shipments.InsertManyAsync(shipmentList);

        // ── Users (admin + vendor accounts) ──
        var users = database.GetCollection<User>("Users");
        var userList = new List<User>
        {
            new() { Username = "admin", Email = "admin@savage.local", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"), Role = "Admin" },
            new() { Username = "vendor1", Email = "vendor1@neontech.io", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Vendor123!"), Role = "Vendor", VendorId = vendorList[0].Id },
            new() { Username = "vendor2", Email = "vendor2@byteware.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Vendor123!"), Role = "Vendor", VendorId = vendorList[1].Id },
            new() { Username = "vendor3", Email = "vendor3@quantumparts.net", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Vendor123!"), Role = "Vendor", VendorId = vendorList[2].Id }
        };
        await users.InsertManyAsync(userList);

        Console.WriteLine("Database seeded with fake data.");
    }
}
