using Microsoft.EntityFrameworkCore;
using IVFClinic.Data;
using IVFClinic.DTOs.Service;
using IVFClinic.Models;
using IVFClinic.Models.Enums;
using IVFClinic.Services.Interfaces;

namespace IVFClinic.Services.Implementations
{
    public class ServiceCatalog : IServiceCatalog
    {
        private readonly AppDbContext _db;
        private readonly IAuditService _audit;

        public ServiceCatalog(AppDbContext db, IAuditService audit)
        {
            _db = db;
            _audit = audit;
        }

        public async Task<ServiceResponseDto> AddServiceAsync(ServiceCreateDto dto, Guid adminId)
        {
            // Duplicate name check within category (FR_A10)
            var existing = await _db.ClinicServices.AnyAsync(s =>
                s.Name == dto.Name && s.Category == dto.Category);
            if (existing)
            {
                throw new InvalidOperationException($"Service '{dto.Name}' already exists in category '{dto.Category}'.");
            }

            if (dto.Price <= 0)
            {
                throw new ArgumentException("Price must be a positive number.");
            }

            var service = new ClinicService
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Category = dto.Category,
                Description = dto.Description,
                Price = dto.Price,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _db.ClinicServices.Add(service);
            await _db.SaveChangesAsync();

            await _audit.LogActionAsync(adminId, AuditAction.Create, "Service",
                service.Id.ToString(), newValues: dto);

            return MapToDto(service);
        }

        public async Task<ServiceResponseDto> UpdateServiceAsync(Guid serviceId, ServiceUpdateDto dto, Guid adminId)
        {
            var service = await _db.ClinicServices.FindAsync(serviceId)
                ?? throw new KeyNotFoundException($"Service {serviceId} not found");

            var previousValues = new { service.Name, service.Description, service.Price, service.Category };

            // Track price changes in history
            if (dto.Price.HasValue && dto.Price.Value != service.Price)
            {
                _db.PriceHistory.Add(new PriceHistoryEntry
                {
                    ServiceId = serviceId,
                    OldPrice = service.Price,
                    NewPrice = dto.Price.Value,
                    ChangedById = adminId,
                    ChangedAt = DateTime.UtcNow
                });
                service.Price = dto.Price.Value;
            }

            if (!string.IsNullOrEmpty(dto.Name)) service.Name = dto.Name;
            if (dto.Description != null) service.Description = dto.Description;
            if (!string.IsNullOrEmpty(dto.Category)) service.Category = dto.Category;

            service.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            await _audit.LogActionAsync(adminId, AuditAction.Update, "Service",
                service.Id.ToString(), previousValues: previousValues, newValues: dto);

            return MapToDto(service);
        }

        public async Task<bool> DeactivateServiceAsync(Guid serviceId, Guid adminId, bool forceConfirm = false)
        {
            var service = await _db.ClinicServices.FindAsync(serviceId);
            if (service == null) return false;

            // Warn if billed in last 30 days
            if (!forceConfirm)
            {
                var thirtyDaysAgo = DateTime.UtcNow.AddDays(-30);
                var recentlyBilled = await _db.InvoiceItems
                    .Where(ii => ii.ServiceId == serviceId && ii.Invoice!.CreatedAt >= thirtyDaysAgo)
                    .AnyAsync();

                if (recentlyBilled)
                {
                    throw new InvalidOperationException(
                        "Service has been billed in the last 30 days. Confirm deactivation explicitly.");
                }
            }

            service.IsActive = false;
            service.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            await _audit.LogActionAsync(adminId, AuditAction.Deactivate, "Service",
                service.Id.ToString());
            return true;
        }

        public async Task<bool> ReactivateServiceAsync(Guid serviceId, Guid adminId)
        {
            var service = await _db.ClinicServices.FindAsync(serviceId);
            if (service == null) return false;

            service.IsActive = true;
            service.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            await _audit.LogActionAsync(adminId, AuditAction.Update, "Service",
                service.Id.ToString(), description: "Service reactivated");
            return true;
        }

        public async Task<ServiceResponseDto?> GetServiceByIdAsync(Guid serviceId)
        {
            var service = await _db.ClinicServices.FindAsync(serviceId);
            return service == null ? null : MapToDto(service);
        }

        public async Task<IEnumerable<ServiceResponseDto>> GetAllServicesAsync(bool includeInactive = false)
        {
            var query = _db.ClinicServices.AsQueryable();
            if (!includeInactive) query = query.Where(s => s.IsActive);

            var services = await query.OrderBy(s => s.Category).ThenBy(s => s.Name).ToListAsync();
            return services.Select(MapToDto);
        }

        public async Task<IEnumerable<ServiceResponseDto>> GetServicesByCategoryAsync(string category)
        {
            var services = await _db.ClinicServices
                .Where(s => s.Category == category && s.IsActive)
                .OrderBy(s => s.Name)
                .ToListAsync();
            return services.Select(MapToDto);
        }

        public async Task<IEnumerable<PriceHistoryEntry>> GetPriceHistoryAsync(Guid serviceId)
        {
            return await _db.PriceHistory
                .Where(p => p.ServiceId == serviceId)
                .OrderByDescending(p => p.ChangedAt)
                .ToListAsync();
        }

        private static ServiceResponseDto MapToDto(ClinicService s) => new()
        {
            Id = s.Id,
            Name = s.Name,
            Category = s.Category,
            Description = s.Description,
            Price = s.Price,
            IsActive = s.IsActive,
            CreatedAt = s.CreatedAt
        };
    }
}
