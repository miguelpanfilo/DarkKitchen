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
}
