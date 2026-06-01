using Contract.Orders;
using Contract.Reports;
using Domain;

namespace Mappers;

public static class OrderMapper
{
    public static OrderResponse ToOrderResponse(this Order order, User user, Dictionary<int, string> productNames)
    {
        var iva = CalculateIva(order);

        return new OrderResponse
        {
            Id = order.Id,
            OrderNumber = order.OrderNumber,
            Client = MapClient(user),
            DeliveryType = order.DeliveryType,
            Address = order.Address.ToAddressResponse(),
            Items = order.Items.Select(i => i.ToOrderItemResponse(productNames)).ToList(),

            ItemsCount = CalculateItemsCount(order),

            Status = order.Status.ToString(),
            CreatedAt = order.CreatedAt,
            StatusUpdatedAt = order.StatusUpdatedAt,

            Subtotal = Round(order.Subtotal),
            ShippingCost = Round(order.ShippingCost),

            Iva = iva,
            IvaPercentage = VatPercentage,

            Total = Round(order.Total),
        };
    }

    private const decimal VatPercentage = TaxPolicy.VatPercentage;

    private static decimal CalculateIva(Order order)
    {
        return Round(order.Total - (order.Subtotal + order.ShippingCost));
    }

    private static int CalculateItemsCount(Order order)
    {
        return order.Items.Sum(i => i.Quantity);
    }

    private static decimal Round(decimal value)
    {
        return Math.Round(value, 2);
    }

    private static ClientOrderResponse MapClient(User user)
    {
        return new ClientOrderResponse
        {
            Id = user.Id,
            Name = user.Name,
            LastName = user.LastName,
            Email = user.Email.ToString()
        };
    }

    private static AddressResponse ToAddressResponse(this Address address)
    {
        return new AddressResponse
        {
            Street = address.Street, DoorNumber = address.DoorNumber, Apartment = address.Apartment
        };
    }

    private static OrderItemResponse ToOrderItemResponse(this OrderItem item, Dictionary<int, string> productNames)
    {
        return new OrderItemResponse
        {
            ProductId = item.ProductId,
            ProductName = productNames.TryGetValue(item.ProductId, out var name) ? name : string.Empty,
            Quantity = item.Quantity,
            UnitPrice = Math.Round(item.UnitPrice, 2),
            DiscountApplied = Math.Round(item.DiscountApplied, 2),
            LineTotal = Math.Round(item.LineTotal, 2)
        };
    }
}
