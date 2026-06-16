using DataAccessAbstractions;
using Domain;

namespace DataAccess.Repositories;

public class RepositorySession(SqlContext context) : IRepositorySession
{
    private readonly SqlContext _context = context;

    public Session Add(Session session)
    {
        _context.Sessions.Add(session);
        _context.SaveChanges();
        return session;
    }

    public Session? FindBySessionId(Guid sessionId)
    {
        return _context.Sessions
            .SingleOrDefault(s => s.SessionId == sessionId);
    }

    public void RemoveAll()
    {
        _context.Sessions.RemoveRange(_context.Sessions);
        _context.SaveChanges();
    }
}
