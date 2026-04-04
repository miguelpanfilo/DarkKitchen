using AlmacenamientoAbstracciones;
using Dominio;
using Dominio.Excepciones;
using LogicaDeNegocio;
using Moq;

namespace Pruebas.Usuarios;

[TestClass]
public class DeleteUserTests
{
    [TestMethod]
    public void EliminarUsuarioInterno_DatosValidos_LlamaRemoveUnaVez()
    {
        var repoMock = new Mock<IRepositoryUser>();

        var usuarioExistente = User.MakeIntern(
            "Juan",
            "Perez",
            "admin@gmail.com",
            Phone.MakePhone("099123456"),
            "PasswordSeguraee3!",
            UserRoles.Administrative
        );
        usuarioExistente.Id = 5;
        var usuarioAEliminar = User.MakeIntern(
            "Juan",
            "Perez",
            "admin2@gmail.com",
            Phone.MakePhone("099123456"),
            "PasswordSeguraee3!",
            UserRoles.Administrative
        );
        usuarioAEliminar.Id = 52;

        repoMock
            .Setup(r => r.FindById(5))
            .Returns(usuarioExistente);

        repoMock
            .Setup(r => r.FindById(52))
            .Returns(usuarioAEliminar);

        var service = new DeleteUserService(repoMock.Object);
        service.DeleteInterUser(52, 5);

        repoMock.Verify(r => r.Remove(52), Times.Once);
    }

[TestMethod]
public void DeleteInternalUser_WhenDeletingItself_ThrowsExceptionWithCorrectMessage()
{
    var repoMock = new Mock<IRepositoryUser>();

    var existingUser = User.MakeIntern(
        "Juan",
        "Perez",
        "admin@gmail.com",
        Phone.MakePhone("099123456"),
        "PasswordSeguraee3!",
        UserRoles.Administrative
    );
    existingUser.Id = 5;

    repoMock
        .Setup(r => r.FindById(5))
        .Returns(existingUser);

    var service = new DeleteUserService(repoMock.Object);
    var ex = Assert.ThrowsException<UsuarioExcepcion>(() =>
        service.DeleteInterUser(5, 5)
    );

    Assert.AreEqual("A user cannot delete themselves", ex.Message);
}

[TestMethod]
public void DeleteInternalUser_UserDoesNotExist_ThrowsExceptionWithCorrectMessage()
{
    var repoMock = new Mock<IRepositoryUser>();

    repoMock
        .Setup(r => r.FindById(99))
        .Returns((User?)null);

    var service = new DeleteUserService(repoMock.Object);
    var ex = Assert.ThrowsException<UsuarioExcepcion>(() =>
        service.DeleteInterUser(5, 99)
    );

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
    var ex = Assert.ThrowsException<UsuarioExcepcion>(() =>
        service.DeleteInterUser(5, 99)
    );

    Assert.AreEqual("User does not exist", ex.Message);

    repoMock.Verify(r => r.Remove(It.IsAny<int>()), Times.Never);
}

[TestMethod]
public void DeleteInternalUser_WhenDeletingItself_DoesNotCallRemove_AndThrowsException()
{
    var repoMock = new Mock<IRepositoryUser>();

    var existingUser = User.MakeIntern(
        "Juan",
        "Perez",
        "admin@gmail.com",
        Phone.MakePhone("099123456"),
        "PasswordSeguraee3!",
        UserRoles.Administrative
    );
    existingUser.Id = 5;

    repoMock
        .Setup(r => r.FindById(5))
        .Returns(existingUser);

    var service = new DeleteUserService(repoMock.Object);
    var ex = Assert.ThrowsException<UsuarioExcepcion>(() =>
        service.DeleteInterUser(5, 5)
    );

    Assert.AreEqual("A user cannot delete themselves", ex.Message);

    repoMock.Verify(r => r.Remove(It.IsAny<int>()), Times.Never);
}

[TestMethod]
public void DeleteInternalUser_LoggedInUserIsNotAdministrative_ThrowsExceptionWithCorrectMessage()
{
    var repoMock = new Mock<IRepositoryUser>();

    var userToDelete = User.MakeIntern(
        "Pedro",
        "Gomez",
        "pedro@gmail.com",
        Phone.MakePhone("099111111"),
        "PasswordSeguraee3!",
        UserRoles.Preparer
    );
    userToDelete.Id = 10;

    var loggedInUser = User.MakeClient(
        "Juan",
        "Perez",
        "cliente@gmail.com",
        Phone.MakePhone("099123456"),
        "PasswordSeguraee3!"
    );
    loggedInUser.Id = 5;

    repoMock
        .Setup(r => r.FindById(10))
        .Returns(userToDelete);

    repoMock
        .Setup(r => r.FindById(5))
        .Returns(loggedInUser);

    var service = new DeleteUserService(repoMock.Object);
    var ex = Assert.ThrowsException<UsuarioExcepcion>(() =>
        service.DeleteInterUser(10, 5)
    );

    Assert.AreEqual("Only an administrative user can perform this action", ex.Message);
}

    [TestMethod]
    public void EliminarUsuarioInterno_UsuarioLogueadoNoEsAdministrativo_NoLlamaRemove()
    {
        var repoMock = new Mock<IRepositoryUser>();

        var usuarioAEliminar = User.MakeIntern(
            "Pedro",
            "Gomez",
            "pedro@gmail.com",
            Phone.MakePhone("099111111"),
            "PasswordSeguraee3!",
            UserRoles.Preparer
        );
        usuarioAEliminar.Id = 10;

        var usuarioLogueado = User.MakeClient(
            "Juan",
            "Perez",
            "cliente@gmail.com",
            Phone.MakePhone("099123456"),
            "PasswordSeguraee3!"
        );
        usuarioLogueado.Id = 5;

        repoMock
            .Setup(r => r.FindById(10))
            .Returns(usuarioAEliminar);

        repoMock
            .Setup(r => r.FindById(5))
            .Returns(usuarioLogueado);

        var service = new DeleteUserService(repoMock.Object);
        Assert.ThrowsException<UsuarioExcepcion>(() =>
            service.DeleteInterUser(10, 5)
        );

        repoMock.Verify(r => r.Remove(It.IsAny<int>()), Times.Never);
    }
}
