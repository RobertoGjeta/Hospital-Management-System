using System.ComponentModel.DataAnnotations;
using IvfClinic.Models;

namespace IVF_Managment_Api.Dtos;

public class CreateMedicalRecordEntryDto
{
    [Required]
    public Guid PatientId { get; set; }

    [Required]
    public Guid DoctorId { get; set; }

    [Required]
    public RecordEntryType EntryType { get; set; }

    [Required]
    public string Content { get; set; }

    public Guid? AmendsEntryId { get; set; }
}

public class MedicalRecordEntryResponseDto
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public Guid DoctorId { get; set; }
    public RecordEntryType EntryType { get; set; }
    public string Content { get; set; }
    public Guid? AmendsEntryId { get; set; }
    public DateTime CreatedAt { get; set; }
}