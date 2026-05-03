namespace Contract.Orders;

public class AddressResponse
{
    public string Street { get; set; } = string.Empty;
    public string DoorNumber { get; set; } = string.Empty;
    public string? Apartment { get; set; }
}
