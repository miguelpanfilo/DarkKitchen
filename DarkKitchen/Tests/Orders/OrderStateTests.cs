using Domain.Exceptions;

namespace Tests.Orders;

using Domain;
using FluentAssertions;

[TestClass]
public class OrderStateTests
{
    [TestMethod]
    public void
        PendingState_WhenPrepare_ShouldTransitionToPreparedState()
    {
        var state = new PendingState();

        var nextState = state.Prepare();

        nextState.Should().BeOfType<PreparedState>();
    }

    [TestMethod]
    public void PendingState_WhenCancel_ShouldTransitionToCancelledState()
    {
        var state = new PendingState();

        var nextState = state.Cancel();

        nextState.Should().BeOfType<CancelledState>();
    }

    [TestMethod]
    public void PreparedState_WhenDispatch_ShouldTransitionToOnTheWayState()
    {
        var state = new PreparedState();

        var nextState = state.Dispatch();

        nextState.Should().BeOfType<OnTheWayState>();
    }

    [TestMethod]
    public void PreparedState_WhenCancel_ShouldThrowInvalidOperationException()
    {
        var state = new PreparedState();

        Action act = () => state.Cancel();

        act.Should()
            .Throw<OrderException>()
            .WithMessage("Solo se puede cancelar un pedido pendiente o demorado");
    }

    [TestMethod]
    public void PendingState_WhenDelay_ShouldTransitionToDelayedState()
    {
        var state = new PendingState();

        var nextState = state.Delay();

        nextState.Should().BeOfType<DelayedState>();
    }

    [TestMethod]
    public void DelayedState_WhenPrepare_ShouldTransitionToPreparedState()
    {
        var state = new DelayedState();

        var nextState = state.Prepare();

        nextState.Should().BeOfType<PreparedState>();
    }

    [TestMethod]
    public void DelayedState_WhenCancel_ShouldTransitionToCancelledState()
    {
        var state = new DelayedState();

        var nextState = state.Cancel();

        nextState.Should().BeOfType<CancelledState>();
    }

    [TestMethod]
    public void PreparedState_WhenDelay_ShouldThrowInvalidOperationException()
    {
        var state = new PreparedState();

        Action act = () => state.Delay();

        act.Should().Throw<OrderException>();
    }

    [TestMethod]
    public void OnTheWayState_WhenDeliver_ShouldTransitionToDeliveredState()
    {
        var state = new OnTheWayState();

        var nextState = state.Deliver();

        nextState.Should().BeOfType<DeliveredState>();
    }

    [TestMethod]
    public void OnTheWayState_WhenMarkNotDelivered_ShouldTransitionToNotDeliveredState()
    {
        var state = new OnTheWayState();

        var nextState = state.MarkNotDelivered();

        nextState.Should().BeOfType<NotDeliveredState>();
    }

    [TestMethod]
    public void PendingState_WhenDispatch_ShouldThrowInvalidOperationException()
    {
        var state = new PendingState();

        Action act = () => state.Dispatch();

        act.Should()
            .Throw<OrderException>()
            .WithMessage("Solo se puede poner en camino un pedido preparado");
    }

    [TestMethod]
    public void DelayedState_WhenDispatch_ShouldThrowInvalidOperationException()
    {
        var state = new DelayedState();

        Action act = () => state.Dispatch();

        act.Should()
            .Throw<OrderException>()
            .WithMessage("Primero se debe preparar el pedido");
    }

    [TestMethod]
    public void OnTheWayState_WhenCancel_ShouldThrowInvalidOperationException()
    {
        var state = new OnTheWayState();

        Action act = () => state.Cancel();

        act.Should()
            .Throw<OrderException>()
            .WithMessage("No es posible cancelar un pedido que ya esta en camino");
    }

    [TestMethod]
    public void OrderFlow_ShouldGoThroughAllValidStates()
    {
        IOrderState state = new PendingState();

        state = state.Prepare();
        state.Should().BeOfType<PreparedState>();

        state = state.Dispatch();
        state.Should().BeOfType<OnTheWayState>();

        state = state.Deliver();
        state.Should().BeOfType<DeliveredState>();
    }

    [TestMethod]
    public void DelayedState_WhenDelay_ShouldThrowInvalidOperationException()
    {
        var state = new DelayedState();

        Action act = () => state.Delay();

        act.Should()
            .Throw<OrderException>()
            .WithMessage("El pedido ya esta demorado");
    }

