using BusinessLogicsAbstractions.Imports;
using Contract.Importers;
using DarkKitchen.Controllers;
using Microsoft.AspNetCore.Http;
using Moq;

namespace Tests.WebApi;

[TestClass]
public class ProductImportsControllerTest
{
    [TestMethod]
    public void CreateImport_ShouldCallService()
    {
        var serviceMock = new Mock<IProductImportService>();

        var controller = new ProductImportsController(serviceMock.Object);

        var fileMock = new Mock<IFormFile>();

        controller.CreateImport(
            "CsvProductImporter",
            fileMock.Object);

        serviceMock.Verify(
            s => s.ImportProducts(It.IsAny<ProductImportRequestDto>()),
            Times.Once);
    }
}
