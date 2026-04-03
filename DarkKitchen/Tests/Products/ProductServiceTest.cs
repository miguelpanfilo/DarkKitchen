using BusinessLogic;
using Domain;
using Moq;
using Storage.Abstractions;

namespace Tests.Products;

[TestClass]
public class ProductServiceTest
{
    [TestMethod]
    public void GivenValidProduct_WhenCreating_ThenProductIsReturned()
    {
        var product = new Product(
            "COD123",
            "Milanesa al pan",
            "Milanesa al pan completa",
            "Minutas clásicas",
            "Sandwiches"
        );

        var repoMock = new Mock<IProductRepository>();
        repoMock
            .Setup(repository => repository.Create(product))
            .Returns(product);

        var service = new ProductService(repoMock.Object);

        var productCreated = service.Create(product);

        Assert.IsNotNull(productCreated);
        Assert.AreEqual("XXXX", productCreated.Name);
    }
}
