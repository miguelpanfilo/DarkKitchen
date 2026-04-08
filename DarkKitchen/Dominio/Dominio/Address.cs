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
            throw new OrderException("Street cannot be empty");
        }

        if(string.IsNullOrWhiteSpace(doorNumber))
        {
            throw new OrderException("Door number cannot be empty");
        }

        return new Address(street.Trim(), doorNumber.Trim(), apartment?.Trim());
    }
}
