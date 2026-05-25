namespace Tests.Orders;

using Domain;
using FluentAssertions;

[TestClass]
public class OrderTransitionStateTests
{
    private static Order MakeOrder()
    {
        var address = Address.MakeAddress("Rivera", "1234");

        var items = new List<OrderItemData>
        {
            new() { ProductId = 1, Quantity = 2, UnitPrice = 100m, DiscountApplied = 0m }
        };

        return Order.MakeOrder(1, "Express", address, items, 200m);
    }

    [TestMethod]
    public void Order_WhenDelay_ShouldHaveDelayedStatus()
    {
        var order = MakeOrder();

        order.Delay();

        order.Status.Should().Be(OrderStatus.Delayed);
    }

    [TestMethod]
    public void
        Order_WhenPrepareFromDelayed_ShouldHavePreparedStatus()
    {
        var order = MakeOrder();
        order.Delay();

        order.Prepare();

        order.Status.Should().Be(OrderStatus.Prepared);
    }
}
