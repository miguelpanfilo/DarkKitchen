namespace LogicaDeNegocioAbstracciones.Exceptions;

public class UserAuthenticationException : LogicException
{
    public UserAuthenticationException(string message)
        : base(message)
    {
    }
}
