using IVF_Managment_Api.Dtos;

namespace IVF_Managment_Api.Services;

public interface IRoomService
{
    Task<IEnumerable<RoomResponseDto>> GetAllAsync();
    Task<RoomResponseDto?> GetByIdAsync(Guid id);
    Task<RoomResponseDto> CreateAsync(CreateRoomDto dto);
    Task<RoomResponseDto?> UpdateAsync(Guid id, UpdateRoomDto dto);
    Task<bool> DeleteAsync(Guid id);
    Task AssignRoomAsync(Guid appointmentId, Guid roomId);
}