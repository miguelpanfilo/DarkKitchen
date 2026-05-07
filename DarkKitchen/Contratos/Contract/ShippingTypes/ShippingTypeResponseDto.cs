namespace Contract.ShippingTypes;

public class ShippingTypeResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Cost { get; set; }
}
