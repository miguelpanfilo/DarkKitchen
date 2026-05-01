using Domain;

namespace DataAccessAbstractions;

public interface IRepositorySession
{
    Session Add(Session session);
    Session? FindBySessionId(Guid sessionId);
    void RemoveAll();
}
