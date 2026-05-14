using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IVF_Managment_Api.Models.HelperModels;

[Table("AuditLogEntries")]
public class AuditLogEntry
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public Guid UserId { get; set; }

    [Required]
    [MaxLength(100)]
    public string Action { get; set; }

    [Required]
    [MaxLength(100)]
    public string EntityType { get; set; }

    public Guid? EntityId { get; set; }

    public string? BeforeJson { get; set; }
    public string? AfterJson { get; set; }

    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}