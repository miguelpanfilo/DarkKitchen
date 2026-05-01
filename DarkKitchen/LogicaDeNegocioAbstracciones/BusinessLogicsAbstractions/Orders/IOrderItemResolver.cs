using Contract.Orders;

namespace BusinessLogicsAbstractions.Orders;

public interface IOrderItemResolver
{
    List<ResolvedOrderItemDto> Resolve(List<CreateOrderItemRequest> requestItems);
}
