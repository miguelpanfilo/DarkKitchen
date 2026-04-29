using AlmacenamientoAbstracciones;
using Contratos.Orders;
using Dominio;
using Dominio.Exceptions;
using FluentAssertions;
using LogicaDeNegocio.Orders;
using LogicaDeNegocioAbstracciones.Exceptions;
using LogicaDeNegocioAbstracciones.Orders;
using Moq;

namespace Pruebas.Pedidos;

[TestClass]
public sealed class OrderFacadeTests
{
    private Mock<IOrderService> _orderServiceMock = null!;
    private Mock<IRepositoryUser> _repositoryUserMock = null!;
    private Mock<IOrderItemResolver> _orderItemResolverMock = null!;
    private OrderFacade _orderFacade = null!;

    [TestInitialize]
    public void Initialize()
    {
        _orderServiceMock = new Mock<IOrderService>(MockBehavior.Strict);
        _repositoryUserMock = new Mock<IRepositoryUser>(MockBehavior.Strict);
        _orderItemResolverMock = new Mock<IOrderItemResolver>(MockBehavior.Strict);

        _orderFacade = new OrderFacade(
            _orderServiceMock.Object,
            _orderItemResolverMock.Object,
            _repositoryUserMock.Object);
    }

    private static CreateOrderRequest MakeCreateOrderRequest(string deliveryType = "Express") => new()
    {
        DeliveryType = deliveryType,
        Address = new CreateAddressRequest
        {
            Street = "Rivera",
            DoorNumber = "1234"
        },
        Items =
        [
            new CreateOrderItemRequest
            {
                ProductId = 1,
                Quantity = 2
            }

        ]
    };

