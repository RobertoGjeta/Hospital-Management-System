using System.Collections.Concurrent;
using IVF_Managment_Api.Models;
using IVFClinic.Models;

namespace IVF_Managment_Api.Services;

public class LabTechnicianService : ILabTechnicianService
{
    // In-memory store until a persistence layer (EF Core / DbContext) is wired up.
    private readonly ConcurrentDictionary<Guid, LabTechnician> _technicians = new();

    public Task<IEnumerable<LabTechnician>> GetAllAsync()
    {
        return Task.FromResult(_technicians.Values.AsEnumerable());
    }

    public Task<IEnumerable<LabTechnician>> GetActiveAsync()
    {
        return Task.FromResult(_technicians.Values.Where(t => t.IsActive));
    }

    public Task<LabTechnician?> GetByIdAsync(Guid id)
    {
        _technicians.TryGetValue(id, out var technician);
        return Task.FromResult(technician);
    }

    public Task<LabTechnician?> GetByUsernameAsync(string username)
    {
        var technician = _technicians.Values.FirstOrDefault(
            t => string.Equals(t.Username, username, StringComparison.OrdinalIgnoreCase));
        return Task.FromResult(technician);
    }

    public Task<LabTechnician?> GetByEmailAsync(string email)
    {
        var technician = _technicians.Values.FirstOrDefault(
            t => string.Equals(t.Email, email, StringComparison.OrdinalIgnoreCase));
        return Task.FromResult(technician);
    }

    public async Task<LabTechnician> CreateAsync(LabTechnician technician)
    {
        if (await GetByUsernameAsync(technician.Username) is not null)
            throw new InvalidOperationException($"Username '{technician.Username}' is already taken.");

        if (await GetByEmailAsync(technician.Email) is not null)
            throw new InvalidOperationException($"Email '{technician.Email}' is already registered.");

        technician.Id = technician.Id == Guid.Empty ? Guid.NewGuid() : technician.Id;
        technician.CreatedAt = DateTime.UtcNow;
        technician.UpdatedAt = DateTime.UtcNow;
        technician.IsActive = true;

        _technicians[technician.Id] = technician;
        return technician;
    }

    public Task<LabTechnician?> UpdateAsync(Guid id, LabTechnician updated)
    {
        if (!_technicians.TryGetValue(id, out var existing))
            return Task.FromResult<LabTechnician?>(null);

        existing.FirstName = updated.FirstName;
        existing.LastName = updated.LastName;
        existing.Email = updated.Email;
        existing.Phone = updated.Phone;
        existing.NationalId = updated.NationalId;
        existing.Gender = updated.Gender;
        existing.DateOfBirth = updated.DateOfBirth;
        existing.ProfileImageUrl = updated.ProfileImageUrl;
        existing.EmployeeId = updated.EmployeeId;
        existing.HireDate = updated.HireDate;
        existing.Specialization = updated.Specialization;
        existing.LicenseNumber = updated.LicenseNumber;
        existing.Qualifications = updated.Qualifications;
        existing.UpdatedAt = DateTime.UtcNow;

        return Task.FromResult<LabTechnician?>(existing);
    }

    public Task<bool> DeleteAsync(Guid id)
    {
        return Task.FromResult(_technicians.TryRemove(id, out _));
    }

    public Task<bool> DeactivateAsync(Guid id)
    {
        if (!_technicians.TryGetValue(id, out var technician))
            return Task.FromResult(false);

        technician.IsActive = false;
        technician.UpdatedAt = DateTime.UtcNow;
        return Task.FromResult(true);
    }

    public Task<bool> ActivateAsync(Guid id)
    {
        if (!_technicians.TryGetValue(id, out var technician))
            return Task.FromResult(false);

        technician.IsActive = true;
        technician.UpdatedAt = DateTime.UtcNow;
        return Task.FromResult(true);
    }
}