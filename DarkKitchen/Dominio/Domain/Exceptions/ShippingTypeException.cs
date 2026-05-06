namespace Domain.Exceptions;

public class ShippingTypeException : Exception
{
    public ShippingTypeException(string message)
        : base(message)
    {
    }
}
