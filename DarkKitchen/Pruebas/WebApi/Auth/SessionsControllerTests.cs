using AlmacenamientoAbstracciones;
using Contratos;
using DarkKitchen.Controllers;
using Dominio;
using Dominio.Exceptions;
using LogicaDeNegocio;
using LogicaDeNegocioAbstracciones;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Pruebas.WebApi.Auth;

[TestClass]
public class SessionsControllerTests
{
    [TestMethod]
    public void Login_ValidRequest_ReturnsOk()
    {
        var loginServiceMock = new Mock<ILoginService>();

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

        loginServiceMock
            .Setup(s => s.Login(request))
            .Returns(expectedResponse);

        var controller = new SessionsController(loginServiceMock.Object);

        var result = controller.CreateSession(request);

        var okResult = result as OkObjectResult;

        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);

        var value = okResult.Value as LoginResponseDTO;
        Assert.IsNotNull(value);
        Assert.AreEqual(1, value.UserId);
        Assert.AreEqual("Administrative", value.Role);
    }

    [TestMethod]
    public void CreateSession_InvalidCredentials_ReturnsUnauthorized()
    {
        var loginServiceMock = new Mock<ILoginService>();

        var request = new LoginRequestDTO
        {
            Email = "admin@gmail.com",
            Password = "wrongPassword"
        };

        loginServiceMock
            .Setup(s => s.Login(request))
            .Throws(new AuthenticationException("Invalid credentials"));

        var controller = new SessionsController(loginServiceMock.Object);

        var result = controller.CreateSession(request);

        var unauthorizedResult = result as UnauthorizedObjectResult;

        Assert.IsNotNull(unauthorizedResult);
        Assert.AreEqual(401, unauthorizedResult.StatusCode);
    }

    [TestMethod]
    public void CreateSession_ValidRequest_CallsLoginServiceOnce()
    {
        var loginServiceMock = new Mock<ILoginService>();

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

        loginServiceMock
            .Setup(s => s.Login(request))
            .Returns(expectedResponse);

        var controller = new SessionsController(loginServiceMock.Object);

        controller.CreateSession(request);

        loginServiceMock.Verify(s => s.Login(request), Times.Once);
    }

    [TestMethod]
    public void Login_NonExistingEmail_ThrowsAuthenticationException()
    {
        var repoUserMock = new Mock<IRepositoryUser>();
        var repoSessionMock = new Mock<IRepositorySession>();
        var watchMock = new Mock<IReloj>();

        repoUserMock
            .Setup(r => r.FindByEmail("miguel@gmail.com"))
            .Returns((User?)null);

        var service = new LoginService(
            repoUserMock.Object,
            repoSessionMock.Object,
            watchMock.Object);

        var request = new LoginRequestDTO
        {
            Email = "miguel@gmail.com",
            Password = "Password@2Ok6Abc"
        };

        var ex = Assert.ThrowsException<AuthenticationException>(() => service.Login(request));

        Assert.AreEqual("Invalid credentials", ex.Message);
        repoSessionMock.Verify(r => r.Add(It.IsAny<Session>()), Times.Never);
    }
}
