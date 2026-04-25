using AlmacenamientoAbstracciones;
using Contratos;
using Contratos.Orders;
using Contratos.Products;
using Dominio;
using Dominio.Exceptions;
using FluentAssertions;
using LogicaDeNegocio;
using LogicaDeNegocio.Orders;
using LogicaDeNegocioAbstracciones;
using LogicaDeNegocioAbstracciones.Exceptions;
using LogicaDeNegocioAbstracciones.Orders;
using LogicaDeNegocioAbstracciones.Products;
using LogicaDeNegocioAbstracciones.Promotions;
using Moq;
using Contratos.Promotions;
using LogicaDeNegocioAbstracciones.Promotions;

namespace Pruebas.Pedidos;

[TestClass]
public sealed class OrderFacadeTests
{
    private Mock<IOrderService> _orderServiceMock = null!;
    private Mock<IProductService> _productServiceMock = null!;
    private OrderFacade _orderFacade = null!;
    private Mock<IRepositoryUser> _repositoryUserMock = null!;
    private Mock<IPromotionService> _promotionServiceMock = null!;

    private static Order MakeTestOrder(int clientId = 1)
    {
        var address = Address.MakeAddress("Rivera", "1234");

        var items = new List<(int ProductId, int Quantity, decimal UnitPrice, decimal DiscountApplied)>
        {
            (1, 2, 100m, 0m)
        };

        return Order.MakeOrder(clientId, DeliveryType.Express, address, items, 200m);
    }

    private static User MakeTestUser(int id = 1)
    {
        var phone = Phone.TryMakePhone("099123456").Value!;

        var user = User.MakeClient(
            "Miguel",
            "Test",
            "miguel@test.com",
            phone,
            "PasswordSeguraee3!").Value!;

        user.Id = id;
        return user;
    }

    [TestInitialize]
    public void Initialize()
    {
        _orderServiceMock = new Mock<IOrderService>(MockBehavior.Strict);
        _productServiceMock = new Mock<IProductService>(MockBehavior.Strict);
        _repositoryUserMock = new Mock<IRepositoryUser>(MockBehavior.Strict);
        _promotionServiceMock = new Mock<IPromotionService>(MockBehavior.Strict);

        _orderFacade = new OrderFacade(
            _orderServiceMock.Object,
            _productServiceMock.Object,
            _promotionServiceMock.Object,
            _repositoryUserMock.Object);
    }

    private static Order MakeTestOrder()
    {
        var address = Address.MakeAddress("Rivera", "1234");
        var items = new List<(int ProductId, int Quantity, decimal UnitPrice, decimal DiscountApplied)>
        {
            (1, 2, 100m, 0m)
        };

        return Order.MakeOrder(1, DeliveryType.Express, address, items, 200m);
    }

    private static ProductResponseDto MakeActiveProduct(decimal price = 100m)
    {
        return new ProductResponseDto
        {
            Code = "COD123",
            Name = "Test Product",
            Description = "Descripción de prueba válida",
            CommercialLine = "Línea",
            Category = "Categoría",
            Price = price,
            IsActive = true,
            Photos = [new PhotoResponseDto { Path = "foto0.jpg" }]
        };
    }

    #region CreateOrder

    #region Success

    [TestMethod]
    public void CreateOrder_WhenValidRequest_ShouldReturnOrderResponse()
    {
        _productServiceMock
            .Setup(p => p.GetById(1))
            .Returns(MakeActiveProduct());

        _orderServiceMock
            .Setup(s => s.CreateOrder(
                It.IsAny<int>(),
                It.IsAny<DeliveryType>(),
                It.IsAny<Address>(),
                It.IsAny<List<(int, int, decimal, decimal)>>(),
                It.IsAny<decimal>()))
            .Returns(MakeTestOrder(1));

        _repositoryUserMock
            .Setup(r => r.FindById(1))
            .Returns(MakeTestUser(1));

        var result = _orderFacade.CreateOrder(1,
            new CreateOrderRequest
            {
                DeliveryType = DeliveryTypeDto.Express,
                Address = new CreateAddressRequest { Street = "Rivera", DoorNumber = "1234" },
                Items = [new CreateOrderItemRequest { ProductId = 1, Quantity = 2 }]
            });

        _productServiceMock.VerifyAll();
        _orderServiceMock.VerifyAll();
        _repositoryUserMock.VerifyAll();

        result.Should().NotBeNull();
        result.Should().BeOfType<OrderResponse>();
    }

