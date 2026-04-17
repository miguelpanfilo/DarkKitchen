using Contratos;

namespace LogicaDeNegocioAbstracciones;

public interface ISessionsValidationService
{
    CurrentUserDto ValidateSession(Guid sessionId);
}
