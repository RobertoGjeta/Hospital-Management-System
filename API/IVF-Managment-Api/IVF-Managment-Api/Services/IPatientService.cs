using IVF_Managment_Api.Dtos;

namespace IVF_Managment_Api.Services;

public interface IPatientService
{
    Task<IEnumerable<PatientResponseDto>> GetAllAsync();
    Task<PatientResponseDto?> GetByIdAsync(Guid id);
    Task<PatientResponseDto> CreateAsync(CreatePatientDto dto);
    Task<PatientResponseDto?> UpdateAsync(Guid id, UpdatePatientDto dto);
    Task<bool> DeleteAsync(Guid id);
}