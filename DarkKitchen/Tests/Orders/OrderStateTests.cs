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

        act.Should().Throw<InvalidOperationException>();
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

        act.Should().Throw<InvalidOperationException>();
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
            .Throw<InvalidOperationException>()
            .WithMessage("Solo se puede poner en camino un pedido preparado");
    }

    [TestMethod]
    public void DelayedState_WhenDispatch_ShouldThrowInvalidOperationException()
    {
        var state = new DelayedState();

        Action act = () => state.Dispatch();

        act.Should()
            .Throw<InvalidOperationException>()
            .WithMessage("Primero se debe preparar el pedido");
    }

    [TestMethod]
    public void OnTheWayState_WhenCancel_ShouldThrowInvalidOperationException()
    {
        var state = new OnTheWayState();

        Action act = () => state.Cancel();

        act.Should()
            .Throw<InvalidOperationException>()
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
            .Throw<InvalidOperationException>()
            .WithMessage("El pedido ya esta demorado");
    }

    [TestMethod]
    public void DelayedState_WhenDeliver_ShouldThrowInvalidOperationException()
    {
        var state = new DelayedState();

        Action act = () => state.Deliver();

        act.Should()
            .Throw<InvalidOperationException>()
            .WithMessage("Solo se puede entregar un pedido en camino");
    }

    [TestMethod]
    public void OnTheWayState_WhenDispatch_ShouldThrowInvalidOperationException()
    {
        var state = new OnTheWayState();

        Action act = () => state.Dispatch();

        act.Should()
            .Throw<InvalidOperationException>()
            .WithMessage("El pedido ya se encuentra en camino");
    }
}
