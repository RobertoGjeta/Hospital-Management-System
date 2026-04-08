using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text;
using IVF_Managment_Api.Models;

namespace IVF_Managment_Api.Services;

public class LabTechnicianService : ILabTechnicianService
{
    // In-memory store until a persistence layer (EF Core / DbContext) is wired up.
    private readonly ConcurrentDictionary<Guid, LabTechnician> _technicians = new();

    private const int MaxFailedLoginAttempts = 5;
    private static readonly TimeSpan LockoutDuration = TimeSpan.FromMinutes(15);

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

    public async Task<LabTechnician> CreateAsync(LabTechnician technician, string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Password is required.", nameof(password));

        if (await GetByUsernameAsync(technician.Username) is not null)
            throw new InvalidOperationException($"Username '{technician.Username}' is already taken.");

        if (await GetByEmailAsync(technician.Email) is not null)
            throw new InvalidOperationException($"Email '{technician.Email}' is already registered.");

        technician.Id = technician.Id == Guid.Empty ? Guid.NewGuid() : technician.Id;
        technician.PasswordHash = HashPassword(password);
        technician.PasswordChangedAt = DateTime.UtcNow;
        technician.CreatedAt = DateTime.UtcNow;
        technician.UpdatedAt = DateTime.UtcNow;
        technician.IsActive = true;
        technician.FailedLoginAttempts = 0;

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
        technician.FailedLoginAttempts = 0;
        technician.AccountLockedUntil = null;
        technician.UpdatedAt = DateTime.UtcNow;
        return Task.FromResult(true);
    }

    public Task<bool> ChangePasswordAsync(Guid id, string currentPassword, string newPassword)
    {
        if (string.IsNullOrWhiteSpace(newPassword))
            throw new ArgumentException("New password is required.", nameof(newPassword));

        if (!_technicians.TryGetValue(id, out var technician))
            return Task.FromResult(false);

        if (!VerifyPassword(currentPassword, technician.PasswordHash))
            return Task.FromResult(false);

        technician.PasswordHash = HashPassword(newPassword);
        technician.PasswordChangedAt = DateTime.UtcNow;
        technician.UpdatedAt = DateTime.UtcNow;
        return Task.FromResult(true);
    }

    public async Task<LabTechnician?> ValidateCredentialsAsync(string usernameOrEmail, string password)
    {
        var technician = await GetByUsernameAsync(usernameOrEmail)
                         ?? await GetByEmailAsync(usernameOrEmail);

        if (technician is null || !technician.IsActive)
            return null;

        if (technician.AccountLockedUntil.HasValue && technician.AccountLockedUntil > DateTime.UtcNow)
            return null;

        if (!VerifyPassword(password, technician.PasswordHash))
        {
            await RegisterFailedLoginAsync(technician.Id);
            return null;
        }

        await RegisterSuccessfulLoginAsync(technician.Id);
        return technician;
    }

    public Task RegisterFailedLoginAsync(Guid id)
    {
        if (!_technicians.TryGetValue(id, out var technician))
            return Task.CompletedTask;

        technician.FailedLoginAttempts++;
        if (technician.FailedLoginAttempts >= MaxFailedLoginAttempts)
            technician.AccountLockedUntil = DateTime.UtcNow.Add(LockoutDuration);

        technician.UpdatedAt = DateTime.UtcNow;
        return Task.CompletedTask;
    }

    public Task RegisterSuccessfulLoginAsync(Guid id)
    {
        if (!_technicians.TryGetValue(id, out var technician))
            return Task.CompletedTask;

        technician.FailedLoginAttempts = 0;
        technician.AccountLockedUntil = null;
        technician.LastLoginAt = DateTime.UtcNow;
        technician.UpdatedAt = DateTime.UtcNow;
        return Task.CompletedTask;
    }

    private static string HashPassword(string password)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(bytes);
    }

    private static bool VerifyPassword(string password, string hash)
    {
        if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(hash))
            return false;
        return CryptographicOperations.FixedTimeEquals(
            Encoding.UTF8.GetBytes(HashPassword(password)),
            Encoding.UTF8.GetBytes(hash));
    }
}
