using System.Diagnostics.CodeAnalysis;
using LogicaDeNegocioAbstracciones.Orders;

namespace LogicaDeNegocio.Shipping;
[ExcludeFromCodeCoverage]
public class TwentyFourHoursShippingStrategy : IShippingCostStrategy
{
    private const decimal ShippingCost = 250m;

    public decimal Calculate(decimal subtotal) => ShippingCost;
}
