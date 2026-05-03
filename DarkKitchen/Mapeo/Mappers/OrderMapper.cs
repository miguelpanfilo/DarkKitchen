using Contract.Orders;
using Contract.Reports;
using Domain;

namespace Mappers;

public static class OrderMapper
{
    public static OrderResponse ToOrderResponse(this Order order, User user)
    {
        var iva = CalculateIva(order);

        return new OrderResponse
        {
            Id = order.Id,
            OrderNumber = order.OrderNumber,
            Client = MapClient(user),
            DeliveryType = order.DeliveryType.ToString(),
            Address = order.Address.ToAddressResponse(),
            Items = order.Items.Select(i => i.ToOrderItemResponse()).ToList(),

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

    private const decimal VatPercentage = 22m;

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

    private static OrderItemResponse ToOrderItemResponse(this OrderItem item)
    {
        return new OrderItemResponse
        {
            ProductId = item.ProductId,
            Quantity = item.Quantity,
            UnitPrice = Math.Round(item.UnitPrice, 2),
            DiscountApplied = Math.Round(item.DiscountApplied, 2),
            LineTotal = Math.Round(item.LineTotal, 2)
        };
    }

    public static List<TopProductReportItemDto> ToTopProductsReport(this List<Order> orders)
    {
        return orders
            .SelectMany(o => o.Items)
            .GroupBy(i => i.ProductId)
            .Select(g => new
            {
                Product = g.First().Product!,
                QuantitySold = g.Sum(i => i.Quantity)
            })
            .OrderByDescending(x => x.QuantitySold)
            .Take(5)
            .Select(x => new TopProductReportItemDto
            {
                Code = x.Product.Code,
                Name = x.Product.Name,
                QuantitySold = x.QuantitySold,
                Images = x.Product.Photos.Select(p => p.Path).ToList()
            })
            .ToList();
    }

    public static SalesReportDto ToSalesReport(this List<Order> orders)
    {
        var periods = orders
            .GroupBy(o => new { o.CreatedAt.Year, o.CreatedAt.Month })
            .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
            .Select(g => new SalesPeriodDto
            {
                Year = g.Key.Year,
                Month = g.Key.Month,
                Customers = g
                    .GroupBy(o => o.ClientId)
                    .Select(cg => new CustomerSalesDto
                    {
                        Customer = $"{cg.First().Client!.Name} {cg.First().Client!.LastName}",
                        TotalSales = cg.Sum(o => o.Total)
                    })
                    .OrderBy(c => c.Customer)
                    .ToList(),
                MonthTotal = g.Sum(o => o.Total)
            })
            .ToList();

        return new SalesReportDto
        {
            Periods = periods,
            GrandTotal = periods.Sum(p => p.MonthTotal)
        };
    }
}
