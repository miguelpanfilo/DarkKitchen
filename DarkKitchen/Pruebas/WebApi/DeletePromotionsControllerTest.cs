using Contratos.Users;
using DarkKitchen.Controllers;
using LogicaDeNegocioAbstracciones.Promotions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Pruebas.WebApi;
[TestClass]
public class DeletePromotionsControllerTest
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
        return new CurrentUserDto
        {
            Id = 1,
            Role = UserRolesDto.Administrative,
            Email = "admin@test.com"
        };
    }

    [TestMethod]
    public void Delete_WhenAdmin_ShouldReturn204()
    {
        _promotionServiceMock
            .Setup(x => x.Delete(1));

        SetCurrentUser(MakeAdmin());

        var result = _controller.Delete(1) as NoContentResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(StatusCodes.Status204NoContent, result.StatusCode);

        _promotionServiceMock.Verify(x => x.Delete(1), Times.Once);
    }
}
