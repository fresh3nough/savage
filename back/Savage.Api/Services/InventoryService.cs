using AutoMapper;
using MongoDB.Driver;
using Savage.Api.DTOs;
using Savage.Api.Models;

namespace Savage.Api.Services;

/// <summary>
/// Provides CRUD operations for inventory items with optional category/status/vendor filtering and sorting.
/// </summary>
public class InventoryService : IInventoryService
{
    private readonly IMongoCollection<InventoryItem> _items;
    private readonly IMapper _mapper;

    public InventoryService(IMongoDatabase database, IMapper mapper)
    {
        _items = database.GetCollection<InventoryItem>("InventoryItems");
        _mapper = mapper;
    }

    /// <summary>
    /// Returns all inventory items, optionally filtered by category, status, vendorId, and sorted.
    /// </summary>
    public async Task<List<InventoryItemDto>> GetAllAsync(string? category = null, string? status = null, string? vendorId = null, string? sortBy = null)
    {
        var builder = Builders<InventoryItem>.Filter;
        var filter = builder.Empty;

        if (!string.IsNullOrEmpty(category))
            filter &= builder.Eq(i => i.Category, category);
        if (!string.IsNullOrEmpty(status))
            filter &= builder.Eq(i => i.Status, status);
        if (!string.IsNullOrEmpty(vendorId))
            filter &= builder.Eq(i => i.VendorId, vendorId);

        var query = _items.Find(filter);

        // Apply optional sort
        query = sortBy?.ToLower() switch
        {
            "title" => query.SortBy(i => i.Title),
            "quantity" => query.SortBy(i => i.Quantity),
            "date" => query.SortByDescending(i => i.IntakeDate),
            "sku" => query.SortBy(i => i.Sku),
            _ => query.SortByDescending(i => i.IntakeDate)
        };

        var items = await query.ToListAsync();
        return _mapper.Map<List<InventoryItemDto>>(items);
    }

    public async Task<InventoryItemDto?> GetByIdAsync(string id)
    {
        var item = await _items.Find(i => i.Id == id).FirstOrDefaultAsync();
        return item == null ? null : _mapper.Map<InventoryItemDto>(item);
    }

    public async Task<InventoryItemDto> CreateAsync(CreateInventoryItemDto dto)
    {
        var item = _mapper.Map<InventoryItem>(dto);
        await _items.InsertOneAsync(item);
        return _mapper.Map<InventoryItemDto>(item);
    }

    /// <summary>
    /// Partially updates an inventory item. Only non-null fields in the DTO are applied.
    /// </summary>
    public async Task<InventoryItemDto?> UpdateAsync(string id, UpdateInventoryItemDto dto)
    {
        var existing = await _items.Find(i => i.Id == id).FirstOrDefaultAsync();
        if (existing == null) return null;

        if (dto.Title != null) existing.Title = dto.Title;
        if (dto.Description != null) existing.Description = dto.Description;
        if (dto.Category != null) existing.Category = dto.Category;
        if (dto.Quantity.HasValue) existing.Quantity = dto.Quantity.Value;
        if (dto.Status != null) existing.Status = dto.Status;
        if (dto.VendorId != null) existing.VendorId = dto.VendorId;
        if (dto.LocationId != null) existing.LocationId = dto.LocationId;
        if (dto.ShipmentStatus != null) existing.ShipmentStatus = dto.ShipmentStatus;
        if (dto.Notes != null) existing.Notes = dto.Notes;

        await _items.ReplaceOneAsync(i => i.Id == id, existing);
        return _mapper.Map<InventoryItemDto>(existing);
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var result = await _items.DeleteOneAsync(i => i.Id == id);
        return result.DeletedCount > 0;
    }
}
