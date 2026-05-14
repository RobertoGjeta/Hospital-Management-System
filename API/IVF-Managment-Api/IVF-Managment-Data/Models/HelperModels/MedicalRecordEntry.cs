using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using IvfClinic.Models;

namespace IVF_Managment_Api.Models.HelperModels;


[Table("MedicalRecordEntries")]
public class MedicalRecordEntry
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public Guid PatientId { get; set; }
    public virtual Patient Patient { get; set; }

    [Required]
    public Guid AuthoringDoctorId { get; set; }
    public virtual Doctor AuthoringDoctor { get; set; }

    [Required]
    public RecordEntryType EntryType { get; set; }

    [Required]
    public string Content { get; set; } // Free text or JSON containing diagnosis/prescription

    public string AttachedFileUrl { get; set; }

    public Guid? AmendsEntryId { get; set; }
    public virtual MedicalRecordEntry? AmendsEntry { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
