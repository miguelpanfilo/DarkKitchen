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
        var photos = new List<PhotoDTO>
        {
            new ("foto1", ".jpg", 1000, "base64-falso")
        };
        var productDto = new ProductDTO(
            "COD123",
            "Milanesa al pan",
            "Milanesa al pan completa",
            "Minutas clásicas",
            "Sandwiches",
            100,
            photos);

        var repoMock = new Mock<IProductRepository>(MockBehavior.Strict);
        var storageMock = new Mock<IProductPhotoStorage>(MockBehavior.Strict);

        storageMock
            .Setup(s => s.Save(It.IsAny<PhotoDTO>()))
            .Returns("uploads/photo1.jpg");

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
            new List<PhotoDTO>());

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
            new ("foto1", ".jpg", 1000, "base64-falso")
        };

        var productDto = new ProductDTO(
            "COD123",
            "Milanesa al pan",
            "Milanesa al pan completa",
            "Minutas clásicas",
            "Sandwiches",
            100,
            photos);

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
            photos);

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
        Assert.AreEqual("uploads/foto1.jpg", result.Photos.First().Path);
    }

    [DataTestMethod]
    [DataRow(1)]
    [DataRow(2)]
    [DataRow(3)]
    public void GivenValidProductDtoWithPhotos_WhenCreating_ThenPhotosAreSavedWithPath(int cantidadFotos)
    {
        var photoDtos = new List<PhotoDTO>();

        for (var i = 1; i <= cantidadFotos; i++)
        {
            photoDtos.Add(new PhotoDTO(
                $"photo{i}",
                ".jpg",
                100,
                "base64fake"));
        }

        var productDto = new ProductDTO(
            "COD123",
            "Milanesa al pan",
            "Milanesa al pan completa",
            "Minutas clásicas",
            "Sandwiches",
            100,
            photoDtos);

        var repoMock = new Mock<IProductRepository>(MockBehavior.Strict);
        var storageMock = new Mock<IProductPhotoStorage>(MockBehavior.Strict);

        storageMock
            .Setup(s => s.Save(It.IsAny<PhotoDTO>()))
            .Returns((PhotoDTO photo) => $"uploads/{photo.FileName}{photo.Extension}");

        repoMock
            .Setup(r => r.Create(It.IsAny<Product>()))
            .Returns((Product p) => p);

        var service = new ProductService(repoMock.Object, storageMock.Object);

        var result = service.Create(productDto);

        Assert.IsNotNull(result);
        Assert.AreEqual(cantidadFotos, result.Photos.Count);

        for (var i = 0; i < cantidadFotos; i++)
        {
            Assert.AreEqual($"uploads/photo{i + 1}.jpg", result.Photos[i].Path);
        }

        storageMock.Verify(s => s.Save(It.IsAny<PhotoDTO>()), Times.Exactly(cantidadFotos));
        repoMock.Verify(r => r.Create(It.IsAny<Product>()), Times.Once);
    }

    [DataTestMethod]
    [DataRow(0)]
    [DataRow(4)]
    public void GivenInvalidProductDtoWithPhotos_WhenCreating_ThenThrowsProductException(int cantidadFotos)
    {
        var photoDtos = new List<PhotoDTO>();

        for (var i = 1; i <= cantidadFotos; i++)
        {
            photoDtos.Add(new PhotoDTO(
                $"photo{i}",
                ".jpg",
                100,
                "base64fake"));
        }

        var productDto = new ProductDTO(
            "COD123",
            "Milanesa al pan",
            "Milanesa al pan completa",
            "Minutas clásicas",
            "Sandwiches",
            100,
            photoDtos);

        var repoMock = new Mock<IProductRepository>(MockBehavior.Strict);
        var storageMock = new Mock<IProductPhotoStorage>(MockBehavior.Strict);

        var service = new ProductService(repoMock.Object, storageMock.Object);

        var exception = Assert.ThrowsException<ProductException>(() => service.Create(productDto));

        Assert.AreEqual("El producto debe tener entre 1 y 3 fotos", exception.Message);
        storageMock.Verify(s => s.Save(It.IsAny<PhotoDTO>()), Times.Never);
        repoMock.Verify(r => r.Create(It.IsAny<Product>()), Times.Never);
    }

    [DataTestMethod]
    [DataRow(".png")]
    [DataRow(".jpeg")]
    [DataRow(".gif")]
    [DataRow(".bmp")]
    [DataRow(".webp")]
    public void GivenNonJpgPhotos_WhenCreatingProduct_ThenThrowsProductException(string extension)
    {
        var photos = new List<PhotoDTO>
        {
            new PhotoDTO("photo1",
                extension,
                100,
                "base64fake")
        };

        var productDto = new ProductDTO(
            "COD123",
            "Milanesa al pan",
            "Milanesa al pan completa con fritas",
            "Minutas clásicas",
            "Sandwiches",
            100,
            photos
        );

        var repoMock = new Mock<IProductRepository>(MockBehavior.Strict);
        var storageMock = new Mock<IProductPhotoStorage>(MockBehavior.Strict);

        var service = new ProductService(repoMock.Object, storageMock.Object);

        var exception = Assert.ThrowsException<ProductException>(() =>
            service.Create(productDto)
        );

        Assert.AreEqual("Las fotos deben estar en formato JPG", exception.Message);

        storageMock.Verify(x => x.Save(It.IsAny<PhotoDTO>()), Times.Never);
    }

    [DataTestMethod]
    [DataRow(".jpg", ".png")]
    [DataRow(".png", "jpg")]
    public void GivenPhotosWithAtLeastOneNonJpgFormat_WhenCreatingProduct_ThenThrowsProductExceptionAndDoesNotSavePhotos(
        string firstExtension, string secondExtension)
    {
        var photos = new List<PhotoDTO>
        {
            new PhotoDTO("photo1", firstExtension, 100, "base64fake"),
            new PhotoDTO("photo2", secondExtension, 100, "base64fake")
        };

        var productDto = new ProductDTO(
            "COD123",
            "Milanesa al pan",
            "Milanesa al pan completa con fritas",
            "Minutas clásicas",
            "Sandwiches",
            100,
            photos
        );

        var repoMock = new Mock<IProductRepository>(MockBehavior.Strict);
        var storageMock = new Mock<IProductPhotoStorage>(MockBehavior.Strict);

        var service = new ProductService(repoMock.Object, storageMock.Object);

        var exception = Assert.ThrowsException<ProductException>(() =>
            service.Create(productDto));

        Assert.AreEqual("Las fotos deben estar en formato JPG", exception.Message);

        storageMock.Verify(x => x.Save(It.IsAny<PhotoDTO>()), Times.Never);
    }

    [TestMethod]
    public void GivenExistingProductAndValidNamePatch_WhenUpdating_ThenProductNameIsUpdatedAndRepositoryUpdateIsCalled()
    {
        var product = Product.MakeProduct(
            "COD123",
            "Milanesa al pan",
            "Milanesa al pan completa con fritas",
            "Minutas clásicas",
            "Sandwiches",
            100,
            new List<ProductPhoto>
            {
                new ProductPhoto("foto1.jpg")
            }
        );

        var patchDto = new ProductPatchDto
        {
            Name = "Chivito al pan"
        };

        var repoMock = new Mock<IProductRepository>(MockBehavior.Strict);
        var storageMock = new Mock<IProductPhotoStorage>(MockBehavior.Strict);

        repoMock
            .Setup(r => r.FindById(It.IsAny<int>()))
            .Returns(product);

        repoMock
            .Setup(r => r.Update(It.IsAny<Product>()))
            .Returns((Product p) => p);

        var service = new ProductService(repoMock.Object, storageMock.Object);

        var result = service.Update(1, patchDto);

        Assert.IsNotNull(result);
        Assert.AreEqual("Chivito al pan", result.Name);

        repoMock.Verify(r => r.FindById(1), Times.Once);
        repoMock.Verify(r => r.Update(It.IsAny<Product>()), Times.Once);
    }

    [TestMethod]
    public void GivenExistingProductAndValidCodePatch_WhenUpdating_ThenProductCodeIsUpdatedAndRepositoryUpdateIsCalled()
    {
        var product = Product.MakeProduct(
            "COD123",
            "Milanesa al pan",
            "Milanesa al pan completa con fritas",
            "Minutas clásicas",
            "Sandwiches",
            100,
            new List<ProductPhoto>
            {
                new ProductPhoto("foto1.jpg")
            }
        );

        var patchDto = new ProductPatchDto
        {
            Code = "COD1234"
        };

        var repoMock = new Mock<IProductRepository>(MockBehavior.Strict);
        var storageMock = new Mock<IProductPhotoStorage>(MockBehavior.Strict);

        repoMock
            .Setup(r => r.FindById(It.IsAny<int>()))
            .Returns(product);

        repoMock
            .Setup(r => r.Update(It.IsAny<Product>()))
            .Returns((Product p) => p);

        var service = new ProductService(repoMock.Object, storageMock.Object);

        var result = service.Update(1, patchDto);

        Assert.IsNotNull(result);
        Assert.AreEqual("COD1234", result.Code);

        repoMock.Verify(r => r.FindById(1), Times.Once);
        repoMock.Verify(r => r.Update(It.IsAny<Product>()), Times.Once);
    }

    [TestMethod]
    public void GivenExistingProductAndValidDescriptionPatch_WhenUpdating_ThenProductDescriptionIsUpdatedAndRepositoryUpdateIsCalled()
    {
        var product = Product.MakeProduct(
            "COD123",
            "Milanesa al pan",
            "Milanesa al pan completa con fritas",
            "Minutas clásicas",
            "Sandwiches",
            100,
            new List<ProductPhoto>
            {
                new ProductPhoto("foto1.jpg")
            }
        );

        var patchDto = new ProductPatchDto
        {
            Description = "Milanesa al plato completa con fritas"
        };

        var repoMock = new Mock<IProductRepository>(MockBehavior.Strict);
        var storageMock = new Mock<IProductPhotoStorage>(MockBehavior.Strict);

        repoMock
            .Setup(r => r.FindById(It.IsAny<int>()))
            .Returns(product);

        repoMock
            .Setup(r => r.Update(It.IsAny<Product>()))
            .Returns((Product p) => p);

        var service = new ProductService(repoMock.Object, storageMock.Object);

        var result = service.Update(1, patchDto);

        Assert.IsNotNull(result);
        Assert.AreEqual("Milanesa al plato completa con fritas", result.Description);

        repoMock.Verify(r => r.FindById(1), Times.Once);
        repoMock.Verify(r => r.Update(It.IsAny<Product>()), Times.Once);
    }

    [TestMethod]
    public void GivenExistingProductAndValidCommercialLinePatch_WhenUpdating_ThenProductCommercialLineIsUpdatedAndRepositoryUpdateIsCalled()
    {
        var product = Product.MakeProduct(
            "COD123",
            "Milanesa al pan",
            "Milanesa al pan completa con fritas",
            "Minutas clásicas",
            "Sandwiches",
            100,
            new List<ProductPhoto>
            {
                new ProductPhoto("foto1.jpg")
            }
        );

        var patchDto = new ProductPatchDto
        {
            CommercialLine = "Comida Rapida"
        };

        var repoMock = new Mock<IProductRepository>(MockBehavior.Strict);
        var storageMock = new Mock<IProductPhotoStorage>(MockBehavior.Strict);

        repoMock
            .Setup(r => r.FindById(It.IsAny<int>()))
            .Returns(product);

        repoMock
            .Setup(r => r.Update(It.IsAny<Product>()))
            .Returns((Product p) => p);

        var service = new ProductService(repoMock.Object, storageMock.Object);

        var result = service.Update(1, patchDto);

        Assert.IsNotNull(result);
        Assert.AreEqual("Comida Rapida", result.CommercialLine);

        repoMock.Verify(r => r.FindById(1), Times.Once);
        repoMock.Verify(r => r.Update(It.IsAny<Product>()), Times.Once);
    }

    [TestMethod]
    public void GivenExistingProductAndValidCategoryPatch_WhenUpdating_ThenProductCategoryIsUpdatedAndRepositoryUpdateIsCalled()
    {
        var product = Product.MakeProduct(
            "COD123",
            "Milanesa al pan",
            "Milanesa al pan completa con fritas",
            "Minutas clásicas",
            "Sandwiches",
            100,
            new List<ProductPhoto>
            {
                new ProductPhoto("foto1.jpg")
            }
        );

        var patchDto = new ProductPatchDto
        {
            Category = "Milanesas al pan"
        };

        var repoMock = new Mock<IProductRepository>(MockBehavior.Strict);
        var storageMock = new Mock<IProductPhotoStorage>(MockBehavior.Strict);

        repoMock
            .Setup(r => r.FindById(It.IsAny<int>()))
            .Returns(product);

        repoMock
            .Setup(r => r.Update(It.IsAny<Product>()))
            .Returns((Product p) => p);

        var service = new ProductService(repoMock.Object, storageMock.Object);

        var result = service.Update(1, patchDto);

        Assert.IsNotNull(result);
        Assert.AreEqual("Milanesas al pan", result.Category);

        repoMock.Verify(r => r.FindById(1), Times.Once);
        repoMock.Verify(r => r.Update(It.IsAny<Product>()), Times.Once);
    }

    [TestMethod]
    public void GivenExistingProductAndValidPricePatch_WhenUpdating_ThenProductPriceIsUpdatedAndRepositoryUpdateIsCalled()
    {
        var product = Product.MakeProduct(
            "COD123",
            "Milanesa al pan",
            "Milanesa al pan completa con fritas",
            "Minutas clásicas",
            "Sandwiches",
            100,
            new List<ProductPhoto>
            {
                new ProductPhoto("foto1.jpg")
            }
        );

        var patchDto = new ProductPatchDto
        {
            Price = 1000
        };

        var repoMock = new Mock<IProductRepository>(MockBehavior.Strict);
        var storageMock = new Mock<IProductPhotoStorage>(MockBehavior.Strict);

        repoMock
            .Setup(r => r.FindById(It.IsAny<int>()))
            .Returns(product);

        repoMock
            .Setup(r => r.Update(It.IsAny<Product>()))
            .Returns((Product p) => p);

        var service = new ProductService(repoMock.Object, storageMock.Object);

        var result = service.Update(1, patchDto);

        Assert.IsNotNull(result);
        Assert.AreEqual(1000, result.Price);

        repoMock.Verify(r => r.FindById(1), Times.Once);
        repoMock.Verify(r => r.Update(It.IsAny<Product>()), Times.Once);
    }

    [TestMethod]
    public void GivenNonExistingProduct_WhenUpdating_ThenThrowsProductException()
    {
        var patchDto = new ProductPatchDto
        {
            Name = "Chivito al pan"
        };

        var repoMock = new Mock<IProductRepository>(MockBehavior.Strict);
        var storageMock = new Mock<IProductPhotoStorage>(MockBehavior.Strict);

        repoMock
            .Setup(r => r.FindById(It.IsAny<int>()))
            .Returns((Product)null!);

        repoMock
            .Setup(r => r.Update(It.IsAny<Product>()))
            .Returns((Product p) => p);

        var service = new ProductService(repoMock.Object, storageMock.Object);

        var exception = Assert.ThrowsException<ProductException>(() => service.Update(1, patchDto));

        Assert.AreEqual("El producto no existe", exception.Message);

        repoMock.Verify(r => r.FindById(1), Times.Once);
        repoMock.Verify(r => r.Update(It.IsAny<Product>()), Times.Never);
    }
}
