using AlmacenamientoAbstracciones;
using Contratos;
using Dominio;
using LogicaDeNegocio;
using Moq;

namespace Pruebas.Products;

[TestClass]
public class ProductServiceTest
{
    [TestMethod]
    public void GivenValidProductDto_WhenCreating_ThenRepositoryIsCalledAndProductIsReturned()
    {
        var dto = new ProductDTO(
            "COD123",
            "Milanesa al pan",
            "Milanesa al pan completa",
            "Minutas clásicas",
            "Sandwiches"
        );

        var repoMock = new Mock<IProductRepository>(MockBehavior.Strict);

        repoMock
            .Setup<Product>(repository => repository.Create(It.IsAny<Product>()))
            .Returns((Product product) => product);

        var service = new ProductService(repoMock.Object);

        var result = service.Create(dto);

        Assert.IsNotNull(result);
        Assert.AreEqual(dto.Name, result.Name);

        repoMock.Verify(repository => repository.Create(It.IsAny<Product>()), Times.Once);
    }
}
