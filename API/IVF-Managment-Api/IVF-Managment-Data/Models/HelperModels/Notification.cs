using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using IvfClinic.Models;

namespace IVF_Managment_Api.Models.HelperModels;

[Table("Notifications")]
public class Notification
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public Guid UserId { get; set; }

    [Required]
    public NotificationType Type { get; set; }

    [Required]
    [MaxLength(200)]
    public string Title { get; set; }

    [Required]
    public string Body { get; set; }

    public bool IsRead { get; set; }

    [Required]
    public NotificationChannel Channel { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}