 using System.Data.Common;
  using DataAccess;
  using DataAccess.Repositories;
  using Domain;
  using Tests.Persistence;

  namespace Tests.Reports;

  [TestClass]
  public sealed class SalesReportVisitorTests
  {
      private DbConnection _connection = null!;
      private SqlContext _context = null!;

      [TestInitialize]
      public void Initialize()
      {
          _context = SqliteTestContextFactory.CreateContext(out
  _connection);
      }

      [TestCleanup]
      public void Cleanup()
      {
          _context.Dispose();
          _connection.Dispose();
      }

      private static User MakeClient(string name, string lastName,
  string email)
      {
          var phone = Phone.TryMakePhone("099123456").Value!;
          return User.MakeClient(name, lastName, email, phone,
  "PasswordSeguraee3!").Value!;
      }

      private static Product MakeProduct()
      {
          return Product.MakeProduct("PZ123", "Pizza Grande",
  "Descripción válida de prueba",
              "Línea", "Categoría", 100m, [new
  ProductPhoto("img.jpg")]);
      }

      private static Order MakeOrder(int clientId, int productId, int
  quantity, decimal price)
      {
          var items = new List<OrderItemData>
          {
              new()
                {
                    ProductId = productId, Quantity = quantity,
  UnitPrice = price, DiscountApplied = 0m
                }
          };
          return Order.MakeOrder(clientId, DeliveryType.Express,
  Address.MakeAddress("Rivera", "1234"), items, 200m);
      }

      [TestMethod]
      public void
  GivenOrdersFromTwoClients_WhenVisited_ThenGroupsByPeriodAndCustomer()
      {
          var client1 = MakeClient("Juan", "Perez", "juan@test.com");
          var client2 = MakeClient("Maria", "Lopez", "maria@test.com");
          var product = MakeProduct();

          _context.Users.AddRange(client1, client2);
          _context.Products.Add(product);
          _context.SaveChanges();

          _context.Orders.AddRange(
              MakeOrder(client1.Id, product.Id, 2, 100m),
              MakeOrder(client2.Id, product.Id, 1, 100m));
          _context.SaveChanges();

          var orders = new OrderRepository(_context)
              .GetByFilters(DateTime.MinValue, DateTime.MaxValue, null,
  null);

          var data = new OrderReportData(orders);
          var visitor = new SalesReportVisitor();

          data.Accept(visitor);

          Assert.AreEqual(1, visitor.Result.Periods.Count);
          Assert.AreEqual(2, visitor.Result.Periods[0].Customers.Count);
          Assert.AreEqual("Juan Perez",
  visitor.Result.Periods[0].Customers[0].Customer);
          Assert.AreEqual("Maria Lopez",
  visitor.Result.Periods[0].Customers[1].Customer);
          Assert.IsTrue(visitor.Result.GrandTotal > 0);
      }
  }
