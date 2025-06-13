using System.Text.Json;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using RealEstateApi.Models;

public class AuditEntry
{
    public AuditEntry(EntityEntry entry)
    {
        Entry = entry;
    }

    public EntityEntry Entry { get; }
    public string? UserId { get; set; }
    public string TableName { get; set; } = string.Empty;
    public string ActionType { get; set; } = string.Empty;
    public Dictionary<string, object?> KeyValues { get; } = new();
    public Dictionary<string, object?> OldValues { get; } = new();
    public Dictionary<string, object?> NewValues { get; } = new();

    public AuditLog ToAuditLog()
    {
        return new AuditLog
        {
            UserId = UserId,
            TableName = TableName,
            ActionType = ActionType,
            PrimaryKey = JsonSerializer.Serialize(KeyValues),
            OldValues = OldValues.Count == 0 ? null : JsonSerializer.Serialize(OldValues),
            NewValues = NewValues.Count == 0 ? null : JsonSerializer.Serialize(NewValues),
            Timestamp = DateTime.UtcNow
        };
    }
}
