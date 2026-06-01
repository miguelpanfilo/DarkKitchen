using BusinessLogics;
using BusinessLogicsAbstractions;
using Contract.Users;
using DarkKitchen.Controllers;
using DataAccessAbstractions;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Tests.WebApi;

[TestClass]
public class SessionsControllerTests
{
    private static Phone CreateValidPhone(string number)
    {
        var phoneResult = Phone.TryMakePhone(number);
        Assert.IsTrue(phoneResult.Success);
        return phoneResult.Value!;
    }

    private static User CreateValidUser(string email, string password)
    {
        var phone = CreateValidPhone("099123456");

        var userResult = User.MakeClient(
            "Miguel",
            "Lopez",
            email,
            phone,
            password);

        Assert.IsTrue(userResult.Success);
        return userResult.Value!;
    }

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
        var repoUserMock = new Mock<IRepositoryUser>(MockBehavior.Strict);
        var repoSessionMock = new Mock<IRepositorySession>(MockBehavior.Strict);
        var watchMock = new Mock<IWatch>(MockBehavior.Strict);

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

        var ex = Assert.ThrowsException<UserAuthenticationException>(() =>
            service.Login(request));

        Assert.AreEqual("Credenciales inválidas", ex.Message);

        repoUserMock.Verify(r => r.FindByEmail("miguel@gmail.com"), Times.Once);
        repoSessionMock.Verify(r => r.Add(It.IsAny<Session>()), Times.Never);
    }

    [TestMethod]
    public void Login_WrongPassword_ThrowsAuthenticationException()
    {
        var repoUserMock = new Mock<IRepositoryUser>(MockBehavior.Strict);
        var repoSessionMock = new Mock<IRepositorySession>(MockBehavior.Strict);
        var watchMock = new Mock<IWatch>(MockBehavior.Strict);

        var user = CreateValidUser("miguel@gmail.com", "Password@2Ok6Abc");
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

        var ex = Assert.ThrowsException<UserAuthenticationException>(() =>
            service.Login(request));

        Assert.AreEqual("Credenciales inválidas", ex.Message);

        repoUserMock.Verify(r => r.FindByEmail("miguel@gmail.com"), Times.Once);
        repoSessionMock.Verify(r => r.Add(It.IsAny<Session>()), Times.Never);
    }

    [TestMethod]
    public void Login_ValidCredentials_ReturnsLoginResponseDto()
    {
        var repoUserMock = new Mock<IRepositoryUser>(MockBehavior.Strict);
        var repoSessionMock = new Mock<IRepositorySession>(MockBehavior.Strict);
        var watchMock = new Mock<IWatch>(MockBehavior.Strict);

        var fixedNow = new DateTime(2026, 1, 1, 10, 0, 0);

        watchMock
            .Setup(r => r.Now())
            .Returns(fixedNow);

        var user = CreateValidUser("miguel@gmail.com", "Password@2Ok6Abc");
        user.Id = 25;

        repoUserMock
            .Setup(r => r.FindByEmail("miguel@gmail.com"))
            .Returns(user);

        repoSessionMock
            .Setup(r => r.RemoveAll());

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

        repoUserMock.Verify(r => r.FindByEmail("miguel@gmail.com"), Times.Once);
        watchMock.Verify(r => r.Now(), Times.Once);
        repoSessionMock.Verify(r => r.RemoveAll(), Times.Once);
        repoSessionMock.Verify(r => r.Add(It.IsAny<Session>()), Times.Once);
    }

    [TestMethod]
    public void CreateSession_HasAllowAnonymousAttribute()
    {
        var method = typeof(SessionsController)
            .GetMethod("CreateSession");

        var hasAttribute = method!
            .GetCustomAttributes(typeof(AllowAnonymousAttribute), false)
            .Any();

        Assert.IsTrue(hasAttribute);
    }
}
