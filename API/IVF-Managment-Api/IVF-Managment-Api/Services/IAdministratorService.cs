using IVF_Managment_Api.Dtos;

namespace IVF_Managment_Api.Services;

public interface IAdministratorService
{
    Task<IEnumerable<AdministratorResponseDto>> GetAllAsync();
    Task<AdministratorResponseDto?> GetByIdAsync(Guid id);
    Task<AdministratorResponseDto> CreateAsync(CreateAdministratorDto dto);
    Task<AdministratorResponseDto?> UpdateAsync(Guid id, UpdateAdministratorDto dto);
    Task<bool> DeleteAsync(Guid id);
}