using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using IvfClinic.Models;

namespace IVF_Managment_Api.Models.HelperModels;


    [Table("Appointments")]
    public class Appointment
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid PatientId { get; set; }
        public virtual Patient Patient { get; set; }

        [Required]
        public Guid DoctorId { get; set; }
        public virtual Doctor Doctor { get; set; }

        public Guid? RoomId { get; set; }
        public virtual Room AssignedRoom { get; set; }

        [Required]
        public DateTime ScheduledAt { get; set; }
        public int DurationMinutes { get; set; }

        [Required]
        [MaxLength(100)]
        public string ServiceType { get; set; }

        public AppointmentStatus Status { get; set; } = AppointmentStatus.Confirmed;
        
        public string PatientNotes { get; set; }
        public string CancellationReason { get; set; }

        public DateTime StartsAt { get; set; }
        public DateTime EndsAt { get; set; }
        public Guid? ServiceId { get; set; }
        public Guid CreatedByUserId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }

    [Table("Rooms")]
    public class Room
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string RoomName { get; set; }
        [Required]
        public string RoomType { get; set; }
        public int Capacity { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsMaintenance { get; set; }
    }
