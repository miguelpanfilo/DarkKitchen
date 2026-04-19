using AlmacenamientoAbstracciones;
using Contratos.Users;
using Dominio;
using LogicaDeNegocioAbstracciones.Exceptions;
using LogicaDeNegocioAbstracciones.Users;
using Mapeo;

namespace LogicaDeNegocio.Sessions;

public class SessionsValidationService(
    IRepositorySession repositorySession,
    IRepositoryUser repositoryUser,
    IWatch watch) : ISessionsValidationService
{
    private const string SessionNotFoundMessage = "La sesión no existe";
    private const string SessionExpiredMessage = "La sesión expiró";
    private const string SessionUserNotFoundMessage = "El usuario de la sesión no existe";

    private readonly IRepositorySession _repositorySession = repositorySession;
    private readonly IRepositoryUser _repositoryUser = repositoryUser;
    private readonly IWatch _watch = watch;

    public CurrentUserDto ValidateSession(Guid sessionId)
    {
        var session = GetSessionOrThrow(sessionId);
        ValidateSessionIsNotExpired(session);

        var user = GetUserOrThrow(session.UserId);

        return user.ToCurrentUserDto();
    }

    private Session GetSessionOrThrow(Guid sessionId)
    {
        var session = _repositorySession.FindBySessionId(sessionId);

        if(session == null)
        {
            throw new UserAuthenticationException(SessionNotFoundMessage);
        }

        return session;
    }

    private void ValidateSessionIsNotExpired(Session session)
    {
        if(session.ExpireDate <= _watch.Now())
        {
            throw new UserAuthenticationException(SessionExpiredMessage);
        }
    }

    private User GetUserOrThrow(int userId)
    {
        var user = _repositoryUser.FindById(userId);

        if(user == null)
        {
            throw new UserAuthenticationException(SessionUserNotFoundMessage);
        }

        return user;
    }
}
