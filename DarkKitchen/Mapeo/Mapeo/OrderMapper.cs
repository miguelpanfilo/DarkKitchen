using Contratos;
using Contratos.Orders;
using Dominio;

namespace Mapeo;

public static class OrderMapper
{
    public static OrderResponse ToOrderResponse(this Order order)
    {
        return new OrderResponse
        {
            Id = order.Id,
            OrderNumber = order.OrderNumber,
            ClientId = order.ClientId,
            DeliveryType = order.DeliveryType.ToString(),
            Address = order.Address.ToAddressResponse(),
            Items = order.Items.Select(i => i.ToOrderItemResponse()).ToList(),
            ItemsCount = order.Items.Count,
            Status = order.Status.ToString(),
            CreatedAt = order.CreatedAt,
            StatusUpdatedAt = order.StatusUpdatedAt,
            Subtotal = order.Subtotal,
            ShippingCost = order.ShippingCost,
            Total = order.Total
        };
    }

    private static AddressResponse ToAddressResponse(this Address address)
    {
        return new AddressResponse
        {
            Street = address.Street,
            DoorNumber = address.DoorNumber,
            Apartment = address.Apartment
        };
    }

    private static OrderItemResponse ToOrderItemResponse(this OrderItem item)
    {
        return new OrderItemResponse
        {
            Id = item.Id,
            ProductId = item.ProductId,
            Quantity = item.Quantity,
            UnitPrice = item.UnitPrice,
            DiscountApplied = item.DiscountApplied,
            LineTotal = item.LineTotal
        };
    }
}
