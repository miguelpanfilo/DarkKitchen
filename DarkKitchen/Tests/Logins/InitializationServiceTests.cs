using BusinessLogics.Initialization;
using DataAccessAbstractions;
using Domain;
using Moq;

namespace Tests.Logins;

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

        _service = new InitializationService(
            _repositoryUserMock.Object);

        _addedUser = null;
    }

    [TestMethod]
    public void EnsureInitialized_WhenAdminDoesNotExist_CreatesAdminUser()
    {
        _repositoryUserMock
            .Setup(x => x.EmailExist("admin@gmail.com"))
            .Returns(false);

        _repositoryUserMock
            .Setup(x => x.Add(It.IsAny<User>()))
            .Returns((User user) => user);

        _service.EnsureInitialized();

        _repositoryUserMock.Verify(
            x => x.EmailExist("admin@gmail.com"),
            Times.Once);

        _repositoryUserMock.Verify(
            x => x.Add(It.IsAny<User>()),
            Times.Once);
    }

    [TestMethod]
    public void EnsureInitialized_WhenAdminAlreadyExists_DoesNotCreateAdminUser()
    {
        _repositoryUserMock
            .Setup(x => x.EmailExist("admin@gmail.com"))
            .Returns(true);

        _service.EnsureInitialized();

        _repositoryUserMock.Verify(
            x => x.EmailExist("admin@gmail.com"),
            Times.Once);

        _repositoryUserMock.Verify(
            x => x.Add(It.IsAny<User>()),
            Times.Never);
    }

    [TestMethod]
    public void MakeIntern_WhenRoleIsClient_ShouldReturnFail()
    {
        var phone = Phone.TryMakePhone("099123456").Value!;

        var result = User.MakeIntern(
            "Miguel",
            "Lopez",
            "miguel@gmail.com",
            phone,
            "PasswordSeguraee3!",
            UserRoles.Client);

        Assert.IsFalse(result.Success);
        Assert.AreEqual("El rol no es válido para usuario interno", result.ErrorMessage);
    }

    [TestMethod]
    public void EnsureInitialized_WhenAdminDoesNotExist_AddsUserWithExpectedEmail()
    {
        _repositoryUserMock
            .Setup(x => x.EmailExist("admin@gmail.com"))
            .Returns(false);

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
            .Setup(x => x.EmailExist("admin@gmail.com"))
            .Returns(false);

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
            .Setup(x => x.EmailExist("admin@gmail.com"))
            .Returns(false);

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
        _repositoryUserMock
            .SetupSequence(x => x.EmailExist("admin@gmail.com"))
            .Returns(false)
            .Returns(true);

        _repositoryUserMock
            .Setup(x => x.Add(It.IsAny<User>()))
            .Returns((User user) => user);

        _service.EnsureInitialized();
        _service.EnsureInitialized();

        _repositoryUserMock.Verify(
            x => x.EmailExist("admin@gmail.com"),
            Times.Exactly(2));

        _repositoryUserMock.Verify(
            x => x.Add(It.IsAny<User>()),
            Times.Once);
    }
}
