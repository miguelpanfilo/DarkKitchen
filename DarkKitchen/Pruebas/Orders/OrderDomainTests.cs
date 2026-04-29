using Dominio;
using Dominio.Exceptions;
using FluentAssertions;

namespace Pruebas.Orders;

[TestClass]
public sealed class OrderDomainTests
{
    private static List<OrderItemData> MakeItems()
    {
        return
        [
            new OrderItemData
            {
                ProductId = 1,
                Quantity = 2,
                UnitPrice = 100m,
                DiscountApplied = 0m
            }

        ];
    }

    #region Address
    #region Success
    [TestMethod]
    public void MakeAddress_WhenApartmentProvided_ShouldSetApartment()
    {
        var address = Address.MakeAddress("Rivera", "1234", "3A");

        address.Apartment.Should().Be("3A");
    }
    #endregion

    #region Error
    [TestMethod]
    public void MakeAddress_WhenStreetIsEmpty_ShouldThrowOrderException()
    {
        var act = () => Address.MakeAddress(string.Empty, "1234");

        act.Should().Throw<OrderException>().WithMessage("La calle no puede estar vacía");
    }

    [TestMethod]
    public void MakeAddress_WhenDoorNumberIsEmpty_ShouldThrowOrderException()
    {
        var act = () => Address.MakeAddress("Rivera", string.Empty);

        act.Should().Throw<OrderException>().WithMessage("El número de puerta no puede estar vacío");
    }
    #endregion
    #endregion

    #region Order
    #region Error
    [TestMethod]
    public void MakeOrder_WhenNoItems_ShouldThrowOrderException()
    {
        var address = Address.MakeAddress("Rivera", "1234");

        var act = () => Order.MakeOrder(1, DeliveryType.Express, address, [], 200m);

        act.Should().Throw<OrderException>().WithMessage("El pedido debe tener al menos un producto");
    }

    [TestMethod]
    public void Prepare_WhenOrderIsNotPending_ShouldThrowOrderException()
    {
        var order = Order.MakeOrder(1, DeliveryType.Express, Address.MakeAddress("Rivera", "1234"), MakeItems(), 200m);
        order.Prepare();

        var act = order.Prepare;

        act.Should().Throw<OrderException>();
    }

    [TestMethod]
    public void Cancel_WhenOrderIsNotPending_ShouldThrowOrderException()
    {
        var order = Order.MakeOrder(1, DeliveryType.Express, Address.MakeAddress("Rivera", "1234"), MakeItems(), 200m);
        order.Prepare();

        var act = order.Cancel;

        act.Should().Throw<OrderException>();
    }

    [TestMethod]
    public void Dispatch_WhenOrderIsNotPrepared_ShouldThrowOrderException()
    {
        var order = Order.MakeOrder(1, DeliveryType.Express, Address.MakeAddress("Rivera", "1234"), MakeItems(), 200m);

        var act = order.Dispatch;

        act.Should().Throw<OrderException>();
    }

    [TestMethod]
    public void Deliver_WhenOrderIsNotOnTheWay_ShouldThrowOrderException()
    {
        var order = Order.MakeOrder(1, DeliveryType.Express, Address.MakeAddress("Rivera", "1234"), MakeItems(), 200m);

        var act = order.Deliver;

        act.Should().Throw<OrderException>();
    }

    [TestMethod]
    public void MarkNotDelivered_WhenOrderIsNotOnTheWay_ShouldThrowOrderException()
    {
        var order = Order.MakeOrder(1, DeliveryType.Express, Address.MakeAddress("Rivera", "1234"), MakeItems(), 200m);

        var act = order.MarkNotDelivered;

        act.Should().Throw<OrderException>();
    }
    #endregion
    #endregion

    #region OrderItem
    #region Error
    [TestMethod]
    public void MakeOrderItem_WhenQuantityIsZero_ShouldThrowOrderException()
    {
        var act = () => OrderItem.MakeOrderItem(1, 0, 100m);

        act.Should().Throw<OrderException>().WithMessage("La cantidad debe ser mayor a cero");
    }

    [TestMethod]
    public void MakeOrderItem_WhenUnitPriceIsNegative_ShouldThrowOrderException()
    {
        var act = () => OrderItem.MakeOrderItem(1, 1, -1m);

        act.Should().Throw<OrderException>().WithMessage("El precio no puede ser negativo");
    }

    [TestMethod]
    public void MakeOrderItem_WhenDiscountIsNegative_ShouldThrowOrderException()
    {
        var act = () => OrderItem.MakeOrderItem(1, 1, 100m, -0.1m);

        act.Should().Throw<OrderException>().WithMessage("El descuento debe estar entre 0 y 1");
    }

    [TestMethod]
    public void MakeOrderItem_WhenDiscountExceedsOne_ShouldThrowOrderException()
    {
        var act = () => OrderItem.MakeOrderItem(1, 1, 100m, 1.1m);

        act.Should().Throw<OrderException>().WithMessage("El descuento debe estar entre 0 y 1");
    }
    #endregion
    #endregion
}
