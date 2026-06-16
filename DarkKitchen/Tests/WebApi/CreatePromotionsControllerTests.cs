using BusinessLogicsAbstractions;
using Contract;
using DarkKitchen.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Tests.WebApi;

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

    private void SetCurrentUser(string email)
    {
        var claims = new List<System.Security.Claims.Claim>
        {
            new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Email, email)
        };
        var httpContext = new DefaultHttpContext
        {
            User = new System.Security.Claims.ClaimsPrincipal(
                new System.Security.Claims.ClaimsIdentity(claims))
        };
        _controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
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
            new List<ProductSummaryDto> { new(5, "Hamburguesa Clásica") }
        );

        _promotionServiceMock.Setup(x => x.Create(request)).Returns(response);

        SetCurrentUser("admin@test.com");

        var result = _controller.Create(request) as CreatedResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(StatusCodes.Status201Created, result.StatusCode);
        Assert.AreEqual(response, result.Value);

        _promotionServiceMock.Verify(x => x.Create(request), Times.Once);
    }
}
