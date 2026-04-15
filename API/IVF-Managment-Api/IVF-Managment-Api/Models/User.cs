using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using IVFClinic.Models.Enums;

namespace IVFClinic.Models
{
    /// <summary>
    /// Base entity for all users in the system.
    /// Uses Table Per Hierarchy (TPH) inheritance — all roles share one Users table
    /// distinguished by a Discriminator column configured in AppDbContext.
    /// </summary>
    public class User
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required, MaxLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string Username { get; set; } = string.Empty;

        [Required, MaxLength(150), EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [Required, MaxLength(20)]
        public string NationalId { get; set; } = string.Empty;

        [Required, MaxLength(20)]
        public string Phone { get; set; } = string.Empty;

        public Gender Gender { get; set; }

        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        [MaxLength(300)]
        public string? ProfileImageUrl { get; set; }

        [Required]
        public UserRole Role { get; set; }

        public bool IsActive { get; set; } = true;

        // Security fields (NFR_A07–A09)
        public int FailedLoginAttempts { get; set; } = 0;
        public DateTime? AccountLockedUntil { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public DateTime PasswordChangedAt { get; set; } = DateTime.UtcNow;
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }

        // Audit fields
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public Guid? CreatedById { get; set; }

        [ForeignKey(nameof(CreatedById))]
        public User? CreatedBy { get; set; }

        // Computed
        [NotMapped]
        public string FullName => $"{FirstName} {LastName}";

        [NotMapped]
        public bool IsLocked => AccountLockedUntil.HasValue && AccountLockedUntil > DateTime.UtcNow;
    }

    public class Admin : User
    {
        [MaxLength(100)]
        public string? Department { get; set; }

        [MaxLength(50)]
        public string? EmployeeId { get; set; }

        public DateTime? HireDate { get; set; }

        // Navigation
        public ICollection<Patient>? RegisteredPatients { get; set; }
        public ICollection<Invoice>? GeneratedBills { get; set; }
        public ICollection<Payment>? RecordedPayments { get; set; }
    }

    public class Patient : User
    {
        [MaxLength(300)]
        public string? Address { get; set; }

        [MaxLength(100)]
        public string? InsuranceProvider { get; set; }

        [MaxLength(100)]
        public string? InsurancePolicyNumber { get; set; }

        [MaxLength(100)]
        public string? InsuranceCoverageType { get; set; }

        [MaxLength(1000)]
        public string? MedicalHistoryNotes { get; set; }

        [MaxLength(500)]
        public string? Allergies { get; set; }

        // Navigation
        public ICollection<Appointment>? Appointments { get; set; }
        public ICollection<Invoice>? Invoices { get; set; }
        public ICollection<RenderedService>? RenderedServices { get; set; }
    }

    public class Doctor : User
    {
        [Required, MaxLength(50)]
        public string LicenseNumber { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string Specialization { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Qualifications { get; set; }

        // Navigation
        public ICollection<DoctorAvailability>? AvailabilitySchedule { get; set; }
        public ICollection<Appointment>? Appointments { get; set; }
    }

}
