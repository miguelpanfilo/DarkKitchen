using BusinessLogicsAbstractions;
using BusinessLogicsAbstractions.Exceptions;
using Contract;
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

    public List<AuditLogResponse> GetByFilters(AuditLogFilterRequest
        request)
    {
        if (!request.DateTimeFrom.HasValue)
        {
            throw new ValidationException("La fecha-hora desde es obligatoria.");
        }

        return new List<AuditLogResponse>();
    }
}
