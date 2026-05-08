using Domain;

namespace DataAccessAbstractions;

public interface IAuditLogRepository
{
    void Add(AuditLog auditLog);
    List<AuditLog> GetByFilters(DateTime dateTimeFrom, DateTime
        dateTimeTo, string entityName, int entityId);
}
