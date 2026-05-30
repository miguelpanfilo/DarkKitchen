using System.Security.Claims;
using BusinessLogicsAbstractions.Imports;
using Contract.Importers;
using DarkKitchen.Controllers;
using DarkKitchen.Filters;
using FluentAssertions;
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
        var serviceMock =
            new Mock<IProductImportService>(MockBehavior.Strict);

        var controller = new ProductImportsController(serviceMock.Object);

        var fileMock = new Mock<IFormFile>();

        serviceMock.Setup(s => s.ImportProducts(
                It.IsAny<ProductImportRequestDto>()))
            .Returns([]);

        controller.ImportProducts(
            "CsvProductImporter",
            fileMock.Object);

        serviceMock.Verify(
            s => s.ImportProducts(
                It.IsAny<ProductImportRequestDto>()),
            Times.Once);
    }

    [TestMethod]
    public void ImportProducts_ShouldHaveHttpPostAttribute()
    {
        var method = typeof(ProductImportsController)
            .GetMethod("ImportProducts");

        method.Should().NotBeNull();

        var attribute = method!
            .GetCustomAttributes(typeof(HttpPostAttribute), false)
            .Cast<HttpPostAttribute>()
            .FirstOrDefault();

        attribute.Should().NotBeNull();
    }

    [TestMethod]
    public void CreateImport_ShouldRequireAdministrativeOrClientRole()
    {
        var method = typeof(ProductImportsController)
            .GetMethod("ImportProducts");

        method.Should().NotBeNull();

        var attribute = method!
            .GetCustomAttributes(typeof(RoleAuthorizationAttribute), false)
            .Cast<RoleAuthorizationAttribute>()
            .FirstOrDefault();

        attribute.Should().NotBeNull();
    }
}
