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
}
