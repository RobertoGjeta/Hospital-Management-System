using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IVFClinic.Models
{
    /// <summary>
    /// Represents a weekly recurring availability slot for a doctor.
    /// Example: Monday 09:00–17:00.
    /// </summary>
    public class DoctorAvailability
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid DoctorId { get; set; }

        [ForeignKey(nameof(DoctorId))]
        public Doctor? Doctor { get; set; }

        public DayOfWeek DayOfWeek { get; set; }

        public TimeOnly StartTime { get; set; }

        public TimeOnly EndTime { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
