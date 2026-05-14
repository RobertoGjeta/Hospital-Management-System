using IVF_Managment_Api.Dtos;

namespace IVF_Managment_Api.Services;

public interface IServiceCatalogService
{
    Task<IEnumerable<ClinicServiceResponseDto>> GetAllAsync();
    Task<ClinicServiceResponseDto?> GetByIdAsync(Guid id);
    Task<ClinicServiceResponseDto> CreateAsync(CreateClinicServiceDto dto);
    Task<ClinicServiceResponseDto?> UpdateAsync(Guid id, UpdateClinicServiceDto dto);
    Task<bool> DeactivateAsync(Guid id);
}