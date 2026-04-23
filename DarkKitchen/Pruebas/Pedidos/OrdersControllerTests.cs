using Contratos;
using Contratos.Orders;
using Contratos.Users;
using DarkKitchen.Controllers;
using FluentAssertions;
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
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };
    }

    private static CurrentUserDto MakeClient(int id = 1) => new()
    {
        Id = id,
        Role = UserRolesDto.Client,
        Email = "client@test.com"
    };

    private static CurrentUserDto MakePreparer(int id = 2) => new()
    {
        Id = id,
        Role = UserRolesDto.Preparer,
        Email = "preparer@test.com"
    };

    private static CurrentUserDto MakeAdmin(int id = 3) => new()
    {
        Id = id,
        Role = UserRolesDto.Administrative,
        Email = "admin@test.com"
    };

    private static OrderResponse MakeOrderResponse(int clientId = 1, string status = "Pending") => new()
    {
        Id = 1,
        OrderNumber = "ORD-123",
        Client = new ClientOrderResponse
        {
            Id = clientId,
            Name = "Miguel",
            LastName = "Panfilo",
            Email = "miguel@test.com"
        },
        DeliveryType = "Express",
        Address = new AddressResponse
        {
            Street = "Rivera",
            DoorNumber = "1234",
            Apartment = null
        },
        Items = new List<OrderItemResponse>
        {
            new()
            {
                Id = 1,
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
        DeliveryType = DeliveryTypeDto.Express,
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

    #region ChangeStatus

    [TestMethod]
    public void ChangeStatus_WhenClientCancelsOwnOrder_ShouldReturnOk()
    {
        var order = MakeOrderResponse(clientId: 1, status: "Cancelado");

        _orderFacadeMock
            .Setup(f => f.GetById(1))
            .Returns(order);

        _orderFacadeMock
            .Setup(f => f.Cancel(1));

        _orderFacadeMock
            .Setup(f => f.GetById(1))
            .Returns(order);

        var result = _controller.ChangeStatus(1, new UpdateOrderStatusRequest
        {
            Estado = "Cancelado"
        });

        _orderFacadeMock.VerifyAll();

        result.Should().BeOfType<OkObjectResult>();
    }

    [TestMethod]
    public void ChangeStatus_WhenStatusHasExtraSpaces_ShouldNormalizeAndReturnOk()
    {
        SetCurrentUser(MakePreparer());

        var updatedOrder = MakeOrderResponse(clientId: 1, status: "Prepared");

        _orderFacadeMock.Setup(f => f.Prepare(1));
        _orderFacadeMock.Setup(f => f.GetById(1)).Returns(updatedOrder);

        var result = _controller.ChangeStatus(1, new UpdateOrderStatusRequest
        {
            Estado = "   preparado   "
        });

        _orderFacadeMock.VerifyAll();

        result.Should().BeOfType<OkObjectResult>();
    }

    [TestMethod]
    public void ChangeStatus_WhenStatusIsUppercase_ShouldNormalizeAndReturnOk()
    {
        SetCurrentUser(MakePreparer());

        var updatedOrder = MakeOrderResponse(clientId: 1, status: "Prepared");

        _orderFacadeMock.Setup(f => f.Prepare(1));
        _orderFacadeMock.Setup(f => f.GetById(1)).Returns(updatedOrder);

        var result = _controller.ChangeStatus(1, new UpdateOrderStatusRequest
        {
            Estado = "PREPARADO"
        });

        _orderFacadeMock.VerifyAll();

        result.Should().BeOfType<OkObjectResult>();
    }

    [TestMethod]
    public void ChangeStatus_WhenStatusHasMixedCase_ShouldNormalizeAndReturnOk()
    {
        SetCurrentUser(MakePreparer());

        var updatedOrder = MakeOrderResponse(clientId: 1, status: "Prepared");

        _orderFacadeMock.Setup(f => f.Prepare(1));
        _orderFacadeMock.Setup(f => f.GetById(1)).Returns(updatedOrder);

        var result = _controller.ChangeStatus(1, new UpdateOrderStatusRequest
        {
            Estado = "PrEpArAdO"
        });

        _orderFacadeMock.VerifyAll();

        result.Should().BeOfType<OkObjectResult>();
    }

    [TestMethod]
    public void ChangeStatus_WhenStatusHasSpaceBetweenWords_ShouldNormalizeAndReturnOk()
    {
        SetCurrentUser(MakePreparer());

        var updatedOrder = MakeOrderResponse(clientId: 1, status: "OnTheWay");

        _orderFacadeMock.Setup(f => f.Dispatch(1));
        _orderFacadeMock.Setup(f => f.GetById(1)).Returns(updatedOrder);

        var result = _controller.ChangeStatus(1, new UpdateOrderStatusRequest
        {
            Estado = "en camino"
        });

        _orderFacadeMock.VerifyAll();

        result.Should().BeOfType<OkObjectResult>();
    }

    [TestMethod]
    public void ChangeStatus_WhenStatusHasHyphen_ShouldNormalizeAndReturnOk()
    {
        SetCurrentUser(MakePreparer());

        var updatedOrder = MakeOrderResponse(clientId: 1, status: "OnTheWay");

        _orderFacadeMock.Setup(f => f.Dispatch(1));
        _orderFacadeMock.Setup(f => f.GetById(1)).Returns(updatedOrder);

        var result = _controller.ChangeStatus(1, new UpdateOrderStatusRequest
        {
            Estado = "en-camino"
        });

        _orderFacadeMock.VerifyAll();

        result.Should().BeOfType<OkObjectResult>();
    }

    [TestMethod]
    public void ChangeStatus_WhenStatusIsWhitespace_ShouldReturnBadRequest()
    {
        var result = _controller.ChangeStatus(1, new UpdateOrderStatusRequest
        {
            Estado = "   "
        });

        result.Should().BeOfType<ObjectResult>();
        var objectResult = (ObjectResult)result;
        objectResult.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
    }

    [TestMethod]
    public void ChangeStatus_WhenStatusIsNull_ShouldReturnBadRequest()
    {
        var result = _controller.ChangeStatus(1, new UpdateOrderStatusRequest
        {
            Estado = null!
        });

        result.Should().BeOfType<ObjectResult>();
        var objectResult = (ObjectResult)result;
        objectResult.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
    }

    [TestMethod]
    public void ChangeStatus_WhenStatusHasUnderscore_ShouldNormalizeAndReturnOk()
    {
        SetCurrentUser(MakePreparer());

        var updatedOrder = MakeOrderResponse(clientId: 1, status: "OnTheWay");

        _orderFacadeMock.Setup(f => f.Dispatch(1));
        _orderFacadeMock.Setup(f => f.GetById(1)).Returns(updatedOrder);

        var result = _controller.ChangeStatus(1, new UpdateOrderStatusRequest
        {
            Estado = "en_camino"
        });

        _orderFacadeMock.VerifyAll();

        result.Should().BeOfType<OkObjectResult>();
    }

    [TestMethod]
    public void ChangeStatus_WhenClientCancelsOrderOfAnotherClient_ShouldReturn403ObjectResult()
    {
        SetCurrentUser(MakeClient(id: 99));

        _orderFacadeMock
            .Setup(f => f.GetById(1))
            .Returns(MakeOrderResponse(clientId: 1));

        var result = _controller.ChangeStatus(1, new UpdateOrderStatusRequest
        {
            Estado = "Cancelado"
        });

        _orderFacadeMock.VerifyAll();

        result.Should().BeOfType<ObjectResult>();
        var objectResult = (ObjectResult)result;
        objectResult.StatusCode.Should().Be(StatusCodes.Status403Forbidden);
    }

    [TestMethod]
    public void ChangeStatus_WhenClientTriesToPrepare_ShouldReturn403ObjectResult()
    {
        var result = _controller.ChangeStatus(1, new UpdateOrderStatusRequest
        {
            Estado = "Preparado"
        });

        result.Should().BeOfType<ObjectResult>();
        var objectResult = (ObjectResult)result;
        objectResult.StatusCode.Should().Be(StatusCodes.Status403Forbidden);
    }

    [TestMethod]
    public void ChangeStatus_WhenPreparerPrepares_ShouldReturnOk()
    {
        SetCurrentUser(MakePreparer());

        var updatedOrder = MakeOrderResponse(clientId: 1, status: "Preparado");

        _orderFacadeMock
            .Setup(f => f.Prepare(1));

        _orderFacadeMock
            .Setup(f => f.GetById(1))
            .Returns(updatedOrder);

        var result = _controller.ChangeStatus(1, new UpdateOrderStatusRequest
        {
            Estado = "Preparado"
        });

        _orderFacadeMock.VerifyAll();

        result.Should().BeOfType<OkObjectResult>();
    }

    [TestMethod]
    public void ChangeStatus_WhenPreparerDispatches_ShouldReturnOk()
    {
        SetCurrentUser(MakePreparer());

        var updatedOrder = MakeOrderResponse(clientId: 1, status: "EnCamino");

        _orderFacadeMock
            .Setup(f => f.Dispatch(1));

        _orderFacadeMock
            .Setup(f => f.GetById(1))
            .Returns(updatedOrder);

        var result = _controller.ChangeStatus(1, new UpdateOrderStatusRequest
        {
            Estado = "EnCamino"
        });

        _orderFacadeMock.VerifyAll();

        result.Should().BeOfType<OkObjectResult>();
    }

    [TestMethod]
    public void ChangeStatus_WhenPreparerDelivers_ShouldReturnOk()
    {
        SetCurrentUser(MakePreparer());

        var updatedOrder = MakeOrderResponse(clientId: 1, status: "Entregado");

        _orderFacadeMock
            .Setup(f => f.Deliver(1));

        _orderFacadeMock
            .Setup(f => f.GetById(1))
            .Returns(updatedOrder);

        var result = _controller.ChangeStatus(1, new UpdateOrderStatusRequest
        {
            Estado = "Entregado"
        });

        _orderFacadeMock.VerifyAll();

        result.Should().BeOfType<OkObjectResult>();
    }

    [TestMethod]
    public void ChangeStatus_WhenPreparerMarksNotDelivered_ShouldReturnOk()
    {
        SetCurrentUser(MakePreparer());

        var updatedOrder = MakeOrderResponse(clientId: 1, status: "NoEntregado");

        _orderFacadeMock
            .Setup(f => f.MarkNotDelivered(1));

        _orderFacadeMock
            .Setup(f => f.GetById(1))
            .Returns(updatedOrder);

        var result = _controller.ChangeStatus(1, new UpdateOrderStatusRequest
        {
            Estado = "NoEntregado"
        });

        _orderFacadeMock.VerifyAll();

        result.Should().BeOfType<OkObjectResult>();
    }

    [TestMethod]
    public void ChangeStatus_WhenEstadoIsInvalid_ShouldReturnBadRequestObjectResult()
    {
        var result = _controller.ChangeStatus(1, new UpdateOrderStatusRequest
        {
            Estado = "Invalido"
        });

        result.Should().BeOfType<ObjectResult>();
        var objectResult = (ObjectResult)result;
        objectResult.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
    }

    #endregion
}
