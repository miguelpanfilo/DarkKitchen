using LogicaDeNegocioAbstracciones;

namespace LogicaDeNegocio;

public class TwentyFourHoursShippingStrategy : IShippingCostStrategy
{
    private const decimal ShippingCost = 250m;

    public decimal Calculate(decimal subtotal) => ShippingCost;
}
