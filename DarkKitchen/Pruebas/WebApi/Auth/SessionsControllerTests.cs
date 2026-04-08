using AlmacenamientoAbstracciones;
using Contratos;
using DarkKitchen.Controllers;
using Dominio;
using Dominio.Exceptions;
using LogicaDeNegocio;
using LogicaDeNegocioAbstracciones;
using LogicaDeNegocioAbstracciones.Exceptions;
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
            .Throws(new UserAuthenticationException("Invalid credentials"));

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

        var ex = Assert.ThrowsException<UserAuthenticationException>(() => service.Login(request));

        Assert.AreEqual("Invalid credentials", ex.Message);
        repoSessionMock.Verify(r => r.Add(It.IsAny<Session>()), Times.Never);
    }

    [TestMethod]
    public void Login_WrongPassword_ThrowsAuthenticationException()
    {
        var repoUserMock = new Mock<IRepositoryUser>();
        var repoSessionMock = new Mock<IRepositorySession>();
        var watchMock = new Mock<IReloj>();

        var user = User.MakeClient(
            "Miguel",
            "Lopez",
            "miguel@gmail.com",
            Phone.MakePhone("099123456"),
            "Password@2Ok6Abc");
        user.Id = 1;

        repoUserMock
            .Setup(r => r.FindByEmail("miguel@gmail.com"))
            .Returns(user);

        var service = new LoginService(
            repoUserMock.Object,
            repoSessionMock.Object,
            watchMock.Object);

        var request = new LoginRequestDTO
        {
            Email = "miguel@gmail.com",
            Password = "WrongPassword@123"
        };

        var ex = Assert.ThrowsException<UserAuthenticationException>(() => service.Login(request));

        Assert.AreEqual("Invalid credentials", ex.Message);
        repoSessionMock.Verify(r => r.Add(It.IsAny<Session>()), Times.Never);
    }

    [TestMethod]
    public void Login_ValidCredentials_ReturnsLoginResponseDto()
    {
        var repoUserMock = new Mock<IRepositoryUser>();
        var repoSessionMock = new Mock<IRepositorySession>();
        var watchMock = new Mock<IReloj>();

        var fixedNow = new DateTime(2026, 1, 1, 10, 0, 0);

        watchMock
            .Setup(r => r.Ahora())
            .Returns(fixedNow);

        var user = User.MakeClient(
            "Miguel",
            "Lopez",
            "miguel@gmail.com",
            Phone.MakePhone("099123456"),
            "Password@2Ok6Abc");
        user.Id = 25;

        repoUserMock
            .Setup(r => r.FindByEmail("miguel@gmail.com"))
            .Returns(user);

        repoSessionMock
            .Setup(r => r.Add(It.IsAny<Session>()))
            .Returns((Session s) => s);

        var service = new LoginService(
            repoUserMock.Object,
            repoSessionMock.Object,
            watchMock.Object);

        var request = new LoginRequestDTO
        {
            Email = "miguel@gmail.com",
            Password = "Password@2Ok6Abc"
        };

        var result = service.Login(request);

        Assert.IsNotNull(result);
        Assert.AreEqual(25, result.UserId);
        Assert.AreEqual("Client", result.Role);
        Assert.AreEqual(fixedNow.AddHours(2), result.ExpireDate);
        Assert.AreNotEqual(Guid.Empty, result.SessionId);
    }

    [TestMethod]
    public void Login_ValidCredentials_SetsExpirationTwoHoursLater()
    {
        var repoUserMock = new Mock<IRepositoryUser>();
        var repoSessionMock = new Mock<IRepositorySession>();
        var watchMock = new Mock<IReloj>();

        var fixedNow = new DateTime(2026, 1, 1, 10, 0, 0);

        watchMock
            .Setup(r => r.Ahora())
            .Returns(fixedNow);

        var user = User.MakeClient(
            "Miguel",
            "Lopez",
            "miguel@gmail.com",
            Phone.MakePhone("099123456"),
            "Password@2Ok6Abc");
        user.Id = 1;

        Session? savedSession = null;

        repoUserMock
            .Setup(r => r.FindByEmail("miguel@gmail.com"))
            .Returns(user);

        repoSessionMock
            .Setup(r => r.Add(It.IsAny<Session>()))
            .Callback<Session>(s => savedSession = s)
            .Returns((Session s) => s);

        var service = new LoginService(
            repoUserMock.Object,
            repoSessionMock.Object,
            watchMock.Object);

        var request = new LoginRequestDTO
        {
            Email = "miguel@gmail.com",
            Password = "Password@2Ok6Abc"
        };

        service.Login(request);

        Assert.IsNotNull(savedSession);
        Assert.AreEqual(fixedNow.AddHours(2), savedSession.ExpireDate);
    }
}
