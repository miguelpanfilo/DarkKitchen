using AlmacenamientoAbstracciones;
using Contratos;
using Dominio;
using LogicaDeNegocio;
using LogicaDeNegocioAbstracciones;
using LogicaDeNegocioAbstracciones.Exceptions;
using Moq;

namespace Pruebas.InicioSesion;

[TestClass]
public class LoginServiceTests
{
    [TestMethod]
    public void LoginRightCredentials_ReturnSession()
    {
        var repoUsuarioMock = new Mock<IRepositoryUser>(MockBehavior.Strict);
        var repoSesionMock = new Mock<IRepositorySession>(MockBehavior.Strict);
        var relojMock = new Mock<IReloj>(MockBehavior.Strict);

        var usuario = User.MakeClient(
            "Miguel",
            "Lopez",
            "miguel@gmail.com",
            Phone.MakePhone("099123456"),
            "Password@2Ok6Abc");
        usuario.Id = 1;

        repoUsuarioMock
            .Setup(r => r.FindByEmail("miguel@gmail.com"))
            .Returns(usuario);

        repoSesionMock
            .Setup(r => r.Add(It.IsAny<Session>()))
            .Returns((Session s) => s);

        relojMock
            .Setup(r => r.Ahora())
            .Returns(DateTime.Now);

        var service = new LoginService(
            repoUsuarioMock.Object,
            repoSesionMock.Object,
            relojMock.Object);

        var request = new LoginRequestDTO
        {
            Email = "miguel@gmail.com",
            Password = "Password@2Ok6Abc"
        };

        var response = service.Login(request);

        Assert.IsNotNull(response);
        Assert.AreEqual(1, response.UserId);
        Assert.AreEqual("Client", response.Role);
        Assert.AreNotEqual(Guid.Empty, response.SessionId);
    }

    [TestMethod]
    public void Login_NoExistingEmail_Exception()
    {
        var repoUserMock = new Mock<IRepositoryUser>(MockBehavior.Strict);
        var repoSesionMock = new Mock<IRepositorySession>(MockBehavior.Strict);
        var watchMock = new Mock<IReloj>(MockBehavior.Strict);

        repoUserMock
            .Setup(r => r.FindByEmail("miguel@gmail.com"))
            .Returns((User?)null);

        var service = new LoginService(
            repoUserMock.Object,
            repoSesionMock.Object,
            watchMock.Object);

        var request = new LoginRequestDTO
        {
            Email = "miguel@gmail.com",
            Password = "Password@2Ok6Abc"
        };

        var ex = Assert.ThrowsException<AuthenticationException>(() => service.Login(request));

        Assert.AreEqual("Invalid credentials", ex.Message);

        repoSesionMock.Verify(r => r.Add(It.IsAny<Session>()), Times.Never);
    }

    [TestMethod]
    public void Login_WrongPassword_Exception()
    {
        var repoUserMock = new Mock<IRepositoryUser>(MockBehavior.Strict);
        var repoSesionMock = new Mock<IRepositorySession>(MockBehavior.Strict);
        var watchMock = new Mock<IReloj>(MockBehavior.Strict);

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
            repoSesionMock.Object,
            watchMock.Object);

        var request = new LoginRequestDTO
        {
            Email = "miguel@gmail.com",
            Password = "OtraClave@1aaaaaaaaaa"
        };

        var ex = Assert.ThrowsException<AuthenticationException>(() => service.Login(request));

        Assert.AreEqual("Invalid credentials", ex.Message);

