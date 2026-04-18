using AlmacenamientoAbstracciones;
using Contratos;
using Dominio;
using Dominio.Builders;
using Dominio.Exceptions;
using LogicaDeNegocio;
using Mapeo;
using Moq;

namespace Pruebas.Products;

[TestClass]
public class ProductServiceTest
{
    private static ProductService CreateService(
        Mock<IProductRepository> repoMock,
        Mock<IProductPhotoStorage> storageMock)
    {
        var photoService = new ProductPhotoService(storageMock.Object);
        return new ProductService(repoMock.Object, photoService);
    }

    private Product CreateValidProduct()
    {
        return new ProductBuilder()
            .WithCode("COD123")
            .WithName("Nombre válido producto")
            .WithDescription("Descripción válida con más de veinte caracteres")
            .WithCommercialLine("Línea")
            .WithCategory("Categoría")
            .WithPrice(100)
            .WithPhotos(new List<ProductPhoto>
            {
                new ProductPhoto("photo-1.jpg")
            })
            .Build();
    }

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

        var service = CreateService(repoMock, storageMock);

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

        var service = CreateService(repoMock, storageMock);

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

        var service = CreateService(repoMock, storageMock);

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

        var service = CreateService(repoMock, storageMock);

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

        var service = CreateService(repoMock, storageMock);

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

        var service = CreateService(repoMock, storageMock);

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

        var service = CreateService(repoMock, storageMock);

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

        var service = CreateService(repoMock, storageMock);

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
            .Setup(r => r.GetById(It.IsAny<int>()))
            .Returns(product);

        repoMock
            .Setup(r => r.Update(It.IsAny<Product>()))
            .Returns((Product p) => p);

        var service = CreateService(repoMock, storageMock);

        var result = service.Update(1, patchDto);

        Assert.IsNotNull(result);
        Assert.AreEqual("Chivito al pan", result.Name);

        repoMock.Verify(r => r.GetById(1), Times.Once);
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
            .Setup(r => r.GetById(It.IsAny<int>()))
            .Returns(product);

        repoMock
            .Setup(r => r.Update(It.IsAny<Product>()))
            .Returns((Product p) => p);

        var service = CreateService(repoMock, storageMock);

        var result = service.Update(1, patchDto);

        Assert.IsNotNull(result);
        Assert.AreEqual("COD1234", result.Code);

        repoMock.Verify(r => r.GetById(1), Times.Once);
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
            .Setup(r => r.GetById(It.IsAny<int>()))
            .Returns(product);

        repoMock
            .Setup(r => r.Update(It.IsAny<Product>()))
            .Returns((Product p) => p);

        var service = CreateService(repoMock, storageMock);

        var result = service.Update(1, patchDto);

        Assert.IsNotNull(result);
        Assert.AreEqual("Milanesa al plato completa con fritas", result.Description);

        repoMock.Verify(r => r.GetById(1), Times.Once);
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
            .Setup(r => r.GetById(It.IsAny<int>()))
            .Returns(product);

        repoMock
            .Setup(r => r.Update(It.IsAny<Product>()))
            .Returns((Product p) => p);

        var service = CreateService(repoMock, storageMock);

        var result = service.Update(1, patchDto);

        Assert.IsNotNull(result);
        Assert.AreEqual("Comida Rapida", result.CommercialLine);

        repoMock.Verify(r => r.GetById(1), Times.Once);
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
            .Setup(r => r.GetById(It.IsAny<int>()))
            .Returns(product);

        repoMock
            .Setup(r => r.Update(It.IsAny<Product>()))
            .Returns((Product p) => p);

        var service = CreateService(repoMock, storageMock);

        var result = service.Update(1, patchDto);

        Assert.IsNotNull(result);
        Assert.AreEqual("Milanesas al pan", result.Category);

        repoMock.Verify(r => r.GetById(1), Times.Once);
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
            .Setup(r => r.GetById(It.IsAny<int>()))
            .Returns(product);

        repoMock
            .Setup(r => r.Update(It.IsAny<Product>()))
            .Returns((Product p) => p);

        var service = CreateService(repoMock, storageMock);

        var result = service.Update(1, patchDto);

        Assert.IsNotNull(result);
        Assert.AreEqual(1000, result.Price);

