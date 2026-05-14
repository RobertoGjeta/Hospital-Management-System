using System.ComponentModel.DataAnnotations;
using IvfClinic.Models;

namespace IVF_Managment_Api.Dtos;

public class CreateLabTestOrderDto
{
    [Required]
    public Guid PatientId { get; set; }

    [Required]
    public Guid DoctorId { get; set; }

    [Required]
    [MaxLength(200)]
    public string TestType { get; set; }

    [Required]
    public TestPriority Priority { get; set; }

    public string? ClinicalInstructions { get; set; }
}

public class UploadLabTestResultDto
{
    [Required]
    public Guid TechnicianId { get; set; }

    [MaxLength(500)]
    public string? FilePath { get; set; }

    [Required]
    public string ResultValuesJson { get; set; }
}

public class LabTestOrderResponseDto
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public Guid DoctorId { get; set; }
    public string TestType { get; set; }
    public TestPriority Priority { get; set; }
    public string? ClinicalInstructions { get; set; }
    public TestStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class LabTestReportResponseDto
{
    public Guid Id { get; set; }
    public Guid? OrderId { get; set; }
    public Guid TechnicianId { get; set; }
    public string? FilePath { get; set; }
    public string ResultValuesJson { get; set; }
    public bool IsAbnormal { get; set; }
    public bool IsReleasedToPatient { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ReleasedAt { get; set; }
}