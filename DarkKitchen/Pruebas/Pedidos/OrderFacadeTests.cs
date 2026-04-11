using Contratos;
using Dominio;
using Dominio.Exceptions;
using LogicaDeNegocio;
using LogicaDeNegocioAbstracciones;
using Moq;

namespace Pruebas.Pedidos;

[TestClass]
public class OrderFacadeTests
{
    private static Order MakeTestOrder()
    {
        var address = Address.MakeAddress("Rivera", "1234");
        var items = new List<(int ProductId, int Quantity, decimal UnitPrice, decimal DiscountApplied)>
        {
            (1, 2, 100m, 0m)
        };
        return Order.MakeOrder(1, DeliveryType.Express, address, items, 200m);
    }

    private static Product MakeActiveProduct(decimal price = 100m)
    {
        return Product.MakeProduct(
            "COD123",
            "Test Product",
            "Descripción de prueba válida",
            "Línea",
            "Categoría",
            price,
            new List<ProductPhoto>());
    }

    [TestMethod]
    public void CreateOrder_ValidRequest_ReturnsOrderResponse()
    {
        var orderServiceMock = new Mock<IOrderService>();
        var productServiceMock = new Mock<IProductService>();
        var order = MakeTestOrder();

        productServiceMock
            .Setup(p => p.GetById(1))
            .Returns(MakeActiveProduct());

        orderServiceMock
            .Setup(s => s.CreateOrder(
                It.IsAny<int>(),
                It.IsAny<DeliveryType>(),
                It.IsAny<Address>(),
                It.IsAny<List<(int, int, decimal, decimal)>>(),
                It.IsAny<decimal>()))
            .Returns(order);

        var facade = new OrderFacade(orderServiceMock.Object, productServiceMock.Object);
        var request = new CreateOrderRequest
        {
            DeliveryType = DeliveryType.Express,
            Address = new CreateAddressRequest { Street = "Rivera", DoorNumber = "1234" },
            Items = [new CreateOrderItemRequest { ProductId = 1, Quantity = 2 }]
        };

        var result = facade.CreateOrder(1, request);

        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result, typeof(OrderResponse));
    }

    [TestMethod]
    public void CreateOrder_InactiveProduct_ThrowsOrderException()
    {
        var orderServiceMock = new Mock<IOrderService>();
        var productServiceMock = new Mock<IProductService>();

        var product = Product.MakeProduct(
            "COD123",
            "Test Producto",
            "Descripción de prueba válida",
            "Línea",
            "Categoría",
            100m,
            new List<ProductPhoto>());
        product.Deactivate();

        productServiceMock
            .Setup(p => p.GetById(1))
            .Returns(product);

        var facade = new OrderFacade(orderServiceMock.Object, productServiceMock.Object);
        var request = new CreateOrderRequest
        {
            DeliveryType = DeliveryType.Express,
            Address = new CreateAddressRequest { Street = "Rivera", DoorNumber = "1234" },
            Items = [new CreateOrderItemRequest { ProductId = 1, Quantity = 2 }]
        };

        var ex = Assert.ThrowsException<OrderException>(() =>
            facade.CreateOrder(1, request));

        Assert.AreEqual("Product Test Producto is not available", ex.Message);
    }

    [TestMethod]
    public void CreateOrder_ValidRequest_CallsOrderServiceOnce()
    {
        var orderServiceMock = new Mock<IOrderService>();
        var productServiceMock = new Mock<IProductService>();

        productServiceMock
            .Setup(p => p.GetById(1))
            .Returns(MakeActiveProduct());

        orderServiceMock
            .Setup(s => s.CreateOrder(
                It.IsAny<int>(),
                It.IsAny<DeliveryType>(),
                It.IsAny<Address>(),
                It.IsAny<List<(int, int, decimal, decimal)>>(),
                It.IsAny<decimal>()))
            .Returns(MakeTestOrder());

        var facade = new OrderFacade(orderServiceMock.Object, productServiceMock.Object);
        var request = new CreateOrderRequest
        {
            DeliveryType = DeliveryType.Express,
            Address = new CreateAddressRequest { Street = "Rivera", DoorNumber = "1234" },
            Items = [new CreateOrderItemRequest { ProductId = 1, Quantity = 2 }]
        };

        facade.CreateOrder(1, request);

        orderServiceMock.Verify(s => s.CreateOrder(
            It.IsAny<int>(),
            It.IsAny<DeliveryType>(),
            It.IsAny<Address>(),
            It.IsAny<List<(int, int, decimal, decimal)>>(),
            It.IsAny<decimal>()), Times.Once);
    }

    [TestMethod]
    public void GetClientOrders_ValidRequest_ReturnsListOfOrderResponses()
    {
        var orderServiceMock = new Mock<IOrderService>();
        var productServiceMock = new Mock<IProductService>();

        orderServiceMock
            .Setup(s => s.GetClientOrders(1, null, null, null))
            .Returns([MakeTestOrder(), MakeTestOrder()]);

        var facade = new OrderFacade(orderServiceMock.Object, productServiceMock.Object);

        var result = facade.GetClientOrders(1, null, null, null);

        Assert.AreEqual(2, result.Count);
    }

    [TestMethod]
    public void GetClientOrders_NoOrders_ReturnsEmptyList()
    {
        var orderServiceMock = new Mock<IOrderService>();
        var productServiceMock = new Mock<IProductService>();

        orderServiceMock
            .Setup(s => s.GetClientOrders(1, null, null, null))
            .Returns([]);

        var facade = new OrderFacade(orderServiceMock.Object, productServiceMock.Object);

        var result = facade.GetClientOrders(1, null, null, null);

        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public void GetById_ValidId_ReturnsOrderResponse()
    {
        var orderServiceMock = new Mock<IOrderService>();
        var productServiceMock = new Mock<IProductService>();

        orderServiceMock
            .Setup(s => s.GetById(1))
            .Returns(MakeTestOrder());

        var facade = new OrderFacade(orderServiceMock.Object, productServiceMock.Object);

        var result = facade.GetById(1);

        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result, typeof(OrderResponse));
    }

    [TestMethod]
    public void Prepare_ValidId_CallsServiceOnce()
    {
        var orderServiceMock = new Mock<IOrderService>();
        var productServiceMock = new Mock<IProductService>();
        var facade = new OrderFacade(orderServiceMock.Object, productServiceMock.Object);

        facade.Prepare(1);

        orderServiceMock.Verify(s => s.Prepare(1), Times.Once);
    }

    [TestMethod]
    public void Cancel_ValidId_CallsServiceOnce()
    {
        var orderServiceMock = new Mock<IOrderService>();
        var productServiceMock = new Mock<IProductService>();
        var facade = new OrderFacade(orderServiceMock.Object, productServiceMock.Object);

        facade.Cancel(1);

        orderServiceMock.Verify(s => s.Cancel(1), Times.Once);
    }

    [TestMethod]
    public void Dispatch_ValidId_CallsServiceOnce()
    {
        var orderServiceMock = new Mock<IOrderService>();
        var productServiceMock = new Mock<IProductService>();
        var facade = new OrderFacade(orderServiceMock.Object, productServiceMock.Object);

        facade.Dispatch(1);

        orderServiceMock.Verify(s => s.Dispatch(1), Times.Once);
    }

    [TestMethod]
    public void Deliver_ValidId_CallsServiceOnce()
    {
        var orderServiceMock = new Mock<IOrderService>();
        var productServiceMock = new Mock<IProductService>();
        var facade = new OrderFacade(orderServiceMock.Object, productServiceMock.Object);

        facade.Deliver(1);

        orderServiceMock.Verify(s => s.Deliver(1), Times.Once);
    }

    [TestMethod]
    public void MarkNotDelivered_ValidId_CallsServiceOnce()
    {
        var orderServiceMock = new Mock<IOrderService>();
        var productServiceMock = new Mock<IProductService>();
        var facade = new OrderFacade(orderServiceMock.Object, productServiceMock.Object);

        facade.MarkNotDelivered(1);

        orderServiceMock.Verify(s => s.MarkNotDelivered(1), Times.Once);
    }
}
