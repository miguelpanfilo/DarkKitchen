using Contratos.Products;
using Contratos.Users;
using DarkKitchen.Controllers;
using DarkKitchen.Filters;
using LogicaDeNegocioAbstracciones.Products;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Pruebas.WebApi;

[TestClass]
public class ProductControllerTest
{
    [TestMethod]
    public void GivenValidProductDto_WhenCreating_ThenReturnsCreated()
    {
        var validBase64 = Convert.ToBase64String(new byte[] { 1, 2, 3 });

        var request = new ProductDTO(
            "COD123",
            "Milanesa al pan",
            "Milanesa al pan completa con fritas",
            "Minutas",
            "Sandwiches",
            100,
            new List<PhotoDTO>
            {
                new PhotoDTO("foto1", ".jpg", 100, validBase64)
            }
        );

        var response = new ProductResponseDto
        {
            Id = 1,
            Name = request.Name
        };

        var serviceMock = new Mock<IProductService>(MockBehavior.Strict);

        serviceMock
            .Setup(s => s.Create(request))
            .Returns(response);

        var controller = new ProductController(serviceMock.Object);

        var result = controller.Create(request) as CreatedResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(201, result.StatusCode);

        var value = result.Value as ProductResponseDto;
        Assert.IsNotNull(value);
        Assert.AreEqual(request.Name, value.Name);

        serviceMock.Verify(s => s.Create(request), Times.Once);
    }

    [TestMethod]
    public void Create_ShouldHaveHttpPostAttribute()
    {
        var method = typeof(ProductController)
            .GetMethod("Create");

        var attribute = method!
            .GetCustomAttributes(typeof(HttpPostAttribute), false)
            .FirstOrDefault();

        Assert.IsNotNull(attribute);
    }

    [TestMethod]
    public void Create_ShouldHaveRoleAuthorizationAttribute()
    {
        var method = typeof(ProductController)
            .GetMethod("Create");

        var attribute = method!
            .GetCustomAttributes(typeof(RoleAuthorizationAttribute), false)
            .FirstOrDefault() as RoleAuthorizationAttribute;

        Assert.IsNotNull(attribute);
    }
}
