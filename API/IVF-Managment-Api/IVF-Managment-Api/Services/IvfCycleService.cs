using IVF_Managment_Api.Data;
using IVF_Managment_Api.Dtos;
using IVF_Managment_Api.Models.HelperModels;
using IvfClinic.Models;
using Microsoft.EntityFrameworkCore;

namespace IVF_Managment_Api.Services;

public class IvfCycleService : IIvfCycleService
{
    private readonly IvfDbContext _db;
    private readonly IAuditLogger _audit;

    public IvfCycleService(IvfDbContext db, IAuditLogger audit)
    {
        _db = db;
        _audit = audit;
    }

    public async Task<IvfCycleResponseDto> CreateAsync(CreateIvfCycleDto dto)
    {
        var entity = new IvfCycle
        {
            Id = Guid.NewGuid(),
            PatientId = dto.PatientId,
            AssignedDoctorId = dto.DoctorId,
            CurrentPhase = CyclePhase.Stimulation,
            StartDate = DateTime.UtcNow,
            IsActive = true
        };

        _db.IvfCycles.Add(entity);
        await _db.SaveChangesAsync();

        await _audit.LogAsync(dto.DoctorId, "IvfCycleCreated", "IvfCycle", entity.Id);

        return MapToResponse(entity);
    }

    public async Task<IEnumerable<IvfCycleResponseDto>> GetByPatientAsync(Guid patientId)
    {
        var entities = await _db.IvfCycles
            .AsNoTracking()
            .Where(c => c.PatientId == patientId)
            .OrderByDescending(c => c.StartDate)
            .ToListAsync();

        return entities.Select(MapToResponse);
    }

    public async Task<IvfCycleResponseDto?> AdvancePhaseAsync(Guid cycleId, Guid doctorId, string? justification)
    {
        var entity = await _db.IvfCycles.FindAsync(cycleId);
        if (entity is null) return null;

        var nextPhase = GetNextPhase(entity.CurrentPhase);
        if (nextPhase is null)
            throw new InvalidOperationException("Cycle is already completed or cancelled — cannot advance further.");

        entity.CurrentPhase = nextPhase.Value;

        if (nextPhase == CyclePhase.Completed)
        {
            entity.EndDate = DateTime.UtcNow;
            entity.IsActive = false;
        }

        await _db.SaveChangesAsync();

        await _audit.LogAsync(
            doctorId,
            "IvfCyclePhaseAdvanced",
            "IvfCycle",
            entity.Id,
            null,
            System.Text.Json.JsonSerializer.Serialize(new { newPhase = nextPhase.ToString(), justification }));

        return MapToResponse(entity);
    }

    private static CyclePhase? GetNextPhase(CyclePhase current) => current switch
    {
        CyclePhase.Stimulation => CyclePhase.EggRetrieval,
        CyclePhase.EggRetrieval => CyclePhase.Fertilization,
        CyclePhase.Fertilization => CyclePhase.EmbryoCulture,
        CyclePhase.EmbryoCulture => CyclePhase.Transfer,
        CyclePhase.Transfer => CyclePhase.LutealPhase,
        CyclePhase.LutealPhase => CyclePhase.Completed,
        _ => null
    };

    private static IvfCycleResponseDto MapToResponse(IvfCycle e) => new()
    {
        Id = e.Id,
        PatientId = e.PatientId,
        DoctorId = e.AssignedDoctorId,
        CurrentPhase = e.CurrentPhase,
        StartedAt = e.StartDate,
        CompletedAt = e.EndDate,
        Outcome = e.Outcome,
        IsActive = e.IsActive
    };
}