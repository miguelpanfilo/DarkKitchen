namespace Contract;

public class AuditLogFilterRequest
{
    public DateTime? DateTimeFrom { get; set; }
    public DateTime? DateTimeTo { get; set; }
    public string EntityName { get; set; } = string.Empty;
    public int? EntityId { get; set; }
}
