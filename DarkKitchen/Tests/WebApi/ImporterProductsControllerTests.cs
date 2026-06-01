using BusinessLogicsAbstractions;
using Contract;
using DarkKitchen.Controllers;
using DarkKitchen.Filters;
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

        var serviceMock = new Mock<IProductImporterQueryService>();

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

    [TestMethod]
    public void GetImporters_ShouldHaveRoleAuthorization()
    {
        var method = typeof(ProductImportersController)
            .GetMethod("GetImporters");

        var hasAttribute = method!
            .GetCustomAttributes(typeof(RoleAuthorizationAttribute), false)
            .Any();

        hasAttribute.Should().BeTrue();
    }
}
