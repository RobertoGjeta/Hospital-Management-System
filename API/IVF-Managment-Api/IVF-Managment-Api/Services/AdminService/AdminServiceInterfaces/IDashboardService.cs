using IVFClinic.DTOs.Dashboard;

namespace IVFClinic.Services.Interfaces
{
    public interface IDashboardService
    {
        Task<DashboardSummaryDto> GetDashboardSummaryAsync(Guid adminId);
    }
}
