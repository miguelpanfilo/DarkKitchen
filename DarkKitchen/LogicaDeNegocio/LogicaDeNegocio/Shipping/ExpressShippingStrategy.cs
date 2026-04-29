using System.Diagnostics.CodeAnalysis;
using LogicaDeNegocioAbstracciones.Orders;

namespace LogicaDeNegocio.Shipping;
[ExcludeFromCodeCoverage]
public class ExpressShippingStrategy : IShippingCostStrategy
{
    private const decimal ShippingCost = 300m;

    public decimal Calculate(decimal subtotal) => ShippingCost;
}
