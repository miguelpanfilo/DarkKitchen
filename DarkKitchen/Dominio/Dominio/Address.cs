using Dominio.Exceptions;

namespace Dominio;

public class Address
{
    private const string EmptyStreetMessage = "La calle no puede estar vacía";
    private const string EmptyDoorNumberMessage = "El número de puerta no puede estar vacío";

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
        var trimmedStreet = street?.Trim();
        var trimmedDoorNumber = doorNumber?.Trim();
        var trimmedApartment = apartment?.Trim();

        ValidateStreet(trimmedStreet);
        ValidateDoorNumber(trimmedDoorNumber);

        return new Address(trimmedStreet!, trimmedDoorNumber!, trimmedApartment);
    }

    private static void ValidateStreet(string? street)
    {
        if (string.IsNullOrWhiteSpace(street))
        {
            throw new OrderException(EmptyStreetMessage);
        }
    }

    private static void ValidateDoorNumber(string? doorNumber)
    {
        if (string.IsNullOrWhiteSpace(doorNumber))
        {
            throw new OrderException(EmptyDoorNumberMessage);
        }
    }
}
