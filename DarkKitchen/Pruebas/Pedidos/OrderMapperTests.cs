using Dominio;
using FluentAssertions;
using Mapeo;

namespace Pruebas.Pedidos;

[TestClass]
public sealed class OrderMapperTests
{
    private static Order MakeOrder(int itemCount = 1)
    {
        var address = Address.MakeAddress("Rivera", "1234");
        var items = Enumerable.Range(1, itemCount)
            .Select(i => (ProductId: i, Quantity: 1, UnitPrice: 100m, DiscountApplied: 0m))
            .ToList();

        return Order.MakeOrder(1, DeliveryType.Express, address, items, 200m);
    }

    #region ToOrderResponse
    #region Success
    [TestMethod]
    public void ToOrderResponse_WhenOrderHasItems_ShouldMapItemsCount()
    {
        var order = MakeOrder(itemCount: 3);

        var result = order.ToOrderResponse();

        result.ItemsCount.Should().Be(3);
    }

    [TestMethod]
    public void ToOrderResponse_WhenOrderHasSingleItem_ShouldMapAllFields()
    {
        var order = MakeOrder();

        var result = order.ToOrderResponse();

        result.Should().NotBeNull();
        result.ClientId.Should().Be(1);
        result.DeliveryType.Should().Be(nameof(DeliveryType.Express));
        result.Status.Should().Be(nameof(OrderStatus.Pending));
        result.Items.Should().HaveCount(1);
        result.ItemsCount.Should().Be(1);
        result.Subtotal.Should().BeGreaterThan(0);
        result.Total.Should().BeGreaterThan(0);
    }
    #endregion
    #endregion
}
