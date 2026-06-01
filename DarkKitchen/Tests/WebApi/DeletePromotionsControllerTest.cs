using BusinessLogicsAbstractions;
using DarkKitchen.Controllers;
using DarkKitchen.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Tests.WebApi;
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

   [TestMethod]
    public void Delete_WhenAdmin_ShouldReturn204()
    {
        _promotionServiceMock
            .Setup(x => x.Delete(1));

        SetCurrentUser("admin@test.com");
        var result = _controller.Delete(1) as NoContentResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(StatusCodes.Status204NoContent, result.StatusCode);

        _promotionServiceMock.Verify(x => x.Delete(1), Times.Once);
    }

    [TestMethod]
    public void Delete_HasHttpDeleteAttributeWithIdTemplate()
    {
        var method = typeof(PromotionsController).GetMethod(nameof(PromotionsController.Delete));

        Assert.IsNotNull(method);

        var attribute = method.GetCustomAttributes(typeof(HttpDeleteAttribute), false)
            .Cast<HttpDeleteAttribute>()
            .SingleOrDefault();

        Assert.IsNotNull(attribute);
        Assert.AreEqual("{id}", attribute.Template);
    }

    [TestMethod]
    public void Delete_HasRoleAuthorizationAttribute()
    {
        var method = typeof(PromotionsController).GetMethod(nameof(PromotionsController.Delete));

        Assert.IsNotNull(method);

        var attribute = method.GetCustomAttributes(typeof(RoleAuthorizationAttribute), false)
            .Cast<RoleAuthorizationAttribute>()
            .SingleOrDefault();

        Assert.IsNotNull(attribute);
    }
}
