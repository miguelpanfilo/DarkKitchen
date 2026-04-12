namespace LogicaDeNegocioAbstracciones;

public interface ISessionValidationService
{
    void ValidateSession(Guid sessionId);
}
