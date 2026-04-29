using Dominio;
using FluentAssertions;
using LogicaDeNegocio.Shipping;
using LogicaDeNegocioAbstracciones.Exceptions;

namespace Pruebas.Orders;

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
    public void Create_WhenDeliveryTypeIsExpress_ShouldCalculate300()
    {
        var result = ShippingStrategyFactory.Create(DeliveryType.Express);

        result.Calculate(500m).Should().Be(300m);
    }

    [TestMethod]
    public void Create_WhenDeliveryTypeIsTwentyFourHours_ShouldReturnTwentyFourHoursShippingStrategy()
    {
        var result = ShippingStrategyFactory.Create(DeliveryType.TwentyFourHours);

        result.Should().BeOfType<TwentyFourHoursShippingStrategy>();
    }

    [TestMethod]
    public void Create_WhenDeliveryTypeIsTwentyFourHours_ShouldCalculate200()
    {
        var result = ShippingStrategyFactory.Create(DeliveryType.TwentyFourHours);

        result.Calculate(500m).Should().Be(200m);
    }
    #endregion

    #region Error
    [TestMethod]
    public void Create_WhenDeliveryTypeIsUnknown_ShouldThrowValidationException()
    {
        var unknownType = (DeliveryType)99;

        var act = () => ShippingStrategyFactory.Create(unknownType);

        act.Should().Throw<ValidationException>()
            .WithMessage("Tipo de entrega inválido: 99");
    }
    #endregion
    #endregion
}
