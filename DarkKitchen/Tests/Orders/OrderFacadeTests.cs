using BusinessLogics;
using BusinessLogicsAbstractions;
using Contract;
using Domain;
using Domain.Exceptions;
using FluentAssertions;
using Moq;

namespace Tests.Orders;

[TestClass]
public sealed class OrderFacadeTests
{
    private Mock<IOrderService> _orderServiceMock = null!;
    private Mock<IUserService> _userServiceMock = null!;
    private Mock<IOrderItemResolver> _orderItemResolverMock = null!;
    private Mock<IShippingTypeService> _shippingTypeServiceMock = null!;
    private Mock<IProductService> _productServiceMock = null!;
    private OrderFacade _orderFacade = null!;

    [TestInitialize]
    public void Initialize()
    {
        _orderServiceMock = new Mock<IOrderService>(MockBehavior.Strict);
        _userServiceMock = new Mock<IUserService>(MockBehavior.Strict);
        _orderItemResolverMock = new Mock<IOrderItemResolver>(MockBehavior.Strict);
        _shippingTypeServiceMock = new Mock<IShippingTypeService>(MockBehavior.Strict);
        _productServiceMock = new Mock<IProductService>(MockBehavior.Strict);

        _orderFacade = new OrderFacade(
            _orderServiceMock.Object,
            _orderItemResolverMock.Object,
            _userServiceMock.Object,
            _shippingTypeServiceMock.Object,
            _productServiceMock.Object);
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
        string deliveryType = "Express")
    {
        var address = Address.MakeAddress("Rivera", "1234");

        var items = new List<OrderItemData>
        {
            new()
            {
                ProductId = 1,
                Quantity = 2,
                UnitPrice = 100m,
                DiscountApplied = 0m
            }
        };

        var shippingCost = deliveryType == "TwentyFourHours" ? 200m : 300m;

        return Order.MakeOrder(clientId, deliveryType, address, items, shippingCost);
    }

    private static UserResponseDTO MakeTestUserDto(int id = 1) => new()
    {
        Id = id,
        Name = "Miguel",
        LastName = "Test",
        Email = "miguel@test.com",
        Phone = "099123456",
        Role = "Client"
    };

    private static ShippingTypeResponseDto MakeShippingTypeDto(string name, decimal cost) => new()
    {
        Name = name,
        Cost = cost
    };

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

    private void SetupUserAndProducts(int userId = 1)
    {
        _userServiceMock
            .Setup(s => s.GetByIdIncludingInactive(userId))
            .Returns(MakeTestUserDto(userId));

        _productServiceMock
            .Setup(s => s.GetNamesByIds(It.IsAny<List<int>>()))
            .Returns(new Dictionary<int, string>());
    }

    [TestMethod]
    public void CreateOrder_WhenValidRequest_ShouldReturnOrderResponse()
    {
        _shippingTypeServiceMock
            .Setup(s => s.GetByName("Express"))
            .Returns(MakeShippingTypeDto("Express", 300m));

        _orderItemResolverMock
            .Setup(r => r.Resolve(It.IsAny<List<CreateOrderItemRequest>>()))
            .Returns(MakeResolvedItems());

        _orderServiceMock
            .Setup(s => s.CreateOrder(It.Is<CreateOrderData>(data =>
                data.ClientId == 1 &&
                data.DeliveryType == "Express" &&
                data.ShippingCost == 300m &&
                data.Items.Count == 1)))
            .Returns(MakeTestOrder(1, "Express"));

        SetupUserAndProducts();

        var result = _orderFacade.CreateOrder(1, MakeCreateOrderRequest());

        result.Should().NotBeNull();
        result.Should().BeOfType<OrderResponse>();
        result.DeliveryType.Should().Be("Express");

        _orderItemResolverMock.VerifyAll();
        _orderServiceMock.VerifyAll();
        _userServiceMock.VerifyAll();
        _productServiceMock.VerifyAll();
    }

    [TestMethod]
    public void CreateOrder_WhenDeliveryTypeIsTwentyFourHours_ShouldCreateOrderWithTwentyFourHoursAndShipping200()
    {
        _shippingTypeServiceMock
            .Setup(s => s.GetByName("TwentyFourHours"))
            .Returns(MakeShippingTypeDto("TwentyFourHours", 200m));

        _orderItemResolverMock
            .Setup(r => r.Resolve(It.IsAny<List<CreateOrderItemRequest>>()))
            .Returns(MakeResolvedItems());

        _orderServiceMock
            .Setup(s => s.CreateOrder(It.Is<CreateOrderData>(data =>
                data.ClientId == 1 &&
                data.DeliveryType == "TwentyFourHours" &&
                data.ShippingCost == 200m &&
                data.Items.Count == 1)))
            .Returns(MakeTestOrder(1, "TwentyFourHours"));

        SetupUserAndProducts();

        var result = _orderFacade.CreateOrder(1, MakeCreateOrderRequest("TwentyFourHours"));

        result.Should().NotBeNull();
        result.Should().BeOfType<OrderResponse>();
        result.DeliveryType.Should().Be("TwentyFourHours");

        _orderItemResolverMock.VerifyAll();
        _orderServiceMock.VerifyAll();
        _userServiceMock.VerifyAll();
        _productServiceMock.VerifyAll();
    }

