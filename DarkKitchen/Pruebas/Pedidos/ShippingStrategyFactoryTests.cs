using Dominio;
using Dominio.Exceptions;
using LogicaDeNegocio;

namespace Pruebas.Pedidos;

[TestClass]
public class ShippingStrategyFactoryTests
{
    [TestMethod]
    public void Create_ExpressDeliveryType_ReturnsExpressShippingStrategy()
    {
        var result = ShippingStrategyFactory.Create(DeliveryType.Express);
        Assert.IsInstanceOfType(result, typeof(ExpressShippingStrategy));
    }

    [TestMethod]
    public void Create_TwentyFourHoursDeliveryType_ReturnsTwentyFourHoursShippingStrategy()
    {
        var result = ShippingStrategyFactory.Create(DeliveryType.TwentyFourHours);
        Assert.IsInstanceOfType(result, typeof(TwentyFourHoursShippingStrategy));
    }

    [TestMethod]
    public void Create_ExpressDeliveryType_ReturnsStrategyThatCalculates200()
    {
        var result = ShippingStrategyFactory.Create(DeliveryType.Express);
        Assert.AreEqual(200m, result.Calculate(500m));
    }

    [TestMethod]
    public void Create_TwentyFourHoursDeliveryType_ReturnsStrategyThatCalculates250()
    {
        var result = ShippingStrategyFactory.Create(DeliveryType.TwentyFourHours);
        Assert.AreEqual(250m, result.Calculate(500m));
    }

    [TestMethod]
    public void Create_UnknownDeliveryType_ThrowsOrderException()
    {
        var unknownType = (DeliveryType)99;

        var exception = Assert.ThrowsException<OrderException>(() =>
            ShippingStrategyFactory.Create(unknownType));

        Assert.AreEqual("No shipping strategy found for delivery type: 99", exception.Message);
    }
}
