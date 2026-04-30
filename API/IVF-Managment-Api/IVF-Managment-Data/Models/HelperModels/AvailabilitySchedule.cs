using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IVF_Managment_Api.Models.HelperModels;

[Table("AvailabilitySchedules")]
public class AvailabilitySchedule
{
    [Key]
    public Guid Id { get; set; }

    // Links back to the specific Doctor
    [Required]
    public Guid DoctorId { get; set; }
    public virtual Doctor Doctor { get; set; }

    [Required]
    public DayOfWeek DayOfWeek { get; set; }

    [Required]
    public TimeSpan StartTime { get; set; }

    [Required]
    public TimeSpan EndTime { get; set; }
}