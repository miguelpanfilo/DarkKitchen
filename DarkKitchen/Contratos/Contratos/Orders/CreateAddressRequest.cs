namespace Contratos.Orders;

public class CreateAddressRequest
{
    public string Street { get; set; } = string.Empty;
    public string DoorNumber { get; set; } = string.Empty;
    public string? Apartment { get; set; }
}
