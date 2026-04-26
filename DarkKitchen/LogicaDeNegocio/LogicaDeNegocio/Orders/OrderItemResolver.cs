using Contratos.Orders;
using Contratos.Promotions;
using Dominio.Exceptions;
using LogicaDeNegocioAbstracciones.Exceptions;
using LogicaDeNegocioAbstracciones.Orders;
using LogicaDeNegocioAbstracciones.Products;
using LogicaDeNegocioAbstracciones.Promotions;

namespace LogicaDeNegocio.Orders;

public class OrderItemResolver(
    IProductService productService,
    IPromotionService promotionService) : IOrderItemResolver
{
    private readonly IProductService _productService = productService;
    private readonly IPromotionService _promotionService = promotionService;

    public List<(int ProductId, int Quantity, decimal UnitPrice, decimal DiscountApplied)> Resolve(
        List<CreateOrderItemRequest> requestItems)
    {
        return requestItems.Select(i =>
        {
            try
            {
                var product = _productService.GetById(i.ProductId);

                if(!product.IsActive)
                {
                    throw new ValidationException($"El producto {product.Name} no está disponible");
                }

                var discountApplied = GetHighestCurrentDiscount(i.ProductId);

                return (i.ProductId, i.Quantity, product.Price, discountApplied);
            }
            catch(ProductException)
            {
                throw new NotFoundException($"El producto con id {i.ProductId} no existe");
            }
        }).ToList();
    }

    private decimal GetHighestCurrentDiscount(int productId)
    {
        var promotions = _promotionService.GetPromotions(new PromotionSearchRequestDto
        {
            Date = DateOnly.FromDateTime(DateTime.UtcNow),
            ProductId = productId
        });

        if(!promotions.Any())
        {
            return 0m;
        }

        return promotions.Max(p => p.DiscountPercentage) / 100m;
    }
}
