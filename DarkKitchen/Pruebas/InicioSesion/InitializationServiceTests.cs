using AlmacenamientoAbstracciones;
using Dominio;
using Moq;

namespace Pruebas.InicioSesion;

[TestClass]
public class InitializationServiceTests
{
    [TestMethod]
    public void EnsureInitialized_WhenAdminDoesNotExist_CreatesAdminUser()
    {
        var repositoryUserMock = new Mock<IRepositoryUser>(MockBehavior.Strict);

        repositoryUserMock
            .Setup(x => x.FindByEmail("admin@gmail.com"))
            .Returns((User?)null);

        repositoryUserMock
            .Setup(x => x.Add(It.IsAny<User>()));

        var service = new InitializationService(repositoryUserMock.Object);

        service.EnsureInitialized();

        repositoryUserMock.Verify(
            x => x.FindByEmail("admin@gmail.com"),
            Times.Once);

        repositoryUserMock.Verify(
            x => x.Add(It.IsAny<User>()),
            Times.Once);
    }
}
