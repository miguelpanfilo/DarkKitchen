namespace LogicaDeNegocioAbstracciones;

public interface ISessionValidationService
{
    CurrentUserDto ValidateSession(Guid sessionId);
}
