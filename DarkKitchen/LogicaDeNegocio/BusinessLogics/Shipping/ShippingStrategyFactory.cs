using BusinessLogicsAbstractions.Exceptions;
using BusinessLogicsAbstractions.Orders;
using Domain;

namespace BusinessLogics.Shipping;

public static class ShippingStrategyFactory
{
    public static IShippingCostStrategy Create(DeliveryType deliveryType)
    {
        return deliveryType switch
        {
            DeliveryType.Express => new ExpressShippingStrategy(),
            DeliveryType.TwentyFourHours => new TwentyFourHoursShippingStrategy(),
            _ => throw new ValidationException(
                $"Tipo de entrega inválido: {deliveryType}"
            )
        };
    }
}
