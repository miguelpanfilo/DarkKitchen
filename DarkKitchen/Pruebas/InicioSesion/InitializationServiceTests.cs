using AlmacenamientoAbstracciones;
using Dominio;
using LogicaDeNegocio;
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
            .Setup(x => x.Add(It.IsAny<User>()))
            .Returns((User user) => user);

        var service = new InitializationService(repositoryUserMock.Object);

        service.EnsureInitialized();

        repositoryUserMock.Verify(
            x => x.FindByEmail("admin@gmail.com"),
            Times.Once);

        repositoryUserMock.Verify(
            x => x.Add(It.IsAny<User>()),
            Times.Once);
    }

    [TestMethod]
    public void EnsureInitialized_WhenAdminAlreadyExists_DoesNotCreateAdminUser()
    {
        var phoneResult = Phone.TryMakePhone("099123456");
        Assert.IsTrue(phoneResult.Success);

        var existingUserResult = User.MakeIntern(
            "Admin",
            "System",
            "admin@gmail.com",
            phoneResult.Value!,
            "PasswordSeguraee3!",
            UserRoles.Administrative);

        Assert.IsTrue(existingUserResult.Success);

        var repositoryUserMock = new Mock<IRepositoryUser>(MockBehavior.Strict);

        repositoryUserMock
            .Setup(x => x.FindByEmail("admin@gmail.com"))
            .Returns(existingUserResult.Value!);

        var service = new InitializationService(repositoryUserMock.Object);

        service.EnsureInitialized();

        repositoryUserMock.Verify(
            x => x.FindByEmail("admin@gmail.com"),
            Times.Once);

        repositoryUserMock.Verify(
            x => x.Add(It.IsAny<User>()),
            Times.Never);
    }

    [TestMethod]
    public void EnsureInitialized_WhenAdminDoesNotExist_AddsUserWithExpectedEmail()
    {
        User? addedUser = null;

        var repositoryUserMock = new Mock<IRepositoryUser>(MockBehavior.Strict);

        repositoryUserMock
            .Setup(x => x.FindByEmail("admin@gmail.com"))
            .Returns((User?)null);

        repositoryUserMock
            .Setup(x => x.Add(It.IsAny<User>()))
            .Callback<User>(user => addedUser = user)
            .Returns((User user) => user);

        var service = new InitializationService(repositoryUserMock.Object);

        service.EnsureInitialized();

        Assert.IsNotNull(addedUser);
        Assert.AreEqual("admin@gmail.com", addedUser.Email.ToString());
    }

    [TestMethod]
    public void EnsureInitialized_WhenAdminDoesNotExist_AddsAdministrativeUser()
    {
        User? addedUser = null;

        var repositoryUserMock = new Mock<IRepositoryUser>(MockBehavior.Strict);

        repositoryUserMock
            .Setup(x => x.FindByEmail("admin@gmail.com"))
            .Returns((User?)null);

        repositoryUserMock
            .Setup(x => x.Add(It.IsAny<User>()))
            .Callback<User>(user => addedUser = user)
            .Returns((User user) => user);

        var service = new InitializationService(repositoryUserMock.Object);

        service.EnsureInitialized();

        Assert.IsNotNull(addedUser);
        Assert.AreEqual(UserRoles.Administrative, addedUser.Role);
    }

    [TestMethod]
    public void EnsureInitialized_WhenAdminDoesNotExist_AddsUserWithExpectedNameAndLastName()
    {
        User? addedUser = null;

        var repositoryUserMock = new Mock<IRepositoryUser>(MockBehavior.Strict);

        repositoryUserMock
            .Setup(x => x.FindByEmail("admin@gmail.com"))
            .Returns((User?)null);

        repositoryUserMock
            .Setup(x => x.Add(It.IsAny<User>()))
            .Callback<User>(user => addedUser = user)
            .Returns((User user) => user);

        var service = new InitializationService(repositoryUserMock.Object);

        service.EnsureInitialized();

        Assert.IsNotNull(addedUser);
        Assert.AreEqual("Admin", addedUser.Name);
        Assert.AreEqual("System", addedUser.LastName);
    }
}
