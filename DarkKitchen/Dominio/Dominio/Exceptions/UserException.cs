namespace Dominio.Excepciones;

public class UserException : Exception
{
    public UserException(string message)
        : base(message)
    {
    }
}
