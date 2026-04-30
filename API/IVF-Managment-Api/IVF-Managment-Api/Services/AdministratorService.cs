using IVF_Managment_Api.Data;
using IVF_Managment_Api.Dtos;
using IVF_Managment_Api.Models;
using IvfClinic.Models;
using Microsoft.EntityFrameworkCore;

namespace IVF_Managment_Api.Services;

public class AdministratorService : IAdministratorService
{
    private readonly IvfDbContext _db;

    public AdministratorService(IvfDbContext db) => _db = db;

    public async Task<IEnumerable<AdministratorResponseDto>> GetAllAsync()
    {
        var entities = await _db.Administrators.AsNoTracking().ToListAsync();
        return entities.Select(MapToResponse);
    }

    public async Task<AdministratorResponseDto?> GetByIdAsync(Guid id)
    {
        var entity = await _db.Administrators.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id);
        return entity is null ? null : MapToResponse(entity);
    }

    public async Task<AdministratorResponseDto> CreateAsync(CreateAdministratorDto dto)
    {
        var entity = new Administrator
        {
            Id = Guid.NewGuid(),
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Username = dto.Username,
            Email = dto.Email,
            PasswordHash = HashPassword(dto.Password),
            PhoneNumber = dto.PhoneNumber,
            Role = UserRole.Administrator,
            Department = dto.Department,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        _db.Administrators.Add(entity);
        await _db.SaveChangesAsync();
        return MapToResponse(entity);
    }

    public async Task<AdministratorResponseDto?> UpdateAsync(Guid id, UpdateAdministratorDto dto)
    {
        var entity = await _db.Administrators.FindAsync(id);
        if (entity is null) return null;

        if (dto.FirstName is not null) entity.FirstName = dto.FirstName;
        if (dto.LastName is not null) entity.LastName = dto.LastName;
        if (dto.Email is not null) entity.Email = dto.Email;
        if (dto.PhoneNumber is not null) entity.PhoneNumber = dto.PhoneNumber;
        if (dto.Department is not null) entity.Department = dto.Department;

        await _db.SaveChangesAsync();
        return MapToResponse(entity);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var entity = await _db.Administrators.FindAsync(id);
        if (entity is null) return false;

        _db.Administrators.Remove(entity);
        await _db.SaveChangesAsync();
        return true;
    }

    private static AdministratorResponseDto MapToResponse(Administrator e) => new()
    {
        Id = e.Id,
        FirstName = e.FirstName,
        LastName = e.LastName,
        Username = e.Username,
        Email = e.Email,
        PhoneNumber = e.PhoneNumber,
        Department = e.Department,
        CreatedAt = e.CreatedAt,
        IsActive = e.IsActive
    };

    private static string HashPassword(string password) =>
        Convert.ToBase64String(System.Security.Cryptography.SHA256.HashData(
            System.Text.Encoding.UTF8.GetBytes(password)));
}
