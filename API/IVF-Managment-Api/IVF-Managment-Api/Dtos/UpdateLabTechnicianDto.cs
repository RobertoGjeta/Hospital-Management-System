using System.ComponentModel.DataAnnotations;
using IVF_Managment_Api.Models;

namespace IVF_Managment_Api.Dtos;

/// <summary>
/// Editable profile fields only. Username, password, and account state are not updated here.
/// </summary>
public class UpdateLabTechnicianDto
{
    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(256)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [Phone]
    [StringLength(32)]
    public string Phone { get; set; } = string.Empty;

    [Required]
    [StringLength(32)]
    public string NationalId { get; set; } = string.Empty;

    [Required]
    public Gender Gender { get; set; }

    [Required]
    public DateTime DateOfBirth { get; set; }

    [Url]
    [StringLength(2048)]
    public string? ProfileImageUrl { get; set; }

    [StringLength(64)]
    public string? EmployeeId { get; set; }

    public DateTime? HireDate { get; set; }

    [StringLength(128)]
    public string? Specialization { get; set; }

    [StringLength(128)]
    public string? LicenseNumber { get; set; }

    [StringLength(512)]
    public string? Qualifications { get; set; }
}
