using FluentAssertions;
using LogicaDeNegocio.Shipping;

namespace Pruebas.Pedidos;

[TestClass]
public sealed class ExpressShippingStrategyTests
{
    private ExpressShippingStrategy _strategy = null!;

    [TestInitialize]
    public void Initialize()
    {
        _strategy = new ExpressShippingStrategy();
    }

    #region Calculate
    #region Success
    [TestMethod]
    public void Calculate_WhenAnySubtotal_ShouldReturn300()
    {
        var result = _strategy.Calculate(100m);

        result.Should().Be(300m);
    }

    [TestMethod]
    public void Calculate_WhenSubtotalIsZero_ShouldReturn300()
    {
        var result = _strategy.Calculate(0m);

        result.Should().Be(300m);
    }

    [TestMethod]
    public void Calculate_WhenSubtotalIsLarge_ShouldReturn300()
    {
        var result = _strategy.Calculate(10000m);

        result.Should().Be(300m);
    }
    #endregion
    #endregion
}

[TestClass]
public sealed class TwentyFourHoursShippingStrategyTests
{
    private TwentyFourHoursShippingStrategy _strategy = null!;

    [TestInitialize]
    public void Initialize()
    {
        _strategy = new TwentyFourHoursShippingStrategy();
    }

    #region Calculate
    #region Success

    [TestMethod]
    public void Calculate_WhenAnySubtotal_ShouldReturn200()
    {
        var result = _strategy.Calculate(100m);

        result.Should().Be(200m);
    }

    [TestMethod]
    public void Calculate_WhenSubtotalIsZero_ShouldReturn200()
    {
        var result = _strategy.Calculate(0m);

        result.Should().Be(200m);
    }

    [TestMethod]
    public void Calculate_WhenSubtotalIsLarge_ShouldReturn200()
    {
        var result = _strategy.Calculate(10000m);

        result.Should().Be(200m);
    }

    #endregion
    #endregion
}