    [TestMethod]
    public void CreateOrder_WhenUserNotFound_ShouldThrowNotFoundException()
    {
        _shippingTypeServiceMock
            .Setup(s => s.GetByName("Express"))
            .Returns(MakeShippingTypeDto("Express", 300m));

        _orderItemResolverMock
            .Setup(r => r.Resolve(It.IsAny<List<CreateOrderItemRequest>>()))
            .Returns(MakeResolvedItems(1));

        _orderServiceMock
            .Setup(s => s.CreateOrder(It.IsAny<CreateOrderData>()))
            .Returns(MakeTestOrder(1));

        _userServiceMock
            .Setup(s => s.GetByIdIncludingInactive(1))
            .Throws(new NotFoundException("No se encontró el usuario con id 1"));

        var act = () => _orderFacade.CreateOrder(1, new CreateOrderRequest
        {
            DeliveryType = "Express",
            Address = new CreateAddressRequest { Street = "Rivera", DoorNumber = "1234" },
            Items = [new CreateOrderItemRequest { ProductId = 1, Quantity = 1 }]
        });

        act.Should()
            .Throw<NotFoundException>()
            .WithMessage("No se encontró el usuario con id 1");

        _orderItemResolverMock.VerifyAll();
        _orderServiceMock.VerifyAll();
        _userServiceMock.VerifyAll();
    }

    [TestMethod]
    public void CreateOrder_WhenDeliveryTypeIsInvalid_ShouldThrowValidationException()
    {
        _shippingTypeServiceMock
            .Setup(s => s.GetByName("DeliveryTypeInvalido"))
            .Returns((ShippingTypeResponseDto?)null);

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
        _shippingTypeServiceMock
            .Setup(s => s.GetByName("Express"))
            .Returns(MakeShippingTypeDto("Express", 300m));

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
        _shippingTypeServiceMock
            .Setup(s => s.GetByName("Express"))
            .Returns(MakeShippingTypeDto("Express", 300m));

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
        _shippingTypeServiceMock
            .Setup(s => s.GetByName("Express"))
            .Returns(MakeShippingTypeDto("Express", 300m));

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
        _shippingTypeServiceMock
            .Setup(s => s.GetByName("Express"))
            .Returns(MakeShippingTypeDto("Express", 300m));

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
        _shippingTypeServiceMock
            .Setup(s => s.GetByName("Express"))
            .Returns(MakeShippingTypeDto("Express", 300m));

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
        _shippingTypeServiceMock
            .Setup(s => s.GetByName("Express"))
            .Returns(MakeShippingTypeDto("Express", 300m));

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
        _shippingTypeServiceMock
            .Setup(s => s.GetByName("Express"))
            .Returns(MakeShippingTypeDto("Express", 300m));

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

        SetupUserAndProducts();

        var result = _orderFacade.GetById(1, currentUserId: 1, UserRolesDto.Administrative);

        result.Should().NotBeNull();
        result.Should().BeOfType<OrderResponse>();

        _orderServiceMock.VerifyAll();
        _userServiceMock.Verify(s => s.GetByIdIncludingInactive(1), Times.Once);
        _productServiceMock.VerifyAll();
    }

