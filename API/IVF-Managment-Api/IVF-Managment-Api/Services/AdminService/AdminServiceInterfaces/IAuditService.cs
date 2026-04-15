using IVFClinic.Models.Audit;
using IVFClinic.Models.Enums;

namespace IVFClinic.Services.Interfaces
{
    public interface IAuditService
    {
        Task LogActionAsync(
            Guid userId,
            AuditAction action,
            string entityType,
            string? entityId = null,
            object? previousValues = null,
            object? newValues = null,
            string? description = null,
            string? ipAddress = null);

        Task<IEnumerable<AuditLog>> GetLogsAsync(
            Guid? userId = null,
            string? entityType = null,
            DateTime? from = null,
            DateTime? to = null,
            int page = 1,
            int pageSize = 50);
    }
}
