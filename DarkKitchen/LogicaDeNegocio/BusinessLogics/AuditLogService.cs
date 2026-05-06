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

        if (!request.DateTimeTo.HasValue)
        {
            throw new ValidationException("La fecha-hora hasta es obligatoria.");
        }

        if (request.DateTimeFrom.Value >= request.DateTimeTo.Value)
        {
            throw new ValidationException("La fecha-hora desde debe ser menor que la fecha-hora hasta.");
        }

        if (string.IsNullOrWhiteSpace(request.EntityName))
        {
            throw new ValidationException("La entidad es obligatoria.");
        }

        if (!request.EntityId.HasValue || request.EntityId.Value <= 0)
        {
            throw new ValidationException("El id de la entidad es obligatorio.");
        }

        return repository.GetByFilters(
                request.DateTimeFrom!.Value,
                request.DateTimeTo!.Value,
                request.EntityName,
                request.EntityId!.Value)
            .Select(log => new AuditLogResponse
            {
                Id = log.Id,
                DateTime = log.DateTime,
                EntityName = log.EntityName,
                EntityId = log.EntityId,
                ChangeDescription = log.ChangeDescription,
                ResponsibleUser = log.ResponsibleUser
            })
            .ToList();
    }
}
