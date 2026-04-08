using System.ComponentModel.DataAnnotations;

namespace IVF_Managment_Api.Dtos;

public class ChangePasswordDto
{
    [Required]
    [StringLength(256)]
    public string CurrentPassword { get; set; } = string.Empty;

    [Required]
    [MinLength(8, ErrorMessage = "New password must be at least 8 characters.")]
    [StringLength(256)]
    [RegularExpression(
        @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{8,}$",
        ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one digit, and one special character.")]
    public string NewPassword { get; set; } = string.Empty;

    [Required]
    [Compare(nameof(NewPassword), ErrorMessage = "Passwords do not match.")]
    public string ConfirmNewPassword { get; set; } = string.Empty;
}
