using DataAccess.Repositories;
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

        var repository = new ShippingTypeRepository(context);
        var shippingType = ShippingType.MakeShippingType("Envío express", 150m);

        var result = repository.Create(shippingType);

        Assert.IsNotNull(result);
        Assert.AreNotEqual(0, result.Id);
        Assert.IsNotNull(context.ShippingTypes.FirstOrDefault(s => s.Id == result.Id));
    }

    [TestMethod]
    public void GivenValidShippingType_WhenCreating_ThenStoredShippingTypeHasCorrectData()
    {
        using var context = SqliteTestContextFactory.CreateContext(out var connection);
        using var _ = connection;

        var repository = new ShippingTypeRepository(context);
        var shippingType = ShippingType.MakeShippingType("Envío estándar", 80m);

        var result = repository.Create(shippingType);

        var stored = context.ShippingTypes.FirstOrDefault(s => s.Id == result.Id);

        Assert.IsNotNull(stored);
        Assert.AreEqual("Envío estándar", stored.Name);
        Assert.AreEqual(80m, stored.Cost);
    }

    [TestMethod]
    public void GivenStoredShippingTypes_WhenGetAll_ThenReturnsAll()
    {
        using var context = SqliteTestContextFactory.CreateContext(out var connection);
        using var _ = connection;

        var repository = new ShippingTypeRepository(context);

        repository.Create(ShippingType.MakeShippingType("Envío express", 150m));
        repository.Create(ShippingType.MakeShippingType("Envío estándar", 80m));

        var result = repository.GetAll();

        Assert.AreEqual(2, result.Count);
    }

    [TestMethod]
    public void GivenStoredShippingType_WhenGetById_ThenReturnsCorrectShippingType()
    {
        using var context = SqliteTestContextFactory.CreateContext(out var connection);
        using var _ = connection;

        var repository = new ShippingTypeRepository(context);
        var created = repository.Create(ShippingType.MakeShippingType("Envío express", 150m));

        var result = repository.GetById(created.Id);

        Assert.IsNotNull(result);
        Assert.AreEqual(created.Id, result.Id);
        Assert.AreEqual("Envío express", result.Name);
    }

    [TestMethod]
    public void GivenNonExistentId_WhenGetById_ThenReturnsNull()
    {
        using var context = SqliteTestContextFactory.CreateContext(out var connection);
        using var _ = connection;

        var repository = new ShippingTypeRepository(context);

        var result = repository.GetById(999);

        Assert.IsNull(result);
    }

    [TestMethod]
    public void GivenStoredShippingType_WhenUpdating_ThenDataIsUpdated()
    {
        using var context = SqliteTestContextFactory.CreateContext(out var connection);
        using var _ = connection;

        var repository = new ShippingTypeRepository(context);
        var created = repository.Create(ShippingType.MakeShippingType("Envío express", 150m));

        created.UpdateName("Envío premium");
        created.UpdateCost(200m);
        repository.Update(created);

        var stored = context.ShippingTypes.FirstOrDefault(s => s.Id == created.Id);

        Assert.IsNotNull(stored);
        Assert.AreEqual("Envío premium", stored.Name);
        Assert.AreEqual(200m, stored.Cost);
    }
}
