using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IVFClinic.Models
{
    public class ClinicService
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required, MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        public string Category { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        [Required, Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        public ICollection<PriceHistoryEntry>? PriceHistory { get; set; }
        public ICollection<RenderedService>? RenderedServices { get; set; }
    }

    /// <summary>
    /// A service that has actually been delivered to a patient.
    /// Gets marked as IsBilled = true when included in an invoice.
    /// </summary>
    public class RenderedService
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid PatientId { get; set; }
        [ForeignKey(nameof(PatientId))]
        public Patient? Patient { get; set; }

        [Required]
        public Guid ServiceId { get; set; }
        [ForeignKey(nameof(ServiceId))]
        public ClinicService? Service { get; set; }

        public Guid? AppointmentId { get; set; }
        [ForeignKey(nameof(AppointmentId))]
        public Appointment? Appointment { get; set; }

        public Guid? DoctorId { get; set; }
        [ForeignKey(nameof(DoctorId))]
        public Doctor? Doctor { get; set; }

        public int Quantity { get; set; } = 1;

        public DateTime RenderedAt { get; set; } = DateTime.UtcNow;

        public bool IsBilled { get; set; } = false;

        [MaxLength(500)]
        public string? Notes { get; set; }
    }

    public class PriceHistoryEntry
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid ServiceId { get; set; }
        [ForeignKey(nameof(ServiceId))]
        public ClinicService? Service { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal OldPrice { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal NewPrice { get; set; }

        public Guid ChangedById { get; set; }
        [ForeignKey(nameof(ChangedById))]
        public User? ChangedBy { get; set; }

        public DateTime ChangedAt { get; set; } = DateTime.UtcNow;

        [MaxLength(500)]
        public string? Reason { get; set; }
    }
}
