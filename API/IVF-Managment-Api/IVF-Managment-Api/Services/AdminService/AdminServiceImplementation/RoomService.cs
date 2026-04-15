using Microsoft.EntityFrameworkCore;
using IVFClinic.Data;
using IVFClinic.DTOs.Room;
using IVFClinic.Models;
using IVFClinic.Models.Enums;
using IVFClinic.Services.Interfaces;

namespace IVFClinic.Services.Implementations
{
    public class RoomService : IRoomService
    {
        private readonly AppDbContext _db;
        private readonly IAuditService _audit;
        private readonly INotificationService _notif;

        public RoomService(AppDbContext db, IAuditService audit, INotificationService notif)
        {
            _db = db;
            _audit = audit;
            _notif = notif;
        }

        public async Task<IEnumerable<RoomDto>> GetAvailableRoomsAsync(RoomType type, DateTime start, DateTime end)
        {
            // Find rooms of the right type that are not under maintenance
            // and not booked in the requested time window (FR_A09)
            var available = await _db.Rooms
                .Where(r => r.Type == type
                    && r.Status != RoomStatus.Maintenance
                    && !_db.Appointments.Any(a =>
                        a.RoomId == r.Id
                        && a.Status != AppointmentStatus.Cancelled
                        && a.ScheduledStart < end
                        && a.ScheduledEnd > start))
                .Select(r => new RoomDto
                {
                    Id = r.Id,
                    Number = r.Number,
                    Type = r.Type,
                    Floor = r.Floor,
                    Status = r.Status
                })
                .ToListAsync();

            return available;
        }

        public async Task<bool> AssignRoomAsync(Guid appointmentId, Guid roomId, Guid adminId)
        {
            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                var appt = await _db.Appointments.FindAsync(appointmentId)
                    ?? throw new KeyNotFoundException("Appointment not found");
                var room = await _db.Rooms.FindAsync(roomId)
                    ?? throw new KeyNotFoundException("Room not found");

                if (room.Status == RoomStatus.Maintenance)
                    throw new InvalidOperationException("Room is under maintenance");

                // Optimistic lock check: verify no conflicting appointment was just inserted
                var conflict = await _db.Appointments.AnyAsync(a =>
                    a.RoomId == roomId
                    && a.Id != appointmentId
                    && a.Status != AppointmentStatus.Cancelled
                    && a.ScheduledStart < appt.ScheduledEnd
                    && a.ScheduledEnd > appt.ScheduledStart);

                if (conflict)
                {
                    throw new InvalidOperationException("Room was just assigned to another appointment");
                }

                appt.RoomId = roomId;
                appt.UpdatedAt = DateTime.UtcNow;
                await _db.SaveChangesAsync();
                await transaction.CommitAsync();

                await _audit.LogActionAsync(adminId, AuditAction.AssignRoom, "Appointment",
                    appointmentId.ToString(), newValues: new { roomId });

                await _notif.SendRoomAssignedAsync(appointmentId, roomId);
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> ReleaseRoomAsync(Guid appointmentId)
        {
            var appt = await _db.Appointments.FindAsync(appointmentId);
            if (appt == null || !appt.RoomId.HasValue) return false;

            appt.RoomId = null;
            appt.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<RoomDto>> GetAllRoomsAsync()
        {
            return await _db.Rooms.Select(r => new RoomDto
            {
                Id = r.Id,
                Number = r.Number,
                Type = r.Type,
                Floor = r.Floor,
                Status = r.Status
            }).ToListAsync();
        }
    }
}
