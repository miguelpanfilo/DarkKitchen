using AlmacenamientoAbstracciones;
using Dominio;
using LogicaDeNegocioAbstracciones;
using LogicaDeNegocioAbstracciones.Exceptions;

namespace LogicaDeNegocio;

public class SessionValidationService(
    IRepositorySession repositorySession,
    IReloj watch) : ISessionValidationService
{
    private readonly IRepositorySession _repositorySession = repositorySession;
    private readonly IReloj _watch = watch;

    public User ValidateSession(Guid sessionId)
    {
        var session = _repositorySession.FindBySessionId(sessionId);

        if(session == null)
        {
            throw new UserAuthenticationException("La sesión no existe");
        }

        if(session.ExpireDate <= _watch.Ahora())
        {
            throw new UserAuthenticationException("La sesión expiró");
        }

        return null!;
    }
}