    [TestMethod]
    public void DelayedState_WhenDeliver_ShouldThrowInvalidOperationException()
    {
        var state = new DelayedState();

        Action act = () => state.Deliver();

        act.Should()
            .Throw<OrderException>()
            .WithMessage("Solo se puede entregar un pedido en camino");
    }

    [TestMethod]
    public void OnTheWayState_WhenDispatch_ShouldThrowInvalidOperationException()
    {
        var state = new OnTheWayState();

        Action act = () => state.Dispatch();

        act.Should()
            .Throw<OrderException>()
            .WithMessage("El pedido ya se encuentra en camino");
    }

    [TestMethod]
    public void DeliveredState_WhenDeliver_ShouldThrowInvalidOperationException()
    {
        var state = new DeliveredState();

        Action act = () => state.Deliver();

        act.Should()
            .Throw<OrderException>()
            .WithMessage("El pedido ya se encuentra entregado");
    }

    [TestMethod]
    public void CancelledState_WhenCancel_ShouldThrowInvalidOperationException()
    {
        var state = new CancelledState();

        Action act = () => state.Cancel();

        act.Should()
            .Throw<OrderException>()
            .WithMessage("El pedido ya esta cancelado");
    }

    [TestMethod]
    public void NotDeliveredState_WhenMarkNotDelivered_ShouldThrowInvalidOperationException()
    {
        var state = new NotDeliveredState();

        Action act = () => state.MarkNotDelivered();

        act.Should()
            .Throw<OrderException>()
            .WithMessage("El pedido ya se encuentra como no entregado");
    }

    [TestMethod]
    public void PreparedState_WhenPrepare_ShouldThrowInvalidOperationException()
    {
        var state = new PreparedState();

        Action act = () => state.Prepare();

        act.Should()
            .Throw<OrderException>()
            .WithMessage("El pedido ya se encuentra preparado");
    }

    [TestMethod]
    public void PreparedState_WhenDeliver_ShouldThrowInvalidOperationException()
    {
        var state = new PreparedState();

        Action act = () => state.Deliver();

        act.Should()
            .Throw<OrderException>()
            .WithMessage("Primero hay que despachar el pedido");
    }

    [TestMethod]
    public void OnTheWayState_WhenDelay_ShouldThrowInvalidOperationException()
    {
        var state = new OnTheWayState();

        Action act = () => state.Delay();

        act.Should()
            .Throw<OrderException>()
            .WithMessage("No se puede demorar un pedido que ya esta en camino");
    }

    [TestMethod]
    public void PendingState_WhenMarkNotDelivered_ShouldThrowInvalidOperationException()
    {
        var state = new PendingState();

        Action act = () => state.MarkNotDelivered();

        act.Should()
            .Throw<OrderException>()
            .WithMessage("Solo se puede marcar como no entregado un pedido en camino");
    }

    [TestMethod]
    public void DeliveredState_WhenCancel_ShouldThrowInvalidOperationException()
    {
        var state = new DeliveredState();

        Action act = () => state.Cancel();

        act.Should().Throw<OrderException>();
    }

    [TestMethod]
    public void PendingState_WhenDeliver_ShouldThrowOrderException()
    {
        var state = new PendingState();

        Action act = () => state.Deliver();

        act.Should()
            .Throw<OrderException>()
            .WithMessage("Solo se puede entregar un pedido en camino");
    }

    [TestMethod]
    public void PendingState_WhenGetStatus_ShouldReturnPending()
    {
        var state = new PendingState();

        var status = state.GetStatus();

        status.Should().Be(OrderStatus.Pending);
    }

    [TestMethod]
    public void CancelledState_WhenPrepare_ShouldThrowOrderException()
    {
        var state = new CancelledState();

        Action act = () => state.Prepare();

        act.Should()
            .Throw<OrderException>()
            .WithMessage("No se puede modificar un pedido cancelado");
    }

    [TestMethod]
    public void CancelledState_WhenDispatch_ShouldThrowOrderException()
    {
        var state = new CancelledState();

        Action act = () => state.Dispatch();

        act.Should()
            .Throw<OrderException>()
            .WithMessage("No se puede modificar un pedido cancelado");
    }

