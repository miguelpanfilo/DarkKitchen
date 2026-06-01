using Contract.Orders;

namespace BusinessLogicsAbstractions;

public interface IOrderItemResolver
{
    List<ResolvedOrderItemDto> Resolve(List<CreateOrderItemRequest> requestItems);
}
