namespace LogicaDeNegocioAbstracciones;

public interface IShippingCostStrategy
{
    decimal Calculate(decimal subtotal);
}
