using Domain.Exceptions;

namespace Tests.Orders;

using Domain;
using FluentAssertions;

[TestClass]
public class OrderStateTests
{
    private static Order CreateOrder()
    {
        var address = Address.MakeAddress("Av. Italia", "1234");
        var items = new List<OrderItemData>
        {
            new OrderItemData { ProductId = 1, Quantity = 1, UnitPrice = 100, DiscountApplied = 0 }
        };
        return Order.MakeOrder(1, "delivery", address, items, 50);
    }

    [TestMethod]
    public void PendingState_WhenPrepare_ShouldTransitionToPreparedState()
    {
        var order = CreateOrder();

        order.Prepare();

        order.Status.Should().Be(OrderStatus.Prepared);
    }

    [TestMethod]
    public void PendingState_WhenCancel_ShouldTransitionToCancelledState()
    {
        var order = CreateOrder();

        order.Cancel();

        order.Status.Should().Be(OrderStatus.Cancelled);
    }

    [TestMethod]
    public void PendingState_WhenDelay_ShouldTransitionToDelayedState()
    {
        var order = CreateOrder();

        order.Delay();

        order.Status.Should().Be(OrderStatus.Delayed);
    }

    [TestMethod]
    public void PendingState_WhenDispatch_ShouldThrowOrderException()
    {
        var order = CreateOrder();

        Action act = () => order.Dispatch();

        act.Should().Throw<OrderException>();
    }

    [TestMethod]
    public void PendingState_WhenDeliver_ShouldThrowOrderException()
    {
        var order = CreateOrder();

        Action act = () => order.Deliver();

        act.Should().Throw<OrderException>();
    }

    [TestMethod]
    public void PendingState_WhenMarkNotDelivered_ShouldThrowOrderException()
    {
        var order = CreateOrder();

        Action act = () => order.MarkNotDelivered();

        act.Should().Throw<OrderException>();
    }

    [TestMethod]
    public void PendingState_WhenGetStatus_ShouldReturnPending()
    {
        var order = CreateOrder();

        order.Status.Should().Be(OrderStatus.Pending);
    }

    [TestMethod]
    public void PreparedState_WhenDispatch_ShouldTransitionToOnTheWayState()
    {
        var order = CreateOrder();
        order.Prepare();

        order.Dispatch();

        order.Status.Should().Be(OrderStatus.OnTheWay);
    }

    [TestMethod]
    public void PreparedState_WhenCancel_ShouldThrowOrderException()
    {
        var order = CreateOrder();
        order.Prepare();

        Action act = () => order.Cancel();

        act.Should().Throw<OrderException>();
    }

    [TestMethod]
    public void PreparedState_WhenPrepare_ShouldThrowOrderException()
    {
        var order = CreateOrder();
        order.Prepare();

        Action act = () => order.Prepare();

        act.Should().Throw<OrderException>();
    }

    [TestMethod]
    public void PreparedState_WhenDeliver_ShouldThrowOrderException()
    {
        var order = CreateOrder();
        order.Prepare();

        Action act = () => order.Deliver();

        act.Should().Throw<OrderException>();
    }

    [TestMethod]
    public void PreparedState_WhenDelay_ShouldThrowOrderException()
    {
        var order = CreateOrder();
        order.Prepare();

        Action act = () => order.Delay();

        act.Should().Throw<OrderException>();
    }

    [TestMethod]
    public void OnTheWayState_WhenDeliver_ShouldTransitionToDeliveredState()
    {
        var order = CreateOrder();
        order.Prepare();
        order.Dispatch();

        order.Deliver();

        order.Status.Should().Be(OrderStatus.Delivered);
    }

    [TestMethod]
    public void OnTheWayState_WhenMarkNotDelivered_ShouldTransitionToNotDeliveredState()
    {
        var order = CreateOrder();
        order.Prepare();
        order.Dispatch();

        order.MarkNotDelivered();

        order.Status.Should().Be(OrderStatus.NotDelivered);
    }

    [TestMethod]
    public void OnTheWayState_WhenCancel_ShouldThrowOrderException()
    {
        var order = CreateOrder();
        order.Prepare();
        order.Dispatch();

        Action act = () => order.Cancel();

        act.Should().Throw<OrderException>();
    }

    [TestMethod]
    public void OnTheWayState_WhenDispatch_ShouldThrowOrderException()
    {
        var order = CreateOrder();
        order.Prepare();
        order.Dispatch();

        Action act = () => order.Dispatch();

        act.Should().Throw<OrderException>();
    }

