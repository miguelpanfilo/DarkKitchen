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

    [TestMethod]
    public void GivenSearchRequest_WhenGettingProducts_ThenReturnsOk()
    {
        var request = new ProductSearchRequestDto
        {
            CommercialLine = "Minutas",
            Categories = new List<string> { "Sandwiches" },
            Name = "Milanesa"
        };

        var response = new List<ProductResponseDto>
        {
            new ProductResponseDto
            {
                Id = 1,
                Code = "COD123",
                Name = "Milanesa al pan",
                Description = "Milanesa al pan completa con fritas",
                CommercialLine = "Minutas",
                Category = "Sandwiches",
                Price = 100,
                IsActive = true,
                Photos = new List<PhotoResponseDto>
                {
                    new PhotoResponseDto
                    {
                        Path = "uploads/photo1.jpg"
                    }
                }
            }
        };

        var serviceMock = new Mock<IProductService>(MockBehavior.Strict);

        serviceMock
            .Setup(s => s.GetProducts(request))
            .Returns(response);

        var controller = new ProductController(serviceMock.Object);

        var result = controller.Get(request) as OkObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);

        var value = result.Value as List<ProductResponseDto>;
        Assert.IsNotNull(value);
        Assert.AreEqual(1, value.Count);
        Assert.AreEqual("Milanesa al pan", value[0].Name);
        Assert.AreEqual("uploads/photo1.jpg", value[0].Photos[0].Path);

        serviceMock.Verify(s => s.GetProducts(request), Times.Once);
    }

    [TestMethod]
    public void Get_ShouldHaveHttpGetAttribute()
    {
        var method = typeof(ProductController)
            .GetMethod("Get");

        var attribute = method!
            .GetCustomAttributes(typeof(HttpGetAttribute), false)
            .Cast<HttpGetAttribute>()
            .SingleOrDefault();

        Assert.IsNotNull(attribute);
    }
}
