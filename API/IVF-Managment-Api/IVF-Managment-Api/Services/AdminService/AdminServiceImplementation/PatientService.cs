using Microsoft.EntityFrameworkCore;
using IVFClinic.Data;
using IVFClinic.DTOs.Patient;
using IVFClinic.Models;
using IVFClinic.Models.Enums;
using IVFClinic.Services.Interfaces;

namespace IVFClinic.Services.Implementations
{
    public class PatientService : IPatientService
    {
        private readonly AppDbContext _db;
        private readonly IAuditService _audit;
        private readonly INotificationService _notif;

        public PatientService(AppDbContext db, IAuditService audit, INotificationService notif)
        {
            _db = db;
            _audit = audit;
            _notif = notif;
        }

        public async Task<PatientResponseDto> RegisterPatientAsync(PatientRegisterDto dto, Guid adminId, bool forceCreate = false)
        {
            // Duplicate check (FR_A02)
            if (!forceCreate)
            {
                var duplicate = await CheckDuplicateAsync(dto.NationalId, dto.FirstName, dto.LastName, dto.DateOfBirth);
                if (duplicate != null)
                {
                    throw new DuplicatePatientException("A patient with the same national ID or name+DOB already exists.", duplicate.Id);
                }
            }

            // Generate temp password and username
            var tempPassword = GenerateTempPassword();
            var username = GenerateUsername(dto.FirstName, dto.LastName);

            var patient = new Patient
            {
                Id = Guid.NewGuid(),
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Username = username,
                Email = dto.Email,
                PasswordHash = tempPassword,
                NationalId = dto.NationalId,
                Phone = dto.Phone,
                Gender = dto.Gender,
                DateOfBirth = dto.DateOfBirth,
                Role = UserRole.Patient,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                CreatedById = adminId,
                Address = dto.Address,
                InsuranceProvider = dto.InsuranceProvider,
                InsurancePolicyNumber = dto.InsurancePolicyNumber,
                MedicalHistoryNotes = dto.MedicalHistoryNotes,
                Allergies = dto.Allergies
            };

            _db.Patients.Add(patient);
            await _db.SaveChangesAsync();

            await _audit.LogActionAsync(adminId, AuditAction.Create, "Patient",
                patient.Id.ToString(), newValues: dto, description: "Registered new patient");

            await _notif.SendWelcomeNotificationAsync(patient.Id, username, tempPassword);

            return MapToDto(patient);
        }

        public async Task<Patient?> CheckDuplicateAsync(string nationalId, string firstName, string lastName, DateTime dob)
        {
            return await _db.Patients.FirstOrDefaultAsync(p =>
                p.NationalId == nationalId ||
                (p.FirstName == firstName && p.LastName == lastName && p.DateOfBirth == dob));
        }

        public async Task<PagedResult<PatientResponseDto>> SearchPatientsAsync(
            string? searchTerm, PatientFilters? filters, int page, int pageSize)
        {
            var query = _db.Patients.AsQueryable();

            // Real-time search (min 2 chars per FR_A03)
            if (!string.IsNullOrWhiteSpace(searchTerm) && searchTerm.Length >= 2)
            {
                var term = searchTerm.ToLower();
                query = query.Where(p =>
                    p.FirstName.ToLower().Contains(term) ||
                    p.LastName.ToLower().Contains(term) ||
                    p.NationalId.Contains(term) ||
                    p.Phone.Contains(term) ||
                    p.Id.ToString().Contains(term));
            }

            if (filters != null)
            {
                if (filters.IsActive.HasValue)
                    query = query.Where(p => p.IsActive == filters.IsActive.Value);

                if (filters.AppointmentDateFrom.HasValue)
                    query = query.Where(p => p.Appointments!.Any(a => a.ScheduledStart >= filters.AppointmentDateFrom.Value));

                if (filters.AppointmentDateTo.HasValue)
                    query = query.Where(p => p.Appointments!.Any(a => a.ScheduledStart <= filters.AppointmentDateTo.Value));
            }

            var total = await query.CountAsync();

            var items = await query
                .OrderBy(p => p.LastName)
                .ThenBy(p => p.FirstName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => MapToDto(p))
                .ToListAsync();

            return new PagedResult<PatientResponseDto>
            {
                Items = items,
                TotalCount = total,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<PatientResponseDto?> GetPatientByIdAsync(Guid patientId)
        {
            var patient = await _db.Patients.FindAsync(patientId);
            return patient == null ? null : MapToDto(patient);
        }

        public async Task<PatientResponseDto> UpdatePatientAsync(Guid patientId, PatientUpdateDto dto, Guid adminId)
        {
            var patient = await _db.Patients.FindAsync(patientId)
                ?? throw new KeyNotFoundException($"Patient {patientId} not found");

            // Capture previous values for audit
            var previousValues = new
            {
                patient.Phone,
                patient.Email,
                patient.Address,
                patient.InsuranceProvider,
                patient.InsurancePolicyNumber
            };

            if (!string.IsNullOrEmpty(dto.Phone)) patient.Phone = dto.Phone;
            if (!string.IsNullOrEmpty(dto.Email)) patient.Email = dto.Email;
            if (dto.Address != null) patient.Address = dto.Address;
            if (dto.InsuranceProvider != null) patient.InsuranceProvider = dto.InsuranceProvider;
            if (dto.InsurancePolicyNumber != null) patient.InsurancePolicyNumber = dto.InsurancePolicyNumber;

            patient.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            await _audit.LogActionAsync(adminId, AuditAction.Update, "Patient",
                patient.Id.ToString(), previousValues: previousValues, newValues: dto,
                description: "Updated patient information");

            return MapToDto(patient);
        }

        private static string GenerateTempPassword()
        {
            return Guid.NewGuid().ToString("N").Substring(0, 12);
        }

        private static string GenerateUsername(string firstName, string lastName)
        {
            var baseName = $"{firstName.ToLower()}.{lastName.ToLower()}".Replace(" ", "");
            return $"{baseName}{Random.Shared.Next(100, 999)}";
        }

        private static PatientResponseDto MapToDto(Patient p) => new()
        {
            Id = p.Id,
            FullName = p.FullName,
            Email = p.Email,
            Phone = p.Phone,
            NationalId = p.NationalId,
            Gender = p.Gender,
            DateOfBirth = p.DateOfBirth,
            Address = p.Address,
            InsuranceProvider = p.InsuranceProvider,
            IsActive = p.IsActive,
            CreatedAt = p.CreatedAt
        };
    }

    public class DuplicatePatientException : Exception
    {
        public Guid ExistingPatientId { get; }
        public DuplicatePatientException(string message, Guid existingPatientId) : base(message)
        {
            ExistingPatientId = existingPatientId;
        }
    }
}
