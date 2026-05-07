using Contract;

namespace BusinessLogicsAbstractions;

public interface IAuditLogService
{
    void Register(string entityName, int entityId, string description, string responsibleUser);
    List<AuditLogResponse> GetByFilters(AuditLogFilterRequest
        request);
}
