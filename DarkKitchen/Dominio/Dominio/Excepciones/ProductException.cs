namespace Dominio.Excepciones;

public class ProductException : Exception
{
    public ProductException(string message)
        : base(message)
    {
    }
}
