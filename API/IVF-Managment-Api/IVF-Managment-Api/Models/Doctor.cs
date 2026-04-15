using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using IVF_Managment_Api.Models;
using IVF_Managment_Api.Models.BaseModel;
using IVF_Managment_Api.Models.HelperModels;

[Table("Doctors")]
public class Doctor : User
{
    [Required]
    [MaxLength(100)]
    public string Specialization { get; set; } 
    [Required]
    [MaxLength(50)]
    public string LicenseNumber { get; set; } // Must be unique 

    public string Qualifications { get; set; } 
    // Navigation Properties
    public virtual ICollection<Patient> AssignedPatients { get; set; }
    public virtual ICollection<Appointment> Appointments { get; set; }
        
    // Links to a separate entity managing their weekly time slots
    public virtual ICollection<AvailabilitySchedule> AvailabilitySchedules { get; set; }
}