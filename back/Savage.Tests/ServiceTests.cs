using MongoDB.Driver;
using Moq;
using AutoMapper;
using Savage.Api.DTOs;
using Savage.Api.Models;
using Savage.Api.Services;
using Savage.Api.Mapping;
using Microsoft.Extensions.Configuration;

namespace Savage.Tests;

/// <summary>
/// Tests for AuthService: login validation and registration logic.
/// </summary>
public class AuthServiceTests
{
    private readonly Mock<IMongoCollection<User>> _mockUsers;
    private readonly Mock<IMongoDatabase> _mockDb;
    private readonly IConfiguration _config;

    public AuthServiceTests()
    {
        _mockUsers = new Mock<IMongoCollection<User>>();
        _mockDb = new Mock<IMongoDatabase>();
        _mockDb.Setup(db => db.GetCollection<User>("Users", null)).Returns(_mockUsers.Object);

        var inMemorySettings = new Dictionary<string, string?>
        {
            { "Jwt:Key", "S4v4g3-Sup3r-S3cr3t-K3y-2026-M1n1mum-32-Ch4rs!!" },
            { "Jwt:Issuer", "SavageApi" },
            { "Jwt:Audience", "SavageClient" }
        };
        _config = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();
    }

    [Fact]
    public void BCrypt_HashAndVerify_Works()
    {
        var password = "TestPassword123!";
        var hash = BCrypt.Net.BCrypt.HashPassword(password);
        Assert.True(BCrypt.Net.BCrypt.Verify(password, hash));
        Assert.False(BCrypt.Net.BCrypt.Verify("wrong", hash));
    }

    [Fact]
    public void JWT_Config_HasRequiredKeys()
    {
        Assert.NotNull(_config["Jwt:Key"]);
        Assert.NotNull(_config["Jwt:Issuer"]);
        Assert.NotNull(_config["Jwt:Audience"]);
        Assert.True(_config["Jwt:Key"]!.Length >= 32);
    }
}

/// <summary>
/// Tests for AutoMapper profile: ensures all mappings are valid.
/// </summary>
public class MappingProfileTests
{
    [Fact]
    public void AutoMapper_Configuration_IsValid()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        config.AssertConfigurationIsValid();
    }

    [Fact]
    public void Maps_InventoryItem_To_Dto()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        var mapper = config.CreateMapper();

        var item = new InventoryItem
        {
            Id = "abc123",
            Sku = "SVG-0001",
            Title = "Test Item",
            Category = "Electronics",
            Quantity = 10,
            Status = "Available",
            ShipmentStatus = "None"
        };

        var dto = mapper.Map<InventoryItemDto>(item);
        Assert.Equal("SVG-0001", dto.Sku);
        Assert.Equal("Test Item", dto.Title);
        Assert.Equal(10, dto.Quantity);
    }

    [Fact]
    public void Maps_CreateVendorDto_To_Vendor()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        var mapper = config.CreateMapper();

        var dto = new CreateVendorDto
        {
            Name = "Test Vendor",
            ContactEmail = "test@vendor.com"
        };

        var vendor = mapper.Map<Vendor>(dto);
        Assert.Equal("Test Vendor", vendor.Name);
        Assert.Equal("test@vendor.com", vendor.ContactEmail);
    }

    [Fact]
    public void Maps_CreateShipmentDto_To_Shipment()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        var mapper = config.CreateMapper();

        var dto = new CreateShipmentDto
        {
            Origin = "Warehouse A",
            Destination = "Store B",
            TrackingNumber = "TRK-999"
        };

        var shipment = mapper.Map<Shipment>(dto);
        Assert.Equal("Warehouse A", shipment.Origin);
        Assert.Equal("TRK-999", shipment.TrackingNumber);
        Assert.Equal("Pending", shipment.Status);
    }
}

/// <summary>
/// Tests for model defaults and data integrity constraints.
/// </summary>
public class ModelTests
{
    [Fact]
    public void User_Defaults_RoleToVendor()
    {
        var user = new User();
        Assert.Equal("Vendor", user.Role);
    }

    [Fact]
    public void InventoryItem_Defaults_StatusToAvailable()
    {
        var item = new InventoryItem();
        Assert.Equal("Available", item.Status);
        Assert.Equal("None", item.ShipmentStatus);
    }

    [Fact]
    public void Shipment_Defaults_StatusToPending()
    {
        var shipment = new Shipment();
        Assert.Equal("Pending", shipment.Status);
        Assert.NotNull(shipment.InventoryItemIds);
        Assert.Empty(shipment.InventoryItemIds);
    }

    [Fact]
    public void Location_Defaults_TypeToWarehouse()
    {
        var location = new Location();
        Assert.Equal("Warehouse", location.Type);
    }

    [Fact]
    public void Vendor_Defaults_StatusToActive()
    {
        var vendor = new Vendor();
        Assert.Equal("Active", vendor.Status);
    }
}

/// <summary>
/// Tests for DTO validation: verifies create/update DTOs carry correct defaults.
/// </summary>
public class DtoTests
{
    [Fact]
    public void CreateInventoryItemDto_HasCorrectDefaults()
    {
        var dto = new CreateInventoryItemDto();
        Assert.Equal("Available", dto.Status);
        Assert.Equal("None", dto.ShipmentStatus);
        Assert.Equal(string.Empty, dto.Description);
    }

    [Fact]
    public void UpdateInventoryItemDto_AllFieldsNullable()
    {
        var dto = new UpdateInventoryItemDto();
        Assert.Null(dto.Title);
        Assert.Null(dto.Quantity);
        Assert.Null(dto.Status);
        Assert.Null(dto.VendorId);
    }

    [Fact]
    public void CreateLocationDto_DefaultsTypeToWarehouse()
    {
        var dto = new CreateLocationDto();
        Assert.Equal("Warehouse", dto.Type);
    }

    [Fact]
    public void RegisterRequest_DefaultsRoleToVendor()
    {
        var req = new RegisterRequest();
        Assert.Equal("Vendor", req.Role);
    }
}
