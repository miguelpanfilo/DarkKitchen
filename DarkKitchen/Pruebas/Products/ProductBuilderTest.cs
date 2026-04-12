using Dominio;
using Dominio.Builders;

namespace Pruebas.Products;

[TestClass]
public class ProductBuilderTest
{
    private List<ProductPhoto> ValidPhotos(int count = 1)
    {
        var photos = new List<ProductPhoto>();

        for (var i = 0; i < count; i++)
        {
            photos.Add(new ProductPhoto($"foto{i}.jpg"));
        }

        return photos;
    }

    [TestMethod]
    public void GivenValidData_WhenBuildingProduct_ThenProductIsCreated()
    {
        var builder = new ProductBuilder()
            .WithCode("COD123")
            .WithName("Milanesa al pan")
            .WithDescription("Milanesa al pan completa")
            .WithCommercialLine("Minutas clásicas")
            .WithCategory("Sandwiches")
            .WithPrice(100)
            .WithPhotos(ValidPhotos(1));

        var product = builder.Build();

        Assert.IsNotNull(product);
        Assert.AreEqual("COD123", product.Code);
    }
}
