using IVFClinic.DTOs.Report;

namespace IVFClinic.Services.Interfaces
{
    public interface IReportService
    {
        Task<OccupancyReportDto> GenerateOccupancyReportAsync(DateTime start, DateTime end, ReportFilters? filters);
        Task<RevenueReportDto> GenerateRevenueReportAsync(DateTime start, DateTime end, ReportFilters? filters);
        Task<UtilizationReportDto> GenerateUtilizationReportAsync(DateTime start, DateTime end, ReportFilters? filters);
        Task<byte[]> ExportReportAsPdfAsync(Guid reportId, Guid adminId);
        Task<IEnumerable<ReportHistoryDto>> GetReportHistoryAsync(int page, int pageSize);
    }

    public class ReportFilters
    {
        public Guid? DoctorId { get; set; }
        public string? Department { get; set; }
        public string? ServiceCategory { get; set; }
    }
}
