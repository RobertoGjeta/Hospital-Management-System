using IVF_Managment_Api.Data;
using IVF_Managment_Api.Dtos;
using IVF_Managment_Api.Models.HelperModels;
using Microsoft.EntityFrameworkCore;

namespace IVF_Managment_Api.Services;

public class ServiceCatalogService : IServiceCatalogService
{
    private readonly IvfDbContext _db;

    public ServiceCatalogService(IvfDbContext db) => _db = db;

    public async Task<IEnumerable<ClinicServiceResponseDto>> GetAllAsync()
    {
        var services = await _db.ClinicServices.AsNoTracking().ToListAsync();
        return services.Select(MapToResponse);
    }

    public async Task<ClinicServiceResponseDto?> GetByIdAsync(Guid id)
    {
        var entity = await _db.ClinicServices.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id);
        return entity is null ? null : MapToResponse(entity);
    }

    public async Task<ClinicServiceResponseDto> CreateAsync(CreateClinicServiceDto dto)
    {
        var duplicate = await _db.ClinicServices
            .AnyAsync(s => s.Name == dto.Name && s.Category == dto.Category);

        if (duplicate)
            throw new InvalidOperationException("A service with the same name already exists in this category.");

        var entity = new ClinicService
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Category = dto.Category,
            Description = dto.Description,
            Price = dto.Price,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _db.ClinicServices.Add(entity);
        await _db.SaveChangesAsync();

        return MapToResponse(entity);
    }

    public async Task<ClinicServiceResponseDto?> UpdateAsync(Guid id, UpdateClinicServiceDto dto)
    {
        var entity = await _db.ClinicServices.FindAsync(id);
        if (entity is null) return null;

        if (dto.Name is not null) entity.Name = dto.Name;
        if (dto.Category is not null) entity.Category = dto.Category;
        if (dto.Description is not null) entity.Description = dto.Description;
        if (dto.Price.HasValue) entity.Price = dto.Price.Value;

        entity.UpdatedAt = DateTime.UtcNow;

        if (dto.Name is not null || dto.Category is not null)
        {
            var duplicate = await _db.ClinicServices
                .AnyAsync(s => s.Id != id && s.Name == entity.Name && s.Category == entity.Category);

            if (duplicate)
                throw new InvalidOperationException("A service with the same name already exists in this category.");
        }

        await _db.SaveChangesAsync();

        return MapToResponse(entity);
    }

    public async Task<bool> DeactivateAsync(Guid id)
    {
        var entity = await _db.ClinicServices.FindAsync(id);
        if (entity is null) return false;

        entity.IsActive = false;
        entity.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        return true;
    }

    private static ClinicServiceResponseDto MapToResponse(ClinicService e) => new()
    {
        Id = e.Id,
        Name = e.Name,
        Category = e.Category,
        Description = e.Description,
        Price = e.Price,
        IsActive = e.IsActive,
        CreatedAt = e.CreatedAt,
        UpdatedAt = e.UpdatedAt
    };
}