namespace LogicaDeNegocioAbstracciones.Exceptions;

public class ConflictException : LogicException
{
    public ConflictException(string message)
        : base(message) { }
}
