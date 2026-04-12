using AlmacenamientoAbstracciones;
using Dominio;
using LogicaDeNegocioAbstracciones;
using LogicaDeNegocioAbstracciones.Exceptions;

namespace LogicaDeNegocio;

public class SessionValidationService(IRepositorySession repositorySession)
    : ISessionValidationService
{
    private readonly IRepositorySession _repositorySession = repositorySession;

    public User ValidateSession(Guid sessionId)
    {
        var session = _repositorySession.FindBySessionId(sessionId);

        if(session == null)
        {
            throw new UserAuthenticationException("La sesión no existe");
        }

        return null!;
    }
}
