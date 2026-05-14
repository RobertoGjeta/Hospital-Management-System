using IVF_Managment_Api.Data;
using IVF_Managment_Api.Dtos;
using IVF_Managment_Api.Models.HelperModels;
using IvfClinic.Models;
using Microsoft.EntityFrameworkCore;

namespace IVF_Managment_Api.Services;

public class AppointmentService : IAppointmentService
{
    private readonly IvfDbContext _db;
    private readonly IAuditLogger _audit;

    public AppointmentService(IvfDbContext db, IAuditLogger audit)
    {
        _db = db;
        _audit = audit;
    }

    public async Task<AppointmentResponseDto> CreateAsync(CreateAppointmentDto dto)
    {
        if (await CheckConflictAsync(dto.DoctorId, dto.StartsAt, dto.EndsAt))
            throw new InvalidOperationException("Doctor has a conflicting appointment.");

        var entity = new Appointment
        {
            Id = Guid.NewGuid(),
            PatientId = dto.PatientId,
            DoctorId = dto.DoctorId,
            RoomId = dto.RoomId,
            ServiceId = dto.ServiceId,
            StartsAt = dto.StartsAt,
            EndsAt = dto.EndsAt,
            ScheduledAt = dto.StartsAt,
            DurationMinutes = (int)(dto.EndsAt - dto.StartsAt).TotalMinutes,
            ServiceType = string.Empty,
            Status = AppointmentStatus.Scheduled,
            CreatedByUserId = dto.CreatedByUserId,
            CreatedAt = DateTime.UtcNow
        };

        _db.Appointments.Add(entity);
        await _db.SaveChangesAsync();

        await _audit.LogAsync(dto.CreatedByUserId, "AppointmentCreated", "Appointment", entity.Id);

        return MapToResponse(entity);
    }

    public async Task<AppointmentResponseDto?> RescheduleAsync(Guid id, DateTime newStart, DateTime newEnd, string? reason)
    {
        var entity = await _db.Appointments.FindAsync(id);
        if (entity is null) return null;

        if (await CheckConflictAsync(entity.DoctorId, newStart, newEnd, id))
            throw new InvalidOperationException("Doctor has a conflicting appointment at the new time.");

        entity.StartsAt = newStart;
        entity.EndsAt = newEnd;
        entity.ScheduledAt = newStart;
        entity.DurationMinutes = (int)(newEnd - newStart).TotalMinutes;
        entity.Status = AppointmentStatus.Rescheduled;
        entity.CancellationReason = reason;
        entity.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        await _audit.LogAsync(entity.CreatedByUserId, "AppointmentRescheduled", "Appointment", entity.Id);

        return MapToResponse(entity);
    }

    public async Task<AppointmentResponseDto?> CancelAsync(Guid id, string reason)
    {
        var entity = await _db.Appointments.FindAsync(id);
        if (entity is null) return null;

        entity.Status = AppointmentStatus.Cancelled;
        entity.CancellationReason = reason;
        entity.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        await _audit.LogAsync(entity.CreatedByUserId, "AppointmentCancelled", "Appointment", entity.Id);

        return MapToResponse(entity);
    }

    public async Task<IEnumerable<AppointmentResponseDto>> GetByPatientAsync(Guid patientId, AppointmentStatus? status = null)
    {
        var query = _db.Appointments.AsNoTracking().Where(a => a.PatientId == patientId);
        if (status.HasValue)
            query = query.Where(a => a.Status == status.Value);

        var entities = await query.OrderByDescending(a => a.StartsAt).ToListAsync();
        return entities.Select(MapToResponse);
    }

    public async Task<IEnumerable<AppointmentResponseDto>> GetByDoctorAsync(Guid doctorId, DateTime from, DateTime to)
    {
        var entities = await _db.Appointments
            .AsNoTracking()
            .Where(a => a.DoctorId == doctorId && a.StartsAt >= from && a.StartsAt <= to)
            .OrderBy(a => a.StartsAt)
            .ToListAsync();

        return entities.Select(MapToResponse);
    }

    public async Task<bool> CheckConflictAsync(Guid doctorId, DateTime start, DateTime end, Guid? excludeAppointmentId = null)
    {
        var query = _db.Appointments
            .Where(a => a.DoctorId == doctorId &&
                        a.Status != AppointmentStatus.Cancelled &&
                        a.StartsAt < end &&
                        a.EndsAt > start);

        if (excludeAppointmentId.HasValue)
            query = query.Where(a => a.Id != excludeAppointmentId.Value);

        return await query.AnyAsync();
    }

    private static AppointmentResponseDto MapToResponse(Appointment e) => new()
    {
        Id = e.Id,
        PatientId = e.PatientId,
        DoctorId = e.DoctorId,
        RoomId = e.RoomId,
        ServiceId = e.ServiceId,
        StartsAt = e.StartsAt,
        EndsAt = e.EndsAt,
        Status = e.Status,
        CreatedByUserId = e.CreatedByUserId,
        CancellationReason = e.CancellationReason,
        CreatedAt = e.CreatedAt,
        UpdatedAt = e.UpdatedAt
    };
}