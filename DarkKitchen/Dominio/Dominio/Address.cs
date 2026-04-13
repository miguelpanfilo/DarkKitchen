using Dominio.Exceptions;

namespace Dominio;

public class Address
{
    public string Street { get; private set; } = string.Empty;
    public string DoorNumber { get; private set; } = string.Empty;
    public string? Apartment { get; private set; }

    private Address(string street, string doorNumber, string? apartment)
    {
        Street = street;
        DoorNumber = doorNumber;
        Apartment = apartment;
    }

    public static Address MakeAddress(string street, string doorNumber, string? apartment = null)
    {
        if(string.IsNullOrWhiteSpace(street))
        {
            throw new OrderException("La calle no puede estar vacía");
        }

        if(string.IsNullOrWhiteSpace(doorNumber))
        {
            throw new OrderException("El número de puerta no puede estar vacío");
        }

        return new Address(street.Trim(), doorNumber.Trim(), apartment?.Trim());
    }
}