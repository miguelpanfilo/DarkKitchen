namespace LogicaDeNegocioAbstracciones.Exceptions;

public class NotFoundException : LogicException
{
    public NotFoundException(string message)
        : base(message) { }
}
