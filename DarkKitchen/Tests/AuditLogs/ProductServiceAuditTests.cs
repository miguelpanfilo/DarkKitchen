using BusinessLogics.Products;
using BusinessLogicsAbstractions;
using BusinessLogicsAbstractions.Users;
using Contract.Products;
using DataAccessAbstractions;
using Domain;
using Domain.Builders;
using Moq;

namespace Tests.AuditLogs;

[TestClass]
public class ProductServiceAuditTests
{
    [TestMethod]
    public void Create_ValidProduct_RegistersAuditLog()
    {
        var repoMock = new
            Mock<IProductRepository>(MockBehavior.Strict);
        var storageMock = new
            Mock<IProductPhotoStorage>(MockBehavior.Strict);
        var auditMock = new
            Mock<IAuditLogService>(MockBehavior.Strict);

        storageMock
            .Setup(s => s.Save(It.IsAny<string>(), It.IsAny<string>()))
            .Returns("uploads/photo1.jpg");

        repoMock
            .Setup(r => r.Create(It.IsAny<Product>()))
            .Returns((Product p) => p);

        auditMock
            .Setup(a => a.Register(AuditedEntities.Product,
                It.IsAny<int>(), AuditDescriptions.ProductCreated));

        var photoService = new
            ProductPhotoService(storageMock.Object);
        var service = new ProductService(repoMock.Object, photoService, auditMock.Object);

        service.Create(new ProductDTO(
            "CODE12345",
            "Producto valido 1",
            "Descripcion valida con mas de veinte caracteres",
            "Linea",
            "Categoria",
            100,
            new List<PhotoDTO> { new PhotoDTO("photo", ".jpg", 1024, "dGVzdA==") }
        ));

        auditMock.Verify(a => a.Register(AuditedEntities.Product,
            It.IsAny<int>(), AuditDescriptions.ProductCreated), Times.Once);
    }

    [TestMethod]
    public void Update_ValidProduct_RegistersAuditLog()
    {
        var repoMock = new Mock<IProductRepository>(MockBehavior.Strict);
        var storageMock = new
            Mock<IProductPhotoStorage>(MockBehavior.Strict);
        var auditMock = new Mock<IAuditLogService>(MockBehavior.Strict);

        var existingProduct = new ProductBuilder()
            .WithCode("CODE12345")
            .WithName("Producto valido 1")
            .WithDescription("Descripcion valida con mas de veinte caracteres")
            .WithCommercialLine("Linea")
            .WithCategory("Categoria")
            .WithPrice(100)
            .WithPhotos(new List<ProductPhoto>
            {
                new
                    ProductPhoto("photo.jpg")
            })
            .Build();

        repoMock
            .Setup(r => r.GetById(7))
            .Returns(existingProduct);

        repoMock
            .Setup(r => r.Update(It.IsAny<Product>()))
            .Returns((Product p) => p);

        auditMock
            .Setup(a => a.Register(AuditedEntities.Product, It.IsAny<int>(),
                AuditDescriptions.ProductModified));

        var photoService = new ProductPhotoService(storageMock.Object);
        var service = new ProductService(repoMock.Object, photoService,
            auditMock.Object);

        service.Update(7, new ProductPatchDto { Name = "Nuevo nombre valido" });

        auditMock.Verify(a => a.Register(AuditedEntities.Product,
            It.IsAny<int>(), AuditDescriptions.ProductModified), Times.Once);
    }

    [TestMethod]
    public void Delete_ValidProduct_RegistersAuditLog()
    {
        var repoMock = new Mock<IProductRepository>(MockBehavior.Strict);
        var storageMock = new
            Mock<IProductPhotoStorage>(MockBehavior.Strict);
        var auditMock = new Mock<IAuditLogService>(MockBehavior.Strict);

        var existingProduct = new ProductBuilder()
            .WithCode("CODE12345")
            .WithName("Producto valido 1")
            .WithDescription("Descripcion valida con mas de veinte caracteres")
            .WithCommercialLine("Linea")
            .WithCategory("Categoria")
            .WithPrice(100)
            .WithPhotos(new List<ProductPhoto>
            {
                new
                    ProductPhoto("photo.jpg")
            })
            .Build();

        repoMock
            .Setup(r => r.GetById(5))
            .Returns(existingProduct);

        repoMock
            .Setup(r => r.Update(It.IsAny<Product>()))
            .Returns((Product p) => p);

        auditMock
            .Setup(a => a.Register(AuditedEntities.Product,
                It.IsAny<int>(), AuditDescriptions.ProductDeleted));

        var photoService = new ProductPhotoService(storageMock.Object);
        var service = new ProductService(repoMock.Object, photoService,
            auditMock.Object);

        service.Delete(5);

        auditMock.Verify(a => a.Register(AuditedEntities.Product,
            It.IsAny<int>(), AuditDescriptions.ProductDeleted), Times.Once);
    }
}
