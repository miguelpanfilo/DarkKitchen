using Contratos.Orders;

namespace LogicaDeNegocioAbstracciones.Orders;

public interface IOrderItemResolver
{
    List<(int ProductId, int Quantity, decimal UnitPrice, decimal DiscountApplied)> Resolve(
        List<CreateOrderItemRequest> requestItems);
}