    [TestMethod]
    public void CancelledState_WhenDeliver_ShouldThrowOrderException()
    {
        var state = new CancelledState();

        Action act = () => state.Deliver();

        act.Should()
            .Throw<OrderException>()
            .WithMessage("No se puede modificar un pedido cancelado");
    }

    [TestMethod]
    public void CancelledState_WhenMarkNotDelivered_ShouldThrowOrderException()
    {
        var state = new CancelledState();

        Action act = () => state.MarkNotDelivered();

        act.Should()
            .Throw<OrderException>()
            .WithMessage("No se puede modificar un pedido cancelado");
    }

    [TestMethod]
    public void CancelledState_WhenDelay_ShouldThrowOrderException()
    {
        var state = new CancelledState();

        Action act = () => state.Delay();

        act.Should()
            .Throw<OrderException>()
            .WithMessage("No se puede modificar un pedido cancelado");
    }

    [TestMethod]
    public void CancelledState_WhenGetStatus_ShouldReturnCancelled()
    {
        var state = new CancelledState();

        var status = state.GetStatus();

        status.Should().Be(OrderStatus.Cancelled);
    }

    [TestMethod]
    public void NotDeliveredState_WhenPrepare_ShouldThrowOrderException()
    {
        var state = new NotDeliveredState();

        Action act = () => state.Prepare();

        act.Should()
            .Throw<OrderException>()
            .WithMessage("No se puede modificar un pedido no entregado");
    }

    [TestMethod]
    public void NotDeliveredState_WhenCancel_ShouldThrowOrderException()
    {
        var state = new NotDeliveredState();

        Action act = () => state.Cancel();

        act.Should()
            .Throw<OrderException>()
            .WithMessage("No se puede modificar un pedido no entregado");
    }

    [TestMethod]
    public void NotDeliveredState_WhenDispatch_ShouldThrowOrderException()
    {
        var state = new NotDeliveredState();

        Action act = () => state.Dispatch();

        act.Should()
            .Throw<OrderException>()
            .WithMessage("No se puede modificar un pedido no entregado");
    }

    [TestMethod]
    public void NotDeliveredState_WhenDeliver_ShouldThrowOrderException()
    {
        var state = new NotDeliveredState();

        Action act = () => state.Deliver();

        act.Should()
            .Throw<OrderException>()
            .WithMessage("No se puede modificar un pedido no entregado");
    }

    [TestMethod]
    public void NotDeliveredState_WhenDelay_ShouldThrowOrderException()
    {
        var state = new NotDeliveredState();

        Action act = () => state.Delay();

        act.Should()
            .Throw<OrderException>()
            .WithMessage("No se puede modificar un pedido no entregado");
    }

    [TestMethod]
    public void NotDeliveredState_WhenGetStatus_ShouldReturnNotDelivered()
    {
        var state = new NotDeliveredState();

        var status = state.GetStatus();

        status.Should().Be(OrderStatus.NotDelivered);
    }

    [TestMethod]
    public void DeliveredState_WhenPrepare_ShouldThrowOrderException()
    {
        var state = new DeliveredState();

        Action act = () => state.Prepare();

        act.Should()
            .Throw<OrderException>()
            .WithMessage("No se puede modificar un pedido entregado");
    }

    [TestMethod]
    public void DeliveredState_WhenDispatch_ShouldThrowOrderException()
    {
        var state = new DeliveredState();

        Action act = () => state.Dispatch();

        act.Should()
            .Throw<OrderException>()
            .WithMessage("No se puede modificar un pedido entregado");
    }

    [TestMethod]
    public void DeliveredState_WhenMarkNotDelivered_ShouldThrowOrderException()
    {
        var state = new DeliveredState();

        Action act = () => state.MarkNotDelivered();

        act.Should()
            .Throw<OrderException>()
            .WithMessage("No se puede modificar un pedido entregado");
    }

    [TestMethod]
    public void DeliveredState_WhenDelay_ShouldThrowOrderException()
    {
        var state = new DeliveredState();

        Action act = () => state.Delay();

        act.Should()
            .Throw<OrderException>()
            .WithMessage("No se puede modificar un pedido entregado");
    }

    [TestMethod]
    public void DeliveredState_WhenGetStatus_ShouldReturnDelivered()
    {
        var state = new DeliveredState();

        var status = state.GetStatus();

        status.Should().Be(OrderStatus.Delivered);
    }
}
