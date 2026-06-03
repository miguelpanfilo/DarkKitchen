using BusinessLogics;
using BusinessLogicsAbstractions;
using Contract;
using DataAccessAbstractions;
using Domain;
using Domain.Exceptions;
using FluentAssertions;
using Moq;

namespace Tests.Orders;

[TestClass]
public sealed class OrderServiceTests
{
    private Mock<IOrderRepository> _orderRepositoryMock = null!;
    private OrderService _orderService = null!;

    [TestInitialize]
    public void Initialize()
    {
        _orderRepositoryMock = new Mock<IOrderRepository>(MockBehavior.Strict);
        _orderService = new OrderService(_orderRepositoryMock.Object);
    }

    private static Order MakeOrder()
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

        return Order.MakeOrder(1, "Express", address, items, 200m);
    }

    #region CreateOrder
    #region Success
    [TestMethod]
    public void CreateOrder_WhenValidData_ShouldReturnCreatedOrder()
    {
        var expected = MakeOrder();

        _orderRepositoryMock
            .Setup(r => r.Add(It.IsAny<Order>()))
            .Returns(expected);

        var items = new List<ResolvedOrderItemDto>
        {
            new()
            {
                ProductId = 1,
                Quantity = 2,
                UnitPrice = 100m,
                DiscountApplied = 0m
            }
        };

        var data = new CreateOrderData
        {
            ClientId = 1,
            DeliveryType = "Express",
            Address = Address.MakeAddress("Rivera", "1234"),
            Items = items,
            ShippingCost = 200m
        };

        var result = _orderService.CreateOrder(data);

        _orderRepositoryMock.VerifyAll();
        result.Should().NotBeNull();
        result.ClientId.Should().Be(1);
    }
    #endregion
    #endregion

    #region GetById
    #region Success
    [TestMethod]
    public void GetById_WhenOrderExists_ShouldReturnOrder()
    {
        var order = MakeOrder();
        _orderRepositoryMock.Setup(r => r.GetById(1)).Returns(order);

        var result = _orderService.GetById(1);

        _orderRepositoryMock.VerifyAll();
        result.Should().NotBeNull();
    }
    #endregion

    #region Error
    [TestMethod]
    public void GetById_WhenOrderNotFound_ShouldThrowNotFoundException()
    {
        _orderRepositoryMock.Setup(r => r.GetById(99)).Returns((Order?)null);

        var act = () => _orderService.GetById(99);

        act.Should().Throw<NotFoundException>();
        _orderRepositoryMock.VerifyAll();
    }
    #endregion
    #endregion

    #region GetClientOrders
    #region Success
    [TestMethod]
    public void GetClientOrders_WhenClientHasOrders_ShouldReturnList()
    {
        _orderRepositoryMock
            .Setup(r => r.GetByClientId(1, null, null, null))
            .Returns([MakeOrder(), MakeOrder()]);

        var result = _orderService.GetClientOrders(1, null, null, null);

        _orderRepositoryMock.VerifyAll();
        result.Should().HaveCount(2);
    }

    [TestMethod]
    public void GetClientOrders_WhenNoOrders_ShouldReturnEmptyList()
    {
        _orderRepositoryMock
            .Setup(r => r.GetByClientId(1, null, null, null))
            .Returns([]);

        var result = _orderService.GetClientOrders(1, null, null, null);

        _orderRepositoryMock.VerifyAll();
        result.Should().BeEmpty();
    }
    #endregion
    #endregion

    #region GetOrdersByFilters
    #region Success
    [TestMethod]
    public void GetOrdersByFilters_WhenFiltersMatch_ShouldReturnList()
    {
        var dateFrom = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var dateTo = new DateTime(2026, 1, 31, 0, 0, 0, DateTimeKind.Utc);

        _orderRepositoryMock
            .Setup(r => r.GetByFilters(dateFrom, dateTo, null, null))
            .Returns([MakeOrder()]);

        var result = _orderService.GetOrdersByFilters(dateFrom, dateTo, null, null);

        _orderRepositoryMock.VerifyAll();
        result.Should().HaveCount(1);
    }

    [TestMethod]
    public void GetOrdersByFilters_WhenNoMatch_ShouldReturnEmptyList()
    {
        var dateFrom = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var dateTo = new DateTime(2026, 1, 31, 0, 0, 0, DateTimeKind.Utc);

        _orderRepositoryMock
            .Setup(r => r.GetByFilters(dateFrom, dateTo, null, null))
            .Returns([]);

        var result = _orderService.GetOrdersByFilters(dateFrom, dateTo, null, null);

        _orderRepositoryMock.VerifyAll();
        result.Should().BeEmpty();
    }
    #endregion

    #region Error
    [TestMethod]
    public void GetOrdersByFilters_WhenDateFromIsAfterDateTo_ShouldThrowValidationException()
    {
        var dateFrom = new DateTime(2026, 1, 31, 0, 0, 0, DateTimeKind.Utc);
        var dateTo = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        var act = () => _orderService.GetOrdersByFilters(dateFrom, dateTo, null, null);

        act.Should().Throw<ValidationException>()
            .WithMessage("Fecha inicio no puede ser posterior a fecha fin");
    }
    #endregion
    #endregion

    #region Prepare
    #region Success
    [TestMethod]
    public void Prepare_WhenOrderIsPending_ShouldTransitionToPrepared()
    {
        var order = MakeOrder();
        _orderRepositoryMock.Setup(r => r.GetById(1)).Returns(order);
        _orderRepositoryMock.Setup(r => r.Update(order));

        _orderService.Prepare(1);

        _orderRepositoryMock.VerifyAll();
        order.Status.Should().Be(OrderStatus.Prepared);
    }
    #endregion

    #region Error
    [TestMethod]
    public void Prepare_WhenOrderNotFound_ShouldThrowNotFoundException()
    {
        _orderRepositoryMock.Setup(r => r.GetById(99)).Returns((Order?)null);

        var act = () => _orderService.Prepare(99);

        act.Should().Throw<NotFoundException>();
        _orderRepositoryMock.VerifyAll();
    }
    #endregion
    #endregion

    #region Cancel
    #region Success
    [TestMethod]
    public void Cancel_WhenOrderIsPending_ShouldTransitionToCancelled()
    {
        var order = MakeOrder();
        _orderRepositoryMock.Setup(r => r.GetById(1)).Returns(order);
        _orderRepositoryMock.Setup(r => r.Update(order));

        _orderService.Cancel(1);

        _orderRepositoryMock.VerifyAll();
        order.Status.Should().Be(OrderStatus.Cancelled);
    }

    [TestMethod]
    public void Dispatch_WhenOrderIsPending_ShouldThrowOrderExceptionAndNotUpdate()
    {
        var order = MakeOrder();

        _orderRepositoryMock
            .Setup(r => r.GetById(1))
            .Returns(order);

        var act = () => _orderService.Dispatch(1);

        act.Should()
            .Throw<OrderException>();

        _orderRepositoryMock.Verify(r => r.GetById(1), Times.Once);
        _orderRepositoryMock.Verify(r => r.Update(It.IsAny<Order>()), Times.Never);
    }
    #endregion

    #region Error
    [TestMethod]
    public void Cancel_WhenOrderNotFound_ShouldThrowNotFoundException()
    {
        _orderRepositoryMock.Setup(r => r.GetById(99)).Returns((Order?)null);

        var act = () => _orderService.Cancel(99);

        act.Should().Throw<NotFoundException>();
        _orderRepositoryMock.VerifyAll();
    }
    #endregion
    #endregion

    #region Dispatch
    #region Success
    [TestMethod]
    public void Dispatch_WhenOrderIsPrepared_ShouldTransitionToOnTheWay()
    {
        var order = MakeOrder();
        order.Prepare();
        _orderRepositoryMock.Setup(r => r.GetById(1)).Returns(order);
        _orderRepositoryMock.Setup(r => r.Update(order));

        _orderService.Dispatch(1);

        _orderRepositoryMock.VerifyAll();
        order.Status.Should().Be(OrderStatus.OnTheWay);
    }
    #endregion

    #region Error
    [TestMethod]
    public void Dispatch_WhenOrderNotFound_ShouldThrowNotFoundException()
    {
        _orderRepositoryMock.Setup(r => r.GetById(99)).Returns((Order?)null);

        var act = () => _orderService.Dispatch(99);

        act.Should().Throw<NotFoundException>();
        _orderRepositoryMock.VerifyAll();
    }
    #endregion
    #endregion

    #region Deliver
    #region Success
    [TestMethod]
    public void Deliver_WhenOrderIsOnTheWay_ShouldTransitionToDelivered()
    {
        var order = MakeOrder();
        order.Prepare();
        order.Dispatch();
        _orderRepositoryMock.Setup(r => r.GetById(1)).Returns(order);
        _orderRepositoryMock.Setup(r => r.Update(order));

        _orderService.Deliver(1);

        _orderRepositoryMock.VerifyAll();
        order.Status.Should().Be(OrderStatus.Delivered);
    }
    #endregion

    #region Error
    [TestMethod]
    public void Deliver_WhenOrderNotFound_ShouldThrowNotFoundException()
    {
        _orderRepositoryMock.Setup(r => r.GetById(99)).Returns((Order?)null);

        var act = () => _orderService.Deliver(99);

        act.Should().Throw<NotFoundException>();
        _orderRepositoryMock.VerifyAll();
    }
    #endregion
    #endregion

    #region MarkNotDelivered
    #region Success
    [TestMethod]
    public void MarkNotDelivered_WhenOrderIsOnTheWay_ShouldTransitionToNotDelivered()
    {
        var order = MakeOrder();
        order.Prepare();
        order.Dispatch();
        _orderRepositoryMock.Setup(r => r.GetById(1)).Returns(order);
        _orderRepositoryMock.Setup(r => r.Update(order));

        _orderService.MarkNotDelivered(1);

        _orderRepositoryMock.VerifyAll();
        order.Status.Should().Be(OrderStatus.NotDelivered);
    }
    #endregion

    #region Error
    [TestMethod]
    public void MarkNotDelivered_WhenOrderNotFound_ShouldThrowNotFoundException()
    {
        _orderRepositoryMock.Setup(r => r.GetById(99)).Returns((Order?)null);

        var act = () => _orderService.MarkNotDelivered(99);

        act.Should().Throw<NotFoundException>();
        _orderRepositoryMock.VerifyAll();
    }
    #endregion
    #endregion
}
