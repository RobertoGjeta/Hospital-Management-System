using IVF_Managment_Api.Data;
using IVF_Managment_Api.Dtos;
using IVF_Managment_Api.Exceptions;
using IVF_Managment_Api.Models;
using IvfClinic.Models;
using Microsoft.EntityFrameworkCore;

namespace IVF_Managment_Api.Services;

public class PatientService : IPatientService
{
    private readonly IvfDbContext _db;
    private readonly IPasswordHasher _hasher;

    public PatientService(IvfDbContext db, IPasswordHasher hasher)
    {
        _db = db;
        _hasher = hasher;
    }

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
        if (!dto.OverrideDuplicateCheck)
        {
            var duplicates = await DetectDuplicatesAsync(dto.NationalIdNumber, dto.FirstName, dto.LastName, dto.DateOfBirth);
            if (duplicates.Count > 0)
                throw new DuplicatePatientException(duplicates);
        }

        var seq = await _db.Patients.CountAsync() + 1;

        var entity = new Patient
        {
            Id = Guid.NewGuid(),
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Username = dto.Username,
            Email = dto.Email,
            PasswordHash = _hasher.Hash(dto.Password),
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

    public async Task<IEnumerable<PatientResponseDto>> SearchAsync(PatientSearchFilter filter)
    {
        var query = _db.Patients.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter.Name) && filter.Name.Length >= 2)
            query = query.Where(p => p.FirstName.Contains(filter.Name) || p.LastName.Contains(filter.Name));

        if (!string.IsNullOrWhiteSpace(filter.PatientSystemId))
            query = query.Where(p => p.PatientSystemId == filter.PatientSystemId);

        if (filter.AssignedDoctorId.HasValue)
            query = query.Where(p => p.AssignedDoctorId == filter.AssignedDoctorId.Value);

        var entities = await query.ToListAsync();
        return entities.Select(MapToResponse);
    }

    public async Task<IReadOnlyList<Guid>> DetectDuplicatesAsync(string nationalId, string firstName, string lastName, DateTime dob)
    {
        var matches = await _db.Patients
            .AsNoTracking()
            .Where(p => p.NationalIdNumber == nationalId ||
                        (p.FirstName == firstName && p.LastName == lastName && p.DateOfBirth == dob))
            .Select(p => p.Id)
            .ToListAsync();

        return matches.AsReadOnly();
    }

    public async Task<IEnumerable<PatientResponseDto>> GetAssignedToDoctorAsync(Guid doctorId)
    {
        var entities = await _db.Patients
            .AsNoTracking()
            .Where(p => p.AssignedDoctorId == doctorId)
            .ToListAsync();

        return entities.Select(MapToResponse);
    }

    public async Task<PatientResponseDto?> UpdateContactInfoAsync(Guid patientId, UpdateContactDto dto)
    {
        var entity = await _db.Patients.FindAsync(patientId);
        if (entity is null) return null;

        if (dto.Email is not null) entity.Email = dto.Email;
        if (dto.PhoneNumber is not null) entity.PhoneNumber = dto.PhoneNumber;
        if (dto.Address is not null) entity.Address = dto.Address;

        await _db.SaveChangesAsync();
        return MapToResponse(entity);
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

}
