using System.Diagnostics.CodeAnalysis;
using BusinessLogicsAbstractions.Orders;

namespace BusinessLogics.Shipping;
[ExcludeFromCodeCoverage]
public class TwentyFourHoursShippingStrategy : IShippingCostStrategy
{
    private const decimal ShippingCost = 200m;

    public decimal Calculate(decimal subtotal) => ShippingCost;
}
