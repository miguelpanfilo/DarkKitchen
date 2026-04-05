using AlmacenamientoAbstracciones;
using Dominio;
using Dominio.Exceptions;
using LogicaDeNegocio;
using Moq;

namespace Pruebas.Usuarios;

[TestClass]
public class DeleteUserTests
{
    [TestMethod]
    public void DeleteInternalUser_ValidData_CallsRemoveOnce()
    {
        var repoMock = new Mock<IRepositoryUser>();

        var existingUser = User.MakeIntern(
            "Juan",
            "Perez",
            "admin@gmail.com",
            Phone.MakePhone("099123456"),
            "PasswordSeguraee3!",
            UserRoles.Administrative);
        existingUser.Id = 5;

        var userToDelete = User.MakeIntern(
            "Juan",
            "Perez",
            "admin2@gmail.com",
            Phone.MakePhone("099123456"),
            "PasswordSeguraee3!",
            UserRoles.Administrative);
        userToDelete.Id = 52;

        repoMock
            .Setup(r => r.FindById(5))
            .Returns(existingUser);

        repoMock
            .Setup(r => r.FindById(52))
            .Returns(userToDelete);

        var service = new DeleteUserService(repoMock.Object);

        service.DeleteInterUser(52, 5);

        repoMock.Verify(r => r.Remove(52), Times.Once);
    }

    [TestMethod]
    public void DeleteInternalUser_WhenDeletingItself_ThrowsException()
    {
        var repoMock = new Mock<IRepositoryUser>();

        var existingUser = User.MakeIntern(
            "Juan",
            "Perez",
            "admin@gmail.com",
            Phone.MakePhone("099123456"),
            "PasswordSeguraee3!",
            UserRoles.Administrative);
        existingUser.Id = 5;

        repoMock
            .Setup(r => r.FindById(5))
            .Returns(existingUser);

        var service = new DeleteUserService(repoMock.Object);

        var ex = Assert.ThrowsException<UserException>(() =>
            service.DeleteInterUser(5, 5));

        Assert.AreEqual("A user cannot delete themselves", ex.Message);
    }

    [TestMethod]
    public void DeleteInternalUser_UserDoesNotExist_ThrowsException()
    {
        var repoMock = new Mock<IRepositoryUser>();

        repoMock
            .Setup(r => r.FindById(99))
            .Returns((User?)null);

        var service = new DeleteUserService(repoMock.Object);

        var ex = Assert.ThrowsException<UserException>(() =>
            service.DeleteInterUser(5, 99));

        Assert.AreEqual("User does not exist", ex.Message);
    }

    [TestMethod]
    public void DeleteInternalUser_UserDoesNotExist_DoesNotCallRemove()
    {
        var repoMock = new Mock<IRepositoryUser>();

        repoMock
            .Setup(r => r.FindById(99))
            .Returns((User?)null);

        var service = new DeleteUserService(repoMock.Object);

        Assert.ThrowsException<UserException>(() =>
            service.DeleteInterUser(5, 99));

        repoMock.Verify(r => r.Remove(It.IsAny<int>()), Times.Never);
    }

    [TestMethod]
    public void DeleteInternalUser_WhenDeletingItself_DoesNotCallRemove()
    {
        var repoMock = new Mock<IRepositoryUser>();

        var existingUser = User.MakeIntern(
            "Juan",
            "Perez",
            "admin@gmail.com",
            Phone.MakePhone("099123456"),
            "PasswordSeguraee3!",
            UserRoles.Administrative);
        existingUser.Id = 5;

        repoMock
            .Setup(r => r.FindById(5))
            .Returns(existingUser);

        var service = new DeleteUserService(repoMock.Object);

        Assert.ThrowsException<UserException>(() =>
            service.DeleteInterUser(5, 5));

        repoMock.Verify(r => r.Remove(It.IsAny<int>()), Times.Never);
    }

    [TestMethod]
    public void DeleteInternalUser_LoggedUserIsNotAdmin_ThrowsException()
    {
        var repoMock = new Mock<IRepositoryUser>();

        var userToDelete = User.MakeIntern(
            "Pedro",
            "Gomez",
            "pedro@gmail.com",
            Phone.MakePhone("099111111"),
            "PasswordSeguraee3!",
            UserRoles.Preparer);
        userToDelete.Id = 10;

        var loggedUser = User.MakeClient(
            "Juan",
            "Perez",
            "cliente@gmail.com",
            Phone.MakePhone("099123456"),
            "PasswordSeguraee3!");
        loggedUser.Id = 5;

        repoMock
            .Setup(r => r.FindById(10))
            .Returns(userToDelete);

        repoMock
            .Setup(r => r.FindById(5))
            .Returns(loggedUser);

        var service = new DeleteUserService(repoMock.Object);

        var ex = Assert.ThrowsException<UserException>(() =>
            service.DeleteInterUser(10, 5));

        Assert.AreEqual("Only an administrative user can perform this action", ex.Message);
    }

    [TestMethod]
    public void DeleteInternalUser_LoggedUserIsNotAdmin_DoesNotCallRemove()
    {
        var repoMock = new Mock<IRepositoryUser>();

        var userToDelete = User.MakeIntern(
            "Pedro",
            "Gomez",
            "pedro@gmail.com",
            Phone.MakePhone("099111111"),
            "PasswordSeguraee3!",
            UserRoles.Preparer);
        userToDelete.Id = 10;

        var loggedUser = User.MakeClient(
            "Juan",
            "Perez",
            "cliente@gmail.com",
            Phone.MakePhone("099123456"),
            "PasswordSeguraee3!");
        loggedUser.Id = 5;

        repoMock
            .Setup(r => r.FindById(10))
            .Returns(userToDelete);

        repoMock
            .Setup(r => r.FindById(5))
            .Returns(loggedUser);

        var service = new DeleteUserService(repoMock.Object);

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
}
