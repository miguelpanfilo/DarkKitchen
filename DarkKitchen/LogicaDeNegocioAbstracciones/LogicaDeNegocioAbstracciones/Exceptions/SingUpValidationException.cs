namespace LogicaDeNegocioAbstracciones.Exceptions;

public class SignUpValidationException : Exception
{
    public SignUpValidationException(string message)
        : base(message)
    {
    }
}
