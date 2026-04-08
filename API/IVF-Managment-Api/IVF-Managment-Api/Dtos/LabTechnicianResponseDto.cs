using IVF_Managment_Api.Models;

namespace IVF_Managment_Api.Dtos;

/// <summary>
/// Lab technician data safe for API clients (no secrets or internal auth counters).
/// </summary>
public class LabTechnicianResponseDto
{
    public Guid Id { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Username { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string NationalId { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    public Gender Gender { get; set; }

    public DateTime DateOfBirth { get; set; }

    public string? ProfileImageUrl { get; set; }

    public string? EmployeeId { get; set; }

    public DateTime? HireDate { get; set; }

    public string? Specialization { get; set; }

    public string? LicenseNumber { get; set; }

    public string? Qualifications { get; set; }

    public bool IsActive { get; set; }

    public DateTime? LastLoginAt { get; set; }

    public DateTime PasswordChangedAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
