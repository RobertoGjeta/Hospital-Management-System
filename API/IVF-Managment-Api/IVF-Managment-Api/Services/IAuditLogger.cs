namespace IVF_Managment_Api.Services;

public interface IAuditLogger
{
    Task LogAsync(Guid userId, string action, string entityType, Guid? entityId, string? before = null, string? after = null);
}