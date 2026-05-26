using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Moq;

namespace Tests.WebApi;

[TestClass]
public class CurrentUserServiceTest
{
    [TestMethod]
    public void GetEmail_WhenUserHasEmailClaim_ReturnsEmail()
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Email, "admin@dark.com")
        };
        var httpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity(claims))
        };

        var accessorMock = new Mock<IHttpContextAccessor>(MockBehavior.Strict);
        accessorMock.Setup(a => a.HttpContext).Returns(httpContext);

        var service = new CurrentUserService(accessorMock.Object);

        var result = service.GetEmail();

        Assert.AreEqual("admin@dark.com", result);
    }
}
