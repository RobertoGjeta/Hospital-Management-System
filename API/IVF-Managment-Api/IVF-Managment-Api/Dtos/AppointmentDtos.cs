using System.ComponentModel.DataAnnotations;
using IvfClinic.Models;

namespace IVF_Managment_Api.Dtos;

public class CreateAppointmentDto
{
    [Required]
    public Guid PatientId { get; set; }

    [Required]
    public Guid DoctorId { get; set; }

    public Guid? RoomId { get; set; }
    public Guid? ServiceId { get; set; }

    [Required]
    public DateTime StartsAt { get; set; }

    [Required]
    public DateTime EndsAt { get; set; }

    [Required]
    public Guid CreatedByUserId { get; set; }
}

public class RescheduleAppointmentDto
{
    [Required]
    public DateTime NewStartsAt { get; set; }

    [Required]
    public DateTime NewEndsAt { get; set; }

    [MaxLength(500)]
    public string? Reason { get; set; }
}

public class CancelAppointmentDto
{
    [Required]
    [MaxLength(500)]
    public string Reason { get; set; }
}

public class AppointmentResponseDto
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public Guid DoctorId { get; set; }
    public Guid? RoomId { get; set; }
    public Guid? ServiceId { get; set; }
    public DateTime StartsAt { get; set; }
    public DateTime EndsAt { get; set; }
    public AppointmentStatus Status { get; set; }
    public Guid CreatedByUserId { get; set; }
    public string? CancellationReason { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}