using System.Text.Json;
using IVF_Managment_Api.Data;
using IVF_Managment_Api.Dtos;
using IVF_Managment_Api.Models.HelperModels;
using IvfClinic.Models;
using Microsoft.EntityFrameworkCore;

namespace IVF_Managment_Api.Services;

public class LabTestService : ILabTestService
{
    private readonly IvfDbContext _db;
    private readonly IAuditLogger _audit;
    private readonly INotificationService _notifications;

    // TODO: move to config
    private static readonly Dictionary<string, (decimal Min, decimal Max)> ReferenceRanges = new()
    {
        { "hemoglobin", (12.0m, 17.5m) },
        { "whiteBloodCells", (4.5m, 11.0m) },
        { "platelets", (150m, 400m) },
        { "glucose", (70m, 100m) },
        { "creatinine", (0.6m, 1.2m) },
        { "estradiol", (30m, 400m) },
        { "progesterone", (1m, 25m) },
        { "fsh", (1.5m, 12.4m) },
        { "lh", (1.7m, 8.6m) },
        { "amh", (1.0m, 10.0m) },
    };

    public LabTestService(IvfDbContext db, IAuditLogger audit, INotificationService notifications)
    {
        _db = db;
        _audit = audit;
        _notifications = notifications;
    }

    public async Task<LabTestOrderResponseDto> CreateOrderAsync(CreateLabTestOrderDto dto)
    {
        var entity = new LabTestOrder
        {
            Id = Guid.NewGuid(),
            PatientId = dto.PatientId,
            RequestingDoctorId = dto.DoctorId,
            TestCategory = dto.TestType,
            Priority = dto.Priority,
            ClinicalInstructions = dto.ClinicalInstructions,
            Status = TestStatus.Pending,
            RequestedAt = DateTime.UtcNow
        };

        _db.LabTestOrders.Add(entity);
        await _db.SaveChangesAsync();

        await _audit.LogAsync(dto.DoctorId, "LabTestOrderCreated", "LabTestOrder", entity.Id);

        return MapOrderToResponse(entity);
    }

    public async Task<IEnumerable<LabTestOrderResponseDto>> GetPendingQueueAsync()
    {
        var entities = await _db.LabTestOrders
            .AsNoTracking()
            .Where(o => o.Status == TestStatus.Pending)
            .OrderByDescending(o => o.Priority)
            .ThenBy(o => o.RequestedAt)
            .ToListAsync();

        return entities.Select(MapOrderToResponse);
    }

    public async Task<LabTestReportResponseDto> UploadResultAsync(Guid orderId, UploadLabTestResultDto dto)
    {
        var order = await _db.LabTestOrders.FindAsync(orderId);
        if (order is null)
            throw new InvalidOperationException("Lab test order not found.");

        var isAbnormal = DetectAbnormalValues(dto.ResultValuesJson);

        var report = new LabTestReport
        {
            Id = Guid.NewGuid(),
            OrderId = orderId,
            PatientId = order.PatientId,
            TechnicianId = dto.TechnicianId,
            ResultFileUrl = dto.FilePath,
            ResultValuesJson = dto.ResultValuesJson,
            IsAbnormal = isAbnormal,
            IsReleasedToPatient = false,
            UploadedAt = DateTime.UtcNow
        };

        order.Status = TestStatus.InProgress;
        order.FulfillingTechnicianId = dto.TechnicianId;
        order.UploadedAt = DateTime.UtcNow;

        _db.LabTestReports.Add(report);
        await _db.SaveChangesAsync();

        await _audit.LogAsync(dto.TechnicianId, "LabTestResultUploaded", "LabTestReport", report.Id);

        if (isAbnormal)
        {
            await _notifications.NotifyAsync(
                order.RequestingDoctorId,
                NotificationType.UrgentLabResult,
                "Abnormal Lab Result",
                $"Abnormal values detected for order {orderId}.",
                new[] { NotificationChannel.InApp });
        }

        return MapReportToResponse(report);
    }

    public async Task<LabTestReportResponseDto?> ReleaseToPatientAsync(Guid reportId)
    {
        var report = await _db.LabTestReports.FindAsync(reportId);
        if (report is null) return null;

        report.IsReleasedToPatient = true;
        report.ReleasedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        await _audit.LogAsync(Guid.Empty, "LabTestReportReleased", "LabTestReport", report.Id);

        return MapReportToResponse(report);
    }

    public async Task<IEnumerable<LabTestReportResponseDto>> GetReleasedForPatientAsync(Guid patientId)
    {
        var entities = await _db.LabTestReports
            .AsNoTracking()
            .Where(r => r.PatientId == patientId && r.IsReleasedToPatient)
            .OrderByDescending(r => r.ReleasedAt)
            .ToListAsync();

        return entities.Select(MapReportToResponse);
    }

    public async Task<LabTestReportResponseDto?> GetReportForDoctorAsync(Guid reportId)
    {
        var entity = await _db.LabTestReports.AsNoTracking().FirstOrDefaultAsync(r => r.Id == reportId);
        return entity is null ? null : MapReportToResponse(entity);
    }

    private static bool DetectAbnormalValues(string resultValuesJson)
    {
        try
        {
            var values = JsonSerializer.Deserialize<Dictionary<string, decimal>>(resultValuesJson);
            if (values is null) return false;

            foreach (var (key, value) in values)
            {
                if (ReferenceRanges.TryGetValue(key, out var range))
                {
                    if (value < range.Min || value > range.Max)
                        return true;
                }
            }
        }
        catch (JsonException)
        {
            // Non-numeric or malformed JSON — can't auto-detect
        }

        return false;
    }

    private static LabTestOrderResponseDto MapOrderToResponse(LabTestOrder e) => new()
    {
        Id = e.Id,
        PatientId = e.PatientId,
        DoctorId = e.RequestingDoctorId,
        TestType = e.TestCategory,
        Priority = e.Priority,
        ClinicalInstructions = e.ClinicalInstructions,
        Status = e.Status,
        CreatedAt = e.RequestedAt
    };

    private static LabTestReportResponseDto MapReportToResponse(LabTestReport e) => new()
    {
        Id = e.Id,
        OrderId = e.OrderId,
        TechnicianId = e.TechnicianId,
        FilePath = e.ResultFileUrl,
        ResultValuesJson = e.ResultValuesJson,
        IsAbnormal = e.IsAbnormal,
        IsReleasedToPatient = e.IsReleasedToPatient,
        CreatedAt = e.UploadedAt,
        ReleasedAt = e.ReleasedAt
    };
}