using Domain;
using Domain.Builders;
using Domain.Exceptions;

namespace Tests.Promotions;

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

    private static DateOnly Today() => DateOnly.FromDateTime(DateTime.UtcNow);

    private Promotion CreateValidPromotion()
    {
        return Promotion.MakePromotion(
            "Black Friday",
            10,
            Today().AddDays(5),
            Today().AddDays(10)
        );
    }

    [TestMethod]
    public void GivenProductNotAssociated_WhenDisassociating_ThenThrowsPromotionException()
    {
        var promotion = Promotion.MakePromotion(
            "Black Friday",
            10,
            Today().AddDays(5),
            Today().AddDays(10));

        var ex = Assert.ThrowsException<PromotionException>(() => promotion.DisassociateProduct(100));

        Assert.AreEqual("El producto no está asociado a la promoción", ex.Message);
    }

    [TestMethod]
    public void GivenProductAlreadyAssociatedToPromotion_WhenAssociatingProduct_ThenThrowsPromotionException()
    {
        var promotion = CreateValidPromotion();

        promotion.AssociateProduct(100);

        var ex = Assert.ThrowsException<PromotionException>(() => promotion.AssociateProduct(100));

        Assert.AreEqual("El producto ya está asociado a la promoción", ex.Message);
    }

    [TestMethod]
    public void GivenDeletedPromotion_WhenDeleting_ThenThrowsPromotionException()
    {
        var promotion = CreateValidPromotion();

        promotion.Delete();

        var ex = Assert.ThrowsException<PromotionException>(() => promotion.Delete());

        Assert.AreEqual("La promoción ya está eliminada", ex.Message);
    }

    [TestMethod]
    public void GivenExpiredPromotion_WhenUpdatingName_ThenThrowsConflictException()
    {
        var promotion = Promotion.MakePromotion(
            "Black Friday",
            20,
            Today().AddDays(5),
            Today().AddDays(10)
        );
        promotion.UpdateDates(Today().AddDays(-10), Today().AddDays(-1));

        var ex = Assert.ThrowsException<PromotionException>(() =>
            promotion.UpdateName("Cyber Monday"));

        Assert.AreEqual("La promoción expiró, no se puede modificar", ex.Message);
    }

    [TestMethod]
    public void GivenExpiredPromotion_WhenUpdatingDiscount_ThenThrowsConflictException()
    {
        var promotion = Promotion.MakePromotion(
            "Black Friday",
            20,
            Today().AddDays(5),
            Today().AddDays(10)
        );
        promotion.UpdateDates(Today().AddDays(-10), Today().AddDays(-1));

        var ex = Assert.ThrowsException<PromotionException>(() =>
            promotion.UpdateDiscountPercentage(30));

        Assert.AreEqual("La promoción expiró, no se puede modificar", ex.Message);
    }
}
