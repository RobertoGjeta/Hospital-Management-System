namespace IVF_Managment_Api.Models;

public class Patient
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Username { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public string NationalId { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    public Gender Gender { get; set; }

    public DateTime DateOfBirth { get; set; }

    public string? ProfileImageUrl { get; set; }

    public string? MedicalHistory { get; set; }

    public string? Allergies { get; set; }

    public string? BloodType { get; set; }

    public string? EmergencyContactName { get; set; }

    public string? EmergencyContactPhone { get; set; }

    public string? Address { get; set; }

    public string? MaritalStatus { get; set; }

    public DateTime? RegistrationDate { get; set; } = DateTime.UtcNow;

    public bool IsActive { get; set; } = true;

    public int FailedLoginAttempts { get; set; } = 0;

    public DateTime? AccountLockedUntil { get; set; }

    public DateTime? LastLoginAt { get; set; }

    public DateTime PasswordChangedAt { get; set; } = DateTime.UtcNow;

    public string? RefreshToken { get; set; }

    public DateTime? RefreshTokenExpiryTime { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}