    #endregion

    #region Error

    [TestMethod]
    public void CreateOrder_WhenProductIsInactive_ShouldThrowOrderException()
    {
        var product = new ProductResponseDto
        {
            Code = "COD123",
            Name = "Producto Test",
            Description = "Descripción de prueba válida",
            CommercialLine = "Línea",
            Category = "Categoría",
            Price = 100m,
            IsActive = false,
            Photos = [new PhotoResponseDto { Path = "foto0.jpg" }]
        };

        _productServiceMock
            .Setup(p => p.GetById(1))
            .Returns(product);

        var act = () => _orderFacade.CreateOrder(1,
            new CreateOrderRequest
            {
                DeliveryType = DeliveryTypeDto.Express,
                Address = new CreateAddressRequest { Street = "Rivera", DoorNumber = "1234" },
                Items = [new CreateOrderItemRequest { ProductId = 1, Quantity = 2 }]
            });

        act.Should()
            .Throw<ValidationException>()
            .WithMessage("El producto Producto Test no está disponible");
        _productServiceMock.VerifyAll();
        _orderServiceMock.VerifyAll();
    }

    [TestMethod]
    public void CreateOrder_WhenProductHasCurrentPromotion_ShouldApplyHighestDiscount()
    {
        _productServiceMock
            .Setup(p => p.GetById(1))
            .Returns(MakeActiveProduct(price: 520m));

        _promotionServiceMock
            .Setup(p => p.GetPromotions(It.Is<PromotionSearchRequestDto>(request =>
                request.ProductId == 1 &&
                request.Date.HasValue)))
            .Returns([
                new PromotionResponseDto
                {
                    Id = 1,
                    Name = "Promo 10",
                    DiscountPercentage = 10,
                    StartDate = new DateOnly(2026, 4, 1),
                    EndDate = new DateOnly(2026, 4, 30)
                },
                new PromotionResponseDto
                {
                    Id = 2,
                    Name = "Promo Abril",
                    DiscountPercentage = 20,
                    StartDate = new DateOnly(2026, 4, 1),
                    EndDate = new DateOnly(2026, 4, 30)
                }
            ]);

        _orderServiceMock
            .Setup(s => s.CreateOrder(
                1,
                DeliveryType.Express,
                It.IsAny<Address>(),
                It.Is<List<(int ProductId, int Quantity, decimal UnitPrice, decimal DiscountApplied)>>(items =>
                    items.Count == 1 &&
                    items[0].ProductId == 1 &&
                    items[0].Quantity == 1 &&
                    items[0].UnitPrice == 520m &&
                    items[0].DiscountApplied == 0.20m),
                200m))
            .Returns((int clientId,
                    DeliveryType deliveryType,
                    Address address,
                    List<(int ProductId, int Quantity, decimal UnitPrice, decimal DiscountApplied)> items,
                    decimal shippingCost) =>
                Order.MakeOrder(clientId, deliveryType, address, items, shippingCost));

        _repositoryUserMock
            .Setup(r => r.FindById(1))
            .Returns(MakeTestUser(1));

        var result = _orderFacade.CreateOrder(1, new CreateOrderRequest
        {
            DeliveryType = DeliveryTypeDto.Express,
            Address = new CreateAddressRequest { Street = "Rivera", DoorNumber = "1234" },
            Items =
            [
                new CreateOrderItemRequest { ProductId = 1, Quantity = 1 }
            ]
        });

        _productServiceMock.VerifyAll();
        _promotionServiceMock.VerifyAll();
        _orderServiceMock.VerifyAll();
        _repositoryUserMock.VerifyAll();

        result.Items.Should().ContainSingle();
        result.Items[0].DiscountApplied.Should().Be(0.20m);
        result.Items[0].LineTotal.Should().Be(416m);
    }

