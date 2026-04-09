using AlmacenamientoAbstracciones;
using Dominio;
using LogicaDeNegocio;
using LogicaDeNegocioAbstracciones.Exceptions;
using Moq;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace Pruebas.Usuarios;

[TestClass]
public class AdminValidationServiceTests
{
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
        var repoMock = new Mock<IRepositoryUser>(MockBehavior.Strict);

        var user = CreateAdministrativeUser(5);

        repoMock
            .Setup(r => r.FindById(5))
            .Returns(user);

        var service = new AdminValidationService(repoMock.Object);

        var result = service.GetExistingUser(5);

        IsNotNull(result);
        AreEqual(5, result.Id);
        AreEqual("Miguel", result.Name);
    }

    [TestMethod]
    public void GetExistingUser_UserDoesNotExist_ThrowsExceptionWithCorrectMessage()
    {
        var repoMock = new Mock<IRepositoryUser>(MockBehavior.Strict);

        repoMock
            .Setup(r => r.FindById(99))
            .Returns((User?)null);

        var service = new AdminValidationService(repoMock.Object);

        var ex = ThrowsException<NotFoundException>(() => service.GetExistingUser(99));

        AreEqual("User does not exist", ex.Message);
    }

    [TestMethod]
    public void EnsureUserIsAdmin_UserIsAdministrative_DoesNotThrowException()
    {
        var repoMock = new Mock<IRepositoryUser>(MockBehavior.Strict);

        var user = CreateAdministrativeUser(5);

        repoMock
            .Setup(r => r.FindById(5))
            .Returns(user);

        var service = new AdminValidationService(repoMock.Object);

        service.EnsureUserIsAdmin(5);
    }

    [TestMethod]
    public void EnsureUserIsAdmin_UserIsNotAdministrative_ThrowsExceptionWithCorrectMessage()
    {
        var repoMock = new Mock<IRepositoryUser>(MockBehavior.Strict);

        var user = CreateClientUser(5);

        repoMock
            .Setup(r => r.FindById(5))
            .Returns(user);

        var service = new AdminValidationService(repoMock.Object);

        var ex = ThrowsException<ForbiddenException>(() =>
            service.EnsureUserIsAdmin(5));

        AreEqual("Only an administrative user can perform this action", ex.Message);
    }

    [TestMethod]
    public void EnsureUserIsAdmin_UserDoesNotExist_ThrowsExceptionWithCorrectMessage()
    {
        var repoMock = new Mock<IRepositoryUser>(MockBehavior.Strict);

        repoMock
            .Setup(r => r.FindById(5))
            .Returns((User?)null);

        var service = new AdminValidationService(repoMock.Object);

        var ex = ThrowsException<NotFoundException>(() =>
            service.EnsureUserIsAdmin(5));

        AreEqual("User does not exist", ex.Message);
    }

    [TestMethod]
    public void EnsureIsNotSelfAction_SameUser_ThrowsException()
    {
        var repoMock = new Mock<IRepositoryUser>(MockBehavior.Strict);
        var service = new AdminValidationService(repoMock.Object);

        var ex = ThrowsException<ConflictException>(() =>
            service.EnsureIsNotSelfAction(5, 5, "A user cannot modify itself"));

        AreEqual("A user cannot modify itself", ex.Message);
    }

    [TestMethod]
    public void EnsureIsNotSelfAction_DifferentUsers_DoesNotThrowException()
    {
        var repoMock = new Mock<IRepositoryUser>(MockBehavior.Strict);
        var service = new AdminValidationService(repoMock.Object);

        service.EnsureIsNotSelfAction(10, 5, "A user cannot modify itself");

        IsTrue(true);
    }
}
