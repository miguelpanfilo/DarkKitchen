using BusinessLogicsAbstractions;
using Contract.Products;
using Contract.Promotions;
using Contract.Users;
using DarkKitchen.Controllers;
using DarkKitchen.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Tests.WebApi;

[TestClass]
public class UpdatePromotionsControllerTests
{
    private Mock<IPromotionService> _promotionServiceMock = null!;
    private PromotionsController _controller = null!;

    [TestInitialize]
    public void Initialize()
    {
        _promotionServiceMock = new Mock<IPromotionService>(MockBehavior.Strict);
        _controller = new PromotionsController(_promotionServiceMock.Object);
    }

    private void SetCurrentUser(string email)
    {
        var claims = new List<System.Security.Claims.Claim>
        {
            new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Email,
                email)
        };
        var httpContext = new DefaultHttpContext
        {
            User = new System.Security.Claims.ClaimsPrincipal(
                new System.Security.Claims.ClaimsIdentity(claims))
        };
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
    public void Update_WhenAdmin_ShouldReturn200()
    {
        var request = new PromotionPatchDto
        {
            Name = "Cyber Monday",
            DiscountPercentage = 25,
            StartDate = new DateOnly(2026, 11, 10),
            EndDate = new DateOnly(2026, 11, 30)
        };

        var response = new PromotionResponseDto(
            1,
            "Cyber Monday",
            25,
            new DateOnly(2026, 11, 10),
            new DateOnly(2026, 11, 30),
            new List<ProductSummaryDto>
            {
                new (5, "Hamburguesa Clásica")
            }
        );

        _promotionServiceMock
            .Setup(x => x.Update(1, request))
            .Returns(response);

        SetCurrentUser("admin@test.com");
        var result = _controller.Update(1, request) as OkObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(StatusCodes.Status200OK, result.StatusCode);
        Assert.AreEqual(response, result.Value);

        _promotionServiceMock.Verify(x => x.Update(1, request),
            Times.Once);
    }

    [TestMethod]
    public void Update_HasHttpPatchAttributeWithIdTemplate()
    {
        var method = typeof(PromotionsController).GetMethod(nameof(PromotionsController.Update));

        Assert.IsNotNull(method);

        var attribute = method.GetCustomAttributes(typeof(HttpPatchAttribute), false)
            .Cast<HttpPatchAttribute>()
            .SingleOrDefault();

        Assert.IsNotNull(attribute);
        Assert.AreEqual("{id}", attribute.Template);
    }

    [TestMethod]
    public void Update_HasRoleAuthorizationAttribute()
    {
        var method = typeof(PromotionsController).GetMethod(nameof(PromotionsController.Update));

        Assert.IsNotNull(method);

        var attribute = method.GetCustomAttributes(typeof(RoleAuthorizationAttribute), false)
            .Cast<RoleAuthorizationAttribute>()
            .SingleOrDefault();

        Assert.IsNotNull(attribute);
    }
}
