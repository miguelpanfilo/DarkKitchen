using BusinessLogicsAbstractions;
using Contract;
using DarkKitchen.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Tests.WebApi;

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
        _controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
    }

    private static CurrentUserDto MakeAdmin() =>
        new CurrentUserDto { Id = 1, Role = UserRolesDto.Administrative, Email = "admin@test.com" };

    [TestMethod]
    public void AssociateProduct_WhenAdmin_ShouldReturn200WithAssociatedProducts()
    {
        var response = new PromotionResponseDto(
            1, "Black Friday", 20,
            new DateOnly(2026, 11, 1), new DateOnly(2026, 11, 30),
            new List<ProductSummaryDto> { new(5, "Producto 5") }
        );

        _promotionServiceMock.Setup(x => x.AssociateProduct(1, 5)).Returns(response);
        SetCurrentUser(MakeAdmin());

        var result = _controller.AssociateProduct(1, 5) as OkObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(StatusCodes.Status200OK, result.StatusCode);
        var value = result!.Value as PromotionResponseDto;
        Assert.IsNotNull(value);
        Assert.AreEqual(1, value.Products.Count);
        Assert.AreEqual(5, value.Products[0].Id);
        Assert.AreEqual("Producto 5", value.Products[0].Name);

        _promotionServiceMock.Verify(x => x.AssociateProduct(1, 5), Times.Once);
    }

    [TestMethod]
    public void DisassociateProduct_WhenAdmin_ShouldReturn200WithNoProducts()
    {
        var response = new PromotionResponseDto(
            1, "Black Friday", 20,
            new DateOnly(2026, 11, 1), new DateOnly(2026, 11, 30),
            new List<ProductSummaryDto>()
        );

        _promotionServiceMock.Setup(x => x.DisassociateProduct(1, 5)).Returns(response);
        SetCurrentUser(MakeAdmin());

        var result = _controller.DisassociateProduct(1, 5) as OkObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(StatusCodes.Status200OK, result.StatusCode);
        var value = result!.Value as PromotionResponseDto;
        Assert.IsNotNull(value);
        Assert.AreEqual(0, value.Products.Count);

        _promotionServiceMock.Verify(x => x.DisassociateProduct(1, 5), Times.Once);
    }
}
