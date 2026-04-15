using IVFClinic.DTOs.Service;
using IVFClinic.Models;

namespace IVFClinic.Services.Interfaces
{
    public interface IServiceCatalog
    {
        Task<ServiceResponseDto> AddServiceAsync(ServiceCreateDto dto, Guid adminId);
        Task<ServiceResponseDto> UpdateServiceAsync(Guid serviceId, ServiceUpdateDto dto, Guid adminId);
        Task<bool> DeactivateServiceAsync(Guid serviceId, Guid adminId, bool forceConfirm = false);
        Task<bool> ReactivateServiceAsync(Guid serviceId, Guid adminId);
        Task<ServiceResponseDto?> GetServiceByIdAsync(Guid serviceId);
        Task<IEnumerable<ServiceResponseDto>> GetAllServicesAsync(bool includeInactive = false);
        Task<IEnumerable<ServiceResponseDto>> GetServicesByCategoryAsync(string category);
        Task<IEnumerable<PriceHistoryEntry>> GetPriceHistoryAsync(Guid serviceId);
    }
}
