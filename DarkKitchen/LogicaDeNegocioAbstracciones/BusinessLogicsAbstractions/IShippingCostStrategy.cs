namespace BusinessLogicsAbstractions;

public interface IShippingCostStrategy
{
    decimal Calculate(decimal subtotal);
}
