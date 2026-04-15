using IVF_Managment_Api.Models;
using IVFClinic.Models;

namespace IVF_Managment_Api.Services;

public interface ILabTechnicianService
{
    Task<IEnumerable<LabTechnician>> GetAllAsync();

    Task<IEnumerable<LabTechnician>> GetActiveAsync();

    Task<LabTechnician?> GetByIdAsync(Guid id);

    Task<LabTechnician?> GetByUsernameAsync(string username);

    Task<LabTechnician?> GetByEmailAsync(string email);

    Task<LabTechnician> CreateAsync(LabTechnician technician);

    Task<LabTechnician?> UpdateAsync(Guid id, LabTechnician updated);

    Task<bool> DeleteAsync(Guid id);

    Task<bool> DeactivateAsync(Guid id);

    Task<bool> ActivateAsync(Guid id);
}
