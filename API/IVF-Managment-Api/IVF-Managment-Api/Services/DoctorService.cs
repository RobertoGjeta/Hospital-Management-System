using IVF_Managment_Api.Data;
using IVF_Managment_Api.Dtos;
using IVF_Managment_Api.Models;
using IvfClinic.Models;
using Microsoft.EntityFrameworkCore;

namespace IVF_Managment_Api.Services;

public class DoctorService : IDoctorService
{
    private readonly IvfDbContext _db;

    public DoctorService(IvfDbContext db) => _db = db;

    public async Task<IEnumerable<DoctorResponseDto>> GetAllAsync()
    {
        var entities = await _db.Doctors.AsNoTracking().ToListAsync();
        return entities.Select(MapToResponse);
    }

    public async Task<DoctorResponseDto?> GetByIdAsync(Guid id)
    {
        var entity = await _db.Doctors.AsNoTracking().FirstOrDefaultAsync(d => d.Id == id);
        return entity is null ? null : MapToResponse(entity);
    }

    public async Task<DoctorResponseDto> CreateAsync(CreateDoctorDto dto)
    {
        var entity = new Doctor
        {
            Id = Guid.NewGuid(),
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Username = dto.Username,
            Email = dto.Email,
            PasswordHash = HashPassword(dto.Password),
            PhoneNumber = dto.PhoneNumber,
            Role = UserRole.Doctor,
            Specialization = dto.Specialization,
            LicenseNumber = dto.LicenseNumber,
            Qualifications = dto.Qualifications,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        _db.Doctors.Add(entity);
        await _db.SaveChangesAsync();
        return MapToResponse(entity);
    }

    public async Task<DoctorResponseDto?> UpdateAsync(Guid id, UpdateDoctorDto dto)
    {
        var entity = await _db.Doctors.FindAsync(id);
        if (entity is null) return null;

        if (dto.FirstName is not null) entity.FirstName = dto.FirstName;
        if (dto.LastName is not null) entity.LastName = dto.LastName;
        if (dto.Email is not null) entity.Email = dto.Email;
        if (dto.PhoneNumber is not null) entity.PhoneNumber = dto.PhoneNumber;
        if (dto.Specialization is not null) entity.Specialization = dto.Specialization;
        if (dto.LicenseNumber is not null) entity.LicenseNumber = dto.LicenseNumber;
        if (dto.Qualifications is not null) entity.Qualifications = dto.Qualifications;

        await _db.SaveChangesAsync();
        return MapToResponse(entity);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var entity = await _db.Doctors.FindAsync(id);
        if (entity is null) return false;

        _db.Doctors.Remove(entity);
        await _db.SaveChangesAsync();
        return true;
    }

    private static DoctorResponseDto MapToResponse(Doctor e) => new()
    {
        Id = e.Id,
        FirstName = e.FirstName,
        LastName = e.LastName,
        Username = e.Username,
        Email = e.Email,
        PhoneNumber = e.PhoneNumber,
        Specialization = e.Specialization,
        LicenseNumber = e.LicenseNumber,
        Qualifications = e.Qualifications,
        CreatedAt = e.CreatedAt,
        IsActive = e.IsActive
    };

    private static string HashPassword(string password) =>
        Convert.ToBase64String(System.Security.Cryptography.SHA256.HashData(
            System.Text.Encoding.UTF8.GetBytes(password)));
}
