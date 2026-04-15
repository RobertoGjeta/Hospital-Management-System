using System.ComponentModel.DataAnnotations;
using IVFClinic.Models.Enums;

namespace IVFClinic.Models
{
    public class Room
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required, MaxLength(20)]
        public string Number { get; set; } = string.Empty;

        [Required]
        public RoomType Type { get; set; }

        public int Floor { get; set; }

        [Required]
        public RoomStatus Status { get; set; } = RoomStatus.Available;

        [MaxLength(500)]
        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        public ICollection<Appointment>? Appointments { get; set; }
    }
}
