using IVF_Managment_Api.Data;
using IVF_Managment_Api.Dtos;
using IVF_Managment_Api.Exceptions;
using IVF_Managment_Api.Models;
using IVF_Managment_Api.Models.HelperModels;
using IvfClinic.Models;
using Microsoft.EntityFrameworkCore;

namespace IVF_Managment_Api.Services;

public class DoctorService : IDoctorService
{
    private readonly IvfDbContext _db;
    private readonly IPasswordHasher _hasher;
    private readonly IAppointmentService _appointments;

    public DoctorService(IvfDbContext db, IPasswordHasher hasher, IAppointmentService appointments)
    {
        _db = db;
        _hasher = hasher;
        _appointments = appointments;
    }

    public async Task<IEnumerable<DoctorResponseDto>> GetAllAsync()
    {
        var entities = await _db.Doctors.AsNoTracking().ToListAsync();
        return entities.Select(MapToResponse);
    }

    public async Task<DoctorResponseDto?> GetByIdAsync(Guid id)
    {
        var entity = await _db.Doctors.AsNoTracking().FirstOrDefaultAsync(d => d.Id == id);
        return entity is null ? null : MapToResponse(entity);
    }

    public async Task<DoctorResponseDto> CreateAsync(CreateDoctorDto dto)
    {
        var entity = new Doctor
        {
            Id = Guid.NewGuid(),
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Username = dto.Username,
            Email = dto.Email,
            PasswordHash = _hasher.Hash(dto.Password),
            PhoneNumber = dto.PhoneNumber,
            Role = UserRole.Doctor,
            Specialization = dto.Specialization,
            LicenseNumber = dto.LicenseNumber,
            Qualifications = dto.Qualifications,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        _db.Doctors.Add(entity);
        await _db.SaveChangesAsync();
        return MapToResponse(entity);
    }

    public async Task<DoctorResponseDto?> UpdateAsync(Guid id, UpdateDoctorDto dto)
    {
        var entity = await _db.Doctors.FindAsync(id);
        if (entity is null) return null;

        if (dto.FirstName is not null) entity.FirstName = dto.FirstName;
        if (dto.LastName is not null) entity.LastName = dto.LastName;
        if (dto.Email is not null) entity.Email = dto.Email;
        if (dto.PhoneNumber is not null) entity.PhoneNumber = dto.PhoneNumber;
        if (dto.Specialization is not null) entity.Specialization = dto.Specialization;
        if (dto.LicenseNumber is not null) entity.LicenseNumber = dto.LicenseNumber;
        if (dto.Qualifications is not null) entity.Qualifications = dto.Qualifications;

        await _db.SaveChangesAsync();
        return MapToResponse(entity);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var entity = await _db.Doctors.FindAsync(id);
        if (entity is null) return false;

        _db.Doctors.Remove(entity);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<DoctorResponseDto>> GetActiveAsync()
    {
        var entities = await _db.Doctors.AsNoTracking().Where(d => d.IsActive).ToListAsync();
        return entities.Select(MapToResponse);
    }

    public async Task DeactivateAsync(Guid id)
    {
        var entity = await _db.Doctors.FindAsync(id);
        if (entity is null)
            throw new InvalidOperationException("Doctor not found.");

        var futureAppointments = await _appointments.GetByDoctorAsync(id, DateTime.UtcNow, DateTime.UtcNow.AddYears(5));
        if (futureAppointments.Any(a => a.Status != AppointmentStatus.Cancelled))
            throw new HasFutureAppointmentsException();

        entity.IsActive = false;
        await _db.SaveChangesAsync();
    }

    public async Task<IEnumerable<DoctorAvailabilityResponseDto>> GetAvailabilityAsync(Guid doctorId, DateOnly from, DateOnly to)
    {
        var schedules = await _db.AvailabilitySchedules
            .AsNoTracking()
            .Where(s => s.DoctorId == doctorId)
            .ToListAsync();

        var appointments = await _db.Appointments
            .AsNoTracking()
            .Where(a => a.DoctorId == doctorId &&
                        a.StartsAt >= from.ToDateTime(TimeOnly.MinValue) &&
                        a.StartsAt <= to.ToDateTime(TimeOnly.MaxValue) &&
                        a.Status != AppointmentStatus.Cancelled)
            .ToListAsync();

        var result = new List<DoctorAvailabilityResponseDto>();

        for (var date = from; date <= to; date = date.AddDays(1))
        {
            var daySchedules = schedules.Where(s => s.DayOfWeek == date.DayOfWeek);
            foreach (var schedule in daySchedules)
            {
                var slotStart = date.ToDateTime(TimeOnly.FromTimeSpan(schedule.StartTime));
                var slotEnd = date.ToDateTime(TimeOnly.FromTimeSpan(schedule.EndTime));

                var hasConflict = appointments.Any(a => a.StartsAt < slotEnd && a.EndsAt > slotStart);
                if (!hasConflict)
                {
                    result.Add(new DoctorAvailabilityResponseDto
                    {
                        Date = date,
                        StartTime = schedule.StartTime,
                        EndTime = schedule.EndTime
                    });
                }
            }
        }

        return result;
    }

    public async Task SetAvailabilityAsync(Guid doctorId, List<AvailabilitySlotDto> slots)
    {
        var existing = await _db.AvailabilitySchedules
            .Where(s => s.DoctorId == doctorId)
            .ToListAsync();

        _db.AvailabilitySchedules.RemoveRange(existing);

        foreach (var slot in slots)
        {
            _db.AvailabilitySchedules.Add(new AvailabilitySchedule
            {
                Id = Guid.NewGuid(),
                DoctorId = doctorId,
                DayOfWeek = slot.DayOfWeek,
                StartTime = slot.StartTime,
                EndTime = slot.EndTime
            });
        }

        await _db.SaveChangesAsync();
    }

    private static DoctorResponseDto MapToResponse(Doctor e) => new()
    {
        Id = e.Id,
        FirstName = e.FirstName,
        LastName = e.LastName,
        Username = e.Username,
        Email = e.Email,
        PhoneNumber = e.PhoneNumber,
        Specialization = e.Specialization,
        LicenseNumber = e.LicenseNumber,
        Qualifications = e.Qualifications,
        CreatedAt = e.CreatedAt,
        IsActive = e.IsActive
    };
}
