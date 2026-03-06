using AutoMapper;
using Savage.Api.Models;
using Savage.Api.DTOs;

namespace Savage.Api.Mapping;

/// <summary>
/// Configures AutoMapper mappings between domain models and DTOs.
/// </summary>
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // User
        CreateMap<User, UserDto>();

        // Inventory
        CreateMap<InventoryItem, InventoryItemDto>();
        CreateMap<CreateInventoryItemDto, InventoryItem>()
            .ForMember(d => d.Id, opt => opt.Ignore())
            .ForMember(d => d.IntakeDate, opt => opt.Ignore());

        // Vendor
        CreateMap<Vendor, VendorDto>();
        CreateMap<CreateVendorDto, Vendor>()
            .ForMember(d => d.Id, opt => opt.Ignore())
            .ForMember(d => d.Status, opt => opt.Ignore())
            .ForMember(d => d.CreatedAt, opt => opt.Ignore());

        // Location
        CreateMap<Location, LocationDto>();
        CreateMap<CreateLocationDto, Location>()
            .ForMember(d => d.Id, opt => opt.Ignore())
            .ForMember(d => d.CurrentOccupancy, opt => opt.Ignore());

        // Shipment
        CreateMap<Shipment, ShipmentDto>();
        CreateMap<CreateShipmentDto, Shipment>()
            .ForMember(d => d.Id, opt => opt.Ignore())
            .ForMember(d => d.Status, opt => opt.Ignore())
            .ForMember(d => d.ShippedDate, opt => opt.Ignore())
            .ForMember(d => d.EstimatedArrival, opt => opt.Ignore());
    }
}
