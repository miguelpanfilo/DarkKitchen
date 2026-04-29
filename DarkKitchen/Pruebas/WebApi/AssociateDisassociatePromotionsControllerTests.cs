using Contratos.Products;
using Contratos.Promotions;
using Contratos.Users;
using DarkKitchen.Controllers;
using DarkKitchen.Filters;
using LogicaDeNegocioAbstracciones.Promotions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Pruebas.WebApi;

[TestClass]
public class AssociateDisassociatePromotionsControllerTests
{
    private Mock<IPromotionService> _promotionServiceMock = null!;
    private PromotionsController _controller = null!;

    [TestInitialize]
    public void Initialize()
    {
        _promotionServiceMock = new Mock<IPromotionService>(MockBehavior.Strict);
        _controller = new PromotionsController(_promotionServiceMock.Object);
    }

    private void SetCurrentUser(CurrentUserDto user)
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Items["CurrentUser"] = user;

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };
    }

    private static CurrentUserDto MakeAdmin()
    {
        return new CurrentUserDto { Id = 1, Role = UserRolesDto.Administrative, Email = "admin@test.com" };
    }

    [TestMethod]
    public void AssociateProduct_WhenAdmin_ShouldReturn200()
    {
        var response = new PromotionResponseDto(
            1,
            "Black Friday",
            20,
            new DateOnly(2026, 11, 1),
            new DateOnly(2026, 11, 30),
            new List<ProductSummaryDto>
            {
                new (5, "Hamburguesa Clásica")
            }
        );

        _promotionServiceMock
            .Setup(x => x.AssociateProduct(1, 5))
            .Returns(response);

        SetCurrentUser(MakeAdmin());

        var result = _controller.AssociateProduct(1, 5) as OkObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(StatusCodes.Status200OK, result.StatusCode);
        Assert.AreEqual(response, result.Value);

        _promotionServiceMock.Verify(x => x.AssociateProduct(1, 5), Times.Once);
    }

    [TestMethod]
    public void AssociateProduct_WhenAdmin_ShouldReturnPromotionWithAssociatedProducts()
    {
        var response = new PromotionResponseDto(
            1,
            "Black Friday",
            20,
            new DateOnly(2026, 11, 1),
            new DateOnly(2026, 11, 30),
            new List<ProductSummaryDto>
            {
                new(5, "Producto 5")
            }
        );

        _promotionServiceMock
            .Setup(x => x.AssociateProduct(1, 5))
            .Returns(response);

        SetCurrentUser(MakeAdmin());

        var result = _controller.AssociateProduct(1, 5) as OkObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(StatusCodes.Status200OK, result.StatusCode);

        var value = result.Value as PromotionResponseDto;
        Assert.IsNotNull(value);
        Assert.IsNotNull(value.Products);
        Assert.AreEqual(1, value.Products.Count);
        Assert.AreEqual(5, value.Products[0].Id);
        Assert.AreEqual("Producto 5", value.Products[0].Name);

        _promotionServiceMock.Verify(x => x.AssociateProduct(1, 5), Times.Once);
    }

    [TestMethod]
    public void AssociateProduct_HasHttpPostAttributeWithRouteTemplate()
    {
        var method = typeof(PromotionsController).GetMethod(nameof(PromotionsController.AssociateProduct));

        Assert.IsNotNull(method);

        var attribute = method.GetCustomAttributes(typeof(HttpPostAttribute), false)
            .Cast<HttpPostAttribute>()
            .SingleOrDefault();

        Assert.IsNotNull(attribute);
        Assert.AreEqual("{promotionId}/products/{productId}", attribute.Template);
    }

    [TestMethod]
    public void DisassociateProduct_WhenAdmin_ShouldReturnPromotionWithAssociatedProducts()
    {
        var response = new PromotionResponseDto(
            1,
            "Black Friday",
            20,
            new DateOnly(2026, 11, 1),
            new DateOnly(2026, 11, 30),
            new List<ProductSummaryDto>()
        );

        _promotionServiceMock
            .Setup(x => x.DisassociateProduct(1, 5))
            .Returns(response);

        SetCurrentUser(MakeAdmin());

        var result = _controller.DisassociateProduct(1, 5) as OkObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(StatusCodes.Status200OK, result.StatusCode);

        var value = result.Value as PromotionResponseDto;
        Assert.IsNotNull(value);
        Assert.IsNotNull(value.Products);
        Assert.AreEqual(0, value.Products.Count);

        _promotionServiceMock.Verify(x => x.DisassociateProduct(1, 5), Times.Once);
    }

    [TestMethod]
    public void AssociateProduct_HasRoleAuthorizationAttribute()
    {
        var method = typeof(PromotionsController).GetMethod(nameof(PromotionsController.AssociateProduct));

        Assert.IsNotNull(method);

        var attribute = method.GetCustomAttributes(typeof(RoleAuthorizationAttribute), false)
            .Cast<RoleAuthorizationAttribute>()
            .SingleOrDefault();

        Assert.IsNotNull(attribute);
    }

    [TestMethod]
    public void DisassociateProduct_WhenAdmin_ShouldReturn200()
    {
        var response = new PromotionResponseDto(
            1,
            "Black Friday",
            20,
            new DateOnly(2026, 11, 1),
            new DateOnly(2026, 11, 30),
            new List<ProductSummaryDto>
            {
                new (5, "Hamburguesa Clásica")
            }
        );

        _promotionServiceMock
            .Setup(x => x.DisassociateProduct(1, 5))
            .Returns(response);

        SetCurrentUser(MakeAdmin());

        var result = _controller.DisassociateProduct(1, 5) as OkObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(StatusCodes.Status200OK, result.StatusCode);
        Assert.AreEqual(response, result.Value);

        _promotionServiceMock.Verify(x => x.DisassociateProduct(1, 5), Times.Once);
    }

    [TestMethod]
    public void DisassociateProduct_HasHttpPostAttributeWithRouteTemplate()
    {
        var method = typeof(PromotionsController).GetMethod(nameof(PromotionsController.DisassociateProduct));

        Assert.IsNotNull(method);

        var attribute = method.GetCustomAttributes(typeof(HttpDeleteAttribute), false)
            .Cast<HttpDeleteAttribute>()
            .SingleOrDefault();

        Assert.IsNotNull(attribute);
        Assert.AreEqual("{promotionId}/products/{productId}", attribute.Template);
    }

    [TestMethod]
    public void DisassociateProduct_HasRoleAuthorizationAttribute()
    {
        var method = typeof(PromotionsController).GetMethod(nameof(PromotionsController.DisassociateProduct));

        Assert.IsNotNull(method);

        var attribute = method.GetCustomAttributes(typeof(RoleAuthorizationAttribute), false)
            .Cast<RoleAuthorizationAttribute>()
            .SingleOrDefault();

        Assert.IsNotNull(attribute);
    }
}
