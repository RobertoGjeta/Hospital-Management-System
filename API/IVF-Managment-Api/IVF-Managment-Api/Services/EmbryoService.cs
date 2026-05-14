using IVF_Managment_Api.Data;
using IVF_Managment_Api.Dtos;
using IVF_Managment_Api.Models.HelperModels;
using IvfClinic.Models;
using Microsoft.EntityFrameworkCore;

namespace IVF_Managment_Api.Services;

public class EmbryoService : IEmbryoService
{
    private readonly IvfDbContext _db;
    private readonly IAuditLogger _audit;
    private readonly IPasswordHasher _hasher;

    public EmbryoService(IvfDbContext db, IAuditLogger audit, IPasswordHasher hasher)
    {
        _db = db;
        _audit = audit;
        _hasher = hasher;
    }

    public async Task<EmbryoResponseDto> CreateAsync(CreateEmbryoDto dto)
    {
        var entity = new Embryo
        {
            Id = Guid.NewGuid(),
            IvfCycleId = dto.IvfCycleId,
            EmbryoIdentifier = dto.Identifier,
            Status = EmbryoStatus.Developing,
            CreatedAt = DateTime.UtcNow
        };

        _db.Embryos.Add(entity);
        await _db.SaveChangesAsync();

        await _audit.LogAsync(Guid.Empty, "EmbryoCreated", "Embryo", entity.Id);

        return MapToResponse(entity);
    }

    public async Task<IEnumerable<EmbryoResponseDto>> GetByCycleAsync(Guid cycleId)
    {
        var entities = await _db.Embryos
            .AsNoTracking()
            .Where(e => e.IvfCycleId == cycleId)
            .OrderBy(e => e.EmbryoIdentifier)
            .ToListAsync();

        return entities.Select(MapToResponse);
    }