        repoMock.Verify(r => r.GetById(1), Times.Once);
        repoMock.Verify(r => r.Update(It.IsAny<Product>()), Times.Once);
    }

    [TestMethod]
    public void GivenExistingProductAndPhotosPatchWithExistingAndNewPhotos_WhenUpdating_ThenRemovesMissingPhotoAndAddsNewPhoto()
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
                new ProductPhoto("/images/products/1/a.jpg"),
                new ProductPhoto("/images/products/1/b.jpg")
            }
        );

        var patchDto = new ProductPatchDto
        {
            Photos = new List<ProductPatchPhotoDto>
            {
                new ProductPatchPhotoDto
                {
                    Path = "/images/products/1/a.jpg"
                },
                new ProductPatchPhotoDto
                {
                    FileName = "nueva",
                    Extension = "jpg",
                    SizeInBytes = 100,
                    Base64 = "base64fake"
                }
            }
        };

        var repoMock = new Mock<IProductRepository>(MockBehavior.Strict);
        var storageMock = new Mock<IProductPhotoStorage>(MockBehavior.Strict);

        repoMock
            .Setup(r => r.GetById(1))
            .Returns(product);

        storageMock
            .Setup(s => s.Save(It.IsAny<PhotoDTO>()))
            .Returns("/images/products/1/nueva.jpg");

        repoMock
            .Setup(r => r.Update(It.IsAny<Product>()))
            .Returns((Product p) => p);

        var service = CreateService(repoMock, storageMock);

        var result = service.Update(1, patchDto);

        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.Photos.Count);
        Assert.IsTrue(result.Photos.Any(p => p.Path == "/images/products/1/a.jpg"));
        Assert.IsTrue(result.Photos.Any(p => p.Path == "/images/products/1/nueva.jpg"));
        Assert.IsFalse(result.Photos.Any(p => p.Path == "/images/products/1/b.jpg"));

        storageMock.Verify(s => s.Save(It.IsAny<PhotoDTO>()), Times.Once);
        repoMock.Verify(r => r.GetById(1), Times.Once);
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
            .Setup(r => r.GetById(It.IsAny<int>()))
            .Returns((Product)null!);

        repoMock
            .Setup(r => r.Update(It.IsAny<Product>()))
            .Returns((Product p) => p);

        var service = CreateService(repoMock, storageMock);

        var exception = Assert.ThrowsException<ProductException>(() => service.Update(1, patchDto));

        Assert.AreEqual("El producto no existe", exception.Message);

        repoMock.Verify(r => r.GetById(1), Times.Once);
        repoMock.Verify(r => r.Update(It.IsAny<Product>()), Times.Never);
    }

    [TestMethod]
    public void GivenPhotosPatchWithPathThatDoesNotBelongToProduct_WhenUpdating_ThenThrowsProductException()
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
                new ProductPhoto("/images/products/1/a.jpg")
            }
        );

        var patchDto = new ProductPatchDto
        {
            Photos = new List<ProductPatchPhotoDto>
            {
                new ProductPatchPhotoDto
                {
                    Path = "/images/products/1/no-existe.jpg"
                }
            }
        };

        var repoMock = new Mock<IProductRepository>(MockBehavior.Strict);
        var storageMock = new Mock<IProductPhotoStorage>(MockBehavior.Strict);

        repoMock
            .Setup(r => r.GetById(1))
            .Returns(product);

        var service = CreateService(repoMock, storageMock);

        var exception = Assert.ThrowsException<ProductException>(() => service.Update(1, patchDto));

        Assert.AreEqual("La foto no pertenece al producto", exception.Message);

        repoMock.Verify(r => r.GetById(1), Times.Once);
        repoMock.Verify(r => r.Update(It.IsAny<Product>()), Times.Never);
        storageMock.Verify(s => s.Save(It.IsAny<PhotoDTO>()), Times.Never);
    }

    [TestMethod]
    public void GivenPhotosPatchWithInvalidNewPhotoData_WhenUpdating_ThenThrowsProductException()
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
                new ProductPhoto("/images/products/1/a.jpg")
            }
        );

        var patchDto = new ProductPatchDto
        {
            Photos = new List<ProductPatchPhotoDto>
            {
                new ProductPatchPhotoDto
                {
                    FileName = "nueva",
                    Extension = "jpg",
                    SizeInBytes = null,
                    Base64 = "base64fake"
                }
            }
        };

        var repoMock = new Mock<IProductRepository>(MockBehavior.Strict);
        var storageMock = new Mock<IProductPhotoStorage>(MockBehavior.Strict);

        repoMock
            .Setup(r => r.GetById(1))
            .Returns(product);

        var service = CreateService(repoMock, storageMock);

        var exception = Assert.ThrowsException<ProductException>(() => service.Update(1, patchDto));

        Assert.AreEqual("Datos de foto inválidos", exception.Message);

        repoMock.Verify(r => r.GetById(1), Times.Once);
        repoMock.Verify(r => r.Update(It.IsAny<Product>()), Times.Never);
        storageMock.Verify(s => s.Save(It.IsAny<PhotoDTO>()), Times.Never);
    }

    [TestMethod]
    public void GivenPhotoWithSizeGreaterThan500KB_WhenCreatingProduct_ThenThrowsProductException()
    {
        var photos = new List<PhotoDTO>
        {
            new PhotoDTO("foto.jpg", "jpg", 512001, "base64")
        };

       var productDto = new ProductDTO(
            "COD123",
            "Nombre válido producto",
            "Descripción válida con más de veinte caracteres",
            "Línea",
            "Categoría",
            100,
            photos
        );

        var repoMock = new Mock<IProductRepository>(MockBehavior.Strict);
        var storageMock = new Mock<IProductPhotoStorage>(MockBehavior.Strict);

        var service = CreateService(repoMock, storageMock);

        var exception = Assert.ThrowsException<ProductException>(
            () => service.Create(productDto));

        Assert.AreEqual("La foto no puede superar los 500 KB", exception.Message);
    }

    [TestMethod]
    public void GivenPatchDtoWithIsActiveTrue_WhenUpdating_ThenProductIsActivated()
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
                new ProductPhoto("/images/products/1/a.jpg")
            });

        product.Deactivate();

        var patchDto = new ProductPatchDto
        {
            IsActive = true
        };

        var repoMock = new Mock<IProductRepository>(MockBehavior.Strict);
        var storageMock = new Mock<IProductPhotoStorage>(MockBehavior.Strict);

        repoMock.Setup(r => r.GetById(product.Id)).Returns(product);
        repoMock.Setup(r => r.Update(product)).Returns(product);

        var service = CreateService(repoMock, storageMock);

        var result = service.Update(product.Id, patchDto);

        Assert.IsTrue(result.IsActive);
        repoMock.Verify(r => r.GetById(product.Id), Times.Once);
        repoMock.Verify(r => r.Update(product), Times.Once);
    }

    [TestMethod]
    public void GivenPatchDtoWithIsActiveFalse_WhenUpdating_ThenProductIsDeactivated()
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
                new ProductPhoto("/images/products/1/a.jpg")
            });

        var patchDto = new ProductPatchDto
        {
            IsActive = false
        };

        var repoMock = new Mock<IProductRepository>(MockBehavior.Strict);
        var storageMock = new Mock<IProductPhotoStorage>(MockBehavior.Strict);

        repoMock.Setup(r => r.GetById(product.Id)).Returns(product);
        repoMock.Setup(r => r.Update(product)).Returns(product);

        var service = CreateService(repoMock, storageMock);

        var result = service.Update(product.Id, patchDto);

        Assert.IsFalse(result.IsActive);
        repoMock.Verify(r => r.GetById(product.Id), Times.Once);
        repoMock.Verify(r => r.Update(product), Times.Once);
    }

    [TestMethod]
    public void GivenExistingProduct_WhenDeleting_ThenDeletesPhotosAndProduct()
    {
        var productId = 1;
        var product = CreateValidProduct();

        var repoMock = new Mock<IProductRepository>(MockBehavior.Strict);
        var storageMock = new Mock<IProductPhotoStorage>(MockBehavior.Strict);

        repoMock.Setup(r => r.GetById(productId)).Returns(product);
        repoMock.Setup(r => r.Delete(productId));

        foreach (var photo in product.Photos)
        {
            storageMock.Setup(s => s.Delete(photo.Path));
        }

        var service = CreateService(repoMock, storageMock);

        service.Delete(productId);

        repoMock.Verify(r => r.GetById(productId), Times.Once);
        repoMock.Verify(r => r.Delete(productId), Times.Once);

        foreach (var photo in product.Photos)
        {
            storageMock.Verify(s => s.Delete(photo.Path), Times.Once);
        }
    }

    [TestMethod]
    public void GivenNonExistingProduct_WhenDeleting_ThenThrowsProductException()
    {
        var productId = 1;

        var repoMock = new Mock<IProductRepository>(MockBehavior.Strict);
        var storageMock = new Mock<IProductPhotoStorage>(MockBehavior.Strict);

        repoMock.Setup(r => r.GetById(productId)).Returns((Product)null!);

        var service = CreateService(repoMock, storageMock);

        var exception = Assert.ThrowsException<ProductException>(
            () => service.Delete(productId));

        Assert.AreEqual("El producto no existe", exception.Message);
    }

    [TestMethod]
    public void GivenProduct_WhenMappingToResponseDto_ThenReturnsProductResponseDtoWithMappedPhotos()
    {
        var product = new ProductBuilder()
            .WithCode("COD123")
            .WithName("Nombre válido producto")
            .WithDescription("Descripción válida con más de veinte caracteres")
            .WithCommercialLine("Línea")
            .WithCategory("Categoría")
            .WithPrice(100)
            .WithPhotos(new List<ProductPhoto>
            {
                new ProductPhoto("photo-1.jpg"),
                new ProductPhoto("photo-2.jpg")
            })
            .Build();

        var responseDto = ProductMapper.ToResponseDto(product);

        Assert.IsNotNull(responseDto);
        Assert.AreEqual(product.Id, responseDto.Id);
        Assert.AreEqual(product.Code, responseDto.Code);
        Assert.AreEqual(product.Name, responseDto.Name);
        Assert.AreEqual(product.Description, responseDto.Description);
        Assert.AreEqual(product.CommercialLine, responseDto.CommercialLine);
        Assert.AreEqual(product.Category, responseDto.Category);
        Assert.AreEqual(product.Price, responseDto.Price);
        Assert.AreEqual(product.IsActive, responseDto.IsActive);

        Assert.AreEqual(2, responseDto.Photos.Count);
        Assert.AreEqual("photo-1.jpg", responseDto.Photos[0].Path);
        Assert.AreEqual("photo-2.jpg", responseDto.Photos[1].Path);
    }

    [TestMethod]
    public void GivenExistingProducts_WhenGettingAll_ThenReturnsProductResponseDtoList()
    {
        var products = new List<Product>
        {
            CreateValidProduct(),
            new ProductBuilder()
                .WithCode("COD456")
                .WithName("Segundo producto válido")
                .WithDescription("Otra descripción válida con más de veinte caracteres")
                .WithCommercialLine("Otra línea")
                .WithCategory("Otra categoría")
                .WithPrice(200)
                .WithPhotos(new List<ProductPhoto>
                {
                    new ProductPhoto("photo-2.jpg")
                })
                .Build()
        };

        var repoMock = new Mock<IProductRepository>(MockBehavior.Strict);
        var storageMock = new Mock<IProductPhotoStorage>(MockBehavior.Strict);

        repoMock.Setup(r => r.GetAll()).Returns(products);

        var service = CreateService(repoMock, storageMock);

        var result = service.GetAll();

        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.Count);

        Assert.AreEqual(products[0].Code, result[0].Code);
        Assert.AreEqual(products[0].Name, result[0].Name);

        Assert.AreEqual(products[1].Code, result[1].Code);
        Assert.AreEqual(products[1].Name, result[1].Name);

        Assert.AreEqual(products[0].Photos.Count, result[0].Photos.Count);

        repoMock.Verify(r => r.GetAll(), Times.Once);
    }

    [TestMethod]
    public void GivenNoProducts_WhenGettingAll_ThenReturnsEmptyList()
    {
        var repoMock = new Mock<IProductRepository>(MockBehavior.Strict);
        var storageMock = new Mock<IProductPhotoStorage>(MockBehavior.Strict);

        repoMock.Setup(r => r.GetAll()).Returns(new List<Product>());

        var service = CreateService(repoMock, storageMock);

        var result = service.GetAll();

        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.Count);

        repoMock.Verify(r => r.GetAll(), Times.Once);
    }

    [TestMethod]
    public void GivenExistingProductId_WhenGettingById_ThenReturnsProductResponseDto()
    {
        var productId = 1;
        var product = CreateValidProduct();

        var repoMock = new Mock<IProductRepository>(MockBehavior.Strict);
        var storageMock = new Mock<IProductPhotoStorage>(MockBehavior.Strict);

        repoMock.Setup(r => r.GetById(productId)).Returns(product);

        var service = CreateService(repoMock, storageMock);

        var result = service.GetById(productId);

        Assert.IsNotNull(result);
        Assert.AreEqual(product.Code, result.Code);
        Assert.AreEqual(product.Name, result.Name);
        Assert.AreEqual(product.Description, result.Description);
        Assert.AreEqual(product.CommercialLine, result.CommercialLine);
        Assert.AreEqual(product.Category, result.Category);
        Assert.AreEqual(product.Price, result.Price);
        Assert.AreEqual(product.IsActive, result.IsActive);
        Assert.AreEqual(product.Photos.Count, result.Photos.Count);

        repoMock.Verify(r => r.GetById(productId), Times.Once);
    }

    [TestMethod]
    public void GivenNonExistingProductId_WhenGettingById_ThenThrowsProductException()
    {
        var productId = 1;

        var repoMock = new Mock<IProductRepository>(MockBehavior.Strict);
        var storageMock = new Mock<IProductPhotoStorage>(MockBehavior.Strict);

        repoMock.Setup(r => r.GetById(productId)).Returns((Product)null!);

        var service = CreateService(repoMock, storageMock);

        var exception = Assert.ThrowsException<ProductException>(
            () => service.GetById(productId));

        Assert.AreEqual("El producto no existe", exception.Message);
    }
}
