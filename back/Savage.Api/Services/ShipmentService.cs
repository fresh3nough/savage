using AutoMapper;
using MongoDB.Driver;
using Savage.Api.DTOs;
using Savage.Api.Models;

namespace Savage.Api.Services;

/// <summary>
/// Provides CRUD operations for shipments. Supports vendor-scoped filtering
/// so vendors only see their own shipments.
/// </summary>
public class ShipmentService : IShipmentService
{
    private readonly IMongoCollection<Shipment> _shipments;
    private readonly IMapper _mapper;

    public ShipmentService(IMongoDatabase database, IMapper mapper)
    {
        _shipments = database.GetCollection<Shipment>("Shipments");
        _mapper = mapper;
    }

    /// <summary>
    /// Returns all shipments, optionally filtered by vendorId for vendor-scoped access.
    /// </summary>
    public async Task<List<ShipmentDto>> GetAllAsync(string? vendorId = null)
    {
        var filter = string.IsNullOrEmpty(vendorId)
            ? Builders<Shipment>.Filter.Empty
            : Builders<Shipment>.Filter.Eq(s => s.VendorId, vendorId);

        var shipments = await _shipments.Find(filter).ToListAsync();
        return _mapper.Map<List<ShipmentDto>>(shipments);
    }

    public async Task<ShipmentDto?> GetByIdAsync(string id)
    {
        var shipment = await _shipments.Find(s => s.Id == id).FirstOrDefaultAsync();
        return shipment == null ? null : _mapper.Map<ShipmentDto>(shipment);
    }

    public async Task<ShipmentDto> CreateAsync(CreateShipmentDto dto)
    {
        var shipment = _mapper.Map<Shipment>(dto);
        await _shipments.InsertOneAsync(shipment);
        return _mapper.Map<ShipmentDto>(shipment);
    }

    /// <summary>
    /// Partially updates shipment details (status, tracking, dates).
    /// </summary>
    public async Task<ShipmentDto?> UpdateAsync(string id, UpdateShipmentDto dto)
    {
        var existing = await _shipments.Find(s => s.Id == id).FirstOrDefaultAsync();
        if (existing == null) return null;

        if (dto.Status != null) existing.Status = dto.Status;
        if (dto.TrackingNumber != null) existing.TrackingNumber = dto.TrackingNumber;
        if (dto.ShippedDate.HasValue) existing.ShippedDate = dto.ShippedDate;
        if (dto.EstimatedArrival.HasValue) existing.EstimatedArrival = dto.EstimatedArrival;
        if (dto.Notes != null) existing.Notes = dto.Notes;
        if (dto.Destination != null) existing.Destination = dto.Destination;

        await _shipments.ReplaceOneAsync(s => s.Id == id, existing);
        return _mapper.Map<ShipmentDto>(existing);
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var result = await _shipments.DeleteOneAsync(s => s.Id == id);
        return result.DeletedCount > 0;
    }
}
