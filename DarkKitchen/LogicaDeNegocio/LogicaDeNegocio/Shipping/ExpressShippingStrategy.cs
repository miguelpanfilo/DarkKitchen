using System.Diagnostics.CodeAnalysis;
using LogicaDeNegocioAbstracciones.Orders;

namespace LogicaDeNegocio.Shipping;
[ExcludeFromCodeCoverage]
public class ExpressShippingStrategy : IShippingCostStrategy
{
    private const decimal ShippingCost = 200m;

    public decimal Calculate(decimal subtotal) => ShippingCost;
}
