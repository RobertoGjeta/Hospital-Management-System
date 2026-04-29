using IVF_Managment_Api.Dtos;

namespace IVF_Managment_Api.Services;

public interface ILabTechnicianService
{
    Task<IEnumerable<LabTechnicianResponseDto>> GetAllAsync();
    Task<LabTechnicianResponseDto?> GetByIdAsync(Guid id);
    Task<LabTechnicianResponseDto> CreateAsync(CreateLabTechnicianDto dto);
    Task<LabTechnicianResponseDto?> UpdateAsync(Guid id, UpdateLabTechnicianDto dto);
    Task<bool> DeleteAsync(Guid id);
}