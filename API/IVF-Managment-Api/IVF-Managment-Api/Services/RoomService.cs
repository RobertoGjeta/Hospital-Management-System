using IVF_Managment_Api.Data;
using IVF_Managment_Api.Dtos;
using IVF_Managment_Api.Models.HelperModels;
using IvfClinic.Models;
using Microsoft.EntityFrameworkCore;

namespace IVF_Managment_Api.Services;

public class RoomService : IRoomService
{
    private readonly IvfDbContext _db;

    public RoomService(IvfDbContext db) => _db = db;

    public async Task<IEnumerable<RoomResponseDto>> GetAllAsync()
    {
        var rooms = await _db.Rooms.AsNoTracking().ToListAsync();
        return rooms.Select(MapToResponse);
    }

    public async Task<RoomResponseDto?> GetByIdAsync(Guid id)
    {
        var room = await _db.Rooms.AsNoTracking().FirstOrDefaultAsync(r => r.Id == id);
        return room is null ? null : MapToResponse(room);
    }

    public async Task<RoomResponseDto> CreateAsync(CreateRoomDto dto)
    {
        var entity = new Room
        {
            Id = Guid.NewGuid(),
            RoomName = dto.Name,
            RoomType = dto.Type,
            Capacity = dto.Capacity,
            IsActive = true,
            IsMaintenance = false
        };

        _db.Rooms.Add(entity);
        await _db.SaveChangesAsync();

        return MapToResponse(entity);
    }

    public async Task<RoomResponseDto?> UpdateAsync(Guid id, UpdateRoomDto dto)
    {
        var entity = await _db.Rooms.FindAsync(id);
        if (entity is null) return null;

        if (dto.Name is not null) entity.RoomName = dto.Name;
        if (dto.Type is not null) entity.RoomType = dto.Type;
        if (dto.Capacity.HasValue) entity.Capacity = dto.Capacity.Value;
        if (dto.IsActive.HasValue) entity.IsActive = dto.IsActive.Value;

        await _db.SaveChangesAsync();

        return MapToResponse(entity);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var entity = await _db.Rooms.FindAsync(id);
        if (entity is null) return false;

        _db.Rooms.Remove(entity);
        await _db.SaveChangesAsync();

        return true;
    }

    public async Task AssignRoomAsync(Guid appointmentId, Guid roomId)
    {
        var appointment = await _db.Appointments.FindAsync(appointmentId)
            ?? throw new InvalidOperationException("Appointment not found.");

        var room = await _db.Rooms.FindAsync(roomId)
            ?? throw new InvalidOperationException("Room not found.");

        if (!room.IsActive || room.IsMaintenance)
            throw new InvalidOperationException("Room is not available.");

        var hasConflict = await _db.Appointments
            .Where(a => a.RoomId == roomId &&
                        a.Id != appointmentId &&
                        a.Status != AppointmentStatus.Cancelled &&
                        a.StartsAt < appointment.EndsAt &&
                        a.EndsAt > appointment.StartsAt)
            .AnyAsync();

        if (hasConflict)
            throw new InvalidOperationException("Room has a conflicting booking during this time slot.");

        appointment.RoomId = roomId;
        appointment.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
    }

    private static RoomResponseDto MapToResponse(Room e) => new()
    {
        Id = e.Id,
        Name = e.RoomName,
        Type = e.RoomType,
        Capacity = e.Capacity,
        IsActive = e.IsActive
    };
}