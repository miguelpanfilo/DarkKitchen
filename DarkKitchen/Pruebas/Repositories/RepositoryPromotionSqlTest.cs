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
}
