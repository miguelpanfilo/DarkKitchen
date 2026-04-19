using Almacenamiento;
using Dominio;
using Microsoft.EntityFrameworkCore;
using Pruebas.Persistence;

namespace Pruebas.Repositories;

[TestClass]
public class RepositoryPromotionSqlTest
{
    private Product CreateValidProduct()
    {
        return Product.MakeProduct(
            "COD123",
            "Nombre válido",
            "Descripción válida con más de veinte caracteres",
            "Línea comercial",
            "Categoría",
            100,
            new List<ProductPhoto> { new ProductPhoto("photo-1.jpg") }
        );
    }

    [TestMethod]
    public void GivenValidPromotion_WhenCreating_ThenPromotionIsStored()
    {
        using var context = SqliteTestContextFactory.CreateContext(out var connection);
        using var _ = connection;

        var repository = new PromotionRepository(context);

        var promotion = Promotion.MakePromotion(
            "Black Friday",
            20,
            new DateOnly(2026, 11, 1),
            new DateOnly(2026, 11, 30)
        );

        var result = repository.Create(promotion);

        Assert.IsNotNull(result);
        Assert.AreNotEqual(0, result.Id);
        Assert.IsNotNull(context.Promotions.FirstOrDefault(p => p.Id == result.Id));
    }

    [TestMethod]
    public void GivenValidPromotion_WhenCreating_ThenStoredPromotionHasCorrectData()
    {
        using var context = SqliteTestContextFactory.CreateContext(out var connection);
        using var _ = connection;

        var repository = new PromotionRepository(context);

        var promotion = Promotion.MakePromotion(
            "Cyber Monday",
            30,
            new DateOnly(2026, 11, 10),
            new DateOnly(2026, 11, 20)
        );

        var result = repository.Create(promotion);

        var stored = context.Promotions.FirstOrDefault(p => p.Id == result.Id);

        Assert.IsNotNull(stored);
        Assert.AreEqual("Cyber Monday", stored.Name);
        Assert.AreEqual(30, stored.DiscountPercentage);
        Assert.AreEqual(new DateOnly(2026, 11, 10), stored.StartDate);
        Assert.AreEqual(new DateOnly(2026, 11, 20), stored.EndDate);
    }

    [TestMethod]
    public void GivenExistingPromotion_WhenGettingById_ThenReturnsPromotion()
    {
        using var context = SqliteTestContextFactory.CreateContext(out var connection);
        using var _ = connection;

        var repository = new PromotionRepository(context);

        var promotion = Promotion.MakePromotion(
            "Black Friday",
            20,
            new DateOnly(2026, 11, 1),
            new DateOnly(2026, 11, 30)
        );

        context.Promotions.Add(promotion);
        context.SaveChanges();

        var result = repository.GetById(promotion.Id);

        Assert.IsNotNull(result);
        Assert.AreEqual(promotion.Id, result.Id);
        Assert.AreEqual("Black Friday", result.Name);
    }

    [TestMethod]
    public void GivenNonExistingPromotion_WhenGettingById_ThenReturnsNull()
    {
        using var context = SqliteTestContextFactory.CreateContext(out var connection);
        using var _ = connection;

        var repository = new PromotionRepository(context);

        var result = repository.GetById(999);

        Assert.IsNull(result);
    }

    [TestMethod]
    public void GivenExistingPromotion_WhenUpdating_ThenStoredPromotionIsUpdated()
    {
        using var context = SqliteTestContextFactory.CreateContext(out var connection);
        using var _ = connection;

        var repository = new PromotionRepository(context);

        var promotion = Promotion.MakePromotion(
            "Black Friday",
            20,
            new DateOnly(2026, 11, 1),
            new DateOnly(2026, 11, 30)
        );

        context.Promotions.Add(promotion);
        context.SaveChanges();

        promotion.UpdateName("Cyber Monday");
        promotion.UpdateDiscountPercentage(30);
        promotion.UpdateDates(
            new DateOnly(2026, 11, 10),
            new DateOnly(2026, 11, 20)
        );

        var result = repository.Update(promotion);
        var storedPromotion = context.Promotions.FirstOrDefault(p => p.Id == promotion.Id);

        Assert.IsNotNull(result);
        Assert.IsNotNull(storedPromotion);
        Assert.AreEqual("Cyber Monday", storedPromotion.Name);
        Assert.AreEqual(30, storedPromotion.DiscountPercentage);
        Assert.AreEqual(new DateOnly(2026, 11, 10), storedPromotion.StartDate);
        Assert.AreEqual(new DateOnly(2026, 11, 20), storedPromotion.EndDate);
    }

    [TestMethod]
    public void GivenExistingPromotions_WhenGettingAll_ThenReturnsAllPromotions()
    {
        using var context = SqliteTestContextFactory.CreateContext(out var connection);
        using var _ = connection;

        var repository = new PromotionRepository(context);

        var p1 = Promotion.MakePromotion("Promo1", 10, new DateOnly(2026,1,1), new DateOnly(2026,1,10));
        var p2 = Promotion.MakePromotion("Promo2", 20, new DateOnly(2026,2,1), new DateOnly(2026,2,10));

        context.Promotions.AddRange(p1, p2);
        context.SaveChanges();

        var result = repository.GetAll();

        Assert.AreEqual(2, result.Count);
    }

    [TestMethod]
    public void GivenPromotionAndProduct_WhenUpdatingWithAssociatedProduct_ThenRelationIsStored()
    {
        using var context = SqliteTestContextFactory.CreateContext(out var connection);
        using var _ = connection;

        var repository = new PromotionRepository(context);

        var product = CreateValidProduct();
        context.Products.Add(product);

        var promotion = Promotion.MakePromotion(
            "Black Friday",
            20,
            new DateOnly(2026, 11, 1),
            new DateOnly(2026, 11, 30)
        );

        context.Promotions.Add(promotion);
        context.SaveChanges();

        promotion.AssociateProduct(product.Id);

        repository.Update(promotion);

        var storedPromotion = context.Promotions
            .Include(p => p.PromotionProducts)
            .FirstOrDefault(p => p.Id == promotion.Id);

        Assert.IsNotNull(storedPromotion);
        Assert.IsTrue(storedPromotion.PromotionProducts.Any(pp => pp.ProductId == product.Id));
    }

    [TestMethod]
    public void GivenAssociatedProduct_WhenUpdatingAfterDisassociating_ThenRelationIsRemoved()
    {
        using var context = SqliteTestContextFactory.CreateContext(out var connection);
        using var _ = connection;

        var repository = new PromotionRepository(context);

        var product = CreateValidProduct();
        context.Products.Add(product);

        var promotion = Promotion.MakePromotion(
            "Black Friday",
            20,
            new DateOnly(2026, 11, 1),
            new DateOnly(2026, 11, 30)
        );

        context.Promotions.Add(promotion);
        context.SaveChanges();

        promotion.AssociateProduct(product.Id);
        repository.Update(promotion);

        promotion.DisassociateProduct(product.Id);
        repository.Update(promotion);

        var storedPromotion = context.Promotions
            .Include(p => p.PromotionProducts)
            .FirstOrDefault(p => p.Id == promotion.Id);

        Assert.IsNotNull(storedPromotion);
        Assert.IsFalse(storedPromotion.PromotionProducts.Any(pp => pp.ProductId == product.Id));
    }
}
