using Dominio;
using Dominio.Exceptions;

namespace Pruebas.Pedidos;

[TestClass]
public class OrderDomainTests
{
    private static List<(int ProductId, int Quantity, decimal UnitPrice, decimal DiscountApplied)> MakeItems()
    {
        return [(1, 2, 100m, 0m)];
    }

    // Address
    [TestMethod]
    public void Address_EmptyStreet_ThrowsOrderException()
    {
        var ex = Assert.ThrowsException<OrderException>(() =>
            Address.MakeAddress(string.Empty, "1234"));

        Assert.AreEqual("Street cannot be empty", ex.Message);
    }

    [TestMethod]
    public void Address_EmptyDoorNumber_ThrowsOrderException()
    {
        var ex = Assert.ThrowsException<OrderException>(() =>
            Address.MakeAddress("Rivera", string.Empty));

        Assert.AreEqual("Door number cannot be empty", ex.Message);
    }

    [TestMethod]
    public void Address_WithApartment_SetsApartment()
    {
        var address = Address.MakeAddress("Rivera", "1234", "3A");

        Assert.AreEqual("3A", address.Apartment);
    }

    // Order
    [TestMethod]
    public void Order_EmptyItems_ThrowsOrderException()
    {
        var address = Address.MakeAddress("Rivera", "1234");

        var ex = Assert.ThrowsException<OrderException>(() =>
            Order.MakeOrder(1, DeliveryType.Express, address, [], 200m));

        Assert.AreEqual("Order must have at least one product", ex.Message);
    }

    [TestMethod]
    public void Order_Prepare_WhenNotPending_ThrowsOrderException()
    {
        var order = Order.MakeOrder(1, DeliveryType.Express, Address.MakeAddress("Rivera", "1234"), MakeItems(), 200m);
        order.Prepare();

        Assert.ThrowsException<OrderException>(order.Prepare);
    }

    [TestMethod]
    public void Order_Cancel_WhenNotPending_ThrowsOrderException()
    {
        var order = Order.MakeOrder(1, DeliveryType.Express, Address.MakeAddress("Rivera", "1234"), MakeItems(), 200m);
        order.Prepare();

        Assert.ThrowsException<OrderException>(order.Cancel);
    }

    [TestMethod]
    public void Order_Dispatch_WhenNotPrepared_ThrowsOrderException()
    {
        var order = Order.MakeOrder(1, DeliveryType.Express, Address.MakeAddress("Rivera", "1234"), MakeItems(), 200m);

        Assert.ThrowsException<OrderException>(order.Dispatch);
    }

    [TestMethod]
    public void Order_Deliver_WhenNotOnTheWay_ThrowsOrderException()
    {
        var order = Order.MakeOrder(1, DeliveryType.Express, Address.MakeAddress("Rivera", "1234"), MakeItems(), 200m);

        Assert.ThrowsException<OrderException>(order.Deliver);
    }

    [TestMethod]
    public void Order_MarkNotDelivered_WhenNotOnTheWay_ThrowsOrderException()
    {
        var order = Order.MakeOrder(1, DeliveryType.Express, Address.MakeAddress("Rivera", "1234"), MakeItems(), 200m);

        Assert.ThrowsException<OrderException>(order.MarkNotDelivered);
    }

    // OrderItem
    [TestMethod]
    public void OrderItem_ZeroQuantity_ThrowsOrderException()
    {
        var ex = Assert.ThrowsException<OrderException>(() =>
            OrderItem.MakeOrderItem(1, 0, 100m));

        Assert.AreEqual("Quantity must be greater than zero", ex.Message);
    }

    [TestMethod]
    public void OrderItem_NegativeUnitPrice_ThrowsOrderException()
    {
        var ex = Assert.ThrowsException<OrderException>(() =>
            OrderItem.MakeOrderItem(1, 1, -1m));

        Assert.AreEqual("Unit price cannot be negative", ex.Message);
    }

    [TestMethod]
    public void OrderItem_NegativeDiscount_ThrowsOrderException()
    {
        var ex = Assert.ThrowsException<OrderException>(() =>
            OrderItem.MakeOrderItem(1, 1, 100m, -0.1m));

        Assert.AreEqual("Discount must be between 0 and 1", ex.Message);
    }

    [TestMethod]
    public void OrderItem_DiscountGreaterThanOne_ThrowsOrderException()
    {
        var ex = Assert.ThrowsException<OrderException>(() =>
            OrderItem.MakeOrderItem(1, 1, 100m, 1.1m));

        Assert.AreEqual("Discount must be between 0 and 1", ex.Message);
    }

    // Product
    [TestMethod]
    public void Product_EmptyName_ThrowsOrderException()
    {
        var ex = Assert.ThrowsException<OrderException>(() =>
            Product.MakeProduct(string.Empty, 100m));

        Assert.AreEqual("Product name cannot be empty", ex.Message);
    }

    [TestMethod]
    public void Product_NegativePrice_ThrowsOrderException()
    {
        var ex = Assert.ThrowsException<OrderException>(() =>
            Product.MakeProduct("Burger", -1m));

        Assert.AreEqual("Product price cannot be negative", ex.Message);
    }
}
