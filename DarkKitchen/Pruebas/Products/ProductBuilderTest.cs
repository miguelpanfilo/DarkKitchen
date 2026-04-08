using Dominio.Builders;

namespace Pruebas.Products;

[TestClass]
public class ProductBuilderTest
{
    [TestMethod]
    public void GivenValidData_WhenBuildingProduct_ThenProductIsCreated()
    {
        var builder = new ProductBuilder()
            .WithCode("COD123")
            .WithName("Milanesa al pan")
            .WithDescription("Milanesa al pan completa")
            .WithCommercialLine("Minutas clásicas")
            .WithCategory("Sandwiches")
            .WithPrice(100);

        var product = builder.Build();

        Assert.IsNotNull(product);
        Assert.AreEqual("COD123", product.Code);
    }
}
