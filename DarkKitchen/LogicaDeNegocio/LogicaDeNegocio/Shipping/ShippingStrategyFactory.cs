using Dominio;
using Dominio.Exceptions;
using LogicaDeNegocioAbstracciones.Orders;

namespace LogicaDeNegocio.Shipping;

public static class ShippingStrategyFactory
{
    public static IShippingCostStrategy Create(DeliveryType deliveryType)
    {
        return deliveryType switch
        {
            DeliveryType.Express => new ExpressShippingStrategy(),
            DeliveryType.TwentyFourHours => new TwentyFourHoursShippingStrategy(),
            _ => throw new OrderException($"No se encontró estrategia de envío para el tipo de entrega: {(int)deliveryType}")
        };
    }
}
