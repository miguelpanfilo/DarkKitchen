using BusinessLogicsAbstractions.Imports;
using Contract.Importers;
using DarkKitchen.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

    [TestMethod]
    public void CreateImport_ShouldHaveHttpPostAttribute()
    {
        var method = typeof(ProductImportsController)
            .GetMethod("CreateImport");

        method.Should().NotBeNull();

        var attribute = method!
            .GetCustomAttributes(typeof(HttpPostAttribute), false)
            .Cast<HttpPostAttribute>()
            .FirstOrDefault();

        attribute.Should().NotBeNull();
    }
}
