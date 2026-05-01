using System.Diagnostics.CodeAnalysis;
using BusinessLogicsAbstractions.Orders;

namespace BusinessLogics.Shipping;
[ExcludeFromCodeCoverage]
public class ExpressShippingStrategy : IShippingCostStrategy
{
    private const decimal ShippingCost = 300m;

    public decimal Calculate(decimal subtotal) => ShippingCost;
}