    private static Order MakeTestOrder(
        int clientId = 1,
        DeliveryType deliveryType = DeliveryType.Express)
    {
        var address = Address.MakeAddress("Rivera", "1234");

        var items = new List<(int ProductId, int Quantity, decimal UnitPrice, decimal DiscountApplied)>
        {
            (1, 2, 100m, 0m)
        };

        var shippingCost = deliveryType == DeliveryType.TwentyFourHours ? 200m : 300m;

        return Order.MakeOrder(clientId, deliveryType, address, items, shippingCost);
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

    private static List<ResolvedOrderItemDto> MakeResolvedItems(int quantity = 2) =>
    [
        new ResolvedOrderItemDto
        {
            ProductId = 1,
            Quantity = quantity,
            UnitPrice = 100m,
            DiscountApplied = 0m
        }

    ];

    [TestMethod]
    public void CreateOrder_WhenValidRequest_ShouldReturnOrderResponse()
    {
        _orderItemResolverMock
            .Setup(r => r.Resolve(It.IsAny<List<CreateOrderItemRequest>>()))
            .Returns(MakeResolvedItems());

        _orderServiceMock
            .Setup(s => s.CreateOrder(
                1,
                DeliveryType.Express,
                It.IsAny<Address>(),
                It.IsAny<List<ResolvedOrderItemDto>>(),
                300m))
            .Returns(MakeTestOrder(1, DeliveryType.Express));

        _repositoryUserMock
            .Setup(r => r.FindByIdIncludingInactive(1))
            .Returns(MakeTestUser(1));

        var result = _orderFacade.CreateOrder(1, MakeCreateOrderRequest());

        result.Should().NotBeNull();
        result.Should().BeOfType<OrderResponse>();
        result.DeliveryType.Should().Be(nameof(DeliveryType.Express));

        _orderItemResolverMock.VerifyAll();
        _orderServiceMock.VerifyAll();
        _repositoryUserMock.VerifyAll();
    }

    [TestMethod]
    public void CreateOrder_WhenDeliveryTypeIsTwentyFourHours_ShouldCreateOrderWithTwentyFourHoursAndShipping200()
    {
        _orderItemResolverMock
            .Setup(r => r.Resolve(It.IsAny<List<CreateOrderItemRequest>>()))
            .Returns(MakeResolvedItems());

        _orderServiceMock
            .Setup(s => s.CreateOrder(
                1,
                DeliveryType.TwentyFourHours,
                It.IsAny<Address>(),
                It.IsAny<List<ResolvedOrderItemDto>>(),
                200m))
            .Returns(MakeTestOrder(1, DeliveryType.TwentyFourHours));

        _repositoryUserMock
            .Setup(r => r.FindByIdIncludingInactive(1))
            .Returns(MakeTestUser(1));

        var result = _orderFacade.CreateOrder(1, MakeCreateOrderRequest("TwentyFourHours"));

        result.Should().NotBeNull();
        result.Should().BeOfType<OrderResponse>();
        result.DeliveryType.Should().Be(nameof(DeliveryType.TwentyFourHours));

        _orderItemResolverMock.VerifyAll();
        _orderServiceMock.VerifyAll();
        _repositoryUserMock.VerifyAll();
    }

    [TestMethod]
    public void CreateOrder_WhenUserNotFound_ShouldThrowNotFoundException()
    {
        _orderItemResolverMock
            .Setup(r => r.Resolve(It.IsAny<List<CreateOrderItemRequest>>()))
            .Returns(MakeResolvedItems(1));

        _orderServiceMock
            .Setup(s => s.CreateOrder(
                It.IsAny<int>(),
                It.IsAny<DeliveryType>(),
                It.IsAny<Address>(),
                It.IsAny<List<ResolvedOrderItemDto>>(),
                It.IsAny<decimal>()))
            .Returns(MakeTestOrder(1));

        _repositoryUserMock
            .Setup(r => r.FindByIdIncludingInactive(1))
            .Returns((User?)null);

        var act = () => _orderFacade.CreateOrder(1, new CreateOrderRequest
        {
            DeliveryType = "Express",
            Address = new CreateAddressRequest { Street = "Rivera", DoorNumber = "1234" },
            Items = [new CreateOrderItemRequest { ProductId = 1, Quantity = 1 }]
        });

        act.Should()
            .Throw<NotFoundException>()
            .WithMessage("No se encontró el cliente con id 1");

        _orderItemResolverMock.VerifyAll();
        _orderServiceMock.VerifyAll();
        _repositoryUserMock.VerifyAll();
    }

    [TestMethod]
    public void CreateOrder_WhenDeliveryTypeIsInvalid_ShouldThrowValidationException()
    {
        var request = new CreateOrderRequest
        {
            DeliveryType = "DeliveryTypeInvalido",
            Address = new CreateAddressRequest { Street = "Rivera", DoorNumber = "1234" },
            Items = [new CreateOrderItemRequest { ProductId = 1, Quantity = 2 }]
        };

        var act = () => _orderFacade.CreateOrder(1, request);

        act.Should()
            .Throw<ValidationException>()
            .WithMessage("El tipo de entrega no es válido");

        _orderServiceMock.VerifyNoOtherCalls();
        _orderItemResolverMock.VerifyNoOtherCalls();
    }

    [TestMethod]
    public void CreateOrder_WhenDeliveryTypeIsEmpty_ShouldThrowValidationException()
    {
        var request = new CreateOrderRequest
        {
            DeliveryType = string.Empty,
            Address = new CreateAddressRequest { Street = "Rivera", DoorNumber = "1234" },
            Items = [new CreateOrderItemRequest { ProductId = 1, Quantity = 2 }]
        };

        var act = () => _orderFacade.CreateOrder(1, request);

        act.Should()
            .Throw<ValidationException>()
            .WithMessage("El tipo de entrega es obligatorio");

        _orderServiceMock.VerifyNoOtherCalls();
        _orderItemResolverMock.VerifyNoOtherCalls();
    }

    [TestMethod]
    public void CreateOrder_WhenItemsAreEmpty_ShouldThrowValidationException()
    {
        var request = new CreateOrderRequest
        {
            DeliveryType = "Express",
            Address = new CreateAddressRequest { Street = "Rivera", DoorNumber = "1234" },
            Items = []
        };

        var act = () => _orderFacade.CreateOrder(1, request);

        act.Should()
            .Throw<ValidationException>()
            .WithMessage("El pedido debe contener al menos un producto");

        _orderServiceMock.VerifyNoOtherCalls();
        _orderItemResolverMock.VerifyNoOtherCalls();
    }

    [TestMethod]
    public void CreateOrder_WhenItemsAreNull_ShouldThrowValidationException()
    {
        var request = new CreateOrderRequest
        {
            DeliveryType = "Express",
            Address = new CreateAddressRequest { Street = "Rivera", DoorNumber = "1234" },
            Items = null!
        };

        var act = () => _orderFacade.CreateOrder(1, request);

        act.Should()
            .Throw<ValidationException>()
            .WithMessage("El pedido debe contener al menos un producto");

        _orderServiceMock.VerifyNoOtherCalls();
        _orderItemResolverMock.VerifyNoOtherCalls();
    }

    [TestMethod]
    public void CreateOrder_WhenItemQuantityIsZero_ShouldThrowValidationException()
    {
        var request = new CreateOrderRequest
        {
            DeliveryType = "Express",
            Address = new CreateAddressRequest { Street = "Rivera", DoorNumber = "1234" },
            Items = [new CreateOrderItemRequest { ProductId = 1, Quantity = 0 }]
        };

        var act = () => _orderFacade.CreateOrder(1, request);

        act.Should()
            .Throw<ValidationException>()
            .WithMessage("La cantidad de cada producto debe ser mayor a 0");

        _orderServiceMock.VerifyNoOtherCalls();
        _orderItemResolverMock.VerifyNoOtherCalls();
    }

    [TestMethod]
    public void CreateOrder_WhenItemQuantityIsNegative_ShouldThrowValidationException()
    {
        var request = new CreateOrderRequest
        {
            DeliveryType = "Express",
            Address = new CreateAddressRequest { Street = "Rivera", DoorNumber = "1234" },
            Items = [new CreateOrderItemRequest { ProductId = 1, Quantity = -2 }]
        };

        var act = () => _orderFacade.CreateOrder(1, request);

        act.Should()
            .Throw<ValidationException>()
            .WithMessage("La cantidad de cada producto debe ser mayor a 0");

        _orderServiceMock.VerifyNoOtherCalls();
        _orderItemResolverMock.VerifyNoOtherCalls();
    }

    [TestMethod]
    public void CreateOrder_WhenAddressIsNull_ShouldThrowValidationException()
    {
        var request = new CreateOrderRequest
        {
            DeliveryType = "Express",
            Address = null,
            Items = [new CreateOrderItemRequest { ProductId = 1, Quantity = 1 }]
        };

        var act = () => _orderFacade.CreateOrder(1, request);

        act.Should()
            .Throw<ValidationException>()
            .WithMessage("La dirección es obligatoria");

        _orderServiceMock.VerifyNoOtherCalls();
        _orderItemResolverMock.VerifyNoOtherCalls();
    }

    [TestMethod]
    public void CreateOrder_WhenStreetIsEmpty_ShouldThrowValidationException()
    {
        var request = new CreateOrderRequest
        {
            DeliveryType = "Express",
            Address = new CreateAddressRequest { Street = string.Empty, DoorNumber = "1234" },
            Items = [new CreateOrderItemRequest { ProductId = 1, Quantity = 1 }]
        };

        var act = () => _orderFacade.CreateOrder(1, request);

        act.Should()
            .Throw<ValidationException>()
            .WithMessage("La calle no puede estar vacía");

        _orderServiceMock.VerifyNoOtherCalls();
        _orderItemResolverMock.VerifyNoOtherCalls();
    }

    [TestMethod]
    public void CreateOrder_WhenDoorNumberIsEmpty_ShouldThrowValidationException()
    {
        var request = new CreateOrderRequest
        {
            DeliveryType = "Express",
            Address = new CreateAddressRequest { Street = "Rivera", DoorNumber = string.Empty },
            Items = [new CreateOrderItemRequest { ProductId = 1, Quantity = 1 }]
        };

        var act = () => _orderFacade.CreateOrder(1, request);

        act.Should()
            .Throw<ValidationException>()
            .WithMessage("El número de puerta no puede estar vacío");

        _orderServiceMock.VerifyNoOtherCalls();
        _orderItemResolverMock.VerifyNoOtherCalls();
    }

    [TestMethod]
    public void GetById_WhenOrderExists_ShouldReturnOrderResponse()
    {
        _orderServiceMock
            .Setup(s => s.GetById(1))
            .Returns(MakeTestOrder(1));

        _repositoryUserMock
            .Setup(r => r.FindByIdIncludingInactive(1))
            .Returns(MakeTestUser(1));

        var result = _orderFacade.GetById(1);

        result.Should().NotBeNull();
        result.Should().BeOfType<OrderResponse>();

        _orderServiceMock.VerifyAll();
        _repositoryUserMock.Verify(r => r.FindByIdIncludingInactive(1), Times.Once);
    }

    [TestMethod]
    public void GetClientOrders_WhenClientHasOrders_ShouldReturnOrderResponseList()
    {
        _orderServiceMock
            .Setup(s => s.GetClientOrders(1, null, null, null))
            .Returns([MakeTestOrder(1), MakeTestOrder(1)]);

        _repositoryUserMock
            .Setup(r => r.FindByIdIncludingInactive(1))
            .Returns(MakeTestUser(1));

        var result = _orderFacade.GetClientOrders(1, null, null, null);

        result.Should().HaveCount(2);

        _orderServiceMock.VerifyAll();
        _repositoryUserMock.Verify(r => r.FindByIdIncludingInactive(1), Times.Exactly(2));
    }

    [TestMethod]
    public void GetClientOrders_WhenNoOrders_ShouldReturnEmptyList()
    {
        _orderServiceMock
            .Setup(s => s.GetClientOrders(1, null, null, null))
            .Returns([]);

        var result = _orderFacade.GetClientOrders(1, null, null, null);

        result.Should().BeEmpty();

        _orderServiceMock.VerifyAll();
    }

    [TestMethod]
    public void GetOrdersByFilters_WhenOrdersMatch_ShouldReturnOrderResponseList()
    {
        var dateFrom = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var dateTo = new DateTime(2026, 1, 31, 0, 0, 0, DateTimeKind.Utc);

        _orderServiceMock
            .Setup(s => s.GetOrdersByFilters(dateFrom, dateTo, null, null))
            .Returns([MakeTestOrder(1), MakeTestOrder(1)]);

        _repositoryUserMock
            .Setup(r => r.FindByIdIncludingInactive(1))
            .Returns(MakeTestUser(1));

        var result = _orderFacade.GetOrdersByFilters(dateFrom, dateTo, null, null);

        result.Should().HaveCount(2);

        _orderServiceMock.VerifyAll();
        _repositoryUserMock.Verify(r => r.FindByIdIncludingInactive(1), Times.Exactly(2));
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

        result.Should().BeEmpty();

        _orderServiceMock.VerifyAll();
    }

    [TestMethod]
    public void Prepare_WhenValidId_ShouldDelegateToService()
    {
        _orderServiceMock.Setup(s => s.Prepare(1));

        _orderFacade.Prepare(1);

        _orderServiceMock.VerifyAll();
    }

    [TestMethod]
    public void Prepare_WhenDomainThrowsOrderException_ShouldThrowConflictException()
    {
        _orderServiceMock
            .Setup(s => s.Prepare(1))
            .Throws(new OrderException("No se puede transicionar"));

        var act = () => _orderFacade.Prepare(1);

        act.Should()
            .Throw<ConflictException>()
            .WithMessage("No se puede transicionar");

        _orderServiceMock.VerifyAll();
    }

    [TestMethod]
    public void Cancel_WhenValidId_ShouldDelegateToService()
    {
        _orderServiceMock.Setup(s => s.Cancel(1));

        _orderFacade.Cancel(1);

        _orderServiceMock.VerifyAll();
    }

    [TestMethod]
    public void Dispatch_WhenValidId_ShouldDelegateToService()
    {
        _orderServiceMock.Setup(s => s.Dispatch(1));

        _orderFacade.Dispatch(1);

        _orderServiceMock.VerifyAll();
    }

    [TestMethod]
    public void Deliver_WhenValidId_ShouldDelegateToService()
    {
        _orderServiceMock.Setup(s => s.Deliver(1));

        _orderFacade.Deliver(1);

        _orderServiceMock.VerifyAll();
    }

    [TestMethod]
    public void MarkNotDelivered_WhenValidId_ShouldDelegateToService()
    {
        _orderServiceMock.Setup(s => s.MarkNotDelivered(1));

        _orderFacade.MarkNotDelivered(1);

        _orderServiceMock.VerifyAll();
    }
}
