using Almacenamiento;
using Dominio;

namespace Pruebas;

[TestClass]
public class OrderRepositoryTests
{
    [TestMethod]
    public void Add_ValidOrder_ReturnsOrderWithAssignedId()
    {
        var repo = new OrderRepository();
        var address = Address.MakeAddress("Rivera", "1234");
        var items = new List<(int ProductId, int Quantity, decimal UnitPrice, decimal DiscountApplied)>
        {
            (1, 2, 100m, 0m)
        };
        var order = Order.MakeOrder(1, DeliveryType.Express, address, items, 200m);

        var result = repo.Add(order);

        Assert.IsTrue(result.Id > 0);
    }

    [TestMethod]
    public void Add_ValidOrder_PersistsToRepository()
    {
        var repo = new OrderRepository();
        var address = Address.MakeAddress("Rivera", "1234");
        var items = new List<(int ProductId, int Quantity, decimal UnitPrice, decimal DiscountApplied)>
        {
            (1, 2, 100m, 0m)
        };
        var order = Order.MakeOrder(1, DeliveryType.Express, address, items, 200m);

        repo.Add(order);

        var result = repo.GetById(order.Id);
        Assert.IsNotNull(result);
    }

    [TestMethod]
    public void GetById_ExistingId_ReturnsOrder()
    {
        var repo = new OrderRepository();
        var address = Address.MakeAddress("Rivera", "1234");
        var items = new List<(int ProductId, int Quantity, decimal UnitPrice, decimal DiscountApplied)>
        {
            (1, 2, 100m, 0m)
        };
        var added = repo.Add(Order.MakeOrder(1, DeliveryType.Express, address, items, 200m));

        var result = repo.GetById(added.Id);

        Assert.IsNotNull(result);
        Assert.AreEqual(added.Id, result.Id);
    }

    [TestMethod]
    public void GetById_NonExistingId_ReturnsNull()
    {
        var repo = new OrderRepository();

        var result = repo.GetById(99);

        Assert.IsNull(result);
    }

    [TestMethod]
    public void GetByClientId_ReturnsOnlyClientOrders()
    {
        var repo = new OrderRepository();
        var address = Address.MakeAddress("Rivera", "1234");
        var items = new List<(int ProductId, int Quantity, decimal UnitPrice, decimal DiscountApplied)>
        {
            (1, 1, 100m, 0m)
        };
        repo.Add(Order.MakeOrder(1, DeliveryType.Express, address, items, 200m));
        repo.Add(Order.MakeOrder(1, DeliveryType.Express, address, items, 200m));
        repo.Add(Order.MakeOrder(2, DeliveryType.Express, address, items, 200m));

        var result = repo.GetByClientId(1, null, null, null);

        Assert.AreEqual(2, result.Count);
        Assert.IsTrue(result.All(o => o.ClientId == 1));
    }

    [TestMethod]
    public void GetByClientId_WithStatusFilter_ReturnsFilteredOrders()
    {
        var repo = new OrderRepository();
        var address = Address.MakeAddress("Rivera", "1234");
        var items = new List<(int ProductId, int Quantity, decimal UnitPrice, decimal DiscountApplied)>
        {
            (1, 1, 100m, 0m)
        };
        var pendingOrder = Order.MakeOrder(1, DeliveryType.Express, address, items, 200m);
        var preparedOrder = Order.MakeOrder(1, DeliveryType.Express, address, items, 200m);
        preparedOrder.Prepare();
        repo.Add(pendingOrder);
        repo.Add(preparedOrder);

        var result = repo.GetByClientId(1, null, null, OrderStatus.Pending);

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual(OrderStatus.Pending, result[0].Status);
    }

