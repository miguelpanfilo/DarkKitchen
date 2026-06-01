using System.Security.Claims;
using BusinessLogicsAbstractions;
using Contract;
using DarkKitchen.Controllers;
using DarkKitchen.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Tests.WebApi;

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
        var claims = new List<Claim> { new Claim(ClaimTypes.Email, "admin@dark.com") };
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(claims))
            }
        };

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
            Category = "Sandwiches",
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

    [TestMethod]
    public void Get_ShouldHaveRoleAuthorizationAttribute()
    {
        var method = typeof(ProductController)
            .GetMethod("Get");

        var attribute = method!
            .GetCustomAttributes(typeof(RoleAuthorizationAttribute), false)
            .Cast<RoleAuthorizationAttribute>()
            .SingleOrDefault();

        Assert.IsNotNull(attribute);
    }

    [TestMethod]
    public void GivenValidPatchRequest_WhenUpdatingProduct_ThenReturnsOk()
    {
        var serviceMock = new Mock<IProductService>(MockBehavior.Strict);

        var productId = 1;
        var request = new ProductPatchDto
        {
            Name = "Producto actualizado"
        };

        var response = new ProductResponseDto
        {
            Id = productId,
            Name = "Producto actualizado"
        };

        serviceMock
            .Setup(s => s.Update(productId, request))
            .Returns(response);

        var controller = new ProductController(serviceMock.Object);
        var claims = new List<Claim> { new Claim(ClaimTypes.Email, "admin@dark.com") };
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(claims))
            }
        };

        var result = controller.Update(productId, request);

        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(response, okResult.Value);

        serviceMock.Verify(s => s.Update(productId, request), Times.Once);
        serviceMock.VerifyAll();
    }

    [TestMethod]
    public void GivenUpdateEndpoint_WhenCheckingHttpAttribute_ThenHasHttpPatch()
    {
        var method = typeof(ProductController).GetMethod("Update");

        var attribute = method.GetCustomAttributes(typeof(HttpPatchAttribute), false);

        Assert.AreEqual(1, attribute.Length);
    }

    [TestMethod]
    public void GivenUpdateEndpoint_WhenCheckingRoleAuthorization_ThenAllowsAdministrative()
    {
        var method = typeof(ProductController).GetMethod("Update");

        var attribute = method
            .GetCustomAttributes(typeof(RoleAuthorizationAttribute), false)
            .Cast<RoleAuthorizationAttribute>()
            .FirstOrDefault();

        Assert.IsNotNull(attribute);
    }

    [TestMethod]
    public void GivenValidId_WhenDeletingProduct_ThenReturnsNoContent()
    {
        var serviceMock = new Mock<IProductService>(MockBehavior.Strict);
        var productId = 1;

        serviceMock
            .Setup(s => s.Delete(productId));

        var controller = new ProductController(serviceMock.Object);
        var claims = new List<Claim> { new Claim(ClaimTypes.Email, "admin@dark.com") };
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(claims))
            }
        };

        var result = controller.Delete(productId);

        Assert.IsInstanceOfType(result, typeof(NoContentResult));

        serviceMock.Verify(s => s.Delete(productId), Times.Once);
        serviceMock.VerifyAll();
    }

    [TestMethod]
    public void GivenDeleteEndpoint_WhenCheckingHttpAttribute_ThenHasHttpDelete()
    {
        var method = typeof(ProductController).GetMethod("Delete");

        var attribute = method.GetCustomAttributes(typeof(HttpDeleteAttribute), false);

        Assert.AreEqual(1, attribute.Length);
    }

    [TestMethod]
    public void GivenDeleteEndpoint_WhenCheckingRoleAuthorization_ThenAllowsAdministrative()
    {
        var method = typeof(ProductController).GetMethod("Delete");

        var attribute = method
            .GetCustomAttributes(typeof(RoleAuthorizationAttribute), false)
            .Cast<RoleAuthorizationAttribute>()
            .FirstOrDefault();

        Assert.IsNotNull(attribute);
    }
}
