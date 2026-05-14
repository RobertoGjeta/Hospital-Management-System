using System.ComponentModel.DataAnnotations;
using IvfClinic.Models;

namespace IVF_Managment_Api.Dtos;

public class CreatePatientDto
{
    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; }

    [Required]
    [MaxLength(100)]
    public string LastName { get; set; }

    [Required]
    [EmailAddress]
    public string Username { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [MinLength(8)]
    public string Password { get; set; }

    [Phone]
    [MaxLength(20)]
    public string? PhoneNumber { get; set; }

    [Required]
    public DateTime DateOfBirth { get; set; }

    [Required]
    public Gender Gender { get; set; }

    [Required]
    [MaxLength(50)]
    public string NationalIdNumber { get; set; }

    public string? Address { get; set; }

    public BillingType BillingType { get; set; }

    public string? InsuranceProvider { get; set; }

    public string? InsurancePolicyNumber { get; set; }

    public string? MedicalHistoryNotes { get; set; }

    public string? KnownAllergies { get; set; }

    public Guid? AssignedDoctorId { get; set; }

    public bool OverrideDuplicateCheck { get; set; }
}

public class UpdatePatientDto
{
    [MaxLength(100)]
    public string? FirstName { get; set; }

    [MaxLength(100)]
    public string? LastName { get; set; }

    [EmailAddress]
    public string? Email { get; set; }

    [Phone]
    [MaxLength(20)]
    public string? PhoneNumber { get; set; }

    public string? Address { get; set; }

    public BillingType? BillingType { get; set; }

    public string? InsuranceProvider { get; set; }

    public string? InsurancePolicyNumber { get; set; }

    public string? MedicalHistoryNotes { get; set; }

    public string? KnownAllergies { get; set; }

    public Guid? AssignedDoctorId { get; set; }
}

public class PatientResponseDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string PatientSystemId { get; set; }
    public DateTime DateOfBirth { get; set; }
    public Gender Gender { get; set; }
    public string NationalIdNumber { get; set; }
    public string? Address { get; set; }
    public BillingType BillingType { get; set; }
    public string? InsuranceProvider { get; set; }
    public string? InsurancePolicyNumber { get; set; }
    public string? MedicalHistoryNotes { get; set; }
    public string? KnownAllergies { get; set; }
    public Guid? AssignedDoctorId { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }
}

public class PatientSearchFilter
{
    [MinLength(2)]
    public string? Name { get; set; }

    public string? PatientSystemId { get; set; }

    public Guid? AssignedDoctorId { get; set; }
}

public class UpdateContactDto
{
    [EmailAddress]
    public string? Email { get; set; }

    [Phone]
    [MaxLength(20)]
    public string? PhoneNumber { get; set; }

    public string? Address { get; set; }
}