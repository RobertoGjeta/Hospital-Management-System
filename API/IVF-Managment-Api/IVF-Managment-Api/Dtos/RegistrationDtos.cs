using System.ComponentModel.DataAnnotations;
using IvfClinic.Models;

namespace IVF_Managment_Api.Dtos;

public class AdminRegisterDto
{
    [Required]
    public UserRole Role { get; set; }

    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Username { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(8)]
    public string Password { get; set; } = string.Empty;

    [Phone]
    [MaxLength(20)]
    public string? PhoneNumber { get; set; }

    // Doctor
    [MaxLength(100)]
    public string? Specialization { get; set; }

    [MaxLength(50)]
    public string? LicenseNumber { get; set; }

    public string? Qualifications { get; set; }

    // Patient
    public DateTime? DateOfBirth { get; set; }

    public Gender? Gender { get; set; }

    [MaxLength(50)]
    public string? NationalIdNumber { get; set; }

    public string? Address { get; set; }

    public BillingType? BillingType { get; set; }

    public string? InsuranceProvider { get; set; }

    public string? InsurancePolicyNumber { get; set; }

    public string? MedicalHistoryNotes { get; set; }

    public string? KnownAllergies { get; set; }

    public Guid? AssignedDoctorId { get; set; }

    // LabTechnician
    [MaxLength(50)]
    public string? TechnicianId { get; set; }

    // Administrator
    [MaxLength(100)]
    public string? Department { get; set; }
}

public class RegisteredUserResponseDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string Role { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    // Role-specific (null when not applicable)
    public string? Specialization { get; set; }
    public string? LicenseNumber { get; set; }
    public string? Qualifications { get; set; }
    public string? PatientSystemId { get; set; }
    public string? NationalIdNumber { get; set; }
    public string? TechnicianId { get; set; }
    public string? Department { get; set; }
}
