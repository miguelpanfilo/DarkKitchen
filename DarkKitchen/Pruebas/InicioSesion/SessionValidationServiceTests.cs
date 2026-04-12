using AlmacenamientoAbstracciones;
using Dominio;
using LogicaDeNegocio;
using LogicaDeNegocioAbstracciones;
using LogicaDeNegocioAbstracciones.Exceptions;
using Moq;

namespace Pruebas.Services;

[TestClass]
public class SessionValidationServiceTests
{
    [TestMethod]
    public void ValidateSession_WhenSessionDoesNotExist_ThrowsException()
    {
        var sessionId = Guid.NewGuid();

        var sessionRepositoryMock = new Mock<IRepositorySession>(MockBehavior.Strict);

        sessionRepositoryMock
            .Setup(x => x.FindBySessionId(sessionId))
            .Returns((Session?)null);

        var service = new SessionValidationService(
            sessionRepositoryMock.Object);

        var ex = Assert.ThrowsException<UserAuthenticationException>(() =>
            service.ValidateSession(sessionId));

        Assert.AreEqual("La sesión no existe", ex.Message);

        sessionRepositoryMock.Verify(x => x.FindBySessionId(sessionId), Times.Once);
    }
}
