namespace Domain;

public class AuditLog
{
    public int Id { get; init; }
    public DateTime DateTime { get; init; } = DateTime.UtcNow;
    public string EntityName { get; init; } = string.Empty;
    public int EntityId { get; init; }
    public string ChangeDescription { get; init; } = string.Empty;
    public string ResponsibleUser { get; init; } = "No hay usuario llogueado";
}
