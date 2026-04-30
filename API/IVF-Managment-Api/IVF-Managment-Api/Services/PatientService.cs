using IVF_Managment_Api.Data;
using IVF_Managment_Api.Dtos;
using IVF_Managment_Api.Models;
using IvfClinic.Models;
using Microsoft.EntityFrameworkCore;

namespace IVF_Managment_Api.Services;

public class PatientService : IPatientService
{
    private readonly IvfDbContext _db;

    public PatientService(IvfDbContext db) => _db = db;

    public async Task<IEnumerable<PatientResponseDto>> GetAllAsync()
    {
        var entities = await _db.Patients.AsNoTracking().ToListAsync();
        return entities.Select(MapToResponse);
    }

    public async Task<PatientResponseDto?> GetByIdAsync(Guid id)
    {
        var entity = await _db.Patients.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
        return entity is null ? null : MapToResponse(entity);
    }

    public async Task<PatientResponseDto> CreateAsync(CreatePatientDto dto)
    {
        // Sequential, human-readable patient code. Unique index on PatientSystemId
        // guards against the rare collision under concurrent creates.
        var seq = await _db.Patients.CountAsync() + 1;

        var entity = new Patient
        {
            Id = Guid.NewGuid(),
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Username = dto.Username,
            Email = dto.Email,
            PasswordHash = HashPassword(dto.Password),
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

        _db.Patients.Add(entity);
        await _db.SaveChangesAsync();
        return MapToResponse(entity);
    }

    public async Task<PatientResponseDto?> UpdateAsync(Guid id, UpdatePatientDto dto)
    {
        var entity = await _db.Patients.FindAsync(id);
        if (entity is null) return null;

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

        await _db.SaveChangesAsync();
        return MapToResponse(entity);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var entity = await _db.Patients.FindAsync(id);
        if (entity is null) return false;

        _db.Patients.Remove(entity);
        await _db.SaveChangesAsync();
        return true;
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

    private static string HashPassword(string password) =>
        Convert.ToBase64String(System.Security.Cryptography.SHA256.HashData(
            System.Text.Encoding.UTF8.GetBytes(password)));
}
