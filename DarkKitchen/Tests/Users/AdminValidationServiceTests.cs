using BusinessLogics.Users;
using BusinessLogicsAbstractions.Exceptions;
using DataAccessAbstractions;
using Domain;
using Moq;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace Tests.Users;

[TestClass]
public class AdminValidationServiceTests
{
    private Mock<IRepositoryUser> _repoMock = null!;
    private AdminValidationService _service = null!;

    [TestInitialize]
    public void Initialize()
    {
        _repoMock = new Mock<IRepositoryUser>(MockBehavior.Strict);
        _service = new AdminValidationService(_repoMock.Object);
    }

    private static Phone CreateValidPhone()
    {
        var phoneResult = Phone.TryMakePhone("099123456");
        IsTrue(phoneResult.Success);
        IsNotNull(phoneResult.Value);
        return phoneResult.Value!;
    }

    private static User CreateAdministrativeUser(int id = 5)
    {
        var phone = CreateValidPhone();

        var userResult = User.MakeIntern(
            "Miguel",
            "Lopez",
            "miguel@gmail.com",
            phone,
            "PasswordSeguraee3!",
            UserRoles.Administrative);

        IsTrue(userResult.Success);
        IsNotNull(userResult.Value);

        var user = userResult.Value!;
        user.Id = id;
        return user;
    }

    private static User CreateClientUser(int id = 5)
    {
        var phone = CreateValidPhone();

        var userResult = User.MakeClient(
            "Miguel",
            "Lopez",
            "miguel@gmail.com",
            phone,
            "PasswordSeguraee3!");

        IsTrue(userResult.Success);
        IsNotNull(userResult.Value);

        var user = userResult.Value!;
        user.Id = id;
        return user;
    }

    [TestMethod]
    public void GetExistingUser_UserExists_ReturnsUser()
    {
        var user = CreateAdministrativeUser(5);

        _repoMock
            .Setup(r => r.FindById(5))
            .Returns(user);

        var result = _service.GetExistingUser(5);

        IsNotNull(result);
        AreEqual(5, result.Id);
        AreEqual("Miguel", result.Name);
    }

    [TestMethod]
    public void GetExistingUser_UserDoesNotExist_ThrowsExceptionWithCorrectMessage()
    {
        _repoMock
            .Setup(r => r.FindById(99))
            .Returns((User?)null);

        var ex = ThrowsException<NotFoundException>(() => _service.GetExistingUser(99));

        AreEqual("El usuario no existe", ex.Message);
    }

    [TestMethod]
    public void EnsureUserIsAdmin_UserIsAdministrative_DoesNotThrowException()
    {
        var user = CreateAdministrativeUser(5);

        _repoMock
            .Setup(r => r.FindById(5))
            .Returns(user);

        _service.EnsureUserIsAdmin(5);
    }

    [TestMethod]
    public void EnsureUserIsAdmin_UserIsNotAdministrative_ThrowsExceptionWithCorrectMessage()
    {
        var user = CreateClientUser(5);

        _repoMock
            .Setup(r => r.FindById(5))
            .Returns(user);

        var ex = ThrowsException<ForbiddenException>(() =>
            _service.EnsureUserIsAdmin(5));

        AreEqual("Solo un usuario administrador puede realizar esta acción", ex.Message);
    }

    [TestMethod]
    public void EnsureUserIsAdmin_UserDoesNotExist_ThrowsExceptionWithCorrectMessage()
    {
        _repoMock
            .Setup(r => r.FindById(5))
            .Returns((User?)null);

        var ex = ThrowsException<NotFoundException>(() =>
            _service.EnsureUserIsAdmin(5));

        AreEqual("El usuario no existe", ex.Message);
    }

    [TestMethod]
    public void EnsureIsNotSelfAction_SameUser_ThrowsException()
    {
        var ex = ThrowsException<ConflictException>(() =>
            _service.EnsureIsNotSelfAction(5, 5, "A user cannot modify itself"));

        AreEqual("A user cannot modify itself", ex.Message);
    }

    [TestMethod]
    public void EnsureIsNotSelfAction_DifferentUsers_DoesNotThrowException()
    {
        _service.EnsureIsNotSelfAction(10, 5, "A user cannot modify itself");

        IsTrue(true);
    }
}