    [TestMethod]
    public void CreateOrder_WhenItemsAreEmpty_ShouldThrowValidationException()
    {
        var request = new CreateOrderRequest
        {
            DeliveryType = DeliveryTypeDto.Express,
            Address = new CreateAddressRequest { Street = "Rivera", DoorNumber = "1234" },
            Items = []
        };

        var act = () => _orderFacade.CreateOrder(1, request);

        act.Should()
            .Throw<ValidationException>()
            .WithMessage("El pedido debe contener al menos un producto");

        _productServiceMock.VerifyNoOtherCalls();
        _orderServiceMock.VerifyNoOtherCalls();
    }

    [TestMethod]
    public void CreateOrder_WhenItemQuantityIsNegative_ShouldThrowValidationException()
    {
        var request = new CreateOrderRequest
        {
            DeliveryType = DeliveryTypeDto.Express,
            Address = new CreateAddressRequest { Street = "Rivera", DoorNumber = "1234" },
            Items =
            [
                new CreateOrderItemRequest { ProductId = 1, Quantity = -2 }
            ]
        };

        var act = () => _orderFacade.CreateOrder(1, request);

        act.Should()
            .Throw<ValidationException>()
            .WithMessage("La cantidad de cada producto debe ser mayor a 0");

        _productServiceMock.VerifyNoOtherCalls();
        _orderServiceMock.VerifyNoOtherCalls();
    }

    [TestMethod]
    public void CreateOrder_WhenDeliveryTypeIsInvalid_ShouldThrowValidationException()
    {
        var request = new CreateOrderRequest
        {
            DeliveryType = (DeliveryTypeDto)99,
            Address = new CreateAddressRequest { Street = "Rivera", DoorNumber = "1234" },
            Items =
            [
                new CreateOrderItemRequest { ProductId = 1, Quantity = 2 }
            ]
        };

        var act = () => _orderFacade.CreateOrder(1, request);

        act.Should()
            .Throw<ValidationException>()
            .WithMessage("El tipo de entrega no es válido");

        _productServiceMock.VerifyNoOtherCalls();
        _orderServiceMock.VerifyNoOtherCalls();
    }

    [TestMethod]
    public void CreateOrder_WhenItemsAreNull_ShouldThrowValidationException()
    {
        var request = new CreateOrderRequest
        {
            DeliveryType = DeliveryTypeDto.Express,
            Address = new CreateAddressRequest { Street = "Rivera", DoorNumber = "1234" },
            Items = null!
        };

        var act = () => _orderFacade.CreateOrder(1, request);

        act.Should()
            .Throw<ValidationException>()
            .WithMessage("El pedido debe contener al menos un producto");

        _productServiceMock.VerifyNoOtherCalls();
        _orderServiceMock.VerifyNoOtherCalls();
    }

    [TestMethod]
    public void CreateOrder_WhenItemQuantityIsZero_ShouldThrowValidationException()
    {
        var request = new CreateOrderRequest
        {
            DeliveryType = DeliveryTypeDto.Express,
            Address = new CreateAddressRequest { Street = "Rivera", DoorNumber = "1234" },
            Items =
            [
                new CreateOrderItemRequest { ProductId = 1, Quantity = 0 }
            ]
        };

        var act = () => _orderFacade.CreateOrder(1, request);

        act.Should()
            .Throw<ValidationException>()
            .WithMessage("La cantidad de cada producto debe ser mayor a 0");

        _productServiceMock.VerifyNoOtherCalls();
        _orderServiceMock.VerifyNoOtherCalls();
    }

    #endregion

    #endregion

    #region GetClientOrders

    #region Success

    [TestMethod]
    public void GetClientOrders_WhenClientHasOrders_ShouldReturnOrderResponseList()
    {
        _orderServiceMock
            .Setup(s => s.GetClientOrders(1, null, null, null))
            .Returns([MakeTestOrder(1), MakeTestOrder(1)]);

        _repositoryUserMock
            .Setup(r => r.FindById(1))
            .Returns(MakeTestUser(1));

        var result = _orderFacade.GetClientOrders(1, null, null, null);

        _orderServiceMock.VerifyAll();
        _repositoryUserMock.Verify(r => r.FindById(1), Times.Exactly(2));

        result.Should().HaveCount(2);
    }

