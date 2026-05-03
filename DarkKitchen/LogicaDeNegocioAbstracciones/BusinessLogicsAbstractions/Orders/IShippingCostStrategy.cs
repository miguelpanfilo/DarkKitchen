namespace BusinessLogicsAbstractions.Orders;

public interface IShippingCostStrategy
{
    decimal Calculate(decimal subtotal);
}
