using IVF_Managment_Api.Dtos;

namespace IVF_Managment_Api.Services;

public interface IDoctorService
{
    Task<IEnumerable<DoctorResponseDto>> GetAllAsync();
    Task<DoctorResponseDto?> GetByIdAsync(Guid id);
    Task<DoctorResponseDto> CreateAsync(CreateDoctorDto dto);
    Task<DoctorResponseDto?> UpdateAsync(Guid id, UpdateDoctorDto dto);
    Task<bool> DeleteAsync(Guid id);
}