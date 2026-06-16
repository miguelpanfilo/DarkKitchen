namespace Contract;

public class AuditLogResponse
{
    public DateTime DateTime { get; set; }
    public string EntityName { get; set; } = string.Empty;
    public int EntityId { get; set; }
    public string ChangeDescription { get; set; } = string.Empty;
    public string ResponsibleUser { get; set; } = string.Empty;
}
