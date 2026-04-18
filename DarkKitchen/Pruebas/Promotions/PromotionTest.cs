using Dominio;
using Dominio.Builders;
using Dominio.Exceptions;

namespace Pruebas.Promotions;

[TestClass]
public class PromotionTest
{
    private Product CreateValidProduct()
    {
        return new ProductBuilder()
            .WithCode("COD123")
            .WithName("Nombre válido producto")
            .WithDescription("Descripción válida con más de veinte caracteres")
            .WithCommercialLine("Línea")
            .WithCategory("Categoría")
            .WithPrice(100)
            .WithPhotos(new List<ProductPhoto> { new ProductPhoto("photo-1.jpg") })
            .Build();
    }

    [TestMethod]
    public void GivenProductNotAssociated_WhenDisassociating_ThenThrowsPromotionException()
    {
        var product = CreateValidProduct();

        var promotion = Promotion.MakePromotion("Black Friday",
            10,
            new DateOnly(2026, 1, 25),
            new DateOnly(2026, 1, 30));

        var ex = Assert.ThrowsException<PromotionException>(() => promotion.DisassociateProduct(product));

        Assert.AreEqual("El producto no está asociado a la promoción", ex.Message);
    }
}
