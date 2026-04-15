using Microsoft.EntityFrameworkCore;
using IVFClinic.Data;
using IVFClinic.DTOs.Doctor;
using IVFClinic.Models;
using IVFClinic.Models.Enums;
using IVFClinic.Services.Interfaces;

namespace IVFClinic.Services.Implementations
{
    public class DoctorService : IDoctorService
    {
        private readonly AppDbContext _db;
        private readonly IAuditService _audit;
        private readonly INotificationService _notif;

        public DoctorService(AppDbContext db, IAuditService audit, INotificationService notif)
        {
            _db = db;
            _audit = audit;
            _notif = notif;
        }

        public async Task<DoctorResponseDto> AddDoctorAsync(DoctorCreateDto dto, Guid adminId)
        {
            // License uniqueness check (FR_A05)
            var existingLicense = await _db.Doctors.AnyAsync(d => d.LicenseNumber == dto.LicenseNumber);
            if (existingLicense)
            {
                throw new InvalidOperationException($"License number {dto.LicenseNumber} already exists.");
            }

            var tempPassword = Guid.NewGuid().ToString("N").Substring(0, 12);
            var username = $"dr.{dto.FirstName.ToLower()}.{dto.LastName.ToLower()}{Random.Shared.Next(100, 999)}";

            var doctor = new Doctor
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
                Role = UserRole.Doctor,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                CreatedById = adminId,
                LicenseNumber = dto.LicenseNumber,
                Specialization = dto.Specialization,
                Qualifications = dto.Qualifications,
                AvailabilitySchedule = dto.WeeklyAvailability?.Select(s => new DoctorAvailability
                {
                    DayOfWeek = s.DayOfWeek,
                    StartTime = s.StartTime,
                    EndTime = s.EndTime
                }).ToList()
            };

            _db.Doctors.Add(doctor);
            await _db.SaveChangesAsync();

            await _audit.LogActionAsync(adminId, AuditAction.Create, "Doctor",
                doctor.Id.ToString(), newValues: dto, description: "Added new doctor");

            await _notif.SendDoctorCredentialsAsync(doctor.Id, username, tempPassword);

            return MapToDto(doctor);
        }

        public async Task<DoctorResponseDto> UpdateDoctorAsync(Guid doctorId, DoctorUpdateDto dto, Guid adminId)
        {
            var doctor = await _db.Doctors
                .Include(d => d.AvailabilitySchedule)
                .FirstOrDefaultAsync(d => d.Id == doctorId)
                ?? throw new KeyNotFoundException($"Doctor {doctorId} not found");

            var previousValues = new { doctor.Phone, doctor.Email, doctor.Specialization, doctor.Qualifications };

            if (!string.IsNullOrEmpty(dto.Phone)) doctor.Phone = dto.Phone;
            if (!string.IsNullOrEmpty(dto.Email)) doctor.Email = dto.Email;
            if (!string.IsNullOrEmpty(dto.Specialization)) doctor.Specialization = dto.Specialization;
            if (dto.Qualifications != null) doctor.Qualifications = dto.Qualifications;

            if (dto.WeeklyAvailability != null)
            {
                _db.DoctorAvailabilities.RemoveRange(doctor.AvailabilitySchedule!);
                doctor.AvailabilitySchedule = dto.WeeklyAvailability.Select(s => new DoctorAvailability
                {
                    DoctorId = doctor.Id,
                    DayOfWeek = s.DayOfWeek,
                    StartTime = s.StartTime,
                    EndTime = s.EndTime
                }).ToList();
            }

            doctor.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            await _audit.LogActionAsync(adminId, AuditAction.Update, "Doctor",
                doctor.Id.ToString(), previousValues: previousValues, newValues: dto);

            return MapToDto(doctor);
        }

        public async Task<bool> DeactivateDoctorAsync(Guid doctorId, Guid adminId, bool reassignAppointments)
        {
            var doctor = await _db.Doctors.FindAsync(doctorId);
            if (doctor == null) return false;

            // FR_A06: cannot deactivate until future appointments are resolved
            var futureAppts = await _db.Appointments
                .Where(a => a.DoctorId == doctorId
                    && a.ScheduledStart > DateTime.UtcNow
                    && a.Status != AppointmentStatus.Cancelled)
                .ToListAsync();

            if (futureAppts.Any() && !reassignAppointments)
            {
                throw new InvalidOperationException(
                    $"Doctor has {futureAppts.Count} future appointments. Reassign or cancel them first.");
            }

            doctor.IsActive = false;
            doctor.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            await _audit.LogActionAsync(adminId, AuditAction.Deactivate, "Doctor",
                doctor.Id.ToString(), description: "Doctor deactivated");
            return true;
        }

        public async Task<bool> ReactivateDoctorAsync(Guid doctorId, Guid adminId)
        {
            var doctor = await _db.Doctors.FindAsync(doctorId);
            if (doctor == null) return false;

            doctor.IsActive = true;
            doctor.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            await _audit.LogActionAsync(adminId, AuditAction.Update, "Doctor",
                doctor.Id.ToString(), description: "Doctor reactivated");
            return true;
        }

        public async Task<DoctorResponseDto?> GetDoctorByIdAsync(Guid doctorId)
        {
            var doctor = await _db.Doctors
                .Include(d => d.AvailabilitySchedule)
                .FirstOrDefaultAsync(d => d.Id == doctorId);
            return doctor == null ? null : MapToDto(doctor);
        }

        public async Task<IEnumerable<DoctorResponseDto>> GetAllDoctorsAsync(bool includeInactive = false)
        {
            var query = _db.Doctors.Include(d => d.AvailabilitySchedule).AsQueryable();
            if (!includeInactive)
                query = query.Where(d => d.IsActive);

            var doctors = await query.OrderBy(d => d.LastName).ToListAsync();
            return doctors.Select(MapToDto);
        }

        public async Task<IEnumerable<Appointment>> GetFutureAppointmentsForDoctorAsync(Guid doctorId)
        {
            return await _db.Appointments
                .Where(a => a.DoctorId == doctorId
                    && a.ScheduledStart > DateTime.UtcNow
                    && a.Status != AppointmentStatus.Cancelled)
                .Include(a => a.Patient)
                .OrderBy(a => a.ScheduledStart)
                .ToListAsync();
        }

        private static DoctorResponseDto MapToDto(Doctor d) => new()
        {
            Id = d.Id,
            FullName = d.FullName,
            Email = d.Email,
            Phone = d.Phone,
            LicenseNumber = d.LicenseNumber,
            Specialization = d.Specialization,
            Qualifications = d.Qualifications,
            IsActive = d.IsActive,
            WeeklyAvailability = d.AvailabilitySchedule?.Select(a => new AvailabilitySlotDto
            {
                DayOfWeek = a.DayOfWeek,
                StartTime = a.StartTime,
                EndTime = a.EndTime
            }).ToList()
        };
    }
}
