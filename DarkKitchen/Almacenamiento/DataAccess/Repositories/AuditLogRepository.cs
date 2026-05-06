using DataAccessAbstractions;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories;

public class AuditLogRepository : IAuditLogRepository
{
    private readonly SqlContext _context;

    public AuditLogRepository(SqlContext context)
    {
        _context = context;
    }

    public void Add(AuditLog auditLog)
    {
        _context.AuditLogs.Add(auditLog);
        _context.SaveChanges();
    }

    public List<AuditLog> GetByFilters(DateTime dateTimeFrom, DateTime
        dateTimeTo, string entityName, int entityId)
    {
        return _context.AuditLogs
            .AsNoTracking()
            .Where(log =>
                log.DateTime >= dateTimeFrom &&
                log.DateTime <= dateTimeTo &&
                log.EntityName.ToLower().Contains(entityName.ToLower()) &&
                log.EntityId == entityId)
            .OrderBy(log => log.DateTime)
            .ToList();
    }
}
