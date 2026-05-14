using IVF_Managment_Api.Data;
using IVF_Managment_Api.Dtos;
using IVF_Managment_Api.Models.HelperModels;
using IvfClinic.Models;
using Microsoft.EntityFrameworkCore;

namespace IVF_Managment_Api.Services;

public class SampleCustodyService : ISampleCustodyService
{
    private readonly IvfDbContext _db;

    public SampleCustodyService(IvfDbContext db) => _db = db;

    public async Task<SampleCustodyEventResponseDto> RecordEventAsync(CreateSampleCustodyEventDto dto)
    {
        if (dto.EventType == CustodyEventType.Discarded && string.IsNullOrWhiteSpace(dto.ReasonCode))
            throw new InvalidOperationException("ReasonCode is mandatory for Discarded events.");

        if (dto.EventType == CustodyEventType.Transferred && string.IsNullOrWhiteSpace(dto.Recipient))
            throw new InvalidOperationException("Recipient is mandatory for Transferred events.");

        var entity = new ChainOfCustodyLog
        {
            Id = Guid.NewGuid(),
            TechnicianId = dto.TechnicianId,
            SampleIdentifier = dto.SampleIdentifier,
            EventType = dto.EventType,
            DestinationRecipient = dto.Recipient,
            DisposalReason = dto.ReasonCode,
            AdditionalNotes = dto.Notes,
            Timestamp = DateTime.UtcNow
        };

        _db.ChainOfCustodyLogs.Add(entity);
        await _db.SaveChangesAsync();

        return MapToResponse(entity);
    }

    public async Task<IEnumerable<SampleCustodyEventResponseDto>> GetBySampleAsync(string sampleIdentifier)
    {
        var entities = await _db.ChainOfCustodyLogs
            .AsNoTracking()
            .Where(l => l.SampleIdentifier == sampleIdentifier)
            .OrderByDescending(l => l.Timestamp)
            .ToListAsync();

        return entities.Select(MapToResponse);
    }

    private static SampleCustodyEventResponseDto MapToResponse(ChainOfCustodyLog e) => new()
    {
        Id = e.Id,
        SampleIdentifier = e.SampleIdentifier,
        EventType = e.EventType,
        TechnicianId = e.TechnicianId,
        Recipient = e.DestinationRecipient,
        ReasonCode = e.DisposalReason,
        Notes = e.AdditionalNotes,
        Timestamp = e.Timestamp
    };
}