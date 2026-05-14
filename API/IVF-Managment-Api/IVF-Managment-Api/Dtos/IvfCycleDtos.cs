using System.ComponentModel.DataAnnotations;
using IvfClinic.Models;

namespace IVF_Managment_Api.Dtos;

public class CreateIvfCycleDto
{
    [Required]
    public Guid PatientId { get; set; }

    [Required]
    public Guid DoctorId { get; set; }
}

public class AdvancePhaseDto
{
    public string? Justification { get; set; }
}

public class IvfCycleResponseDto
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public Guid DoctorId { get; set; }
    public CyclePhase CurrentPhase { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? Outcome { get; set; }
    public bool IsActive { get; set; }
}