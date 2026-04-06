using LogicaDeNegocioAbstracciones;

namespace LogicaDeNegocio;

public class ExpressShippingStrategy : IShippingCostStrategy
{
    private const decimal ShippingCost = 200m;

    public decimal Calculate(decimal subtotal)
    {
        return ShippingCost;
    }
}
