namespace RealEstateApi.Models
{
    public class AuditLog
{
    public Guid Id { get; set; }
    public string TableName { get; set; } = string.Empty;
    public string? UserId { get; set; } // From JWT Claim
    public string ActionType { get; set; } = string.Empty; // Create, Update, Delete
    public string? OldValues { get; set; }
    public string? NewValues { get; set; }
    public string? PrimaryKey { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

}