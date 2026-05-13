using System.Collections.Concurrent;
using IVF_Managment_Api.Dtos;
using IvfClinic.Models;

namespace IVF_Managment_Api.Services;

public class DoctorService : IDoctorService
{
    private static readonly ConcurrentDictionary<Guid, Doctor> Store = new();

    public Task<IEnumerable<DoctorResponseDto>> GetAllAsync()
    {
        var result = Store.Values.Select(MapToResponse);
        return Task.FromResult(result);
    }

    public Task<DoctorResponseDto?> GetByIdAsync(Guid id)
    {
        Store.TryGetValue(id, out var entity);
        return Task.FromResult(entity is null ? null : MapToResponse(entity));
    }

    public Task<DoctorResponseDto> CreateAsync(CreateDoctorDto dto)
    {
        var entity = new Doctor
        {
            Id = Guid.NewGuid(),
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Username = dto.Username,
            Email = dto.Email,
            PasswordHash = BCryptHash(dto.Password),
            PhoneNumber = dto.PhoneNumber,
            Role = UserRole.Doctor,
            Specialization = dto.Specialization,
            LicenseNumber = dto.LicenseNumber,
            Qualifications = dto.Qualifications,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        Store[entity.Id] = entity;
        UserCredentialStore.Register(new UserCredentialStore.UserCredential(
            entity.Id, entity.Username, entity.Email, entity.PasswordHash, entity.Role));
        return Task.FromResult(MapToResponse(entity));
    }

    public Task<DoctorResponseDto?> UpdateAsync(Guid id, UpdateDoctorDto dto)
    {
        if (!Store.TryGetValue(id, out var entity))
            return Task.FromResult<DoctorResponseDto?>(null);

        if (dto.FirstName is not null) entity.FirstName = dto.FirstName;
        if (dto.LastName is not null) entity.LastName = dto.LastName;
        if (dto.Email is not null) entity.Email = dto.Email;
        if (dto.PhoneNumber is not null) entity.PhoneNumber = dto.PhoneNumber;
        if (dto.Specialization is not null) entity.Specialization = dto.Specialization;
        if (dto.LicenseNumber is not null) entity.LicenseNumber = dto.LicenseNumber;
        if (dto.Qualifications is not null) entity.Qualifications = dto.Qualifications;

        return Task.FromResult<DoctorResponseDto?>(MapToResponse(entity));
    }

    public Task<bool> DeleteAsync(Guid id)
    {
        if (Store.TryRemove(id, out var removed))
        {
            UserCredentialStore.Remove(removed.Username, removed.Email);
            return Task.FromResult(true);
        }
        return Task.FromResult(false);
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

    private static string BCryptHash(string password) =>
        Convert.ToBase64String(System.Security.Cryptography.SHA256.HashData(
            System.Text.Encoding.UTF8.GetBytes(password)));
}