using BusinessLogicsAbstractions.Imports;
using Contract.Importers;
using DarkKitchen.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Tests.WebApi;

[TestClass]
public class ImporterProductsControllerTests
{
    [TestMethod]
    public void GetImporters_WhenCalled_ShouldReturnOkWithListOfImporters()
    {
        var expectedImporters = new List<ImporterDto>
        {
            new ImporterDto
            {
                Id = "JsonProductImporter",
                Name = "Importador JSON"
            },
            new ImporterDto
            {
                Id = "XmlProductImporter",
                Name = "Importador XML"
            }
        };

        var serviceMock = new Mock<IProductImportService>();

        serviceMock
            .Setup(s => s.GetImporters())
            .Returns(expectedImporters);

        var controller = new ProductImportersController(serviceMock.Object);

        var result = controller.GetImporters();

        var okResult = result as OkObjectResult;

        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(expectedImporters);
    }
}
