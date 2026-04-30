using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using IVF_Managment_Api.Models.BaseModel;
using IVF_Managment_Api.Models.HelperModels;
using IvfClinic.Models;

namespace IVF_Managment_Api.Models;

[Table("Patients")]
public class Patient : User
{
    [Required]
    [MaxLength(20)]
    public string PatientSystemId { get; set; } // Generated unique ID for the portal [cite: 1854]

    [Required]
    public DateTime DateOfBirth { get; set; } 

    [Required]
    public Gender Gender { get; set; } 

    [Required]
    [MaxLength(50)]
    public string NationalIdNumber { get; set; } // Must be unique 

    public string? Address { get; set; }

    // Billing & Insurance
    public BillingType BillingType { get; set; }
    public string? InsuranceProvider { get; set; }
    public string? InsurancePolicyNumber { get; set; }

    // Medical Overview
    public string? MedicalHistoryNotes { get; set; }
    public string? KnownAllergies { get; set; }

    // Navigation Properties
    public Guid? AssignedDoctorId { get; set; }
    public virtual Doctor AssignedDoctor { get; set; }
        
    public virtual ICollection<Appointment> Appointments { get; set; }
    public virtual ICollection<Bill> Bills { get; set; }
}
