using BusinessLogicsAbstractions.Exceptions;
using BusinessLogicsAbstractions.Orders;
using BusinessLogicsAbstractions.Products;
using BusinessLogicsAbstractions.Promotions;
using Contract.Orders;

namespace BusinessLogics.Orders;

public class OrderItemResolver(
    IProductService productService,
    IPromotionService promotionService) : IOrderItemResolver
{
    private readonly IProductService _productService = productService;
    private readonly IPromotionService _promotionService = promotionService;

    public List<ResolvedOrderItemDto> Resolve(List<CreateOrderItemRequest> requestItems)
    {
        return requestItems.Select(i =>
        {
            var product = _productService.GetById(i.ProductId);

            if (!product.IsActive)
            {
                throw new ValidationException($"El producto {product.Name} no está disponible");
            }

            return new ResolvedOrderItemDto
            {
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                UnitPrice = product.Price,
                DiscountApplied = _promotionService.GetHighestCurrentDiscountForProduct(i.ProductId)
            };
        }).ToList();
    }
}
