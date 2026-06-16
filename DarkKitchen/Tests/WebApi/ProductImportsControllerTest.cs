using BusinessLogicsAbstractions;
using Contract;
using DarkKitchen.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Tests.WebApi;

[TestClass]
public class ProductImportsControllerTest
{
    [TestMethod]
    public void ImportProducts_ShouldCallService()
    {
        var serviceMock = new Mock<IProductImportService>(MockBehavior.Strict);

        var controller = new ProductImportsController(serviceMock.Object);

        var fileMock = new Mock<IFormFile>();

        serviceMock.Setup(s => s.ImportProducts(It.IsAny<ProductImportRequestDto>()))
            .Returns([]);

        controller.ImportProducts("CsvProductImporter", fileMock.Object);

        serviceMock.Verify(
            s => s.ImportProducts(It.IsAny<ProductImportRequestDto>()),
            Times.Once);
    }
}
