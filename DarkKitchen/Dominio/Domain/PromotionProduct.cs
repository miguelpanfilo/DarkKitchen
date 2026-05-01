using System.Diagnostics.CodeAnalysis;

namespace Domain;

public class PromotionProduct
{
    public int PromotionId { get; private set; }
    public int ProductId { get; private set; }

    [ExcludeFromCodeCoverage]
    private PromotionProduct()
    {
    }

    public PromotionProduct(int productId)
    {
        ProductId = productId;
    }
}
