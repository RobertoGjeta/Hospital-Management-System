using IVF_Managment_Api.Dtos;
using IvfClinic.Models;

namespace IVF_Managment_Api.Services;

public interface INotificationService
{
    Task NotifyAsync(Guid userId, NotificationType type, string title, string body, IEnumerable<NotificationChannel> channels);
    Task<IEnumerable<NotificationResponseDto>> GetUnreadAsync(Guid userId);
    Task<bool> MarkAsReadAsync(Guid notificationId);
}