namespace LogicaDeNegocioAbstracciones.Exceptions;

public class ValidationException : LogicException
{
    public ValidationException(string message)
        : base(message) { }
}
