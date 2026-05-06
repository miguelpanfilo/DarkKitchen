using BusinessLogics.Users;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Security.Claims;

namespace Tests.Users;

[TestClass]
public class CurrentUserServiceTests
{
    [TestMethod]
    public void GetEmail_WhenUserIsLoggedIn_ReturnsEmail()
    {
        var httpContextMock = new Mock<IHttpContextAccessor>();
        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.Email, "admin@dark.com")
        }));

        httpContextMock
            .Setup(h => h.HttpContext)
            .Returns(new DefaultHttpContext { User = claimsPrincipal });

        var service = new CurrentUserService(httpContextMock.Object);

        var result = service.GetEmail();

        Assert.AreEqual("admin@dark.com", result);
    }
}
