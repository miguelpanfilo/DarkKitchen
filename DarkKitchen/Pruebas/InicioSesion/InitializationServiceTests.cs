using AlmacenamientoAbstracciones;
using Dominio;
using LogicaDeNegocio;
using LogicaDeNegocio.Initialization;
using Moq;

namespace Pruebas.InicioSesion;

[TestClass]
public class InitializationServiceTests
{
    private Mock<IRepositoryUser> _repositoryUserMock = null!;
    private InitializationService _service = null!;
    private User? _addedUser;

    [TestInitialize]
    public void TestInitialize()
    {
        _repositoryUserMock = new Mock<IRepositoryUser>(MockBehavior.Strict);
        _service = new InitializationService(_repositoryUserMock.Object);
        _addedUser = null;
    }

    [TestMethod]
    public void EnsureInitialized_WhenAdminDoesNotExist_CreatesAdminUser()
    {
        _repositoryUserMock
            .Setup(x => x.FindByEmail("admin@gmail.com"))
            .Returns((User?)null);

        _repositoryUserMock
            .Setup(x => x.Add(It.IsAny<User>()))
            .Returns((User user) => user);

        _service.EnsureInitialized();

        _repositoryUserMock.Verify(
            x => x.FindByEmail("admin@gmail.com"),
            Times.Once);

        _repositoryUserMock.Verify(
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

        _repositoryUserMock
            .Setup(x => x.FindByEmail("admin@gmail.com"))
            .Returns(existingUserResult.Value!);

        _service.EnsureInitialized();

        _repositoryUserMock.Verify(
            x => x.FindByEmail("admin@gmail.com"),
            Times.Once);

        _repositoryUserMock.Verify(
            x => x.Add(It.IsAny<User>()),
            Times.Never);
    }

    [TestMethod]
    public void EnsureInitialized_WhenAdminDoesNotExist_AddsUserWithExpectedEmail()
    {
        _repositoryUserMock
            .Setup(x => x.FindByEmail("admin@gmail.com"))
            .Returns((User?)null);

        _repositoryUserMock
            .Setup(x => x.Add(It.IsAny<User>()))
            .Callback<User>(user => _addedUser = user)
            .Returns((User user) => user);

        _service.EnsureInitialized();

        Assert.IsNotNull(_addedUser);
        Assert.AreEqual("admin@gmail.com", _addedUser.Email.ToString());
    }

    [TestMethod]
    public void EnsureInitialized_WhenAdminDoesNotExist_AddsAdministrativeUser()
    {
        _repositoryUserMock
            .Setup(x => x.FindByEmail("admin@gmail.com"))
            .Returns((User?)null);

        _repositoryUserMock
            .Setup(x => x.Add(It.IsAny<User>()))
            .Callback<User>(user => _addedUser = user)
            .Returns((User user) => user);

        _service.EnsureInitialized();

        Assert.IsNotNull(_addedUser);
        Assert.AreEqual(UserRoles.Administrative, _addedUser.Role);
    }

    [TestMethod]
    public void EnsureInitialized_WhenAdminDoesNotExist_AddsUserWithExpectedNameAndLastName()
    {
        _repositoryUserMock
            .Setup(x => x.FindByEmail("admin@gmail.com"))
            .Returns((User?)null);

        _repositoryUserMock
            .Setup(x => x.Add(It.IsAny<User>()))
            .Callback<User>(user => _addedUser = user)
            .Returns((User user) => user);

        _service.EnsureInitialized();

        Assert.IsNotNull(_addedUser);
        Assert.AreEqual("Admin", _addedUser.Name);
        Assert.AreEqual("System", _addedUser.LastName);
    }

    [TestMethod]
    public void EnsureInitialized_WhenCalledTwice_DoesNotAddAdminTwice()
    {
        User? existingUser = null;

        _repositoryUserMock
            .SetupSequence(x => x.FindByEmail("admin@gmail.com"))
            .Returns((User?)null)
            .Returns(() => existingUser);

        _repositoryUserMock
            .Setup(x => x.Add(It.IsAny<User>()))
            .Callback<User>(user => existingUser = user)
            .Returns((User user) => user);

        _service.EnsureInitialized();
        _service.EnsureInitialized();

        _repositoryUserMock.Verify(
            x => x.Add(It.IsAny<User>()),
            Times.Once);
    }
}
