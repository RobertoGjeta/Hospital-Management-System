using System.Collections.Concurrent;
using IVF_Managment_Api.Dtos;
using IvfClinic.Models;

namespace IVF_Managment_Api.Services;

public class LabTechnicianService : ILabTechnicianService
{
    private static readonly ConcurrentDictionary<Guid, LabTechnician> Store = new();

    public Task<IEnumerable<LabTechnicianResponseDto>> GetAllAsync()
    {
        var result = Store.Values.Select(MapToResponse);
        return Task.FromResult(result);
    }

    public Task<LabTechnicianResponseDto?> GetByIdAsync(Guid id)
    {
        Store.TryGetValue(id, out var entity);
        return Task.FromResult(entity is null ? null : MapToResponse(entity));
    }

    public Task<LabTechnicianResponseDto> CreateAsync(CreateLabTechnicianDto dto)
    {
        var entity = new LabTechnician
        {
            Id = Guid.NewGuid(),
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Username = dto.Username,
            Email = dto.Email,
            PasswordHash = BCryptHash(dto.Password),
            PhoneNumber = dto.PhoneNumber,
            Role = UserRole.LabTechnician,
            TechnicianId = dto.TechnicianId,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        Store[entity.Id] = entity;
        return Task.FromResult(MapToResponse(entity));
    }

    public Task<LabTechnicianResponseDto?> UpdateAsync(Guid id, UpdateLabTechnicianDto dto)
    {
        if (!Store.TryGetValue(id, out var entity))
            return Task.FromResult<LabTechnicianResponseDto?>(null);

        if (dto.FirstName is not null) entity.FirstName = dto.FirstName;
        if (dto.LastName is not null) entity.LastName = dto.LastName;
        if (dto.Email is not null) entity.Email = dto.Email;
        if (dto.PhoneNumber is not null) entity.PhoneNumber = dto.PhoneNumber;
        if (dto.TechnicianId is not null) entity.TechnicianId = dto.TechnicianId;

        return Task.FromResult<LabTechnicianResponseDto?>(MapToResponse(entity));
    }

    public Task<bool> DeleteAsync(Guid id)
    {
        return Task.FromResult(Store.TryRemove(id, out _));
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

    private static string BCryptHash(string password) =>
        Convert.ToBase64String(System.Security.Cryptography.SHA256.HashData(
            System.Text.Encoding.UTF8.GetBytes(password)));
}