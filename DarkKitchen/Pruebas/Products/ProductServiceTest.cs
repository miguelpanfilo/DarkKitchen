using AlmacenamientoAbstracciones;
using Contratos;
using Dominio;
using Dominio.Exceptions;
using LogicaDeNegocio;
using Moq;

namespace Pruebas.Products;

[TestClass]
public class ProductServiceTest
{
    [TestMethod]
    public void GivenValidProductDto_WhenCreating_ThenRepositoryIsCalledAndProductIsReturned()
    {
        var productDto = new ProductDTO(
            "COD123",
            "Milanesa al pan",
            "Milanesa al pan completa",
            "Minutas clásicas",
            "Sandwiches",
            100,
            new List<PhotoDTO>()
        );

        var repoMock = new Mock<IProductRepository>(MockBehavior.Strict);
        var storageMock = new Mock<IProductPhotoStorage>(MockBehavior.Strict);

        repoMock
            .Setup(repository => repository.Create(It.IsAny<Product>()))
            .Returns((Product product) => product);

        var service = new ProductService(repoMock.Object, storageMock.Object);

        var result = service.Create(productDto);

        Assert.IsNotNull(result);
        Assert.AreEqual(productDto.Name, result.Name);

        repoMock.Verify(repository => repository.Create(It.IsAny<Product>()), Times.Once);
    }

    [TestMethod]
    public void GivenProductDtoWithInvalidPrice_WhenCreating_ThenThrowsProductExceptionAndRepositoryIsNotCalled()
    {
        var dto = new ProductDTO(
            "COD123",
            "Milanesa al pan",
            "Milanesa al pan completa",
            "Minutas clásicas",
            "Sandwiches",
            0,
            new List<PhotoDTO>()
        );

        var repoMock = new Mock<IProductRepository>(MockBehavior.Strict);
        var storageMock = new Mock<IProductPhotoStorage>(MockBehavior.Strict);

        var service = new ProductService(repoMock.Object, storageMock.Object);

        Assert.ThrowsException<ProductException>(() => service.Create(dto));

        repoMock.Verify(repository => repository.Create(It.IsAny<Product>()), Times.Never);
    }

    [TestMethod]
    public void GivenValidProductDtoWithPhotos_WhenCreating_ThenPhotosAreSavedAndRepositoryIsCalled()
    {
        var photos = new List<PhotoDTO>
        {
            new PhotoDTO("foto1", ".jpg", 1000, "base64-falso")
        };

        var productDto = new ProductDTO(
            "COD123",
            "Milanesa al pan",
            "Milanesa al pan completa",
            "Minutas clásicas",
            "Sandwiches",
            100,
            photos
        );

        var repoMock = new Mock<IProductRepository>(MockBehavior.Strict);
        var storageMock = new Mock<IProductPhotoStorage>(MockBehavior.Strict);

        storageMock
            .Setup(storage => storage.Save(It.IsAny<PhotoDTO>()))
            .Returns("uploads/foto1.jpg");

        repoMock
            .Setup(repository => repository.Create(It.IsAny<Product>()))
            .Returns((Product product) => product);

        var service = new ProductService(repoMock.Object, storageMock.Object);

        var result = service.Create(productDto);

        Assert.IsNotNull(result);

        storageMock.Verify(storage => storage.Save(It.IsAny<PhotoDTO>()), Times.Once);
        repoMock.Verify(repository => repository.Create(It.IsAny<Product>()), Times.Once);
    }

    [TestMethod]
    public void GivenValidProductDtoWithPhotos_WhenCreating_ThenProductContainsSavedPhotos()
    {
        var photos = new List<PhotoDTO>
        {
            new PhotoDTO("foto1", ".jpg", 1000, "base64-falso")
        };

        var productDto = new ProductDTO(
            "COD123",
            "Milanesa al pan",
            "Milanesa al pan completa",
            "Minutas clásicas",
            "Sandwiches",
            100,
            photos
        );

        var repoMock = new Mock<IProductRepository>(MockBehavior.Strict);
        var storageMock = new Mock<IProductPhotoStorage>(MockBehavior.Strict);

        storageMock
            .Setup(s => s.Save(It.IsAny<PhotoDTO>()))
            .Returns("uploads/foto1.jpg");

        repoMock
            .Setup(r => r.Create(It.IsAny<Product>()))
            .Returns((Product p) => p);

        var service = new ProductService(repoMock.Object, storageMock.Object);

        var result = service.Create(productDto);

        Assert.IsNotNull(result);

        Assert.AreEqual(1, result.Photos.Count);
        Assert.AreEqual("uploads/foto1.jpg", result.Photos.First());
    }
}
