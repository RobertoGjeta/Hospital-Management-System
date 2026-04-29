using System.Collections.Concurrent;
using IVF_Managment_Api.Dtos;
using IVF_Managment_Api.Models;
using IvfClinic.Models;

namespace IVF_Managment_Api.Services;

public class AdministratorService : IAdministratorService
{
    private static readonly ConcurrentDictionary<Guid, Administrator> Store = new();

    public Task<IEnumerable<AdministratorResponseDto>> GetAllAsync()
    {
        var result = Store.Values.Select(MapToResponse);
        return Task.FromResult(result);
    }

    public Task<AdministratorResponseDto?> GetByIdAsync(Guid id)
    {
        Store.TryGetValue(id, out var entity);
        return Task.FromResult(entity is null ? null : MapToResponse(entity));
    }

    public Task<AdministratorResponseDto> CreateAsync(CreateAdministratorDto dto)
    {
        var entity = new Administrator
        {
            Id = Guid.NewGuid(),
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Username = dto.Username,
            Email = dto.Email,
            PasswordHash = BCryptHash(dto.Password),
            PhoneNumber = dto.PhoneNumber,
            Role = UserRole.Administrator,
            Department = dto.Department,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        Store[entity.Id] = entity;
        return Task.FromResult(MapToResponse(entity));
    }

    public Task<AdministratorResponseDto?> UpdateAsync(Guid id, UpdateAdministratorDto dto)
    {
        if (!Store.TryGetValue(id, out var entity))
            return Task.FromResult<AdministratorResponseDto?>(null);

        if (dto.FirstName is not null) entity.FirstName = dto.FirstName;
        if (dto.LastName is not null) entity.LastName = dto.LastName;
        if (dto.Email is not null) entity.Email = dto.Email;
        if (dto.PhoneNumber is not null) entity.PhoneNumber = dto.PhoneNumber;
        if (dto.Department is not null) entity.Department = dto.Department;

        return Task.FromResult<AdministratorResponseDto?>(MapToResponse(entity));
    }

    public Task<bool> DeleteAsync(Guid id)
    {
        return Task.FromResult(Store.TryRemove(id, out _));
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

    private static string BCryptHash(string password) =>
        Convert.ToBase64String(System.Security.Cryptography.SHA256.HashData(
            System.Text.Encoding.UTF8.GetBytes(password)));
}