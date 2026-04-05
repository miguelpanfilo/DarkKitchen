using AlmacenamientoAbstracciones;
using Dominio;
using Dominio.Exceptions;
using LogicaDeNegocio;
using LogicaDeNegocioAbstracciones;
using Moq;

namespace Pruebas.Usuarios;

[TestClass]
public class DeleteUserTests
{
    [TestMethod]
    public void DeleteInternalUser_ValidData_CallsRemoveOnce()
    {
        var repoMock = new Mock<IRepositoryUser>();
        var adminValidationMock = new Mock<IAdminValidationService>();

        var userToDelete = User.MakeIntern(
            "Juan",
            "Perez",
            "admin2@gmail.com",
            Phone.MakePhone("099123456"),
            "PasswordSeguraee3!",
            UserRoles.Administrative);
        userToDelete.Id = 52;

        adminValidationMock
            .Setup(v => v.GetExistingUser(52))
            .Returns(userToDelete);

        var service = new DeleteUserService(repoMock.Object, adminValidationMock.Object);

        service.DeleteInterUser(52, 5);

        repoMock.Verify(r => r.Remove(52), Times.Once);
    }

    [TestMethod]
    public void DeleteInternalUser_WhenDeletingItself_ThrowsException()
    {
        var repoMock = new Mock<IRepositoryUser>();
        var adminValidationMock = new Mock<IAdminValidationService>();

        adminValidationMock
            .Setup(v => v.EnsureIsNotSelfAction(5, 5, "A user cannot delete themselves"))
            .Throws(new UserException("A user cannot delete themselves"));

        var service = new DeleteUserService(repoMock.Object, adminValidationMock.Object);

        var ex = Assert.ThrowsException<UserException>(() =>
            service.DeleteInterUser(5, 5));

        Assert.AreEqual("A user cannot delete themselves", ex.Message);
    }

    [TestMethod]
    public void DeleteInternalUser_WhenDeletingItself_DoesNotCallRemove()
    {
        var repoMock = new Mock<IRepositoryUser>();
        var adminValidationMock = new Mock<IAdminValidationService>();

        adminValidationMock
            .Setup(v => v.EnsureIsNotSelfAction(5, 5, "A user cannot delete themselves"))
            .Throws(new UserException("A user cannot delete themselves"));

        var service = new DeleteUserService(repoMock.Object, adminValidationMock.Object);

        Assert.ThrowsException<UserException>(() =>
            service.DeleteInterUser(5, 5));

        repoMock.Verify(r => r.Remove(It.IsAny<int>()), Times.Never);
    }

    [TestMethod]
    public void DeleteInternalUser_UserDoesNotExist_ThrowsException()
    {
        var repoMock = new Mock<IRepositoryUser>();
        var adminValidationMock = new Mock<IAdminValidationService>();

        adminValidationMock
            .Setup(v => v.GetExistingUser(5))
            .Throws(new UserException("User does not exist"));

        var service = new DeleteUserService(repoMock.Object, adminValidationMock.Object);

        var ex = Assert.ThrowsException<UserException>(() =>
            service.DeleteInterUser(5, 99));

        Assert.AreEqual("User does not exist", ex.Message);
    }

    [TestMethod]
    public void DeleteInternalUser_UserDoesNotExist_DoesNotCallRemove()
    {
        var repoMock = new Mock<IRepositoryUser>();
        var adminValidationMock = new Mock<IAdminValidationService>();

        adminValidationMock
            .Setup(v => v.GetExistingUser(5))
            .Throws(new UserException("User does not exist"));

        var service = new DeleteUserService(repoMock.Object, adminValidationMock.Object);

        Assert.ThrowsException<UserException>(() =>
            service.DeleteInterUser(5, 99));

        repoMock.Verify(r => r.Remove(It.IsAny<int>()), Times.Never);
    }

