using IvfClinic.Models;

namespace IVF_Managment_Api.Dtos;

public class NotificationResponseDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public NotificationType Type { get; set; }
    public string Title { get; set; }
    public string Body { get; set; }
    public bool IsRead { get; set; }
    public NotificationChannel Channel { get; set; }
    public DateTime CreatedAt { get; set; }
}