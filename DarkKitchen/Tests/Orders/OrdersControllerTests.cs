using BusinessLogicsAbstractions.Orders;
using Contract.Orders;
using Contract.Users;
using DarkKitchen.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Tests.Orders;

[TestClass]
public sealed class OrdersControllerTests
{
    private Mock<IOrderFacade> _orderFacadeMock = null!;
    private OrdersController _controller = null!;

    [TestInitialize]
    public void Initialize()
    {
        _orderFacadeMock = new Mock<IOrderFacade>(MockBehavior.Strict);
        _controller = new OrdersController(_orderFacadeMock.Object);
        SetCurrentUser(MakeClient());
    }

    private void SetCurrentUser(CurrentUserDto user)
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Items["CurrentUser"] = user;
        _controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
    }

    private static CurrentUserDto MakeClient(int id = 1) => new()
    {
        Id = id, Role = UserRolesDto.Client, Email = "client@test.com"
    };

    private static CurrentUserDto MakePreparer(int id = 2) => new()
    {
        Id = id, Role = UserRolesDto.Preparer, Email = "preparer@test.com"
    };

    private static CurrentUserDto MakeAdmin(int id = 3) => new()
    {
        Id = id, Role = UserRolesDto.Administrative, Email = "admin@test.com"
    };

    private static OrderResponse MakeOrderResponse(int clientId = 1, string status = "Pending") => new()
    {
        Id = 1,
        OrderNumber = "ORD-123",
        Client =
            new ClientOrderResponse
            {
                Id = clientId, Name = "Miguel", LastName = "Panfilo", Email = "miguel@test.com"
            },
        DeliveryType = "Express",
        Address = new AddressResponse { Street = "Rivera", DoorNumber = "1234", Apartment = null },
        Items = new List<OrderItemResponse>
        {
            new()
            {
                ProductId = 1,
                Quantity = 2,
                UnitPrice = 100m,
                DiscountApplied = 0m,
                LineTotal = 200m
            }
        },
        ItemsCount = 1,
        Status = status,
        CreatedAt = DateTime.UtcNow,
        StatusUpdatedAt = DateTime.UtcNow,
        Subtotal = 200m,
        ShippingCost = 50m,
        Total = 250m
    };

    private static CreateOrderRequest MakeCreateOrderRequest() => new()
    {
        DeliveryType = "Express",
        Address = new CreateAddressRequest { Street = "Rivera", DoorNumber = "1234" },
        Items =
        [
            new CreateOrderItemRequest { ProductId = 1, Quantity = 2 }
        ]
    };

    #region CreateOrder

    [TestMethod]
    public void CreateOrder_WhenClient_ShouldReturnCreatedAtAction()
    {
        var request = MakeCreateOrderRequest();
        var response = MakeOrderResponse();

        _orderFacadeMock
            .Setup(f => f.CreateOrder(1, request))
            .Returns(response);

        var result = _controller.CreateOrder(request);

        _orderFacadeMock.VerifyAll();

        result.Should().BeOfType<CreatedAtActionResult>();
        var createdResult = (CreatedAtActionResult)result;
        createdResult.ActionName.Should().Be(nameof(OrdersController.GetById));
        createdResult.RouteValues!["id"].Should().Be(response.Id);
        createdResult.Value.Should().Be(response);
    }

    #endregion

    #region GetById

    [TestMethod]
    public void GetById_WhenClientOwnsOrder_ShouldReturnOk()
    {
        var order = MakeOrderResponse(clientId: 1);

        _orderFacadeMock
            .Setup(f => f.GetById(1))
            .Returns(order);

        var result = _controller.GetById(1);

        _orderFacadeMock.VerifyAll();

        result.Should().BeOfType<OkObjectResult>();
        ((OkObjectResult)result).Value.Should().Be(order);
    }

    [TestMethod]
    public void GetById_WhenClientDoesNotOwnOrder_ShouldReturnForbid()
    {
        SetCurrentUser(MakeClient(id: 99));
        var order = MakeOrderResponse(clientId: 1);

        _orderFacadeMock
            .Setup(f => f.GetById(1))
            .Returns(order);

        var result = _controller.GetById(1);

        _orderFacadeMock.VerifyAll();

        result.Should().BeOfType<ForbidResult>();
    }

    [TestMethod]
    public void GetById_WhenPreparer_ShouldReturnOk()
    {
        SetCurrentUser(MakePreparer());
        var order = MakeOrderResponse(clientId: 1);

        _orderFacadeMock
            .Setup(f => f.GetById(1))
            .Returns(order);

        var result = _controller.GetById(1);

        _orderFacadeMock.VerifyAll();

        result.Should().BeOfType<OkObjectResult>();
    }

    [TestMethod]
    public void GetById_WhenAdmin_ShouldReturnOk()
    {
        SetCurrentUser(MakeAdmin());
        var order = MakeOrderResponse(clientId: 1);

        _orderFacadeMock
            .Setup(f => f.GetById(1))
            .Returns(order);

        var result = _controller.GetById(1);

        _orderFacadeMock.VerifyAll();

        result.Should().BeOfType<OkObjectResult>();
    }

    #endregion

    #region GetOrders

    [TestMethod]
    public void GetOrders_WhenClient_ShouldReturnOkWithClientOrders()
    {
        var orders = new List<OrderResponse> { MakeOrderResponse(clientId: 1) };

        _orderFacadeMock
            .Setup(f => f.GetClientOrders(1, null, null, null))
            .Returns(orders);

        var result = _controller.GetOrders(null, null, null, null);

        _orderFacadeMock.VerifyAll();

        result.Should().BeOfType<OkObjectResult>();
        ((OkObjectResult)result).Value.Should().BeEquivalentTo(orders);
    }

    [TestMethod]
    public void GetOrders_WhenPreparerWithDates_ShouldReturnOk()
    {
        SetCurrentUser(MakePreparer());

        var dateFrom = new DateTime(2026, 1, 1);
        var dateTo = new DateTime(2026, 1, 31);
        var orders = new List<OrderResponse>();

        _orderFacadeMock
            .Setup(f => f.GetOrdersByFilters(dateFrom, dateTo, null, null))
            .Returns(orders);

        var result = _controller.GetOrders(dateFrom, dateTo, null, null);

        _orderFacadeMock.VerifyAll();

        result.Should().BeOfType<OkObjectResult>();
    }

    [TestMethod]
    public void GetOrders_WhenAdminWithDates_ShouldReturnOk()
    {
        SetCurrentUser(MakeAdmin());

        var dateFrom = new DateTime(2026, 1, 1);
        var dateTo = new DateTime(2026, 1, 31);
        var orders = new List<OrderResponse>();

        _orderFacadeMock
            .Setup(f => f.GetOrdersByFilters(dateFrom, dateTo, null, null))
            .Returns(orders);

        var result = _controller.GetOrders(dateFrom, dateTo, null, null);

        _orderFacadeMock.VerifyAll();

        result.Should().BeOfType<OkObjectResult>();
    }

    [TestMethod]
    public void GetOrders_WhenPreparerWithoutDates_ShouldReturnBadRequest()
    {
        SetCurrentUser(MakePreparer());

        var result = _controller.GetOrders(null, null, null, null);

        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [TestMethod]
    public void GetOrders_WhenAdminWithoutDates_ShouldReturnBadRequest()
    {
        SetCurrentUser(MakeAdmin());

        var result = _controller.GetOrders(null, null, null, null);

        result.Should().BeOfType<BadRequestObjectResult>();
    }

    #endregion

    #region StatusEndpoints

    [TestMethod]
    public void Prepare_WhenPreparer_ShouldReturnOk()
    {
        SetCurrentUser(MakePreparer());

        var updatedOrder = MakeOrderResponse(clientId: 1, status: "Prepared");

        _orderFacadeMock.Setup(f => f.Prepare(1));
        _orderFacadeMock.Setup(f => f.GetById(1)).Returns(updatedOrder);

        var result = _controller.Prepare(1);

        _orderFacadeMock.VerifyAll();

        result.Should().BeOfType<OkObjectResult>();
        var okResult = (OkObjectResult)result;
        okResult.Value.Should().BeOfType<OrderStatusResponse>();
    }

    [TestMethod]
    public void Cancel_WhenClientCancelsOwnOrder_ShouldReturnOk()
    {
        SetCurrentUser(MakeClient(id: 1));

        var order = MakeOrderResponse(clientId: 1, status: "Pending");
        var updatedOrder = MakeOrderResponse(clientId: 1, status: "Cancelled");

        _orderFacadeMock.SetupSequence(f => f.GetById(1))
            .Returns(order)
            .Returns(updatedOrder);

        _orderFacadeMock.Setup(f => f.Cancel(1));

        var result = _controller.Cancel(1);

        _orderFacadeMock.VerifyAll();

        result.Should().BeOfType<OkObjectResult>();
    }

    [TestMethod]
    public void Cancel_WhenClientCancelsAnotherClientOrder_ShouldReturnForbidden()
    {
        SetCurrentUser(MakeClient(id: 99));

        var order = MakeOrderResponse(clientId: 1);

        _orderFacadeMock.Setup(f => f.GetById(1)).Returns(order);

        var result = _controller.Cancel(1);

        _orderFacadeMock.VerifyAll();

        result.Should().BeOfType<ObjectResult>();
        var objectResult = (ObjectResult)result;
        objectResult.StatusCode.Should().Be(StatusCodes.Status403Forbidden);
    }

    [TestMethod]
    public void Prepare_WhenAdmin_ShouldReturnOk()
    {
        SetCurrentUser(MakeAdmin());

        var updatedOrder = MakeOrderResponse(clientId: 1, status: "Prepared");

        _orderFacadeMock.Setup(f => f.Prepare(1));
        _orderFacadeMock.Setup(f => f.GetById(1)).Returns(updatedOrder);

        var result = _controller.Prepare(1);

        _orderFacadeMock.VerifyAll();

        result.Should().BeOfType<OkObjectResult>();

        var okResult = (OkObjectResult)result;
        var response = okResult.Value.Should().BeOfType<OrderStatusResponse>().Subject;

        response.PedidoId.Should().Be(updatedOrder.Id);
        response.Estado.Should().Be(updatedOrder.Status);
        response.FechaCambioEstado.Should().Be(updatedOrder.StatusUpdatedAt);
    }

    [TestMethod]
    public void Dispatch_WhenAdmin_ShouldReturnOk()
    {
        SetCurrentUser(MakeAdmin());

        var updatedOrder = MakeOrderResponse(clientId: 1, status: "OnTheWay");

        _orderFacadeMock.Setup(f => f.Dispatch(1));
        _orderFacadeMock.Setup(f => f.GetById(1)).Returns(updatedOrder);

        var result = _controller.Dispatch(1);

        _orderFacadeMock.VerifyAll();

        result.Should().BeOfType<OkObjectResult>();

        var okResult = (OkObjectResult)result;
        var response = okResult.Value.Should().BeOfType<OrderStatusResponse>().Subject;

        response.PedidoId.Should().Be(updatedOrder.Id);
        response.Estado.Should().Be(updatedOrder.Status);
        response.FechaCambioEstado.Should().Be(updatedOrder.StatusUpdatedAt);
    }

    [TestMethod]
    public void Deliver_WhenAdmin_ShouldReturnOk()
    {
        SetCurrentUser(MakeAdmin());

        var updatedOrder = MakeOrderResponse(clientId: 1, status: "Delivered");

        _orderFacadeMock.Setup(f => f.Deliver(1));
        _orderFacadeMock.Setup(f => f.GetById(1)).Returns(updatedOrder);

        var result = _controller.Deliver(1);

        _orderFacadeMock.VerifyAll();

        result.Should().BeOfType<OkObjectResult>();

        var okResult = (OkObjectResult)result;
        var response = okResult.Value.Should().BeOfType<OrderStatusResponse>().Subject;

        response.PedidoId.Should().Be(updatedOrder.Id);
        response.Estado.Should().Be(updatedOrder.Status);
        response.FechaCambioEstado.Should().Be(updatedOrder.StatusUpdatedAt);
    }

    [TestMethod]
    public void MarkNotDelivered_WhenAdmin_ShouldReturnOk()
    {
        SetCurrentUser(MakeAdmin());

        var updatedOrder = MakeOrderResponse(clientId: 1, status: "NotDelivered");

        _orderFacadeMock.Setup(f => f.MarkNotDelivered(1));
        _orderFacadeMock.Setup(f => f.GetById(1)).Returns(updatedOrder);

        var result = _controller.MarkNotDelivered(1);

        _orderFacadeMock.VerifyAll();

        result.Should().BeOfType<OkObjectResult>();

        var okResult = (OkObjectResult)result;
        var response = okResult.Value.Should().BeOfType<OrderStatusResponse>().Subject;

        response.PedidoId.Should().Be(updatedOrder.Id);
        response.Estado.Should().Be(updatedOrder.Status);
        response.FechaCambioEstado.Should().Be(updatedOrder.StatusUpdatedAt);
    }

    [TestMethod]
    public void Cancel_WhenAdmin_ShouldReturnOk()
    {
        SetCurrentUser(MakeAdmin());

        var updatedOrder = MakeOrderResponse(clientId: 1, status: "Cancelled");

        _orderFacadeMock.Setup(f => f.Cancel(1));
        _orderFacadeMock.Setup(f => f.GetById(1)).Returns(updatedOrder);

        var result = _controller.Cancel(1);

        _orderFacadeMock.VerifyAll();

        result.Should().BeOfType<OkObjectResult>();
    }

    [TestMethod]
    public void Dispatch_WhenPreparer_ShouldReturnOk()
    {
        SetCurrentUser(MakePreparer());

        var updatedOrder = MakeOrderResponse(clientId: 1, status: "OnTheWay");

        _orderFacadeMock.Setup(f => f.Dispatch(1));
        _orderFacadeMock.Setup(f => f.GetById(1)).Returns(updatedOrder);

        var result = _controller.Dispatch(1);

        _orderFacadeMock.VerifyAll();

        result.Should().BeOfType<OkObjectResult>();
    }

    [TestMethod]
    public void Deliver_WhenPreparer_ShouldReturnOk()
    {
        SetCurrentUser(MakePreparer());

        var updatedOrder = MakeOrderResponse(clientId: 1, status: "Delivered");

        _orderFacadeMock.Setup(f => f.Deliver(1));
        _orderFacadeMock.Setup(f => f.GetById(1)).Returns(updatedOrder);

        var result = _controller.Deliver(1);

        _orderFacadeMock.VerifyAll();

        result.Should().BeOfType<OkObjectResult>();
    }

    [TestMethod]
    public void MarkNotDelivered_WhenPreparer_ShouldReturnOk()
    {
        SetCurrentUser(MakePreparer());

        var updatedOrder = MakeOrderResponse(clientId: 1, status: "NotDelivered");

        _orderFacadeMock.Setup(f => f.MarkNotDelivered(1));
        _orderFacadeMock.Setup(f => f.GetById(1)).Returns(updatedOrder);

        var result = _controller.MarkNotDelivered(1);

        _orderFacadeMock.VerifyAll();

        result.Should().BeOfType<OkObjectResult>();
    }

    [TestMethod]
    public void Delay_WhenAdmin_ShouldReturnOk()
    {
        SetCurrentUser(MakeAdmin());

        var updatedOrder = MakeOrderResponse(clientId: 1, status: "Delayed");

        _orderFacadeMock.Setup(f => f.Delay(1));
        _orderFacadeMock.Setup(f => f.GetById(1)).Returns(updatedOrder);

        var result = _controller.Delay(1);

        _orderFacadeMock.VerifyAll();

        result.Should().BeOfType<OkObjectResult>();
    }

    [TestMethod]
    public void Delay_WhenAdmin_ShouldReturnOkWithStatusResponse()
    {
        SetCurrentUser(MakeAdmin());

        var updatedOrder = MakeOrderResponse(clientId: 1, status: "Delayed");

        _orderFacadeMock.Setup(f => f.Delay(1));
        _orderFacadeMock.Setup(f => f.GetById(1)).Returns(updatedOrder);

        var result = _controller.Delay(1);

        _orderFacadeMock.VerifyAll();

        result.Should().BeOfType<OkObjectResult>();

        var okResult = (OkObjectResult)result;
        var response = okResult.Value.Should().BeOfType<OrderStatusResponse>().Subject;

        response.PedidoId.Should().Be(updatedOrder.Id);
        response.Estado.Should().Be(updatedOrder.Status);
        response.FechaCambioEstado.Should().Be(updatedOrder.StatusUpdatedAt);
    }

    #endregion
}