        repoSesionMock.Verify(r => r.Add(It.IsAny<Session>()), Times.Never);
    }

    [TestMethod]
    public void Login_CredentialOk_SaveTheSession()
    {
        var repoUserMock = new Mock<IRepositoryUser>(MockBehavior.Strict);
        var repoSessionMock = new Mock<IRepositorySession>(MockBehavior.Strict);
        var watchMock = new Mock<IReloj>(MockBehavior.Strict);

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

        repoSessionMock
            .Setup(r => r.Add(It.IsAny<Session>()))
            .Returns((Session s) => s);

        watchMock
            .Setup(r => r.Ahora())
            .Returns(DateTime.Now);

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

        repoSessionMock.Verify(r => r.Add(It.IsAny<Session>()), Times.Once);
    }

    [TestMethod]
    public void Login_ValidClient_ReturnsRole()
    {
        var repoUserMock = new Mock<IRepositoryUser>(MockBehavior.Strict);
        var repoSessionMock = new Mock<IRepositorySession>(MockBehavior.Strict);
        var watchMock = new Mock<IReloj>(MockBehavior.Strict);

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

        repoSessionMock
            .Setup(r => r.Add(It.IsAny<Session>()))
            .Returns((Session s) => s);

        watchMock
            .Setup(r => r.Ahora())
            .Returns(DateTime.Now);

        var service = new LoginService(
            repoUserMock.Object,
            repoSessionMock.Object,
            watchMock.Object);

        var request = new LoginRequestDTO
        {
            Email = "miguel@gmail.com",
            Password = "Password@2Ok6Abc"
        };

        var response = service.Login(request);

        Assert.AreEqual("Client", response.Role);
    }

    [TestMethod]
    public void Login_ValidCliente_ReturnIdUser()
    {
        var repoUserMcok = new Mock<IRepositoryUser>(MockBehavior.Strict);
        var repoSessionMock = new Mock<IRepositorySession>(MockBehavior.Strict);
        var watchMock = new Mock<IReloj>(MockBehavior.Strict);

        var user = User.MakeClient(
            "Miguel",
            "Lopez",
            "miguel@gmail.com",
            Phone.MakePhone("099123456"),
            "Password@2Ok6Abc");
        user.Id = 25;

        repoUserMcok
            .Setup(r => r.FindByEmail("miguel@gmail.com"))
            .Returns(user);

        repoSessionMock
            .Setup(r => r.Add(It.IsAny<Session>()))
            .Returns((Session s) => s);

        watchMock
            .Setup(r => r.Ahora())
            .Returns(DateTime.Now);

        var service = new LoginService(
            repoUserMcok.Object,
            repoSessionMock.Object,
            watchMock.Object);

        var request = new LoginRequestDTO
        {
            Email = "miguel@gmail.com",
            Password = "Password@2Ok6Abc"
        };

        var response = service.Login(request);

        Assert.AreEqual(25, response.UserId);
    }

    [TestMethod]
    public void Login_ValidCredentials_GenerateSesssionNotNull()
    {
        var repoUserMock = new Mock<IRepositoryUser>(MockBehavior.Strict);
        var repoSessionMock = new Mock<IRepositorySession>(MockBehavior.Strict);
        var watchMock = new Mock<IReloj>(MockBehavior.Strict);

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

        repoSessionMock
            .Setup(r => r.Add(It.IsAny<Session>()))
            .Returns((Session s) => s);

        watchMock
            .Setup(r => r.Ahora())
            .Returns(DateTime.Now);

        var service = new LoginService(
            repoUserMock.Object,
            repoSessionMock.Object,
            watchMock.Object);

        var request = new LoginRequestDTO
        {
            Email = "miguel@gmail.com",
            Password = "Password@2Ok6Abc"
        };

        var response = service.Login(request);

        Assert.AreNotEqual(Guid.Empty, response.SessionId);
    }

    [TestMethod]
    public void Login_WrongPassword_NotSaveSession()
    {
        var repoUserMock = new Mock<IRepositoryUser>(MockBehavior.Strict);
        var repoSessionMock = new Mock<IRepositorySession>(MockBehavior.Strict);
        var watchMock = new Mock<IReloj>(MockBehavior.Strict);

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
            Password = "Incorrecta@123456"
        };

        Assert.ThrowsException<AuthenticationException>(() => service.Login(request));

        repoSessionMock.Verify(r => r.Add(It.IsAny<Session>()), Times.Never);
    }

    [TestMethod]
    public void Login_NotExistingEmail_NotSaveSession()
    {
        var repoUserMock = new Mock<IRepositoryUser>(MockBehavior.Strict);
        var repoSessionMock = new Mock<IRepositorySession>(MockBehavior.Strict);
        var watchMock = new Mock<IReloj>(MockBehavior.Strict);

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

        Assert.ThrowsException<AuthenticationException>(() => service.Login(request));

        repoSessionMock.Verify(r => r.Add(It.IsAny<Session>()), Times.Never);
    }

    [TestMethod]
    public void Login_ValidCredentials_ReturnExpireDateCorrect()
    {
        var repoUserMock = new Mock<IRepositoryUser>(MockBehavior.Strict);
        var repoSessionMock = new Mock<IRepositorySession>(MockBehavior.Strict);

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

        repoSessionMock
            .Setup(r => r.Add(It.IsAny<Session>()))
            .Returns((Session s) => s);

        var watch = new Mock<IReloj>(MockBehavior.Strict);

        var coordinatedDate = new DateTime(2026, 1, 1, 10, 0, 0);

        watch
            .Setup(r => r.Ahora())
            .Returns(coordinatedDate);

        var service = new LoginService(
            repoUserMock.Object,
            repoSessionMock.Object,
            watch.Object);

        var request = new LoginRequestDTO
        {
            Email = "miguel@gmail.com",
            Password = "Password@2Ok6Abc"
        };

        var response = service.Login(request);

        var expectedExpire = coordinatedDate.AddHours(2);

        Assert.AreEqual(expectedExpire, response.ExpireDate);
    }

    [TestMethod]
    public void Login_EmailWithAlternateCases_ReturnCorrectSession()
    {
        var repoUserMock = new Mock<IRepositoryUser>(MockBehavior.Strict);
        var repoSessionMock = new Mock<IRepositorySession>(MockBehavior.Strict);
        var watchMock = new Mock<IReloj>(MockBehavior.Strict);

        var user = User.MakeClient(
            "Miguel",
            "Lopez",
            "miguel@gmail.com",
            Phone.MakePhone("099123456"),
            "Password@2Ok6Abc");
        user.Id = 1;

        repoUserMock
            .Setup(r => r.FindByEmail("MIGUEL@GMAIL.COM"))
            .Returns(user);

        repoSessionMock
            .Setup(r => r.Add(It.IsAny<Session>()))
            .Returns((Session s) => s);

        watchMock
            .Setup(r => r.Ahora())
            .Returns(DateTime.Now);

        var service = new LoginService(
            repoUserMock.Object,
            repoSessionMock.Object,
            watchMock.Object);

        var request = new LoginRequestDTO
        {
            Email = "MIGUEL@GMAIL.COM",
            Password = "Password@2Ok6Abc"
        };

        var response = service.Login(request);

        Assert.IsNotNull(response);
        Assert.AreEqual(1, response.UserId);
    }

    [TestMethod]
    public void Login_ValidCredentials_SaveRightExpire()
    {
        var repoUserMock = new Mock<IRepositoryUser>(MockBehavior.Strict);
        var repoSessionMock = new Mock<IRepositorySession>(MockBehavior.Strict);
        var watchMock = new Mock<IReloj>(MockBehavior.Strict);

        var fechaFija = new DateTime(2026, 1, 1, 10, 0, 0);

        watchMock
            .Setup(r => r.Ahora())
            .Returns(fechaFija);

        var user = User.MakeClient(
            "Miguel",
            "Lopez",
            "miguel@gmail.com",
            Phone.MakePhone("099123456"),
            "Password@2Ok6Abc");
        user.Id = 1;

        Session? sesionGuardada = null;

        repoUserMock
            .Setup(r => r.FindByEmail("miguel@gmail.com"))
            .Returns(user);

        repoSessionMock
            .Setup(r => r.Add(It.IsAny<Session>()))
            .Callback<Session>(s => sesionGuardada = s)
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

        var response = service.Login(request);

        Assert.IsNotNull(sesionGuardada);
        Assert.AreEqual(sesionGuardada.ExpireDate, response.ExpireDate);
    }
}
