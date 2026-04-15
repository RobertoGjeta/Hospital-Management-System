using IVFClinic.DTOs.Room;
using IVFClinic.Models.Enums;

namespace IVFClinic.Services.Interfaces
{
    public interface IRoomService
    {
        Task<IEnumerable<RoomDto>> GetAvailableRoomsAsync(RoomType type, DateTime start, DateTime end);
        Task<bool> AssignRoomAsync(Guid appointmentId, Guid roomId, Guid adminId);
        Task<bool> ReleaseRoomAsync(Guid appointmentId);
        Task<IEnumerable<RoomDto>> GetAllRoomsAsync();
    }
}
