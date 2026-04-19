using Almacenamiento;
using Dominio;
using Pruebas.Persistence;

namespace Pruebas.Repositories;

[TestClass]
public class RepositoryPromotionSqlTest
{
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
}
