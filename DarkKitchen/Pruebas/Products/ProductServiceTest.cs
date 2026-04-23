using AlmacenamientoAbstracciones;
using Contratos;
using Contratos.Products;
using Contratos.Promotions;
using Dominio;
using Dominio.Builders;
using Dominio.Exceptions;
using LogicaDeNegocio;
using LogicaDeNegocio.Products;
using LogicaDeNegocio.Promotions;
using LogicaDeNegocioAbstracciones.Exceptions;
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

    private Product CreateValidProduct(
        string code = "CODE12345",
        string name = "Producto valido 1",
        string commercialLine = "Linea",
        string category = "Categoria")
    {
        return new ProductBuilder()
            .WithCode(code)
            .WithName(name)
            .WithDescription("Descripcion valida con mas de veinte caracteres")
            .WithCommercialLine(commercialLine)
            .WithCategory(category)
            .WithPrice(100)
            .WithPhotos(new List<ProductPhoto>
            {
                new ProductPhoto($"{code}.jpg")
            })
            .Build();
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
        var validBase64 = Convert.ToBase64String(new byte[] { 1, 2, 3 });
        var photos = new List<PhotoDTO>
        {
            new ("foto1", ".jpg", 1000, validBase64)
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
            .Setup(s => s.Save(It.IsAny<string>(), It.IsAny<string>()))
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
        var validBase64 = Convert.ToBase64String(new byte[] { 1, 2, 3 });
        var photos = new List<PhotoDTO>
        {
            new ("foto1", ".jpg", 1000, validBase64)
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
            .Setup(storage => storage.Save(It.IsAny<string>(), It.IsAny<string>()))
            .Returns("uploads/foto1.jpg");

        repoMock
            .Setup(repository => repository.Create(It.IsAny<Product>()))
            .Returns((Product product) => product);

        var service = CreateService(repoMock, storageMock);

        var result = service.Create(productDto);

        Assert.IsNotNull(result);

        storageMock.Verify(storage => storage.Save(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        repoMock.Verify(repository => repository.Create(It.IsAny<Product>()), Times.Once);
    }

    [TestMethod]
    public void GivenValidProductDtoWithPhotos_WhenCreating_ThenProductContainsSavedPhotos()
    {
        var validBase64 = Convert.ToBase64String(new byte[] { 1, 2, 3 });
        var photos = new List<PhotoDTO>
        {
            new PhotoDTO("foto1", ".jpg", 1000, validBase64)
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
            .Setup(s => s.Save(It.IsAny<string>(), It.IsAny<string>()))
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
        var validBase64 = Convert.ToBase64String(new byte[] { 1, 2, 3 });
        for (var i = 1; i <= cantidadFotos; i++)
        {
            photoDtos.Add(new PhotoDTO(
                $"photo{i}",
                ".jpg",
                100,
                validBase64));
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

        var counter = 0;
        storageMock
            .Setup(s => s.Save(It.IsAny<string>(), It.IsAny<string>()))
            .Returns((string base64, string extension) =>
            {
                counter++;
                return $"uploads/photo{counter}{extension}";
            });

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

        storageMock.Verify(s => s.Save(It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(cantidadFotos));
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
        storageMock.Verify(s => s.Save(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
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
        var validBase64 = Convert.ToBase64String(new byte[] { 1, 2, 3 });
        var photos = new List<PhotoDTO>
        {
            new PhotoDTO("photo1",
                extension,
                100,
                validBase64)
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

        storageMock.Verify(x => x.Save(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [DataTestMethod]
    [DataRow(".jpg", ".png")]
    [DataRow(".png", "jpg")]
    public void GivenPhotosWithAtLeastOneNonJpgFormat_WhenCreatingProduct_ThenThrowsProductExceptionAndDoesNotSavePhotos(
        string firstExtension, string secondExtension)
    {
        var validBase64 = Convert.ToBase64String(new byte[] { 1, 2, 3 });
        var photos = new List<PhotoDTO>
        {
            new PhotoDTO("photo1", firstExtension, 100, validBase64),
            new PhotoDTO("photo2", secondExtension, 100, validBase64)
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

        storageMock.Verify(x => x.Save(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
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
                    Base64 = Convert.ToBase64String(new byte[] { 1, 2, 3 })
                }
            }
        };

        var repoMock = new Mock<IProductRepository>(MockBehavior.Strict);
        var storageMock = new Mock<IProductPhotoStorage>(MockBehavior.Strict);

        repoMock
            .Setup(r => r.GetById(1))
            .Returns(product);

        storageMock
            .Setup(s => s.Save(It.IsAny<string>(), It.IsAny<string>()))
            .Returns("/images/products/1/nueva.jpg");

        storageMock
            .Setup(s => s.Delete("/images/products/1/b.jpg"));

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

        storageMock.Verify(s => s.Save(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        storageMock.Verify(s => s.Delete("/images/products/1/b.jpg"), Times.Once);
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

        var exception = Assert.ThrowsException<NotFoundException>(() => service.Update(1, patchDto));

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

        var exception = Assert.ThrowsException<ValidationException>(() => service.Update(1, patchDto));

        Assert.AreEqual("La foto no pertenece al producto", exception.Message);

        repoMock.Verify(r => r.GetById(1), Times.Once);
        repoMock.Verify(r => r.Update(It.IsAny<Product>()), Times.Never);
        storageMock.Verify(s => s.Save(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
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

        var exception = Assert.ThrowsException<ValidationException>(() => service.Update(1, patchDto));

        Assert.AreEqual("Datos de foto inválidos", exception.Message);

        repoMock.Verify(r => r.GetById(1), Times.Once);
        repoMock.Verify(r => r.Update(It.IsAny<Product>()), Times.Never);
        storageMock.Verify(s => s.Save(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [TestMethod]
    public void GivenPhotoWithSizeGreaterThan500KB_WhenCreatingProduct_ThenThrowsProductException()
    {
        var validBase64 = Convert.ToBase64String(new byte[] { 1, 2, 3 });
        var photos = new List<PhotoDTO>
        {
            new PhotoDTO("foto.jpg", "jpg", 512001, validBase64)
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
    public void GivenExistingProduct_WhenDeleting_ThenMarksItAsDeletedAndUpdatesRepository()
    {
        var productId = 1;
        var product = CreateValidProduct();

        var repoMock = new Mock<IProductRepository>(MockBehavior.Strict);
        var storageMock = new Mock<IProductPhotoStorage>(MockBehavior.Strict);

        repoMock.Setup(r => r.GetById(productId)).Returns(product);

        repoMock
            .Setup(r => r.Update(It.Is<Product>(p => p.IsDeleted)))
            .Returns((Product p) => p);

        var service = CreateService(repoMock, storageMock);

        service.Delete(productId);

        Assert.IsTrue(product.IsDeleted);

        repoMock.Verify(r => r.GetById(productId), Times.Once);
        repoMock.Verify(r => r.Update(It.Is<Product>(p => p.IsDeleted)), Times.Once);

        storageMock.Verify(s => s.Delete(It.IsAny<string>()), Times.Never);
    }

    [TestMethod]
    public void GivenNonExistingProduct_WhenDeleting_ThenThrowsProductException()
    {
        var productId = 1;

        var repoMock = new Mock<IProductRepository>(MockBehavior.Strict);
        var storageMock = new Mock<IProductPhotoStorage>(MockBehavior.Strict);

        repoMock.Setup(r => r.GetById(productId)).Returns((Product)null!);

        var service = CreateService(repoMock, storageMock);

        var exception = Assert.ThrowsException<NotFoundException>(
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
    public void GivenExistingProducts_WhenGettingProductsWithoutFilters_ThenReturnsProductResponseDtoList()
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

        var result = service.GetProducts(new ProductSearchRequestDto());

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
    public void GivenNoProducts_WhenGettingProductsWithoutFilters_ThenReturnsEmptyList()
    {
        var repoMock = new Mock<IProductRepository>(MockBehavior.Strict);
        var storageMock = new Mock<IProductPhotoStorage>(MockBehavior.Strict);

        repoMock.Setup(r => r.GetAll()).Returns(new List<Product>());

        var service = CreateService(repoMock, storageMock);

        var result = service.GetProducts(new ProductSearchRequestDto());

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

        var exception = Assert.ThrowsException<NotFoundException>(
            () => service.GetById(productId));

        Assert.AreEqual("El producto no existe", exception.Message);
    }

    [TestMethod]
    public void GivenDeletedProduct_WhenUpdating_ThenThrowsProductException()
    {
        var product = CreateValidProduct();
        product.Delete();

        var patchDto = new ProductPatchDto
        {
            Name = "Nuevo nombre"
        };

        var repoMock = new Mock<IProductRepository>(MockBehavior.Strict);
        var storageMock = new Mock<IProductPhotoStorage>(MockBehavior.Strict);

        repoMock
            .Setup(r => r.GetById(1))
            .Returns(product);

        var service = CreateService(repoMock, storageMock);

        var exception = Assert.ThrowsException<NotFoundException>(() => service.Update(1, patchDto));

        Assert.AreEqual("El producto no existe", exception.Message);

        repoMock.Verify(r => r.GetById(1), Times.Once);
        repoMock.Verify(r => r.Update(It.IsAny<Product>()), Times.Never);
    }

    [TestMethod]
    public void GivenValidDateProductIdAndCommercialLine_WhenProductDoesNotMatchLine_ThenReturnsEmptyList()
    {
        var promotionRepositoryMock = new Mock<IPromotionRepository>(MockBehavior.Strict);
        var productRepositoryMock = new Mock<IProductRepository>(MockBehavior.Strict);

        var date = new DateOnly(2026, 1, 26);

        var promotion = Promotion.MakePromotion(
            "Promo Mixta",
            10,
            new DateOnly(2026, 1, 25),
            new DateOnly(2026, 1, 30)
        );
        promotion.AssociateProduct(100);
        promotion.AssociateProduct(200);

        var bebidaProduct = new ProductBuilder()
            .WithCode("COD100")
            .WithName("Producto Bebida")
            .WithDescription("Descripción válida con más de veinte caracteres")
            .WithCommercialLine("Bebidas")
            .WithCategory("Categoría")
            .WithPrice(100)
            .WithPhotos(new List<ProductPhoto> { new ProductPhoto("photo-1.jpg") })
            .Build();

        var comidaProduct = new ProductBuilder()
            .WithCode("COD200")
            .WithName("Producto Comida")
            .WithDescription("Descripción válida con más de veinte caracteres")
            .WithCommercialLine("Comida")
            .WithCategory("Categoría")
            .WithPrice(100)
            .WithPhotos(new List<ProductPhoto> { new ProductPhoto("photo-2.jpg") })
            .Build();

        promotionRepositoryMock
            .Setup(r => r.GetAll())
            .Returns(new List<Promotion> { promotion });

        productRepositoryMock
            .Setup(r => r.GetById(100))
            .Returns(bebidaProduct);

        productRepositoryMock
            .Setup(r => r.GetById(200))
            .Returns(comidaProduct);

        var request = new PromotionSearchRequestDto
        {
            Date = date,
            ProductId = 100,
            CommercialLine = "Comida"
        };

        var service = new PromotionService(
            promotionRepositoryMock.Object,
            productRepositoryMock.Object
        );

        var result = service.GetPromotions(request);

        Assert.AreEqual(0, result.Count);

        promotionRepositoryMock.VerifyAll();
        productRepositoryMock.VerifyAll();
    }

    [TestMethod]
    public void GivenProducts_WhenGettingProductsWithoutFilters_ThenReturnsAll()
    {
        var products = new List<Product>
        {
            CreateValidProduct("CODE12345", "Producto valido 1"),
            CreateValidProduct("CODE12346", "Producto valido 2")
        };

        var repositoryMock = new Mock<IProductRepository>(MockBehavior.Strict);

        repositoryMock
            .Setup(r => r.GetAll())
            .Returns(products);

        var service = new ProductService(repositoryMock.Object, null!);

        var result = service.GetProducts(new ProductSearchRequestDto());

        Assert.AreEqual(2, result.Count);

        repositoryMock.Verify(r => r.GetAll(), Times.Once);
    }

    [TestMethod]
    public void GivenProducts_WhenFilteringByName_ThenReturnsMatchingProducts()
    {
        var products = new List<Product>
        {
            CreateValidProduct("CODE12345", "Producto valido 1"),
            CreateValidProduct("CODE12346", "Otro producto distinto")
        };

        var repoMock = new Mock<IProductRepository>(MockBehavior.Strict);
        var storageMock = new Mock<IProductPhotoStorage>(MockBehavior.Strict);

        repoMock.Setup(r => r.GetAll()).Returns(products);

        var service = CreateService(repoMock, storageMock);

        var request = new ProductSearchRequestDto
        {
            Name = "valido"
        };

        var result = service.GetProducts(request);

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("Producto valido 1", result[0].Name);

        repoMock.Verify(r => r.GetAll(), Times.Once);
    }

    [TestMethod]
    public void GivenProducts_WhenFilteringByCommercialLine_ThenReturnsMatchingProducts()
    {
        var products = new List<Product>
        {
            CreateValidProduct("CODE12345", "Producto 1", "Linea1"),
            CreateValidProduct("CODE12346", "Producto 2", "Linea2")
        };

        var repoMock = new Mock<IProductRepository>(MockBehavior.Strict);
        var storageMock = new Mock<IProductPhotoStorage>(MockBehavior.Strict);

        repoMock.Setup(r => r.GetAll()).Returns(products);

        var service = CreateService(repoMock, storageMock);

        var request = new ProductSearchRequestDto
        {
            CommercialLine = "Linea1"
        };

        var result = service.GetProducts(request);

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("Producto 1", result[0].Name);

        repoMock.Verify(r => r.GetAll(), Times.Once);
    }
}
