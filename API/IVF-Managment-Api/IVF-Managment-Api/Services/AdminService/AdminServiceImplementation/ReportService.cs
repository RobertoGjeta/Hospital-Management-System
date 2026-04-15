using Microsoft.EntityFrameworkCore;
using IVFClinic.Data;
using IVFClinic.DTOs.Report;
using IVFClinic.Models;
using IVFClinic.Models.Enums;
using IVFClinic.Services.Interfaces;

namespace IVFClinic.Services.Implementations
{
    public class ReportService : IReportService
    {
        private readonly AppDbContext _db;
        private readonly IAuditService _audit;

        public ReportService(AppDbContext db, IAuditService audit)
        {
            _db = db;
            _audit = audit;
        }

        public async Task<OccupancyReportDto> GenerateOccupancyReportAsync(
            DateTime start, DateTime end, ReportFilters? filters)
        {
            ValidateDateRange(start, end);

            var totalRooms = await _db.Rooms.CountAsync(r => r.Status != RoomStatus.Maintenance);
            var totalSlots = totalRooms * (end - start).TotalHours;

            var bookedHours = await _db.Appointments
                .Where(a => a.RoomId.HasValue
                    && a.Status != AppointmentStatus.Cancelled
                    && a.ScheduledStart >= start
                    && a.ScheduledEnd <= end)
                .SumAsync(a => EF.Functions.DateDiffHour(a.ScheduledStart, a.ScheduledEnd));

            var occupancyRate = totalSlots > 0 ? (bookedHours / totalSlots) * 100 : 0;

            // Peak hours: count appointments per hour
            var peakHours = await _db.Appointments
                .Where(a => a.ScheduledStart >= start && a.ScheduledEnd <= end
                    && a.Status != AppointmentStatus.Cancelled)
                .GroupBy(a => a.ScheduledStart.Hour)
                .Select(g => new PeakHourDto { Hour = g.Key, AppointmentCount = g.Count() })
                .OrderByDescending(p => p.AppointmentCount)
                .Take(5)
                .ToListAsync();

            var report = new OccupancyReportDto
            {
                StartDate = start,
                EndDate = end,
                TotalRooms = totalRooms,
                AverageOccupancyRate = (decimal)occupancyRate,
                BookedHours = bookedHours,
                PeakHours = peakHours,
                GeneratedAt = DateTime.UtcNow
            };

            await SaveReportHistoryAsync("Occupancy", start, end, filters);
            return report;
        }

        public async Task<RevenueReportDto> GenerateRevenueReportAsync(
            DateTime start, DateTime end, ReportFilters? filters)
        {
            ValidateDateRange(start, end);

            var totalRevenue = await _db.Payments
                .Where(p => p.PaidAt >= start && p.PaidAt <= end && !p.IsRefund)
                .SumAsync(p => p.Amount);

            var byCategory = await _db.InvoiceItems
                .Include(ii => ii.Service)
                .Where(ii => ii.Invoice!.CreatedAt >= start && ii.Invoice.CreatedAt <= end)
                .GroupBy(ii => ii.Service!.Category)
                .Select(g => new CategoryRevenueDto
                {
                    Category = g.Key,
                    Revenue = g.Sum(x => x.LineTotal),
                    ItemCount = g.Count()
                })
                .ToListAsync();

            var outstanding = await _db.Invoices
                .Where(i => (i.Status == BillStatus.Pending || i.Status == BillStatus.PartiallyPaid)
                    && i.CreatedAt >= start && i.CreatedAt <= end)
                .SumAsync(i => i.TotalDue - i.AmountPaid);

            var report = new RevenueReportDto
            {
                StartDate = start,
                EndDate = end,
                TotalRevenue = totalRevenue,
                OutstandingAmount = outstanding,
                RevenueByCategory = byCategory,
                GeneratedAt = DateTime.UtcNow
            };

            await SaveReportHistoryAsync("Revenue", start, end, filters);
            return report;
        }

        public async Task<UtilizationReportDto> GenerateUtilizationReportAsync(
            DateTime start, DateTime end, ReportFilters? filters)
        {
            ValidateDateRange(start, end);

            var serviceUsage = await _db.RenderedServices
                .Include(rs => rs.Service)
                .Where(rs => rs.RenderedAt >= start && rs.RenderedAt <= end)
                .GroupBy(rs => new { rs.ServiceId, rs.Service!.Name, rs.Service.Category })
                .Select(g => new ServiceUsageDto
                {
                    ServiceId = g.Key.ServiceId,
                    ServiceName = g.Key.Name,
                    Category = g.Key.Category,
                    UsageCount = g.Sum(x => x.Quantity)
                })
                .OrderByDescending(s => s.UsageCount)
                .ToListAsync();

            var doctorWorkload = await _db.Appointments
                .Include(a => a.Doctor)
                .Where(a => a.ScheduledStart >= start && a.ScheduledEnd <= end
                    && a.Status != AppointmentStatus.Cancelled)
                .GroupBy(a => new { a.DoctorId, a.Doctor!.FirstName, a.Doctor.LastName })
                .Select(g => new DoctorWorkloadDto
                {
                    DoctorId = g.Key.DoctorId,
                    DoctorName = $"{g.Key.FirstName} {g.Key.LastName}",
                    AppointmentCount = g.Count()
                })
                .OrderByDescending(d => d.AppointmentCount)
                .ToListAsync();

            var report = new UtilizationReportDto
            {
                StartDate = start,
                EndDate = end,
                MostUsedServices = serviceUsage.Take(10).ToList(),
                LeastUsedServices = serviceUsage.TakeLast(5).ToList(),
                DoctorWorkload = doctorWorkload,
                GeneratedAt = DateTime.UtcNow
            };

            await SaveReportHistoryAsync("Utilization", start, end, filters);
            return report;
        }

        public async Task<byte[]> ExportReportAsPdfAsync(Guid reportId, Guid adminId)
        {
            await _audit.LogActionAsync(adminId, AuditAction.GenerateReport, "Report",
                reportId.ToString(), description: "Exported report as PDF");

            // TODO: Use a library like QuestPDF or iText to generate the PDF
            // For now, return a placeholder
            return Array.Empty<byte>();
        }

        public async Task<IEnumerable<ReportHistoryDto>> GetReportHistoryAsync(int page, int pageSize)
        {
            return await _db.ReportHistory
                .OrderByDescending(r => r.GeneratedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(r => new ReportHistoryDto
                {
                    Id = r.Id,
                    Type = r.Type,
                    StartDate = r.StartDate,
                    EndDate = r.EndDate,
                    GeneratedAt = r.GeneratedAt
                })
                .ToListAsync();
        }

        private void ValidateDateRange(DateTime start, DateTime end)
        {
            if (end < start)
                throw new ArgumentException("End date must be after start date");

            if ((end - start).TotalDays > 730)
                throw new ArgumentException("Date range cannot exceed 2 years");
        }

        private async Task SaveReportHistoryAsync(string type, DateTime start, DateTime end, ReportFilters? filters)
        {
            _db.ReportHistory.Add(new ReportHistory
            {
                Id = Guid.NewGuid(),
                Type = type,
                StartDate = start,
                EndDate = end,
                Filters = filters != null ? System.Text.Json.JsonSerializer.Serialize(filters) : null,
                GeneratedAt = DateTime.UtcNow
            });
            await _db.SaveChangesAsync();
        }
    }
}