    [TestMethod]
    public void OnTheWayState_WhenDelay_ShouldThrowOrderException()
    {
        var order = CreateOrder();
        order.Prepare();
        order.Dispatch();

        Action act = () => order.Delay();

        act.Should().Throw<OrderException>();
    }

    [TestMethod]
    public void DelayedState_WhenPrepare_ShouldTransitionToPreparedState()
    {
        var order = CreateOrder();
        order.Delay();

        order.Prepare();

        order.Status.Should().Be(OrderStatus.Prepared);
    }

    [TestMethod]
    public void DelayedState_WhenCancel_ShouldTransitionToCancelledState()
    {
        var order = CreateOrder();
        order.Delay();

        order.Cancel();

        order.Status.Should().Be(OrderStatus.Cancelled);
    }

    [TestMethod]
    public void DelayedState_WhenDispatch_ShouldThrowOrderException()
    {
        var order = CreateOrder();
        order.Delay();

        Action act = () => order.Dispatch();

        act.Should().Throw<OrderException>();
    }

    [TestMethod]
    public void DelayedState_WhenDelay_ShouldThrowOrderException()
    {
        var order = CreateOrder();
        order.Delay();

        Action act = () => order.Delay();

        act.Should().Throw<OrderException>();
    }

    [TestMethod]
    public void DelayedState_WhenDeliver_ShouldThrowOrderException()
    {
        var order = CreateOrder();
        order.Delay();

        Action act = () => order.Deliver();

        act.Should().Throw<OrderException>();
    }

    [TestMethod]
    public void DeliveredState_WhenDeliver_ShouldThrowOrderException()
    {
        var order = CreateOrder();
        order.Prepare();
        order.Dispatch();
        order.Deliver();

        Action act = () => order.Deliver();

        act.Should().Throw<OrderException>();
    }

    [TestMethod]
    public void DeliveredState_WhenCancel_ShouldThrowOrderException()
    {
        var order = CreateOrder();
        order.Prepare();
        order.Dispatch();
        order.Deliver();

        Action act = () => order.Cancel();

        act.Should().Throw<OrderException>();
    }

    [TestMethod]
    public void DeliveredState_WhenPrepare_ShouldThrowOrderException()
    {
        var order = CreateOrder();
        order.Prepare();
        order.Dispatch();
        order.Deliver();

        Action act = () => order.Prepare();

        act.Should().Throw<OrderException>();
    }

    [TestMethod]
    public void DeliveredState_WhenDispatch_ShouldThrowOrderException()
    {
        var order = CreateOrder();
        order.Prepare();
        order.Dispatch();
        order.Deliver();

        Action act = () => order.Dispatch();

        act.Should().Throw<OrderException>();
    }

    [TestMethod]
    public void DeliveredState_WhenMarkNotDelivered_ShouldThrowOrderException()
    {
        var order = CreateOrder();
        order.Prepare();
        order.Dispatch();
        order.Deliver();

        Action act = () => order.MarkNotDelivered();

        act.Should().Throw<OrderException>();
    }

    [TestMethod]
    public void DeliveredState_WhenDelay_ShouldThrowOrderException()
    {
        var order = CreateOrder();
        order.Prepare();
        order.Dispatch();
        order.Deliver();

        Action act = () => order.Delay();

        act.Should().Throw<OrderException>();
    }

    [TestMethod]
    public void DeliveredState_WhenGetStatus_ShouldReturnDelivered()
    {
        var order = CreateOrder();
        order.Prepare();
        order.Dispatch();
        order.Deliver();

        order.Status.Should().Be(OrderStatus.Delivered);
    }

    [TestMethod]
    public void CancelledState_WhenCancel_ShouldThrowOrderException()
    {
        var order = CreateOrder();
        order.Cancel();

        Action act = () => order.Cancel();

        act.Should().Throw<OrderException>();
    }

    [TestMethod]
    public void CancelledState_WhenPrepare_ShouldThrowOrderException()
    {
        var order = CreateOrder();
        order.Cancel();

        Action act = () => order.Prepare();

        act.Should().Throw<OrderException>();
    }

    [TestMethod]
    public void CancelledState_WhenDispatch_ShouldThrowOrderException()
    {
        var order = CreateOrder();
        order.Cancel();

        Action act = () => order.Dispatch();

        act.Should().Throw<OrderException>();
    }

    [TestMethod]
    public void CancelledState_WhenDeliver_ShouldThrowOrderException()
    {
        var order = CreateOrder();
        order.Cancel();

        Action act = () => order.Deliver();

        act.Should().Throw<OrderException>();
    }

