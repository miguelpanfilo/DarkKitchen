using AlmacenamientoAbstracciones;
using Dominio;
using Dominio.Exceptions;
using LogicaDeNegocio;
using Moq;

namespace Pruebas.Usuarios;

[TestClass]
public class AdminValidationServiceTests
{
    [TestMethod]
    public void GetExistingUser_UserExists_ReturnsUser()
    {
        var repoMock = new Mock<IRepositoryUser>(MockBehavior.Strict);

        var user = User.MakeIntern(
            "Miguel",
            "Lopez",
            "miguel@gmail.com",
            Phone.MakePhone("099123456"),
            "PasswordSeguraee3!",
            UserRoles.Administrative);
        user.Id = 5;

        repoMock
            .Setup(r => r.FindById(5))
            .Returns(user);

        var service = new AdminValidationService(repoMock.Object);

        var result = service.GetExistingUser(5);

        Assert.IsNotNull(result);
        Assert.AreEqual(5, result.Id);
        Assert.AreEqual("Miguel", result.Name);
    }

    [TestMethod]
    public void GetExistingUser_UserDoesNotExist_ThrowsExceptionWithCorrectMessage()
    {
        var repoMock = new Mock<IRepositoryUser>(MockBehavior.Strict);

        repoMock
            .Setup(r => r.FindById(99))
            .Returns((User?)null);

        var service = new AdminValidationService(repoMock.Object);

        var ex = Assert.ThrowsException<UserException>(() =>
            service.GetExistingUser(99));

        Assert.AreEqual("User does not exist", ex.Message);
    }

    [TestMethod]
    public void EnsureUserIsAdmin_UserIsAdministrative_DoesNotThrowException()
    {
        var repoMock = new Mock<IRepositoryUser>(MockBehavior.Strict);

        var user = User.MakeIntern(
            "Miguel",
            "Lopez",
            "miguel@gmail.com",
            Phone.MakePhone("099123456"),
            "PasswordSeguraee3!",
            UserRoles.Administrative);
        user.Id = 5;

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

        var user = User.MakeClient(
            "Miguel",
            "Lopez",
            "miguel@gmail.com",
            Phone.MakePhone("099123456"),
            "PasswordSeguraee3!");
        user.Id = 5;

        repoMock
            .Setup(r => r.FindById(5))
            .Returns(user);

        var service = new AdminValidationService(repoMock.Object);

        var ex = Assert.ThrowsException<UserException>(() =>
            service.EnsureUserIsAdmin(5));

        Assert.AreEqual("Only an administrative user can perform this action", ex.Message);
    }

    [TestMethod]
    public void EnsureUserIsAdmin_UserDoesNotExist_ThrowsExceptionWithCorrectMessage()
    {
        var repoMock = new Mock<IRepositoryUser>(MockBehavior.Strict);

        repoMock
            .Setup(r => r.FindById(5))
            .Returns((User?)null);

        var service = new AdminValidationService(repoMock.Object);

        var ex = Assert.ThrowsException<UserException>(() =>
            service.EnsureUserIsAdmin(5));

        Assert.AreEqual("User does not exist", ex.Message);
    }

    [TestMethod]
    public void EnsureIsNotSelfAction_SameUser_ThrowsException()
    {
        var repoMock = new Mock<IRepositoryUser>(MockBehavior.Strict);
        var service = new AdminValidationService(repoMock.Object);

        var ex = Assert.ThrowsException<UserException>(() =>
            service.EnsureIsNotSelfAction(5, 5, "A user cannot modify itself"));

        Assert.AreEqual("A user cannot modify itself", ex.Message);
    }

    [TestMethod]
    public void EnsureIsNotSelfAction_DifferentUsers_DoesNotThrowException()
    {
        var repoMock = new Mock<IRepositoryUser>(MockBehavior.Strict);
        var service = new AdminValidationService(repoMock.Object);

        service.EnsureIsNotSelfAction(10, 5, "A user cannot modify itself");

        Assert.IsTrue(true);
    }
}
