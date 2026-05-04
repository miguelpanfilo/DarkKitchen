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
}
