using Contratos.Users;

namespace LogicaDeNegocioAbstracciones.Users;

public interface ISessionsValidationService
{
    CurrentUserDto ValidateSession(Guid sessionId);
}
