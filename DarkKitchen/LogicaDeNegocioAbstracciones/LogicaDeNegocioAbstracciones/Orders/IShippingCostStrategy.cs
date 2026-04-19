namespace LogicaDeNegocioAbstracciones.Orders;

public interface IShippingCostStrategy
{
    decimal Calculate(decimal subtotal);
}
