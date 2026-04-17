namespace Dominio.Exceptions;

public class ProductException : Exception
{
    public ProductException(string message)
        : base(message)
    {
    }
}