    [TestMethod]
    public void DeleteInternalUser_LoggedUserIsNotAdmin_ThrowsException()
    {
        var repoMock = new Mock<IRepositoryUser>();
        var adminValidationMock = new Mock<IAdminValidationService>();

        adminValidationMock
            .Setup(v => v.EnsureUserIsAdmin(5))
            .Throws(new UserException("Only an administrative user can perform this action"));

        var service = new DeleteUserService(repoMock.Object, adminValidationMock.Object);

        var ex = Assert.ThrowsException<UserException>(() =>
            service.DeleteInterUser(10, 5));

        Assert.AreEqual("Only an administrative user can perform this action", ex.Message);
    }

    [TestMethod]
    public void DeleteInternalUser_LoggedUserIsNotAdmin_DoesNotCallRemove()
    {
        var repoMock = new Mock<IRepositoryUser>();
        var adminValidationMock = new Mock<IAdminValidationService>();

        adminValidationMock
            .Setup(v => v.EnsureUserIsAdmin(5))
            .Throws(new UserException("Only an administrative user can perform this action"));

        var service = new DeleteUserService(repoMock.Object, adminValidationMock.Object);

        Assert.ThrowsException<UserException>(() =>
            service.DeleteInterUser(10, 5));

        repoMock.Verify(r => r.Remove(It.IsAny<int>()), Times.Never);
    }

    [TestMethod]
    public void DeleteInterUser_AlwaysValidatesThatLoggedUserIsAdmin()
    {
        var repoMock = new Mock<IRepositoryUser>();
        var adminValidationMock = new Mock<IAdminValidationService>();

        var userToDelete = User.MakeIntern(
            "Juan",
            "Perez",
            "admin2@gmail.com",
            Phone.MakePhone("099123456"),
            "PasswordSeguraee3!",
            UserRoles.Administrative);
        userToDelete.Id = 52;

        adminValidationMock
            .Setup(v => v.GetExistingUser(52))
            .Returns(userToDelete);

        var service = new DeleteUserService(repoMock.Object, adminValidationMock.Object);

        service.DeleteInterUser(52, 5);

        adminValidationMock.Verify(v => v.EnsureUserIsAdmin(5), Times.Once);
    }

    [TestMethod]
    public void DeleteInterUser_AlwaysValidatesThatUserIsNotDeletingThemselves()
    {
        var repoMock = new Mock<IRepositoryUser>();
        var adminValidationMock = new Mock<IAdminValidationService>();

        var userToDelete = User.MakeIntern(
            "Juan",
            "Perez",
            "admin2@gmail.com",
            Phone.MakePhone("099123456"),
            "PasswordSeguraee3!",
            UserRoles.Administrative);
        userToDelete.Id = 52;

        adminValidationMock
            .Setup(v => v.GetExistingUser(52))
            .Returns(userToDelete);

        var service = new DeleteUserService(repoMock.Object, adminValidationMock.Object);

        service.DeleteInterUser(52, 5);

        adminValidationMock.Verify(
            v => v.EnsureIsNotSelfAction(52, 5, "A user cannot delete themselves"),
            Times.Once);
    }

    [TestMethod]
    public void DeleteInterUser_GetsExistingUserFromAdminValidationService()
    {
        var repoMock = new Mock<IRepositoryUser>();
        var adminValidationMock = new Mock<IAdminValidationService>();

        var userToDelete = User.MakeIntern(
            "Juan",
            "Perez",
            "admin2@gmail.com",
            Phone.MakePhone("099123456"),
            "PasswordSeguraee3!",
            UserRoles.Administrative);
        userToDelete.Id = 52;

        adminValidationMock
            .Setup(v => v.GetExistingUser(52))
            .Returns(userToDelete);

        var service = new DeleteUserService(repoMock.Object, adminValidationMock.Object);

        service.DeleteInterUser(52, 5);

        adminValidationMock.Verify(v => v.GetExistingUser(52), Times.Once);
    }
}
