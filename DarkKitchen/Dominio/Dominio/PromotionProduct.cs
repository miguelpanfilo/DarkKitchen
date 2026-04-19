namespace Dominio;

public class PromotionProduct
{
    public int PromotionId { get; private set; }
    public int ProductId { get; private set; }

    private PromotionProduct()
    {
    }

    public PromotionProduct(int productId)
    {
        ProductId = productId;
    }
}
