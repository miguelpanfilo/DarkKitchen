using AlmacenamientoAbstracciones;
using Dominio;

namespace Almacenamiento;

public class RepositorySession : IRepositorySession
{
    private readonly List<Session> _sessions = [];

    public Session Add(Session session)
    {
        _sessions.Add(session);
        return session;
    }

    public Session? FindBySessionId(Guid sessionId)
    {
        return _sessions.FirstOrDefault(s => s.SessionId == sessionId);
    }
}
