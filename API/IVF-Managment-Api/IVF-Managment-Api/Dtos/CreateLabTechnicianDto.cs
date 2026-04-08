using System.ComponentModel.DataAnnotations;
using IVF_Managment_Api.Models;

namespace IVF_Managment_Api.Dtos;

public class CreateLabTechnicianDto
{
    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [StringLength(50, MinimumLength = 3)]
    public string Username { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(256)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters.")]
    [StringLength(256)]
    [RegularExpression(
        @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{8,}$",
        ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one digit, and one special character.")]
    public string Password { get; set; } = string.Empty;

    [Required]
    [Compare(nameof(Password), ErrorMessage = "Passwords do not match.")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Required]
    [StringLength(32)]
    public string NationalId { get; set; } = string.Empty;

    [Required]
    [Phone]
    [StringLength(32)]
    public string Phone { get; set; } = string.Empty;

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
