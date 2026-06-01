using BusinessLogics;
using System.Data.Common;
using DataAccess;
using DataAccess.Repositories;
using Domain;
using Tests.Persistence;

namespace Tests.Reports;

[TestClass]
public sealed class TopProductsReportVisitorTests
{
    private DbConnection _connection = null!;
    private SqlContext _context = null!;

    [TestInitialize]
    public void Initialize()
    {
        _context = SqliteTestContextFactory.CreateContext(out _connection);
    }

    [TestCleanup]
    public void Cleanup()
    {
        _context.Dispose();
        _connection.Dispose();
    }

    private static User MakeClient()
    {
        var phone = Phone.TryMakePhone("099123456").Value!;
        return User.MakeClient("Juan", "Perez", "juan@test.com", phone, "PasswordSeguraee3!").Value!;
    }

    private static Product MakeProduct(string code, string name, decimal
        price)
    {
        return Product.MakeProduct(code, name, "Descripción válida de prueba", "Línea", "Categoría", price,
        [new ProductPhoto("img.jpg")]);
    }

    private static Order MakeOrder(int clientId, int productId, int
        quantity, decimal price)
    {
        var items = new List<OrderItemData>
        {
            new() { ProductId = productId, Quantity = quantity, UnitPrice = price, DiscountApplied = 0m }
        };
        return Order.MakeOrder(clientId, "Express",
            Address.MakeAddress("Rivera", "1234"), items, 200m);
    }

    [TestMethod]
    public void GivenOrders_WhenVisited_ThenReturnsTopProductsOrderedByQuantityDescending()
    {
        var client = MakeClient();
        var pizza = MakeProduct("PZ123", "Pizza Grande", 100m);
        var burger = MakeProduct("HB123", "Hamburguesa", 50m);

        _context.Users.Add(client);
        _context.Products.AddRange(pizza, burger);
        _context.SaveChanges();

        _context.Orders.AddRange(
            MakeOrder(client.Id, pizza.Id, 3, 100m),
            MakeOrder(client.Id, burger.Id, 10, 50m));
        _context.SaveChanges();

        var orders = new OrderRepository(_context)
            .GetByFilters(DateTime.MinValue, DateTime.MaxValue, null,
                null);

        var data = new OrderReportData(orders);
        var visitor = new TopProductsReportVisitor();

        data.Accept(visitor);

        Assert.AreEqual(2, visitor.Result.Count);
        Assert.AreEqual("Hamburguesa", visitor.Result[0].Name);
        Assert.AreEqual("HB123", visitor.Result[0].Code);
        Assert.AreEqual(10, visitor.Result[0].QuantitySold);
        Assert.IsTrue(visitor.Result[0].Images.Contains("img.jpg"));
    }
}