    [TestMethod]
    public void GetByClientId_NoMatchingOrders_ReturnsEmptyList()
    {
        var repo = new OrderRepository();

        var result = repo.GetByClientId(99, null, null, null);

        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public void GetByFilters_WithDateRange_ReturnsOrdersInRange()
    {
        var repo = new OrderRepository();
        var address = Address.MakeAddress("Rivera", "1234");
        var items = new List<(int ProductId, int Quantity, decimal UnitPrice, decimal DiscountApplied)>
        {
            (1, 1, 100m, 0m)
        };
        repo.Add(Order.MakeOrder(1, DeliveryType.Express, address, items, 200m));

        var result = repo.GetByFilters(
            DateTime.UtcNow.AddDays(-1),
            DateTime.UtcNow.AddDays(1),
            null,
            null);

        Assert.AreEqual(1, result.Count);
    }

    [TestMethod]
    public void GetByFilters_WithStreetFilter_ReturnsOrdersOnStreet()
    {
        var repo = new OrderRepository();
        var items = new List<(int ProductId, int Quantity, decimal UnitPrice, decimal DiscountApplied)>
        {
            (1, 1, 100m, 0m)
        };
        repo.Add(Order.MakeOrder(1, DeliveryType.Express, Address.MakeAddress("Rivera", "1234"), items, 200m));
        repo.Add(Order.MakeOrder(1, DeliveryType.Express, Address.MakeAddress("Bvar España", "500"), items, 200m));

        var result = repo.GetByFilters(
            DateTime.UtcNow.AddDays(-1),
            DateTime.UtcNow.AddDays(1),
            "Rivera",
            null);

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("Rivera", result[0].Address.Street);
    }

    [TestMethod]
    public void GetByFilters_WithStatusFilter_ReturnsFilteredOrders()
    {
        var repo = new OrderRepository();
        var address = Address.MakeAddress("Rivera", "1234");
        var items = new List<(int ProductId, int Quantity, decimal UnitPrice, decimal DiscountApplied)>
        {
            (1, 1, 100m, 0m)
        };
        var pendingOrder = Order.MakeOrder(1, DeliveryType.Express, address, items, 200m);
        var preparedOrder = Order.MakeOrder(1, DeliveryType.Express, address, items, 200m);
        preparedOrder.Prepare();
        repo.Add(pendingOrder);
        repo.Add(preparedOrder);

        var result = repo.GetByFilters(
            DateTime.UtcNow.AddDays(-1),
            DateTime.UtcNow.AddDays(1),
            null,
            OrderStatus.Prepared);

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual(OrderStatus.Prepared, result[0].Status);
    }

    [TestMethod]
    public void GetByFilters_NoMatchingOrders_ReturnsEmptyList()
    {
        var repo = new OrderRepository();

        var result = repo.GetByFilters(
            DateTime.UtcNow.AddDays(-1),
            DateTime.UtcNow.AddDays(1),
            null,
            null);

        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public void Update_ExistingOrder_PersistsStatusChange()
    {
        var repo = new OrderRepository();
        var address = Address.MakeAddress("Rivera", "1234");
        var items = new List<(int ProductId, int Quantity, decimal UnitPrice, decimal DiscountApplied)>
        {
            (1, 1, 100m, 0m)
        };
        var added = repo.Add(Order.MakeOrder(1, DeliveryType.Express, address, items, 200m));

        added.Prepare();
        repo.Update(added);

        var result = repo.GetById(added.Id);
        Assert.IsNotNull(result);
        Assert.AreEqual(OrderStatus.Prepared, result.Status);
    }

    [TestMethod]
    public void Update_NonExistingOrder_DoesNothing()
    {
        var repo = new OrderRepository();
        var address = Address.MakeAddress("Rivera", "1234");
        var items = new List<(int ProductId, int Quantity, decimal UnitPrice, decimal DiscountApplied)>
        {
            (1, 1, 100m, 0m)
        };
        var order = Order.MakeOrder(1, DeliveryType.Express, address, items, 200m);
        order.Id = 99;

        repo.Update(order);

        Assert.IsNull(repo.GetById(99));
    }

    [TestMethod]
    public void GetByClientId_WithDateFromFilter_ReturnsOrdersAfterDate()
    {
        var repo = new OrderRepository();
        var address = Address.MakeAddress("Rivera", "1234");
        var items = new List<(int ProductId, int Quantity, decimal UnitPrice, decimal DiscountApplied)>
        {
            (1, 1, 100m, 0m)
        };
        repo.Add(Order.MakeOrder(1, DeliveryType.Express, address, items, 200m));

        var result = repo.GetByClientId(1, DateTime.UtcNow.AddDays(1), null, null);

        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public void GetByClientId_WithDateToFilter_ReturnsOrdersBeforeDate()
    {
        var repo = new OrderRepository();
        var address = Address.MakeAddress("Rivera", "1234");
        var items = new List<(int ProductId, int Quantity, decimal UnitPrice, decimal DiscountApplied)>
        {
            (1, 1, 100m, 0m)
        };
        repo.Add(Order.MakeOrder(1, DeliveryType.Express, address, items, 200m));

        var result = repo.GetByClientId(1, null, DateTime.UtcNow.AddDays(-1), null);

        Assert.AreEqual(0, result.Count);
    }
}
