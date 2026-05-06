using Contract;

namespace BusinessLogicsAbstractions;

public interface IAuditLogService
{
    void Register(string entityName, int entityId, string
        description);
    List<AuditLogResponse> GetByFilters(AuditLogFilterRequest
        request);
}
