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
public class CreatePromotionsControllerTests
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
            Email = "admin@test.com",
            Role = UserRolesDto.Administrative
        };
    }

    [TestMethod]
    public void Create_WhenAdmin_ShouldReturn201()
    {
        var request = new PromotionRequestDto
        {
            Name = "Black Friday",
            DiscountPercentage = 20,
            StartDate = new DateOnly(2026, 11, 1),
            EndDate = new DateOnly(2026, 11, 30)
        };

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
            .Setup(x => x.Create(request))
            .Returns(response);

        SetCurrentUser(MakeAdmin());

        var result = _controller.Create(request) as CreatedResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(StatusCodes.Status201Created, result.StatusCode);
        Assert.AreEqual(response, result.Value);

        _promotionServiceMock.Verify(x => x.Create(request), Times.Once);
    }

    [TestMethod]
    public void Create_HasHttpPostAttribute()
    {
        var method = typeof(DarkKitchen.Controllers.PromotionsController).GetMethod(nameof(DarkKitchen.Controllers.PromotionsController.Create));

        Assert.IsNotNull(method);

        var attribute = method.GetCustomAttributes(typeof(HttpPostAttribute), false)
            .Cast<HttpPostAttribute>()
            .SingleOrDefault();

        Assert.IsNotNull(attribute);
    }

    [TestMethod]
    public void Create_HasRoleAuthorizationAttribute()
    {
        var method = typeof(DarkKitchen.Controllers.PromotionsController).GetMethod(nameof(DarkKitchen.Controllers.PromotionsController.Create));

        Assert.IsNotNull(method);

        var attribute = method.GetCustomAttributes(typeof(RoleAuthorizationAttribute), false)
            .Cast<RoleAuthorizationAttribute>()
            .SingleOrDefault();

        Assert.IsNotNull(attribute);
    }
}
