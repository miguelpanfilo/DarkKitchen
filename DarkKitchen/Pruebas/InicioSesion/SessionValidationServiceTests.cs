using AlmacenamientoAbstracciones;
using Dominio;
using LogicaDeNegocio;
using LogicaDeNegocioAbstracciones;
using LogicaDeNegocioAbstracciones.Exceptions;
using Moq;

namespace Pruebas.InicioSesion;

[TestClass]
public class SessionValidationServiceTests
{
    [TestMethod]
    public void ValidateSession_WhenSessionDoesNotExist_ThrowsException()
    {
        var sessionId = Guid.NewGuid();

        var sessionRepositoryMock = new Mock<IRepositorySession>(MockBehavior.Strict);
        var userRepositoryMock = new Mock<IRepositoryUser>(MockBehavior.Strict);
        var watchMock = new Mock<IReloj>(MockBehavior.Strict);

        sessionRepositoryMock
            .Setup(x => x.FindBySessionId(sessionId))
            .Returns((Session?)null);

        var service = new SessionValidationService(
            sessionRepositoryMock.Object,
            userRepositoryMock.Object,
            watchMock.Object);

        var ex = Assert.ThrowsException<UserAuthenticationException>(() =>
            service.ValidateSession(sessionId));

        Assert.AreEqual("La sesión no existe", ex.Message);

        sessionRepositoryMock.Verify(x => x.FindBySessionId(sessionId), Times.Once);
    }

    [TestMethod]
    public void ValidateSession_WhenSessionIsExpired_ThrowsException()
    {
        var sessionId = Guid.NewGuid();
        var now = new DateTime(2026, 4, 12, 10, 0, 0);
        var session = new Session(5, now.AddMinutes(-1));

        var sessionRepositoryMock = new Mock<IRepositorySession>(MockBehavior.Strict);
        var userRepositoryMock = new Mock<IRepositoryUser>(MockBehavior.Strict);
        var watchMock = new Mock<IReloj>(MockBehavior.Strict);

        sessionRepositoryMock
            .Setup(x => x.FindBySessionId(sessionId))
            .Returns(session);

        watchMock
            .Setup(x => x.Ahora())
            .Returns(now);

        var service = new SessionValidationService(
            sessionRepositoryMock.Object,
            userRepositoryMock.Object,
            watchMock.Object);

        var ex = Assert.ThrowsException<UserAuthenticationException>(() =>
            service.ValidateSession(sessionId));

        Assert.AreEqual("La sesión expiró", ex.Message);

        sessionRepositoryMock.Verify(x => x.FindBySessionId(sessionId), Times.Once);
        watchMock.Verify(x => x.Ahora(), Times.Once);
    }

   [TestMethod]
    public void ValidateSession_WhenSessionUserDoesNotExist_ThrowsException()
    {
        var sessionId = Guid.NewGuid();
        var now = new DateTime(2026, 4, 12, 10, 0, 0);
        var session = new Session(5, now.AddHours(1));

        var sessionRepositoryMock = new Mock<IRepositorySession>(MockBehavior.Strict);
        var userRepositoryMock = new Mock<IRepositoryUser>(MockBehavior.Strict);
        var watchMock = new Mock<IReloj>(MockBehavior.Strict);

        sessionRepositoryMock
            .Setup(x => x.FindBySessionId(sessionId))
            .Returns(session);

        watchMock
            .Setup(x => x.Ahora())
            .Returns(now);

        userRepositoryMock
            .Setup(x => x.FindById(5))
            .Returns((User?)null);

        var service = new SessionValidationService(
            sessionRepositoryMock.Object,
            userRepositoryMock.Object,
            watchMock.Object);

        var ex = Assert.ThrowsException<UserAuthenticationException>(() =>
            service.ValidateSession(sessionId));

        Assert.AreEqual("El usuario de la sesión no existe", ex.Message);

        userRepositoryMock.Verify(x => x.FindById(5), Times.Once);
    }

    [TestMethod]
    public void ValidateSession_WhenSessionIsValid_ReturnsCurrentUserDto()
    {
        var sessionId = Guid.NewGuid();
        var now = new DateTime(2026, 4, 12, 10, 0, 0);
        var session = new Session(5, now.AddHours(1));

        var phoneResult = Phone.TryMakePhone("099123456");
        Assert.IsTrue(phoneResult.Success);

        var userResult = User.MakeClient(
            "Miguel",
            "Lopez",
            "miguel@gmail.com",
            phoneResult.Value!,
            "PasswordSeguraee3!");
        Assert.IsTrue(userResult.Success);
        var user = userResult.Value!;
        user.Id = 5;

        var sessionRepo = new Mock<IRepositorySession>(MockBehavior.Strict);
        var userRepo = new Mock<IRepositoryUser>(MockBehavior.Strict);
        var watch = new Mock<IReloj>(MockBehavior.Strict);

        sessionRepo.Setup(r => r.FindBySessionId(sessionId)).Returns(session);
        watch.Setup(w => w.Ahora()).Returns(now);
        userRepo.Setup(r => r.FindById(5)).Returns(user);

        var service = new SessionValidationService(
            sessionRepo.Object,
            userRepo.Object,
            watch.Object);

        var result = service.ValidateSession(sessionId);

        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result, typeof(CurrentUserDto));

        var dto = (CurrentUserDto)result;
        Assert.AreEqual(5, dto.Id);
        Assert.AreEqual("Client", dto.Role);
        Assert.AreEqual("miguel@gmail.com", dto.Email);

        sessionRepo.Verify(r => r.FindBySessionId(sessionId), Times.Once);
        watch.Verify(w => w.Ahora(), Times.Once);
        userRepo.Verify(r => r.FindById(5), Times.Once);
    }
}
