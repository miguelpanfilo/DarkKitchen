using AlmacenamientoAbstracciones;
using Dominio;
using LogicaDeNegocioAbstracciones;
using LogicaDeNegocioAbstracciones.Exceptions;

namespace LogicaDeNegocio;

public class SessionValidationService(
    IRepositorySession repositorySession,
    IRepositoryUser repositoryUser,
    IReloj watch) : ISessionValidationService
{
    private readonly IRepositorySession _repositorySession = repositorySession;
    private readonly IRepositoryUser _repositoryUser = repositoryUser;
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

        var user = _repositoryUser.FindById(session.UserId);

        if(user == null)
        {
            throw new UserAuthenticationException("El usuario de la sesión no existe");
        }

        return user;
    }
}
