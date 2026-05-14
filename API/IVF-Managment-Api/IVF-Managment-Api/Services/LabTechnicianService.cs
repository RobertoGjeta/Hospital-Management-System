using IVF_Managment_Api.Data;
using IVF_Managment_Api.Dtos;
using IVF_Managment_Api.Models;
using IvfClinic.Models;
using Microsoft.EntityFrameworkCore;

namespace IVF_Managment_Api.Services;

public class LabTechnicianService : ILabTechnicianService
{
    private readonly IvfDbContext _db;
    private readonly IPasswordHasher _hasher;

    public LabTechnicianService(IvfDbContext db, IPasswordHasher hasher)
    {
        _db = db;
        _hasher = hasher;
    }

    public async Task<IEnumerable<LabTechnicianResponseDto>> GetAllAsync()
    {
        var entities = await _db.LabTechnicians.AsNoTracking().ToListAsync();
        return entities.Select(MapToResponse);
    }

    public async Task<LabTechnicianResponseDto?> GetByIdAsync(Guid id)
    {
        var entity = await _db.LabTechnicians.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id);
        return entity is null ? null : MapToResponse(entity);
    }

    public async Task<LabTechnicianResponseDto> CreateAsync(CreateLabTechnicianDto dto)
    {
        var entity = new LabTechnician
        {
            Id = Guid.NewGuid(),
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Username = dto.Username,
            Email = dto.Email,
            PasswordHash = _hasher.Hash(dto.Password),
            PhoneNumber = dto.PhoneNumber,
            Role = UserRole.LabTechnician,
            TechnicianId = dto.TechnicianId,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        _db.LabTechnicians.Add(entity);
        await _db.SaveChangesAsync();
        return MapToResponse(entity);
    }

    public async Task<LabTechnicianResponseDto?> UpdateAsync(Guid id, UpdateLabTechnicianDto dto)
    {
        var entity = await _db.LabTechnicians.FindAsync(id);
        if (entity is null) return null;

        if (dto.FirstName is not null) entity.FirstName = dto.FirstName;
        if (dto.LastName is not null) entity.LastName = dto.LastName;
        if (dto.Email is not null) entity.Email = dto.Email;
        if (dto.PhoneNumber is not null) entity.PhoneNumber = dto.PhoneNumber;
        if (dto.TechnicianId is not null) entity.TechnicianId = dto.TechnicianId;

        await _db.SaveChangesAsync();
        return MapToResponse(entity);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var entity = await _db.LabTechnicians.FindAsync(id);
        if (entity is null) return false;

        _db.LabTechnicians.Remove(entity);
        await _db.SaveChangesAsync();
        return true;
    }

    private static LabTechnicianResponseDto MapToResponse(LabTechnician e) => new()
    {
        Id = e.Id,
        FirstName = e.FirstName,
        LastName = e.LastName,
        Username = e.Username,
        Email = e.Email,
        PhoneNumber = e.PhoneNumber,
        TechnicianId = e.TechnicianId,
        CreatedAt = e.CreatedAt,
        IsActive = e.IsActive
    };

}
