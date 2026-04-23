using Contratos;
using Contratos.Orders;
using Contratos.Users;
using DarkKitchen.Controllers;
using FluentAssertions;
using LogicaDeNegocioAbstracciones;
using LogicaDeNegocioAbstracciones.Orders;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Pruebas.Pedidos;

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
        Id = id,
        Role = UserRolesDto.Client,
        Email = "client@test.com"
    };

    private static CurrentUserDto MakePreparer() => new()
    {
        Id = 2,
        Role = UserRolesDto.Preparer,
        Email = "preparer@test.com"
    };

    private static CurrentUserDto MakeAdmin() => new()
    {
        Id = 3,
        Role = UserRolesDto.Administrative,
        Email = "admin@test.com"
    };

    private static OrderResponse MakeOrderResponse() => new()
    {
        Id = 1,
        OrderNumber = "ORD-123",
        ClientId = 1,
        Status = "Pending"
    };

    private static CreateOrderRequest MakeCreateOrderRequest() => new()
    {
        DeliveryType = DeliveryTypeDto.Express,
        Address = new CreateAddressRequest { Street = "Rivera", DoorNumber = "1234" },
        Items = [new CreateOrderItemRequest { ProductId = 1, Quantity = 2 }]
    };

    #region CreateOrder
    #region Success
    [TestMethod]
    public void CreateOrder_WhenClient_ShouldReturn201()
    {
        var request = MakeCreateOrderRequest();
        _orderFacadeMock.Setup(f => f.CreateOrder(1, request)).Returns(MakeOrderResponse());

        var result = _controller.CreateOrder(request);

        _orderFacadeMock.VerifyAll();
        result.Should().BeOfType<CreatedAtActionResult>();
    }
    #endregion
    #endregion

    #region GetById
    #region Success
    [TestMethod]
    public void GetById_WhenClientOwnsOrder_ShouldReturn200()
    {
        _orderFacadeMock.Setup(f => f.GetById(1)).Returns(MakeOrderResponse());

        var result = _controller.GetById(1);

        _orderFacadeMock.VerifyAll();
        result.Should().BeOfType<OkObjectResult>();
    }

    [TestMethod]
    public void GetById_WhenPreparer_ShouldReturn200()
    {
        SetCurrentUser(MakePreparer());
        _orderFacadeMock.Setup(f => f.GetById(1)).Returns(MakeOrderResponse());

        var result = _controller.GetById(1);

        _orderFacadeMock.VerifyAll();
        result.Should().BeOfType<OkObjectResult>();
    }
    #endregion

    #region Error
    [TestMethod]
    public void GetById_WhenClientDoesNotOwnOrder_ShouldReturn403()
    {
        SetCurrentUser(MakeClient(id: 99));
        _orderFacadeMock.Setup(f => f.GetById(1)).Returns(MakeOrderResponse());

        var result = _controller.GetById(1);

        _orderFacadeMock.VerifyAll();
        result.Should().BeOfType<ForbidResult>();
    }
    #endregion
    #endregion

    #region GetOrders
    #region Success
    [TestMethod]
    public void GetOrders_WhenClient_ShouldReturn200()
    {
        _orderFacadeMock.Setup(f => f.GetClientOrders(1, null, null, null)).Returns([MakeOrderResponse()]);

        var result = _controller.GetOrders(null, null, null, null);

        _orderFacadeMock.VerifyAll();
        result.Should().BeOfType<OkObjectResult>();
    }

    [TestMethod]
    public void GetOrders_WhenPreparer_ShouldReturn200()
    {
        SetCurrentUser(MakePreparer());
        var dateFrom = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var dateTo = new DateTime(2026, 1, 31, 0, 0, 0, DateTimeKind.Utc);
        _orderFacadeMock.Setup(f => f.GetOrdersByFilters(dateFrom, dateTo, null, null)).Returns([]);

        var result = _controller.GetOrders(dateFrom, dateTo, null, null);

        _orderFacadeMock.VerifyAll();
        result.Should().BeOfType<OkObjectResult>();
    }

    [TestMethod]
    public void GetOrders_WhenAdmin_ShouldReturn200()
    {
        SetCurrentUser(MakeAdmin());
        var dateFrom = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var dateTo = new DateTime(2026, 1, 31, 0, 0, 0, DateTimeKind.Utc);
        _orderFacadeMock.Setup(f => f.GetOrdersByFilters(dateFrom, dateTo, null, null)).Returns([]);

        var result = _controller.GetOrders(dateFrom, dateTo, null, null);

        _orderFacadeMock.VerifyAll();
        result.Should().BeOfType<OkObjectResult>();
    }
    #endregion

    #region Error
    [TestMethod]
    public void GetOrders_WhenPreparerWithoutDates_ShouldReturn400()
    {
        SetCurrentUser(MakePreparer());

        var result = _controller.GetOrders(null, null, null, null);

        result.Should().BeOfType<BadRequestObjectResult>();
    }
    #endregion
    #endregion

    #region ChangeStatus
    #region Success
    [TestMethod]
    public void ChangeStatus_WhenClientCancels_ShouldReturn200()
    {
        _orderFacadeMock.Setup(f => f.Cancel(1));
        _orderFacadeMock.Setup(f => f.GetById(1)).Returns(MakeOrderResponse());

        var result = _controller.ChangeStatus(1, new UpdateOrderStatusRequest { Estado = "Cancelado" });

        _orderFacadeMock.VerifyAll();
        result.Should().BeOfType<OkObjectResult>();
    }

    [TestMethod]
    public void ChangeStatus_WhenPreparerPrepares_ShouldReturn200()
    {
        SetCurrentUser(MakePreparer());
        _orderFacadeMock.Setup(f => f.Prepare(1));
        _orderFacadeMock.Setup(f => f.GetById(1)).Returns(MakeOrderResponse());

        var result = _controller.ChangeStatus(1, new UpdateOrderStatusRequest { Estado = "Preparado" });

        _orderFacadeMock.VerifyAll();
        result.Should().BeOfType<OkObjectResult>();
    }
    #endregion

    #region Error
    [TestMethod]
    public void ChangeStatus_WhenClientTriesToPrepare_ShouldReturn403()
    {
        var result = _controller.ChangeStatus(1, new UpdateOrderStatusRequest { Estado = "Preparado" });

        result.Should().BeOfType<ForbidResult>();
    }

    [TestMethod]
    public void ChangeStatus_WhenInvalidEstado_ShouldReturn400()
    {
        var result = _controller.ChangeStatus(1, new UpdateOrderStatusRequest { Estado = "Invalido" });

        result.Should().BeOfType<BadRequestObjectResult>();
    }
    #endregion
    #endregion

    #region Prepare
    #region Success
    [TestMethod]
    public void Prepare_WhenPreparer_ShouldReturn204()
    {
        SetCurrentUser(MakePreparer());
        _orderFacadeMock.Setup(f => f.Prepare(1));

        var result = _controller.Prepare(1);

        _orderFacadeMock.VerifyAll();
        result.Should().BeOfType<NoContentResult>();
    }
    #endregion
    #endregion

    #region Cancel
    #region Success
    [TestMethod]
    public void Cancel_WhenClient_ShouldReturn204()
    {
        _orderFacadeMock.Setup(f => f.Cancel(1));

        var result = _controller.Cancel(1);

        _orderFacadeMock.VerifyAll();
        result.Should().BeOfType<NoContentResult>();
    }

    [TestMethod]
    public void Cancel_WhenAdmin_ShouldReturn204()
    {
        SetCurrentUser(MakeAdmin());
        _orderFacadeMock.Setup(f => f.Cancel(1));

        var result = _controller.Cancel(1);

        _orderFacadeMock.VerifyAll();
        result.Should().BeOfType<NoContentResult>();
    }
    #endregion
    #endregion

    #region Dispatch
    #region Success
    [TestMethod]
    public void Dispatch_WhenPreparer_ShouldReturn204()
    {
        SetCurrentUser(MakePreparer());
        _orderFacadeMock.Setup(f => f.Dispatch(1));

        var result = _controller.Dispatch(1);

        _orderFacadeMock.VerifyAll();
        result.Should().BeOfType<NoContentResult>();
    }
    #endregion
    #endregion

    #region Deliver
    #region Success
    [TestMethod]
    public void Deliver_WhenPreparer_ShouldReturn204()
    {
        SetCurrentUser(MakePreparer());
        _orderFacadeMock.Setup(f => f.Deliver(1));

        var result = _controller.Deliver(1);

        _orderFacadeMock.VerifyAll();
        result.Should().BeOfType<NoContentResult>();
    }
    #endregion
    #endregion

    #region MarkNotDelivered
    #region Success
    [TestMethod]
    public void MarkNotDelivered_WhenPreparer_ShouldReturn204()
    {
        SetCurrentUser(MakePreparer());
        _orderFacadeMock.Setup(f => f.MarkNotDelivered(1));

        var result = _controller.MarkNotDelivered(1);

        _orderFacadeMock.VerifyAll();
        result.Should().BeOfType<NoContentResult>();
    }
    #endregion
    #endregion
}
