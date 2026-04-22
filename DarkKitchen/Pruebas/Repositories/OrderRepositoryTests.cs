using System.Data.Common;
using Almacenamiento;
using Almacenamiento.Repositories;
using Dominio;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pruebas.Persistence;

namespace Pruebas.Repositories;

[TestClass]
public class OrderRepositoryTests
{
    private SqlContext _context = null!;
    private OrderRepository _repository = null!;
    private DbConnection _connection = null!;

    [TestInitialize]
    public void Initialize()
    {
        _context = SqliteTestContextFactory.CreateContext(out _connection);
        _repository = new OrderRepository(_context);
        SeedDatabase();
    }

    [TestCleanup]
    public void Cleanup()
    {
        _context.Dispose();
        _connection.Dispose();
    }

    private void SeedDatabase()
    {
        var phoneResult = Phone.TryMakePhone("099123456");
        var clientResult = User.MakeClient("Juan", "Perez", "juan@test.com", phoneResult.Value!, "PasswordSegura1!");
        _context.Users.Add(clientResult.Value!);

        _context.Products.Add(Product.MakeProduct(
            "PROD01",
            "Producto Test Uno",
            "Descripcion larga del producto de prueba",
            "Linea A",
            "Categoria B",
            100m,
            [new ProductPhoto("foto.jpg")]));

        _context.SaveChanges();
    }

    private User SeededClient => _context.Users.First();
    private Product SeededProduct => _context.Products.First();

    private Address MakeAddress() => Address.MakeAddress("Rivera", "1234");

    private List<(int ProductId, int Quantity, decimal UnitPrice, decimal DiscountApplied)> MakeItems() =>
        [(SeededProduct.Id, 1, 100m, 0m)];

    private Order MakeOrder(int? clientId = null) =>
        Order.MakeOrder(clientId ?? SeededClient.Id, DeliveryType.Express, MakeAddress(), MakeItems(), 200m);

    #region Add
    #region Success
    [TestMethod]
    public void Add_WhenValidOrder_ShouldReturnOrderWithAssignedId()
    {
        var result = _repository.Add(MakeOrder());

        result.Id.Should().BeGreaterThan(0);
    }

    [TestMethod]
    public void Add_WhenValidOrder_ShouldPersistToDatabase()
    {
        var order = MakeOrder();
        _repository.Add(order);

        _repository.GetById(order.Id).Should().NotBeNull();
    }
    #endregion
    #endregion

    #region GetById
    #region Success
    [TestMethod]
    public void GetById_WhenOrderExists_ShouldReturnOrder()
    {
        var added = _repository.Add(MakeOrder());

        var result = _repository.GetById(added.Id);

        result.Should().NotBeNull();
        result!.Id.Should().Be(added.Id);
    }

    [TestMethod]
    public void GetById_WhenOrderExists_ShouldIncludeItems()
    {
        var added = _repository.Add(MakeOrder());

        var result = _repository.GetById(added.Id);

        result!.Items.Should().HaveCount(1);
    }
    #endregion

    #region Error
    [TestMethod]
    public void GetById_WhenOrderDoesNotExist_ShouldReturnNull()
    {
        var result = _repository.GetById(99999);

        result.Should().BeNull();
    }
    #endregion
    #endregion

    #region GetByClientId
    #region Success
    [TestMethod]
    public void GetByClientId_WhenClientHasOrders_ShouldReturnOnlyClientOrders()
    {
        var phoneResult = Phone.TryMakePhone("099654321");
        var otherClient = User.MakeClient("Ana", "Lopez", "ana@test.com", phoneResult.Value!, "PasswordSegura1!").Value!;
        _context.Users.Add(otherClient);
        _context.SaveChanges();

        _repository.Add(MakeOrder());
        _repository.Add(MakeOrder());
        _repository.Add(MakeOrder(clientId: otherClient.Id));

        var result = _repository.GetByClientId(SeededClient.Id, null, null, null);

        result.Should().HaveCount(2);
        result.Should().AllSatisfy(o => o.ClientId.Should().Be(SeededClient.Id));
    }

    [TestMethod]
    public void GetByClientId_WhenStatusFilterApplied_ShouldReturnFilteredOrders()
    {
        var pendingOrder = MakeOrder();
        var preparedOrder = MakeOrder();
        preparedOrder.Prepare();

        _repository.Add(pendingOrder);
        _repository.Add(preparedOrder);

        var result = _repository.GetByClientId(SeededClient.Id, null, null, OrderStatus.Pending);

        result.Should().HaveCount(1);
        result[0].Status.Should().Be(OrderStatus.Pending);
    }

    [TestMethod]
    public void GetByClientId_WhenClientHasNoOrders_ShouldReturnEmptyList()
    {
        var result = _repository.GetByClientId(99999, null, null, null);

        result.Should().BeEmpty();
    }

    [TestMethod]
    public void GetByClientId_WhenDateFromFilterExcludesOrder_ShouldReturnEmpty()
    {
        _repository.Add(MakeOrder());

        var result = _repository.GetByClientId(SeededClient.Id, DateTime.UtcNow.AddDays(1), null, null);

        result.Should().BeEmpty();
    }

    [TestMethod]
    public void GetByClientId_WhenDateToFilterExcludesOrder_ShouldReturnEmpty()
    {
        _repository.Add(MakeOrder());

        var result = _repository.GetByClientId(SeededClient.Id, null, DateTime.UtcNow.AddDays(-1), null);

        result.Should().BeEmpty();
    }
    #endregion
    #endregion

    #region GetByFilters
    #region Success
    [TestMethod]
    public void GetByFilters_WhenDateRangeMatches_ShouldReturnOrdersInRange()
    {
        _repository.Add(MakeOrder());

        var result = _repository.GetByFilters(DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(1), null, null);

        result.Should().HaveCount(1);
    }

    [TestMethod]
    public void GetByFilters_WhenStreetFilterApplied_ShouldReturnOrdersOnStreet()
    {
        _repository.Add(Order.MakeOrder(SeededClient.Id, DeliveryType.Express, Address.MakeAddress("Rivera", "1234"), MakeItems(), 200m));
        _repository.Add(Order.MakeOrder(SeededClient.Id, DeliveryType.Express, Address.MakeAddress("Bvar España", "500"), MakeItems(), 200m));

        var result = _repository.GetByFilters(DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(1), "Rivera", null);

        result.Should().HaveCount(1);
        result[0].Address.Street.Should().Be("Rivera");
    }

    [TestMethod]
    public void GetByFilters_WhenStatusFilterApplied_ShouldReturnFilteredOrders()
    {
        var preparedOrder = MakeOrder();
        preparedOrder.Prepare();

        _repository.Add(MakeOrder());
        _repository.Add(preparedOrder);

        var result = _repository.GetByFilters(
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
        var result = _repository.GetByFilters(DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(1), null, null);

        result.Should().BeEmpty();
    }
    #endregion
    #endregion

    #region Update
    #region Success
    [TestMethod]
    public void Update_WhenOrderExists_ShouldPersistStatusChange()
    {
        var added = _repository.Add(MakeOrder());
        added.Prepare();

        _repository.Update(added);

        _repository.GetById(added.Id)!.Status.Should().Be(OrderStatus.Prepared);
    }
    #endregion
    #endregion
}
