using Contract;

namespace BusinessLogicsAbstractions;

public interface IOrderItemResolver
{
    List<ResolvedOrderItemDto> Resolve(List<CreateOrderItemRequest> requestItems);
}
