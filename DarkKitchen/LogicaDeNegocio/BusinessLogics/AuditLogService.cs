using BusinessLogicsAbstractions;
using DataAccessAbstractions;
using Domain;

namespace BusinessLogics;

public class AuditLogService(IAuditLogRepository repository,
    ICurrentUserService currentUserService)
{
    public void Register(string entityName, int entityId, string
        description)
    {
        var auditLog = new AuditLog
        {
            EntityName = entityName,
            EntityId = entityId,
            ChangeDescription = description,
            ResponsibleUser = currentUserService.GetEmail()
        };

        repository.Add(auditLog);
    }
}