    public async Task<EmbryoResponseDto?> GetByIdAsync(Guid id)
    {
        var entity = await _db.Embryos.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);
        return entity is null ? null : MapToResponse(entity);
    }

    public async Task<EmbryoDevelopmentEntryResponseDto> AddDevelopmentEntryAsync(CreateEmbryoDevelopmentEntryDto dto)
    {
        var entity = new EmbryoObservation
        {
            Id = Guid.NewGuid(),
            EmbryoId = dto.EmbryoId,
            TechnicianId = dto.TechnicianId,
            DayOfDevelopment = dto.DevelopmentDay,
            CellCount = dto.CellCount,
            FragmentationPercentage = dto.FragmentationPct,
            MorphologyGrade = dto.MorphologyGrade,
            FreeTextNotes = dto.Notes,
            MicroscopyImageUrl = dto.MicroscopyImagePath,
            ObservationDate = DateTime.UtcNow
        };

        _db.EmbryoObservations.Add(entity);
        await _db.SaveChangesAsync();

        await _audit.LogAsync(dto.TechnicianId, "EmbryoDevelopmentEntryAdded", "EmbryoObservation", entity.Id);

        return MapDevelopmentToResponse(entity);
    }

    public async Task<IEnumerable<EmbryoDevelopmentEntryResponseDto>> GetDevelopmentEntriesAsync(Guid embryoId)
    {
        var entities = await _db.EmbryoObservations
            .AsNoTracking()
            .Where(o => o.EmbryoId == embryoId)
            .OrderBy(o => o.DayOfDevelopment)
            .ToListAsync();

        return entities.Select(MapDevelopmentToResponse);
    }

    public async Task<EmbryoCryopreservationResponseDto> RecordCryopreservationAsync(CreateEmbryoCryopreservationDto dto)
    {
        var embryo = await _db.Embryos.FindAsync(dto.EmbryoId);
        if (embryo is null)
            throw new InvalidOperationException("Embryo not found.");

        embryo.Status = EmbryoStatus.Frozen;
        embryo.StorageTank = dto.Tank;
        embryo.StorageCane = dto.Cane;
        embryo.VitrificationMethod = dto.VitrificationMethod;

        var cryo = new EmbryoCryopreservation
        {
            Id = Guid.NewGuid(),
            EmbryoId = dto.EmbryoId,
            Tank = dto.Tank,
            Cane = dto.Cane,
            StrawPosition = dto.StrawPosition,
            FreezingDate = dto.FreezingDate,
            VitrificationMethod = dto.VitrificationMethod,
            TechnicianId = dto.TechnicianId,
            CreatedAt = DateTime.UtcNow
        };

        _db.EmbryoCryopreservations.Add(cryo);
        await _db.SaveChangesAsync();

        await _audit.LogAsync(dto.TechnicianId, "EmbryoCryopreserved", "EmbryoCryopreservation", cryo.Id);

        return MapCryoToResponse(cryo);
    }

    public async Task<EmbryoClinicalInstructionResponseDto> AddInstructionAsync(CreateEmbryoClinicalInstructionDto dto)
    {
        if (dto.Type == EmbryoInstructionType.Discard)
        {
            if (string.IsNullOrWhiteSpace(dto.Password))
                throw new InvalidOperationException("Password re-authentication is required for discard instructions.");

            var doctor = await _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == dto.DoctorId);
            if (doctor is null || !_hasher.Verify(dto.Password, doctor.PasswordHash))
                throw new UnauthorizedAccessException("Password verification failed.");
        }

        var instruction = new EmbryoClinicalInstruction
        {
            Id = Guid.NewGuid(),
            EmbryoId = dto.EmbryoId,
            DoctorId = dto.DoctorId,
            Type = dto.Type,
            Rationale = dto.Rationale,
            CreatedAt = DateTime.UtcNow
        };

        _db.EmbryoClinicalInstructions.Add(instruction);

        if (dto.Type == EmbryoInstructionType.Discard)
        {
            var embryo = await _db.Embryos.FindAsync(dto.EmbryoId);
            if (embryo is not null)
                embryo.Status = EmbryoStatus.Discarded;
        }
        else if (dto.Type == EmbryoInstructionType.Transfer)
        {
            var embryo = await _db.Embryos.FindAsync(dto.EmbryoId);
            if (embryo is not null)
                embryo.Status = EmbryoStatus.Transferred;
        }
        else if (dto.Type == EmbryoInstructionType.Cryopreserve)
        {
            var embryo = await _db.Embryos.FindAsync(dto.EmbryoId);
            if (embryo is not null)
                embryo.Status = EmbryoStatus.Frozen;
        }

        await _db.SaveChangesAsync();

        await _audit.LogAsync(dto.DoctorId, $"EmbryoInstruction:{dto.Type}", "EmbryoClinicalInstruction", instruction.Id);

        return MapInstructionToResponse(instruction);
    }

    private static EmbryoResponseDto MapToResponse(Embryo e) => new()
    {
        Id = e.Id,
        IvfCycleId = e.IvfCycleId,
        Identifier = e.EmbryoIdentifier,
        Status = e.Status,
        CreatedAt = e.CreatedAt
    };

    private static EmbryoDevelopmentEntryResponseDto MapDevelopmentToResponse(EmbryoObservation e) => new()
    {
        Id = e.Id,
        EmbryoId = e.EmbryoId,
        TechnicianId = e.TechnicianId,
        DevelopmentDay = e.DayOfDevelopment,
        CellCount = e.CellCount,
        FragmentationPct = e.FragmentationPercentage,
        MorphologyGrade = e.MorphologyGrade,
        Notes = e.FreeTextNotes,
        MicroscopyImagePath = e.MicroscopyImageUrl,
        CreatedAt = e.ObservationDate
    };

    private static EmbryoCryopreservationResponseDto MapCryoToResponse(EmbryoCryopreservation e) => new()
    {
        Id = e.Id,
        EmbryoId = e.EmbryoId,
        Tank = e.Tank,
        Cane = e.Cane,
        StrawPosition = e.StrawPosition,
        FreezingDate = e.FreezingDate,
        VitrificationMethod = e.VitrificationMethod,
        TechnicianId = e.TechnicianId,
        CreatedAt = e.CreatedAt
    };

    private static EmbryoClinicalInstructionResponseDto MapInstructionToResponse(EmbryoClinicalInstruction e) => new()
    {
        Id = e.Id,
        EmbryoId = e.EmbryoId,
        DoctorId = e.DoctorId,
        Type = e.Type,
        Rationale = e.Rationale,
        CreatedAt = e.CreatedAt
    };
}