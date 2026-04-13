using Almacenamiento;
using Dominio;
using FluentAssertions;

namespace Pruebas;

[TestClass]
public sealed class OrderRepositoryTests
{
    private OrderRepository _repo = null!;

    [TestInitialize]
    public void Initialize()
    {
        _repo = new OrderRepository();
    }

    private static Address MakeAddress() => Address.MakeAddress("Rivera", "1234");

    private static List<(int ProductId, int Quantity, decimal UnitPrice, decimal DiscountApplied)> MakeItems()
    {
        return [(1, 1, 100m, 0m)];
    }

    private Order MakeOrder(int clientId = 1) =>
        Order.MakeOrder(clientId, DeliveryType.Express, MakeAddress(), MakeItems(), 200m);

    #region Add
    #region Success
    [TestMethod]
    public void Add_WhenValidOrder_ShouldReturnOrderWithAssignedId()
    {
        var result = _repo.Add(MakeOrder());

        result.Id.Should().BeGreaterThan(0);
    }

    [TestMethod]
    public void Add_WhenValidOrder_ShouldPersistToRepository()
    {
        var order = MakeOrder();
        _repo.Add(order);

        _repo.GetById(order.Id).Should().NotBeNull();
    }
    #endregion
    #endregion

    #region GetById
    #region Success
    [TestMethod]
    public void GetById_WhenOrderExists_ShouldReturnOrder()
    {
        var added = _repo.Add(MakeOrder());

        var result = _repo.GetById(added.Id);

        result.Should().NotBeNull();
        result!.Id.Should().Be(added.Id);
    }
    #endregion

    #region Error
    [TestMethod]
    public void GetById_WhenOrderDoesNotExist_ShouldReturnNull()
    {
        var result = _repo.GetById(99);

        result.Should().BeNull();
    }
    #endregion
    #endregion

    #region GetByClientId
    #region Success
    [TestMethod]
    public void GetByClientId_WhenClientHasOrders_ShouldReturnOnlyClientOrders()
    {
        _repo.Add(MakeOrder(clientId: 1));
        _repo.Add(MakeOrder(clientId: 1));
        _repo.Add(MakeOrder(clientId: 2));

        var result = _repo.GetByClientId(1, null, null, null);

        result.Should().HaveCount(2);
        result.Should().AllSatisfy(o => o.ClientId.Should().Be(1));
    }

    [TestMethod]
    public void GetByClientId_WhenStatusFilterApplied_ShouldReturnFilteredOrders()
    {
        var preparedOrder = MakeOrder();
        preparedOrder.Prepare();
        _repo.Add(MakeOrder());
        _repo.Add(preparedOrder);

        var result = _repo.GetByClientId(1, null, null, OrderStatus.Pending);

        result.Should().HaveCount(1);
        result[0].Status.Should().Be(OrderStatus.Pending);
    }

    [TestMethod]
    public void GetByClientId_WhenClientHasNoOrders_ShouldReturnEmptyList()
    {
        var result = _repo.GetByClientId(99, null, null, null);

        result.Should().BeEmpty();
    }

    [TestMethod]
    public void GetByClientId_WhenDateFromFilterApplied_ShouldReturnOrdersAfterDate()
    {
        _repo.Add(MakeOrder());

        var result = _repo.GetByClientId(1, DateTime.UtcNow.AddDays(1), null, null);

        result.Should().BeEmpty();
    }

    [TestMethod]
    public void GetByClientId_WhenDateToFilterApplied_ShouldReturnOrdersBeforeDate()
    {
        _repo.Add(MakeOrder());

        var result = _repo.GetByClientId(1, null, DateTime.UtcNow.AddDays(-1), null);

        result.Should().BeEmpty();
    }
    #endregion
    #endregion

    #region GetByFilters
    #region Success
    [TestMethod]
    public void GetByFilters_WhenDateRangeMatches_ShouldReturnOrdersInRange()
    {
        _repo.Add(MakeOrder());

        var result = _repo.GetByFilters(DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(1), null, null);

        result.Should().HaveCount(1);
    }

    [TestMethod]
    public void GetByFilters_WhenStreetFilterApplied_ShouldReturnOrdersOnStreet()
    {
        _repo.Add(Order.MakeOrder(1, DeliveryType.Express, Address.MakeAddress("Rivera", "1234"), MakeItems(), 200m));
        _repo.Add(Order.MakeOrder(1, DeliveryType.Express, Address.MakeAddress("Bvar España", "500"), MakeItems(), 200m));

        var result = _repo.GetByFilters(DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(1), "Rivera", null);

        result.Should().HaveCount(1);
        result[0].Address.Street.Should().Be("Rivera");
    }

    [TestMethod]
    public void GetByFilters_WhenStatusFilterApplied_ShouldReturnFilteredOrders()
    {
        var preparedOrder = MakeOrder();
        preparedOrder.Prepare();
        _repo.Add(MakeOrder());
        _repo.Add(preparedOrder);

        var result = _repo.GetByFilters(
            DateTime.UtcNow.AddDays(-1),
            DateTime.UtcNow.AddDays(1),
            null,
            OrderStatus.Prepared);

        result.Should().HaveCount(1);
        result[0].Status.Should().Be(OrderStatus.Prepared);
    }

    [TestMethod]
    public void GetByFilters_WhenNoMatchingOrders_ShouldReturnEmptyList()
    {
        var result = _repo.GetByFilters(DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(1), null, null);

        result.Should().BeEmpty();
    }
    #endregion
    #endregion

    #region Update
    #region Success
    [TestMethod]
    public void Update_WhenOrderExists_ShouldPersistStatusChange()
    {
        var added = _repo.Add(MakeOrder());
        added.Prepare();

        _repo.Update(added);

        _repo.GetById(added.Id)!.Status.Should().Be(OrderStatus.Prepared);
    }

    [TestMethod]
    public void Update_WhenOrderDoesNotExist_ShouldDoNothing()
    {
        var order = MakeOrder();
        order.Id = 99;

        _repo.Update(order);

        _repo.GetById(99).Should().BeNull();
    }
    #endregion
    #endregion
}
