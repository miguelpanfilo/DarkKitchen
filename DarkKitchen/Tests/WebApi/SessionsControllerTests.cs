using BusinessLogicsAbstractions;
using Contract;
using DarkKitchen.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Tests.WebApi;

[TestClass]
public class SessionsControllerTests
{
    [TestMethod]
    public void Login_ValidRequest_ReturnsOk()
    {
        var loginServiceMock = new Mock<ILoginService>(MockBehavior.Strict);

        var request = new LoginRequestDTO
        {
            Email = "admin@gmail.com",
            Password = "Password@123Abc"
        };

        var expectedResponse = new LoginResponseDTO
        {
            SessionId = Guid.NewGuid(),
            UserId = 1,
            Role = "Administrative",
            ExpireDate = new DateTime(2026, 4, 6, 18, 0, 0)
        };

        loginServiceMock.Setup(s => s.Login(request)).Returns(expectedResponse);

        var controller = new SessionsController(loginServiceMock.Object);

        var result = controller.CreateSession(request);

        var okResult = result as OkObjectResult;

        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);

        var value = okResult.Value as LoginResponseDTO;
        Assert.IsNotNull(value);
        Assert.AreEqual(1, value.UserId);
        Assert.AreEqual("Administrative", value.Role);

        loginServiceMock.Verify(s => s.Login(request), Times.Once);
    }

    [TestMethod]
    public void CreateSession_InvalidCredentials_ThrowsUserAuthenticationException()
    {
        var loginServiceMock = new Mock<ILoginService>(MockBehavior.Strict);

        var request = new LoginRequestDTO
        {
            Email = "admin@gmail.com",
            Password = "wrongPassword"
        };

        loginServiceMock
            .Setup(s => s.Login(request))
            .Throws(new UserAuthenticationException("Invalid credentials"));

        var controller = new SessionsController(loginServiceMock.Object);

        var ex = Assert.ThrowsException<UserAuthenticationException>(() =>
            controller.CreateSession(request));

        Assert.AreEqual("Invalid credentials", ex.Message);

        loginServiceMock.Verify(s => s.Login(request), Times.Once);
    }

    [TestMethod]
    public void CreateSession_ValidRequest_CallsLoginServiceOnce()
    {
        var loginServiceMock = new Mock<ILoginService>(MockBehavior.Strict);

        var request = new LoginRequestDTO
        {
            Email = "admin@gmail.com",
            Password = "Password@123Abc"
        };

        var expectedResponse = new LoginResponseDTO
        {
            SessionId = Guid.NewGuid(),
            UserId = 1,
            Role = "Administrative",
            ExpireDate = new DateTime(2026, 4, 6, 18, 0, 0)
        };

        loginServiceMock.Setup(s => s.Login(request)).Returns(expectedResponse);

        var controller = new SessionsController(loginServiceMock.Object);

        controller.CreateSession(request);

        loginServiceMock.Verify(s => s.Login(request), Times.Once);
    }
}