    [TestMethod]
    public void GetClientOrders_WhenClientHasOrders_ShouldReturnOrderResponseList()
    {
        _orderServiceMock
            .Setup(s => s.GetClientOrders(1, null, null, null))
            .Returns([MakeTestOrder(1), MakeTestOrder(1)]);

        _userServiceMock
            .Setup(s => s.GetByIdIncludingInactive(1))
            .Returns(MakeTestUserDto(1));

        _productServiceMock
            .Setup(s => s.GetNamesByIds(It.IsAny<List<int>>()))
            .Returns(new Dictionary<int, string>());

        var result = _orderFacade.GetClientOrders(1, null, null, null);

        result.Should().HaveCount(2);

        _orderServiceMock.VerifyAll();
        _userServiceMock.Verify(s => s.GetByIdIncludingInactive(1), Times.Exactly(2));
        _productServiceMock.Verify(s => s.GetNamesByIds(It.IsAny<List<int>>()), Times.Exactly(2));
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

        _userServiceMock
            .Setup(s => s.GetByIdIncludingInactive(1))
            .Returns(MakeTestUserDto(1));

        _productServiceMock
            .Setup(s => s.GetNamesByIds(It.IsAny<List<int>>()))
            .Returns(new Dictionary<int, string>());

        var result = _orderFacade.GetOrdersByFilters(dateFrom, dateTo, null, null);

        result.Should().HaveCount(2);

        _orderServiceMock.VerifyAll();
        _userServiceMock.Verify(s => s.GetByIdIncludingInactive(1), Times.Exactly(2));
        _productServiceMock.Verify(s => s.GetNamesByIds(It.IsAny<List<int>>()), Times.Exactly(2));
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

        _orderFacade.Cancel(1, currentUserId: 1, UserRolesDto.Administrative);

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

    [TestMethod]
    public void Delay_WhenValidId_ShouldDelegateToService()
    {
        _orderServiceMock.Setup(s => s.Delay(1));

        _orderFacade.Delay(1);

        _orderServiceMock.VerifyAll();
    }

    #region GetById ownership

    [TestMethod]
    public void GetById_WhenClientRequestsOtherClientsOrder_ShouldThrowForbiddenException()
    {
        _orderServiceMock
            .Setup(s => s.GetById(1))
            .Returns(MakeTestOrder(clientId: 1));

        var act = () => _orderFacade.GetById(1, currentUserId: 2, UserRolesDto.Client);

        act.Should().Throw<ForbiddenException>();
    }

    [TestMethod]
    public void GetById_WhenClientRequestsOwnOrder_ShouldReturnOrderResponse()
    {
        _orderServiceMock
            .Setup(s => s.GetById(1))
            .Returns(MakeTestOrder(clientId: 1));

        SetupUserAndProducts();

        var result = _orderFacade.GetById(1, currentUserId: 1, UserRolesDto.Client);

        result.Should().NotBeNull();
        _orderServiceMock.VerifyAll();
    }

    [TestMethod]
    public void GetById_WhenNonClientRequestsAnyOrder_ShouldReturnOrderResponse()
    {
        _orderServiceMock
            .Setup(s => s.GetById(1))
            .Returns(MakeTestOrder(clientId: 1));

        SetupUserAndProducts();

        var result = _orderFacade.GetById(1, currentUserId: 99, UserRolesDto.Administrative);

        result.Should().NotBeNull();
        _orderServiceMock.VerifyAll();
    }

    #endregion

    #region Cancel ownership

    [TestMethod]
    public void Cancel_WhenClientCancelsOtherClientsOrder_ShouldThrowForbiddenException()
    {
        _orderServiceMock
            .Setup(s => s.GetById(1))
            .Returns(MakeTestOrder(clientId: 1));

        var act = () => _orderFacade.Cancel(1, currentUserId: 2, UserRolesDto.Client);

        act.Should().Throw<ForbiddenException>();
        _orderServiceMock.Verify(s => s.Cancel(It.IsAny<int>()), Times.Never);
    }

    [TestMethod]
    public void Cancel_WhenClientCancelsOwnOrder_ShouldDelegateToService()
    {
        _orderServiceMock
            .Setup(s => s.GetById(1))
            .Returns(MakeTestOrder(clientId: 1));

        _orderServiceMock.Setup(s => s.Cancel(1));

        _orderFacade.Cancel(1, currentUserId: 1, UserRolesDto.Client);

        _orderServiceMock.VerifyAll();
    }

    [TestMethod]
    public void Cancel_WhenAdminCancelsAnyOrder_ShouldDelegateToService()
    {
        _orderServiceMock.Setup(s => s.Cancel(1));

        _orderFacade.Cancel(1, currentUserId: 99, UserRolesDto.Administrative);

        _orderServiceMock.VerifyAll();
    }

    #endregion

    [TestMethod]
    public void CreateOrder_WhenShippingTypeHasCost_ShouldPassCorrectShippingCostToService()
    {
        _shippingTypeServiceMock
            .Setup(s => s.GetByName("Rapido"))
            .Returns(MakeShippingTypeDto("Rapido", 350m));

        _orderItemResolverMock
            .Setup(r => r.Resolve(It.IsAny<List<CreateOrderItemRequest>>()))
            .Returns(MakeResolvedItems());

        CreateOrderData? capturedData = null;
        _orderServiceMock
            .Setup(s => s.CreateOrder(It.IsAny<CreateOrderData>()))
            .Callback<CreateOrderData>(d => capturedData = d)
            .Returns(MakeTestOrder(1, "Rapido"));

        SetupUserAndProducts();

        _orderFacade.CreateOrder(1, MakeCreateOrderRequest("Rapido"));

        _shippingTypeServiceMock.VerifyAll();
        _orderItemResolverMock.VerifyAll();
        _orderServiceMock.VerifyAll();
        _userServiceMock.VerifyAll();
        _productServiceMock.VerifyAll();
        capturedData!.ShippingCost.Should().Be(350m);
    }
}
