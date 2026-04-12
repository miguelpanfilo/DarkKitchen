using Dominio;

namespace LogicaDeNegocioAbstracciones;

public interface ISessionValidationService
{
    User ValidateSession(Guid sessionId);
}
