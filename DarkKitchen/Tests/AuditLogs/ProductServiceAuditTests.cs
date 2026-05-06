using BusinessLogics.Products;
using BusinessLogicsAbstractions;
using BusinessLogicsAbstractions.AuditLogs;
using BusinessLogicsAbstractions.Users;
using Contract.Products;
using DataAccessAbstractions;
using Domain;
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
            .Setup(s => s.Save(It.IsAny<string>(),
                It.IsAny<byte[]>()))
            .Returns("photo.jpg");

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
}
