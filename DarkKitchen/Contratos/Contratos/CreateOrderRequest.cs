using Dominio;

namespace Contratos;

public class CreateOrderRequest
{
    public DeliveryType DeliveryType { get; set; }
    public CreateAddressRequest Address { get; set; } = new();
    public List<CreateOrderItemRequest> Items { get; set; } = [];
}
