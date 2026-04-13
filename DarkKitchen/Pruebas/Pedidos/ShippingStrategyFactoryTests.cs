using Dominio;
using Dominio.Exceptions;
using FluentAssertions;
using LogicaDeNegocio;

namespace Pruebas.Pedidos;

[TestClass]
public sealed class ShippingStrategyFactoryTests
{
    #region Create
    #region Success
    [TestMethod]
    public void Create_WhenDeliveryTypeIsExpress_ShouldReturnExpressShippingStrategy()
    {
        var result = ShippingStrategyFactory.Create(DeliveryType.Express);

        result.Should().BeOfType<ExpressShippingStrategy>();
    }

    [TestMethod]
    public void Create_WhenDeliveryTypeIsExpress_ShouldCalculate200()
    {
        var result = ShippingStrategyFactory.Create(DeliveryType.Express);

        result.Calculate(500m).Should().Be(200m);
    }

    [TestMethod]
    public void Create_WhenDeliveryTypeIsTwentyFourHours_ShouldReturnTwentyFourHoursShippingStrategy()
    {
        var result = ShippingStrategyFactory.Create(DeliveryType.TwentyFourHours);

        result.Should().BeOfType<TwentyFourHoursShippingStrategy>();
    }

    [TestMethod]
    public void Create_WhenDeliveryTypeIsTwentyFourHours_ShouldCalculate250()
    {
        var result = ShippingStrategyFactory.Create(DeliveryType.TwentyFourHours);

        result.Calculate(500m).Should().Be(250m);
    }
    #endregion

    #region Error
    [TestMethod]
    public void Create_WhenDeliveryTypeIsUnknown_ShouldThrowOrderException()
    {
        var unknownType = (DeliveryType)99;

        var act = () => ShippingStrategyFactory.Create(unknownType);

        act.Should().Throw<OrderException>()
            .WithMessage("No shipping strategy found for delivery type: 99");
    }
    #endregion
    #endregion
}