    [TestMethod]
    public void CancelledState_WhenMarkNotDelivered_ShouldThrowOrderException()
    {
        var order = CreateOrder();
        order.Cancel();

        Action act = () => order.MarkNotDelivered();

        act.Should().Throw<OrderException>();
    }

    [TestMethod]
    public void CancelledState_WhenDelay_ShouldThrowOrderException()
    {
        var order = CreateOrder();
        order.Cancel();

        Action act = () => order.Delay();

        act.Should().Throw<OrderException>();
    }

    [TestMethod]
    public void CancelledState_WhenGetStatus_ShouldReturnCancelled()
    {
        var order = CreateOrder();
        order.Cancel();

        order.Status.Should().Be(OrderStatus.Cancelled);
    }

    [TestMethod]
    public void NotDeliveredState_WhenMarkNotDelivered_ShouldThrowOrderException()
    {
        var order = CreateOrder();
        order.Prepare();
        order.Dispatch();
        order.MarkNotDelivered();

        Action act = () => order.MarkNotDelivered();

        act.Should().Throw<OrderException>();
    }

    [TestMethod]
    public void NotDeliveredState_WhenPrepare_ShouldThrowOrderException()
    {
        var order = CreateOrder();
        order.Prepare();
        order.Dispatch();
        order.MarkNotDelivered();

        Action act = () => order.Prepare();

        act.Should().Throw<OrderException>();
    }

    [TestMethod]
    public void NotDeliveredState_WhenCancel_ShouldThrowOrderException()
    {
        var order = CreateOrder();
        order.Prepare();
        order.Dispatch();
        order.MarkNotDelivered();

        Action act = () => order.Cancel();

        act.Should().Throw<OrderException>();
    }

    [TestMethod]
    public void NotDeliveredState_WhenDispatch_ShouldThrowOrderException()
    {
        var order = CreateOrder();
        order.Prepare();
        order.Dispatch();
        order.MarkNotDelivered();

        Action act = () => order.Dispatch();

        act.Should().Throw<OrderException>();
    }

    [TestMethod]
    public void NotDeliveredState_WhenDeliver_ShouldThrowOrderException()
    {
        var order = CreateOrder();
        order.Prepare();
        order.Dispatch();
        order.MarkNotDelivered();

        Action act = () => order.Deliver();

        act.Should().Throw<OrderException>();
    }

    [TestMethod]
    public void NotDeliveredState_WhenDelay_ShouldThrowOrderException()
    {
        var order = CreateOrder();
        order.Prepare();
        order.Dispatch();
        order.MarkNotDelivered();

        Action act = () => order.Delay();

        act.Should().Throw<OrderException>();
    }

    [TestMethod]
    public void NotDeliveredState_WhenGetStatus_ShouldReturnNotDelivered()
    {
        var order = CreateOrder();
        order.Prepare();
        order.Dispatch();
        order.MarkNotDelivered();

        order.Status.Should().Be(OrderStatus.NotDelivered);
    }

    [TestMethod]
    public void OrderFlow_ShouldGoThroughAllValidStates()
    {
        var order = CreateOrder();

        order.Status.Should().Be(OrderStatus.Pending);

        order.Prepare();
        order.Status.Should().Be(OrderStatus.Prepared);

        order.Dispatch();
        order.Status.Should().Be(OrderStatus.OnTheWay);

        order.Deliver();
        order.Status.Should().Be(OrderStatus.Delivered);
    }

    #region OrderStateFactory

    [DataTestMethod]
    [DataRow(OrderStatus.Pending)]
    [DataRow(OrderStatus.Prepared)]
    [DataRow(OrderStatus.OnTheWay)]
    [DataRow(OrderStatus.Delivered)]
    [DataRow(OrderStatus.NotDelivered)]
    [DataRow(OrderStatus.Cancelled)]
    [DataRow(OrderStatus.Delayed)]
    public void OrderStateFactory_Create_ShouldReturnStateWithMatchingName(OrderStatus status)
    {
        var state = OrderStateFactory.Create(status);

        state.Should().NotBeNull();
        state.Name.Should().Be(status.ToString());
    }

    [TestMethod]
    public void OrderStateFactory_Create_WhenUnknownStatus_ShouldThrowInvalidOperationException()
    {
        var unknownStatus = (OrderStatus)999;

        Action act = () => OrderStateFactory.Create(unknownStatus);

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Estado desconocido*");
    }

    #endregion
}
