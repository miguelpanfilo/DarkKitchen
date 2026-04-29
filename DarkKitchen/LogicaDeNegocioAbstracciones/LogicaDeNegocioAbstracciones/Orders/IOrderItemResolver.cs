using Contratos.Orders;

namespace LogicaDeNegocioAbstracciones.Orders;

public interface IOrderItemResolver
{
    List<ResolvedOrderItemDto> Resolve(List<CreateOrderItemRequest> requestItems);
}
