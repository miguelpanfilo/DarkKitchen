using DataAccessAbstractions;
using Domain;

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
        throw new NotImplementedException();
    }
}
