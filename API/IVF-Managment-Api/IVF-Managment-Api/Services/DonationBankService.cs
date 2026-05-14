using IVF_Managment_Api.Data;
using IVF_Managment_Api.Dtos;
using IVF_Managment_Api.Models.HelperModels;
using IvfClinic.Models;
using Microsoft.EntityFrameworkCore;

namespace IVF_Managment_Api.Services;

public class DonationBankService : IDonationBankService
{
    private readonly IvfDbContext _db;
    private readonly INotificationService _notifications;
    private const int LowStockThreshold = 5;

    public DonationBankService(IvfDbContext db, INotificationService notifications)
    {
        _db = db;
        _notifications = notifications;
    }

    public async Task<DonationSampleResponseDto> CreateSampleAsync(CreateDonationSampleDto dto)
    {
        var entity = new DonationSample
        {
            Id = Guid.NewGuid(),
            DonorId = dto.DonorId,
            Type = dto.Type,
            Quantity = dto.Quantity,
            CollectionDate = dto.CollectionDate,
            ExpiryDate = dto.ExpiryDate,
            StorageLocation = dto.StorageLocation,
            ScreeningStatus = ScreeningStatus.Pending,
            IsAssignable = true,
            CreatedAt = DateTime.UtcNow
        };

        _db.DonationSamples.Add(entity);
        await _db.SaveChangesAsync();

        return MapToResponse(entity);
    }

    public async Task<IEnumerable<DonationSampleResponseDto>> GetByDonorAsync(Guid donorId)
    {
        var entities = await _db.DonationSamples
            .AsNoTracking()
            .Where(d => d.DonorId == donorId)
            .OrderByDescending(d => d.CreatedAt)
            .ToListAsync();

        return entities.Select(MapToResponse);
    }

    public async Task<IEnumerable<DonationSampleResponseDto>> GetAssignableAsync()
    {
        var entities = await _db.DonationSamples
            .AsNoTracking()
            .Where(d => d.IsAssignable &&
                        d.ScreeningStatus == ScreeningStatus.Cleared &&
                        d.ExpiryDate > DateTime.UtcNow)
            .ToListAsync();

        return entities.Select(MapToResponse);
    }

    public async Task<DonationSampleResponseDto?> UpdateScreeningAsync(Guid sampleId, UpdateScreeningDto dto)
    {
        var entity = await _db.DonationSamples.FindAsync(sampleId);
        if (entity is null) return null;

        entity.ScreeningStatus = dto.Status;
        entity.ScreeningReason = dto.Reason;
        entity.UpdatedAt = DateTime.UtcNow;

        if (dto.Status == ScreeningStatus.Rejected)
            entity.IsAssignable = false;

        await _db.SaveChangesAsync();

        return MapToResponse(entity);
    }

    public async Task<DonationSampleResponseDto?> UpdateQuantityAsync(Guid sampleId, int newQuantity)
    {
        var entity = await _db.DonationSamples.FindAsync(sampleId);
        if (entity is null) return null;

        entity.Quantity = newQuantity;
        entity.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        if (newQuantity <= LowStockThreshold)
            await NotifyLowStockAsync(entity);

        return MapToResponse(entity);
    }

    private async Task NotifyLowStockAsync(DonationSample sample)
    {
        var recipientIds = await _db.Users
            .AsNoTracking()
            .Where(u => u.IsActive && (u.Role == UserRole.LabTechnician || u.Role == UserRole.Administrator))
            .Select(u => u.Id)
            .ToListAsync();

        var title = "Low Donation Stock Alert";
        var body = $"Sample {sample.Id} ({sample.Type}) is at {sample.Quantity} units in {sample.StorageLocation}.";
        var channels = new[] { NotificationChannel.InApp };

        foreach (var userId in recipientIds)
            await _notifications.NotifyAsync(userId, NotificationType.General, title, body, channels);
    }

    private static DonationSampleResponseDto MapToResponse(DonationSample e) => new()
    {
        Id = e.Id,
        DonorId = e.DonorId,
        Type = e.Type,
        Quantity = e.Quantity,
        CollectionDate = e.CollectionDate,
        ExpiryDate = e.ExpiryDate,
        StorageLocation = e.StorageLocation,
        ScreeningStatus = e.ScreeningStatus,
        ScreeningReason = e.ScreeningReason,
        IsAssignable = e.IsAssignable,
        CreatedAt = e.CreatedAt
    };
}