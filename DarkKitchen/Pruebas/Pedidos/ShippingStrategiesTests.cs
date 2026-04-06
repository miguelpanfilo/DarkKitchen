using LogicaDeNegocio;
using LogicaDeNegocioAbstracciones;

namespace Pruebas.Pedidos;

[TestClass]
public class ExpressShippingStrategyTests
{
    [TestMethod]
    public void Calculate_AnySubtotal_Returns200()
    {
        var strategy = new ExpressShippingStrategy();

        var result = strategy.Calculate(100m);

        Assert.AreEqual(200m, result);
    }

    [TestMethod]
    public void Calculate_ZeroSubtotal_Returns200()
    {
        var strategy = new ExpressShippingStrategy();

        var result = strategy.Calculate(0m);

        Assert.AreEqual(200m, result);
    }

    [TestMethod]
    public void Calculate_LargeSubtotal_Returns200()
    {
        var strategy = new ExpressShippingStrategy();

        var result = strategy.Calculate(10000m);

        Assert.AreEqual(200m, result);
    }

    [TestMethod]
    public void ExpressShippingStrategy_ImplementsIShippingCostStrategy()
    {
        IShippingCostStrategy strategy = new ExpressShippingStrategy();

        Assert.IsNotNull(strategy);
    }
}

[TestClass]
public class TwentyFourHoursShippingStrategyTests
{
    [TestMethod]
    public void Calculate_AnySubtotal_Returns250()
    {
        var strategy = new TwentyFourHoursShippingStrategy();

        var result = strategy.Calculate(100m);

        Assert.AreEqual(250m, result);
    }

    [TestMethod]
    public void Calculate_ZeroSubtotal_Returns250()
    {
        var strategy = new TwentyFourHoursShippingStrategy();

        var result = strategy.Calculate(0m);

        Assert.AreEqual(250m, result);
    }

    [TestMethod]
    public void Calculate_LargeSubtotal_Returns250()
    {
        var strategy = new TwentyFourHoursShippingStrategy();

        var result = strategy.Calculate(10000m);

        Assert.AreEqual(250m, result);
    }

    [TestMethod]
    public void TwentyFourHoursShippingStrategy_ImplementsIShippingCostStrategy()
    {
        IShippingCostStrategy strategy = new TwentyFourHoursShippingStrategy();

        Assert.IsNotNull(strategy);
    }
}
