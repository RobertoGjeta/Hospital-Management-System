using IVF_Managment_Api.Data;
using IVF_Managment_Api.Dtos;
using IVF_Managment_Api.Models.HelperModels;
using Microsoft.EntityFrameworkCore;

namespace IVF_Managment_Api.Services;

public class MedicalRecordService : IMedicalRecordService
{
    private readonly IvfDbContext _db;
    private readonly IAuditLogger _audit;

    public MedicalRecordService(IvfDbContext db, IAuditLogger audit)
    {
        _db = db;
        _audit = audit;
    }

    public async Task<MedicalRecordEntryResponseDto> AddEntryAsync(CreateMedicalRecordEntryDto dto)
    {
        var entity = new MedicalRecordEntry
        {
            Id = Guid.NewGuid(),
            PatientId = dto.PatientId,
            AuthoringDoctorId = dto.DoctorId,
            EntryType = dto.EntryType,
            Content = dto.Content,
            AmendsEntryId = dto.AmendsEntryId,
            CreatedAt = DateTime.UtcNow
        };

        _db.MedicalRecordEntries.Add(entity);
        await _db.SaveChangesAsync();

        await _audit.LogAsync(dto.DoctorId, "MedicalRecordEntryAdded", "MedicalRecordEntry", entity.Id);

        return MapToResponse(entity);
    }

    public async Task<IEnumerable<MedicalRecordEntryResponseDto>> GetByPatientAsync(Guid patientId)
    {
        var entities = await _db.MedicalRecordEntries
            .AsNoTracking()
            .Where(m => m.PatientId == patientId)
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync();

        return entities.Select(MapToResponse);
    }

    private static MedicalRecordEntryResponseDto MapToResponse(MedicalRecordEntry e) => new()
    {
        Id = e.Id,
        PatientId = e.PatientId,
        DoctorId = e.AuthoringDoctorId,
        EntryType = e.EntryType,
        Content = e.Content,
        AmendsEntryId = e.AmendsEntryId,
        CreatedAt = e.CreatedAt
    };
}