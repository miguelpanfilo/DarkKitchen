namespace LogicaDeNegocioAbstracciones.Exceptions;

public class ForbiddenException : LogicException
{
    public ForbiddenException(string message)
        : base(message) { }
}
