using AutoMapper;
using MongoDB.Driver;
using Savage.Api.DTOs;
using Savage.Api.Models;

namespace Savage.Api.Services;

/// <summary>
/// Provides CRUD operations for vendor profiles stored in MongoDB.
/// </summary>
public class VendorService : IVendorService
{
    private readonly IMongoCollection<Vendor> _vendors;
    private readonly IMapper _mapper;

    public VendorService(IMongoDatabase database, IMapper mapper)
    {
        _vendors = database.GetCollection<Vendor>("Vendors");
        _mapper = mapper;
    }

    public async Task<List<VendorDto>> GetAllAsync()
    {
        var vendors = await _vendors.Find(_ => true).ToListAsync();
        return _mapper.Map<List<VendorDto>>(vendors);
    }

    public async Task<VendorDto?> GetByIdAsync(string id)
    {
        var vendor = await _vendors.Find(v => v.Id == id).FirstOrDefaultAsync();
        return vendor == null ? null : _mapper.Map<VendorDto>(vendor);
    }

    public async Task<VendorDto> CreateAsync(CreateVendorDto dto)
    {
        var vendor = _mapper.Map<Vendor>(dto);
        await _vendors.InsertOneAsync(vendor);
        return _mapper.Map<VendorDto>(vendor);
    }

    public async Task<VendorDto?> UpdateAsync(string id, UpdateVendorDto dto)
    {
        var existing = await _vendors.Find(v => v.Id == id).FirstOrDefaultAsync();
        if (existing == null) return null;

        if (dto.Name != null) existing.Name = dto.Name;
        if (dto.ContactEmail != null) existing.ContactEmail = dto.ContactEmail;
        if (dto.ContactPhone != null) existing.ContactPhone = dto.ContactPhone;
        if (dto.Address != null) existing.Address = dto.Address;
        if (dto.Status != null) existing.Status = dto.Status;

        await _vendors.ReplaceOneAsync(v => v.Id == id, existing);
        return _mapper.Map<VendorDto>(existing);
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var result = await _vendors.DeleteOneAsync(v => v.Id == id);
        return result.DeletedCount > 0;
    }
}
