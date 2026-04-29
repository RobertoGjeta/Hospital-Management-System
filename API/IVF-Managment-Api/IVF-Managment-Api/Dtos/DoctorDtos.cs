using System.ComponentModel.DataAnnotations;

namespace IVF_Managment_Api.Dtos;

public class CreateDoctorDto
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
    [MaxLength(100)]
    public string Specialization { get; set; }

    [Required]
    [MaxLength(50)]
    public string LicenseNumber { get; set; }

    public string? Qualifications { get; set; }
}

public class UpdateDoctorDto
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

    [MaxLength(100)]
    public string? Specialization { get; set; }

    [MaxLength(50)]
    public string? LicenseNumber { get; set; }

    public string? Qualifications { get; set; }
}

public class DoctorResponseDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string Specialization { get; set; }
    public string LicenseNumber { get; set; }
    public string? Qualifications { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }
}