    [TestMethod]
    public void GetClientOrders_WhenNoOrders_ShouldReturnEmptyList()
    {
        _orderServiceMock
            .Setup(s => s.GetClientOrders(1, null, null, null))
            .Returns([]);

        var result = _orderFacade.GetClientOrders(1, null, null, null);

        _orderServiceMock.VerifyAll();
        result.Should().BeEmpty();
    }

    #endregion

    #endregion

    #region GetById

    #region Success

    [TestMethod]
    public void GetById_WhenOrderExists_ShouldReturnOrderResponse()
    {
        _orderServiceMock
            .Setup(s => s.GetById(1))
            .Returns(MakeTestOrder(1));

        _repositoryUserMock
            .Setup(r => r.FindById(1))
            .Returns(MakeTestUser(1));

        var result = _orderFacade.GetById(1);

        _orderServiceMock.VerifyAll();
        _repositoryUserMock.Verify(r => r.FindById(1), Times.Once);

        result.Should().NotBeNull();
        result.Should().BeOfType<OrderResponse>();
    }

    #endregion

    #endregion

    #region GetOrdersByFilters

    #region Success

    [TestMethod]
    public void GetOrdersByFilters_WhenOrdersMatch_ShouldReturnOrderResponseList()
    {
        var dateFrom = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var dateTo = new DateTime(2026, 1, 31, 0, 0, 0, DateTimeKind.Utc);

        _orderServiceMock
            .Setup(s => s.GetOrdersByFilters(dateFrom, dateTo, null, null))
            .Returns([MakeTestOrder(1), MakeTestOrder(1)]);

        _repositoryUserMock
            .Setup(r => r.FindById(1))
            .Returns(MakeTestUser(1));

        var result = _orderFacade.GetOrdersByFilters(dateFrom, dateTo, null, null);

        _orderServiceMock.VerifyAll();
        _repositoryUserMock.Verify(r => r.FindById(1), Times.Exactly(2));
        result.Should().HaveCount(2);
    }

    [TestMethod]
    public void GetOrdersByFilters_WhenNoMatch_ShouldReturnEmptyList()
    {
        var dateFrom = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var dateTo = new DateTime(2026, 1, 31, 0, 0, 0, DateTimeKind.Utc);

        _orderServiceMock
            .Setup(s => s.GetOrdersByFilters(dateFrom, dateTo, null, null))
            .Returns([]);

        var result = _orderFacade.GetOrdersByFilters(dateFrom, dateTo, null, null);

        _orderServiceMock.VerifyAll();
        result.Should().BeEmpty();
    }

    #endregion

    #endregion

    #region Prepare

    #region Success

    [TestMethod]
    public void Prepare_WhenValidId_ShouldDelegateToService()
    {
        _orderServiceMock.Setup(s => s.Prepare(1));

        _orderFacade.Prepare(1);

        _orderServiceMock.VerifyAll();
    }

    #endregion

    #endregion

    #region Cancel

    #region Success

    [TestMethod]
    public void Cancel_WhenValidId_ShouldDelegateToService()
    {
        _orderServiceMock.Setup(s => s.Cancel(1));

        _orderFacade.Cancel(1);

        _orderServiceMock.VerifyAll();
    }

    #endregion

    #endregion

    #region Dispatch

    #region Success

    [TestMethod]
    public void Dispatch_WhenValidId_ShouldDelegateToService()
    {
        _orderServiceMock.Setup(s => s.Dispatch(1));

        _orderFacade.Dispatch(1);

        _orderServiceMock.VerifyAll();
    }

    #endregion

    #endregion

    #region Deliver

    #region Success

    [TestMethod]
    public void Deliver_WhenValidId_ShouldDelegateToService()
    {
        _orderServiceMock.Setup(s => s.Deliver(1));

        _orderFacade.Deliver(1);

        _orderServiceMock.VerifyAll();
    }

    #endregion

    #endregion

    #region MarkNotDelivered

    #region Success

    [TestMethod]
    public void MarkNotDelivered_WhenValidId_ShouldDelegateToService()
    {
        _orderServiceMock.Setup(s => s.MarkNotDelivered(1));

        _orderFacade.MarkNotDelivered(1);

        _orderServiceMock.VerifyAll();
    }

    #endregion

    #endregion
}
