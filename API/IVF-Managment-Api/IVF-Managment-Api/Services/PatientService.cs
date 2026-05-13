using System.Collections.Concurrent;
using IVF_Managment_Api.Dtos;
using IVF_Managment_Api.Models;
using IvfClinic.Models;

namespace IVF_Managment_Api.Services;

public class PatientService : IPatientService
{
    private static readonly ConcurrentDictionary<Guid, Patient> Store = new();
    private static int _patientCounter;

    public Task<IEnumerable<PatientResponseDto>> GetAllAsync()
    {
        var result = Store.Values.Select(MapToResponse);
        return Task.FromResult(result);
    }

    public Task<PatientResponseDto?> GetByIdAsync(Guid id)
    {
        Store.TryGetValue(id, out var entity);
        return Task.FromResult(entity is null ? null : MapToResponse(entity));
    }

    public Task<PatientResponseDto> CreateAsync(CreatePatientDto dto)
    {
        var seq = Interlocked.Increment(ref _patientCounter);
        var entity = new Patient
        {
            Id = Guid.NewGuid(),
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Username = dto.Username,
            Email = dto.Email,
            PasswordHash = BCryptHash(dto.Password),
            PhoneNumber = dto.PhoneNumber,
            Role = UserRole.Patient,
            PatientSystemId = $"PAT-{seq:D6}",
            DateOfBirth = dto.DateOfBirth,
            Gender = dto.Gender,
            NationalIdNumber = dto.NationalIdNumber,
            Address = dto.Address,
            BillingType = dto.BillingType,
            InsuranceProvider = dto.InsuranceProvider,
            InsurancePolicyNumber = dto.InsurancePolicyNumber,
            MedicalHistoryNotes = dto.MedicalHistoryNotes,
            KnownAllergies = dto.KnownAllergies,
            AssignedDoctorId = dto.AssignedDoctorId,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        Store[entity.Id] = entity;
        UserCredentialStore.Register(new UserCredentialStore.UserCredential(
            entity.Id, entity.Username, entity.Email, entity.PasswordHash, entity.Role));
        return Task.FromResult(MapToResponse(entity));
    }

    public Task<PatientResponseDto?> UpdateAsync(Guid id, UpdatePatientDto dto)
    {
        if (!Store.TryGetValue(id, out var entity))
            return Task.FromResult<PatientResponseDto?>(null);

        if (dto.FirstName is not null) entity.FirstName = dto.FirstName;
        if (dto.LastName is not null) entity.LastName = dto.LastName;
        if (dto.Email is not null) entity.Email = dto.Email;
        if (dto.PhoneNumber is not null) entity.PhoneNumber = dto.PhoneNumber;
        if (dto.Address is not null) entity.Address = dto.Address;
        if (dto.BillingType.HasValue) entity.BillingType = dto.BillingType.Value;
        if (dto.InsuranceProvider is not null) entity.InsuranceProvider = dto.InsuranceProvider;
        if (dto.InsurancePolicyNumber is not null) entity.InsurancePolicyNumber = dto.InsurancePolicyNumber;
        if (dto.MedicalHistoryNotes is not null) entity.MedicalHistoryNotes = dto.MedicalHistoryNotes;
        if (dto.KnownAllergies is not null) entity.KnownAllergies = dto.KnownAllergies;
        if (dto.AssignedDoctorId.HasValue) entity.AssignedDoctorId = dto.AssignedDoctorId;

        return Task.FromResult<PatientResponseDto?>(MapToResponse(entity));
    }

    public Task<bool> DeleteAsync(Guid id)
    {
        if (Store.TryRemove(id, out var removed))
        {
            UserCredentialStore.Remove(removed.Username, removed.Email);
            return Task.FromResult(true);
        }
        return Task.FromResult(false);
    }

    private static PatientResponseDto MapToResponse(Patient e) => new()
    {
        Id = e.Id,
        FirstName = e.FirstName,
        LastName = e.LastName,
        Username = e.Username,
        Email = e.Email,
        PhoneNumber = e.PhoneNumber,
        PatientSystemId = e.PatientSystemId,
        DateOfBirth = e.DateOfBirth,
        Gender = e.Gender,
        NationalIdNumber = e.NationalIdNumber,
        Address = e.Address,
        BillingType = e.BillingType,
        InsuranceProvider = e.InsuranceProvider,
        InsurancePolicyNumber = e.InsurancePolicyNumber,
        MedicalHistoryNotes = e.MedicalHistoryNotes,
        KnownAllergies = e.KnownAllergies,
        AssignedDoctorId = e.AssignedDoctorId,
        CreatedAt = e.CreatedAt,
        IsActive = e.IsActive
    };

    private static string BCryptHash(string password) =>
        Convert.ToBase64String(System.Security.Cryptography.SHA256.HashData(
            System.Text.Encoding.UTF8.GetBytes(password)));
}