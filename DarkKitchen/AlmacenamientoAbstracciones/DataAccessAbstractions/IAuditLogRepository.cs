using Domain;

namespace DataAccessAbstractions;

public interface IAuditLogRepository
{
    void Add(AuditLog auditLog);
}
