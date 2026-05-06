using DataAccess;
using Domain;
using Tests.Persistence;

namespace Tests.Repositories;

[TestClass]
public class RepositoryShippingTypeSqlTest
{
    [TestMethod]
    public void GivenValidShippingType_WhenCreating_ThenShippingTypeIsStored()
    {
        using var context = SqliteTestContextFactory.CreateContext(out var connection);
        using var _ = connection;

        var shippingType = ShippingType.MakeShippingType("Envío express", 150m);

        context.ShippingTypes.Add(shippingType);
        context.SaveChanges();

        Assert.AreNotEqual(0, shippingType.Id);
        Assert.IsNotNull(context.ShippingTypes.FirstOrDefault(s => s.Id == shippingType.Id));
    }

    [TestMethod]
    public void GivenStoredShippingType_WhenRetrieving_ThenReturnsCorrectData()
    {
        using var context = SqliteTestContextFactory.CreateContext(out var connection);
        using var _ = connection;

        var shippingType = ShippingType.MakeShippingType("Envío estándar", 80m);

        context.ShippingTypes.Add(shippingType);
        context.SaveChanges();

        var stored = context.ShippingTypes.FirstOrDefault(s => s.Id == shippingType.Id);

        Assert.IsNotNull(stored);
        Assert.AreEqual("Envío estándar", stored.Name);
        Assert.AreEqual(80m, stored.Cost);
    }
}
