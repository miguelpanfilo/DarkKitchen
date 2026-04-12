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
    public void ValidateSession_WhenSessionIsValid_CallsFindByIdWithSessionUserId()
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

        service.ValidateSession(sessionId);

        userRepositoryMock.Verify(x => x.FindById(5), Times.Once);
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
}
