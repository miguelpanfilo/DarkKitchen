using Contratos.Promotions;
using Contratos.Users;
using DarkKitchen.Controllers;
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
            new DateOnly(2026, 11, 30)
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
}
