using System.Data.Common;
using BusinessLogics.Reports;
using BusinessLogicsAbstractions.Exceptions;
using DataAccess;
using DataAccess.Repositories;
using DataAccessAbstractions;
using Domain;
using FluentAssertions;
using Moq;
using Tests.Persistence;

namespace Tests.Reports;

[TestClass]
public sealed class ReportIntegrationTests
{
    private DbConnection _connection = null!;
    private SqlContext _context = null!;
    private ReportService _reportService = null!;
    private static User MakeClient()
    {
        var phone = Phone.TryMakePhone("099123456").Value!;

        return User.MakeClient(
            "Juan",
            "Perez",
            "juan@test.com",
            phone,
            "PasswordSeguraee3!").Value!;
    }

    private static Product MakeProduct(string code, string name, decimal price)
    {
        return Product.MakeProduct(
            code,
            name,
            "Descripción válida de prueba",
            "Línea comercial",
            "Categoría",
            price,
            [new ProductPhoto("img.jpg")]);
    }

    private static Order MakeOrder(int clientId, int productId, int quantity, decimal price)
    {
        var address = Address.MakeAddress("Rivera", "1234");

        var items = new List<OrderItemData>
        {
            new()
            {
                ProductId = productId,
                Quantity = quantity,
                UnitPrice = price,
                DiscountApplied = 0m
            }
        };

        return Order.MakeOrder(
            clientId,
            DeliveryType.Express,
            address,
            items,
            200m);
    }

    [TestInitialize]
    public void Initialize()
    {
        _context = SqliteTestContextFactory.CreateContext(out _connection);

        var repository = new OrderRepository(_context);
        _reportService = new ReportService(repository);
    }

    [TestCleanup]
    public void Cleanup()
    {
        _context.Dispose();
        _connection.Dispose();
    }

    [TestMethod]
    public void GetTopProducts_ShouldReturnTopProductsOrderedByQuantity()
    {
        var client = MakeClient();
        var pizza = MakeProduct("PZ123", "Pizza Grande", 100m);
        var burger = MakeProduct("HB123", "Hamburguesa", 50m);

        _context.Users.Add(client);
        _context.Products.AddRange(pizza, burger);
        _context.SaveChanges();

        _context.Orders.AddRange(
            MakeOrder(client.Id, pizza.Id, 5, 100m),
            MakeOrder(client.Id, burger.Id, 10, 50m));

        _context.SaveChanges();

        var result = _reportService.GetTopProducts(DateTime.MinValue, DateTime.MaxValue);

        result.Should().HaveCount(2);
        result[0].Name.Should().Be("Hamburguesa");
        result[0].QuantitySold.Should().Be(10);
        result[0].Images.Should().Contain("img.jpg");
    }

    [TestMethod]
    public void GetSalesReport_ShouldGroupByMonthAndCustomer()
    {
        var client = MakeClient();
        var product = MakeProduct("PZ123", "Pizza Grande", 100m);

        _context.Users.Add(client);
        _context.Products.Add(product);
        _context.SaveChanges();

        _context.Orders.Add(MakeOrder(client.Id, product.Id, 2, 100m));
        _context.SaveChanges();

        var result = _reportService.GetSalesReport(DateTime.MinValue, DateTime.MaxValue);

        result.Periods.Should().ContainSingle();
        result.Periods[0].Customers.Should().ContainSingle();
        result.Periods[0].Customers[0].Customer.Should().Be("Juan Perez");
        result.GrandTotal.Should().BeGreaterThan(0);
    }

    [TestMethod]
    public void GivenInvalidDateRange_WhenGettingTopProducts_ThenThrowsValidationExceptionWithMessage()
    {
        var repositoryMock = new Mock<IOrderRepository>(MockBehavior.Strict);
        var service = new ReportService(repositoryMock.Object);

        var dateFrom = new DateTime(2026, 2, 1);
        var dateTo = new DateTime(2026, 1, 1);

        var ex = Assert.ThrowsException<ValidationException>(() =>
            service.GetTopProducts(dateFrom, dateTo));

        Assert.AreEqual("Fecha inicio no puede ser posterior a fecha fin", ex.Message);
    }
}
