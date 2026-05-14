using IVF_Managment_Api.Data;
using IVF_Managment_Api.Dtos;
using IVF_Managment_Api.Models.HelperModels;
using IvfClinic.Models;
using Microsoft.EntityFrameworkCore;

namespace IVF_Managment_Api.Services;

public class NotificationService : INotificationService
{
    private readonly IvfDbContext _db;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(IvfDbContext db, ILogger<NotificationService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task NotifyAsync(Guid userId, NotificationType type, string title, string body, IEnumerable<NotificationChannel> channels)
    {
        foreach (var channel in channels)
        {
            if (channel == NotificationChannel.InApp)
            {
                var notification = new Notification
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    Type = type,
                    Title = title,
                    Body = body,
                    IsRead = false,
                    Channel = channel,
                    CreatedAt = DateTime.UtcNow
                };
                _db.Notifications.Add(notification);
            }
            else
            {
                // TODO: integrate provider
                _logger.LogInformation("[{Channel}] Notification to {UserId}: {Title} - {Body}", channel, userId, title, body);
            }
        }

        await _db.SaveChangesAsync();
    }

    public async Task<IEnumerable<NotificationResponseDto>> GetUnreadAsync(Guid userId)
    {
        var entities = await _db.Notifications
            .AsNoTracking()
            .Where(n => n.UserId == userId && !n.IsRead)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();

        return entities.Select(MapToResponse);
    }

    public async Task<bool> MarkAsReadAsync(Guid notificationId)
    {
        var entity = await _db.Notifications.FindAsync(notificationId);
        if (entity is null) return false;

        entity.IsRead = true;
        await _db.SaveChangesAsync();
        return true;
    }

    private static NotificationResponseDto MapToResponse(Notification e) => new()
    {
        Id = e.Id,
        UserId = e.UserId,
        Type = e.Type,
        Title = e.Title,
        Body = e.Body,
        IsRead = e.IsRead,
        Channel = e.Channel,
        CreatedAt = e.CreatedAt
    };
}