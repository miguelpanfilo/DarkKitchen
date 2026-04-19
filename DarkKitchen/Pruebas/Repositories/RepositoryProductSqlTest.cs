using Almacenamiento;
using Dominio;
using Pruebas.Persistence;

namespace Pruebas.Repositories;

[TestClass]
public class RepositoryProductSqlTest
{
    [TestMethod]
    public void GivenValidProduct_WhenCreating_ThenProductIsStored()
    {
        using var context = SqliteTestContextFactory.CreateContext(out var connection);
        using var _ = connection;

        var repository = new ProductRepository(context);

        var product = Product.MakeProduct(
            "COD123",
            "Nombre válido producto",
            "Descripción válida con más de veinte caracteres",
            "Línea",
            "Categoría",
            100,
            new List<ProductPhoto> { new ProductPhoto("photo-1.jpg") }
        );

        var result = repository.Create(product);

        Assert.IsNotNull(result);
        Assert.AreNotEqual(0, result.Id);
        Assert.IsNotNull(context.Products.FirstOrDefault(p => p.Id == result.Id));
    }

    [TestMethod]
    public void GivenValidProduct_WhenCreating_ThenStoredProductHasCorrectData()
    {
        using var context = SqliteTestContextFactory.CreateContext(out var connection);
        using var _ = connection;

        var repository = new ProductRepository(context);

        var product = Product.MakeProduct(
            "COD123",
            "Nombre válido producto",
            "Descripción válida con más de veinte caracteres",
            "Línea",
            "Categoría",
            100,
            new List<ProductPhoto> { new ProductPhoto("photo-1.jpg") }
        );

        var result = repository.Create(product);

        var storedProduct = context.Products.FirstOrDefault(p => p.Id == result.Id);

        Assert.IsNotNull(storedProduct);
        Assert.AreEqual("COD123", storedProduct.Code);
        Assert.AreEqual("Nombre válido producto", storedProduct.Name);
        Assert.AreEqual("Descripción válida con más de veinte caracteres", storedProduct.Description);
        Assert.AreEqual("Línea", storedProduct.CommercialLine);
        Assert.AreEqual("Categoría", storedProduct.Category);
        Assert.AreEqual(100, storedProduct.Price);
        Assert.IsTrue(storedProduct.IsActive);
        Assert.IsFalse(storedProduct.IsDeleted);
    }

    [TestMethod]
    public void GivenExistingProduct_WhenGettingById_ThenReturnsProduct()
    {
        using var context = SqliteTestContextFactory.CreateContext(out var connection);
        using var _ = connection;

        var repository = new ProductRepository(context);

        var product = Product.MakeProduct(
            "COD123",
            "Nombre válido producto",
            "Descripción válida con más de veinte caracteres",
            "Línea",
            "Categoría",
            100,
            new List<ProductPhoto> { new ProductPhoto("photo-1.jpg") }
        );

        context.Products.Add(product);
        context.SaveChanges();

        var result = repository.GetById(product.Id);

        Assert.IsNotNull(result);
        Assert.AreEqual(product.Id, result.Id);
        Assert.AreEqual("COD123", result.Code);
    }

    [TestMethod]
    public void GivenNonExistingProduct_WhenGettingById_ThenReturnsNull()
    {
        using var context = SqliteTestContextFactory.CreateContext(out var connection);
        using var _ = connection;

        var repository = new ProductRepository(context);

        var result = repository.GetById(999);

        Assert.IsNull(result);
    }

    [TestMethod]
    public void GivenExistingProduct_WhenUpdating_ThenStoredProductIsUpdated()
    {
        using var context = SqliteTestContextFactory.CreateContext(out var connection);
        using var _ = connection;

        var repository = new ProductRepository(context);

        var product = Product.MakeProduct(
            "COD123",
            "Nombre válido producto",
            "Descripción válida con más de veinte caracteres",
            "Línea",
            "Categoría",
            100,
            new List<ProductPhoto> { new ProductPhoto("photo-1.jpg") }
        );

        context.Products.Add(product);
        context.SaveChanges();

        product.UpdateCode("COD456");
        product.UpdateName("Nombre actualizado");
        product.UpdateDescription("Descripción actualizada con más de veinte caracteres");
        product.UpdateCommercialLine("Nueva línea");
        product.UpdateCategory("Nueva categoría");
        product.UpdatePrice(200);

        var result = repository.Update(product);
        var storedProduct = context.Products.FirstOrDefault(p => p.Id == product.Id);

        Assert.IsNotNull(result);
        Assert.IsNotNull(storedProduct);
        Assert.AreEqual("COD456", storedProduct.Code);
        Assert.AreEqual("Nombre actualizado", storedProduct.Name);
        Assert.AreEqual("Descripción actualizada con más de veinte caracteres", storedProduct.Description);
        Assert.AreEqual("Nueva línea", storedProduct.CommercialLine);
        Assert.AreEqual("Nueva categoría", storedProduct.Category);
        Assert.AreEqual(200, storedProduct.Price);
    }
}
