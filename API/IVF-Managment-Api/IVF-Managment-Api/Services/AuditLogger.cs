using IVF_Managment_Api.Data;
using IVF_Managment_Api.Models.HelperModels;

namespace IVF_Managment_Api.Services;

public class AuditLogger : IAuditLogger
{
    private readonly IvfDbContext _db;

    public AuditLogger(IvfDbContext db) => _db = db;

    public async Task LogAsync(Guid userId, string action, string entityType, Guid? entityId, string? before = null, string? after = null)
    {
        var entry = new AuditLogEntry
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Action = action,
            EntityType = entityType,
            EntityId = entityId,
            BeforeJson = before,
            AfterJson = after,
            Timestamp = DateTime.UtcNow
        };

        _db.AuditLogEntries.Add(entry);
        await _db.SaveChangesAsync();
    }
}