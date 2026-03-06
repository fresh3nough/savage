using AutoMapper;
using MongoDB.Driver;
using Savage.Api.DTOs;
using Savage.Api.Models;

namespace Savage.Api.Services;

/// <summary>
/// Provides CRUD operations for storage/distribution locations.
/// </summary>
public class LocationService : ILocationService
{
    private readonly IMongoCollection<Location> _locations;
    private readonly IMapper _mapper;

    public LocationService(IMongoDatabase database, IMapper mapper)
    {
        _locations = database.GetCollection<Location>("Locations");
        _mapper = mapper;
    }

    public async Task<List<LocationDto>> GetAllAsync()
    {
        var locations = await _locations.Find(_ => true).ToListAsync();
        return _mapper.Map<List<LocationDto>>(locations);
    }

    public async Task<LocationDto?> GetByIdAsync(string id)
    {
        var location = await _locations.Find(l => l.Id == id).FirstOrDefaultAsync();
        return location == null ? null : _mapper.Map<LocationDto>(location);
    }

    public async Task<LocationDto> CreateAsync(CreateLocationDto dto)
    {
        var location = _mapper.Map<Location>(dto);
        await _locations.InsertOneAsync(location);
        return _mapper.Map<LocationDto>(location);
    }

    public async Task<LocationDto?> UpdateAsync(string id, UpdateLocationDto dto)
    {
        var existing = await _locations.Find(l => l.Id == id).FirstOrDefaultAsync();
        if (existing == null) return null;

        if (dto.Name != null) existing.Name = dto.Name;
        if (dto.Address != null) existing.Address = dto.Address;
        if (dto.Type != null) existing.Type = dto.Type;
        if (dto.Capacity.HasValue) existing.Capacity = dto.Capacity.Value;
        if (dto.CurrentOccupancy.HasValue) existing.CurrentOccupancy = dto.CurrentOccupancy.Value;

        await _locations.ReplaceOneAsync(l => l.Id == id, existing);
        return _mapper.Map<LocationDto>(existing);
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var result = await _locations.DeleteOneAsync(l => l.Id == id);
        return result.DeletedCount > 0;
    }
}
