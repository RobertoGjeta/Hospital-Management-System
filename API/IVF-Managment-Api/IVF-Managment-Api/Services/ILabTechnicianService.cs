using IVF_Managment_Api.Models;

namespace IVF_Managment_Api.Services;

public interface ILabTechnicianService
{
    Task<IEnumerable<LabTechnician>> GetAllAsync();

    Task<IEnumerable<LabTechnician>> GetActiveAsync();

    Task<LabTechnician?> GetByIdAsync(Guid id);

    Task<LabTechnician?> GetByUsernameAsync(string username);

    Task<LabTechnician?> GetByEmailAsync(string email);

    Task<LabTechnician> CreateAsync(LabTechnician technician, string password);

    Task<LabTechnician?> UpdateAsync(Guid id, LabTechnician updated);

    Task<bool> DeleteAsync(Guid id);

    Task<bool> DeactivateAsync(Guid id);

    Task<bool> ActivateAsync(Guid id);

    Task<bool> ChangePasswordAsync(Guid id, string currentPassword, string newPassword);

    Task<LabTechnician?> ValidateCredentialsAsync(string usernameOrEmail, string password);

    Task RegisterFailedLoginAsync(Guid id);

    Task RegisterSuccessfulLoginAsync(Guid id);
}
