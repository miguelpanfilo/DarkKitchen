using System.Security;
using AlmacenamientoAbstracciones;
using Contratos;
using Dominio;
using Dominio.Excepciones;
using LogicaDeNegocio;
using LogicaDeNegocioAbstracciones;
using Moq;

namespace Pruebas.Usuarios;

[TestClass]
public class ModifyPasswordTests
{
    [TestMethod]
    public void UpdateUserPassword_LoggedInUserDoesNotExist_ThrowsExceptionWithCorrectMessage()
    {
        var repoMock = new Mock<IRepositoryUser>();
        var phoneValidatorMock = new Mock<IPhoneValidator>();

        var service = new ModifyUserService(repoMock.Object, phoneValidatorMock.Object);
        var userToModify = User.MakeIntern(
            "Juan",
            "Perez",
            "admin@gmail.com",
            Phone.MakePhone("099123456"),
            "PasswordSeguraee3!",
            UserRoles.Administrative
        );
        userToModify.Id = 5;

        repoMock
            .Setup(r => r.FindById(5))
            .Returns(userToModify);

        repoMock
            .Setup(r => r.FindById(99))
            .Returns((User?)null);

        var request = new ModificarClaveRequestDTO
        {
            OldPassword = "PasswordSeguraee3!",
            NewPassword = "ClaveNuevaSegura4!"
        };

        var ex = Assert.ThrowsException<UsuarioExcepcion>(() =>
            service.UpdateUserPassword(5, 99, request)
        );

        Assert.AreEqual("User does not exist", ex.Message);
    }

    [TestMethod]
    public void ModificarClaveUsuario_DatosValidos_CambiaLaClave()
    {
        var repoMock = new Mock<IRepositoryUser>();
        var validadorTelefonoMock = new Mock<IPhoneValidator>();

        var usuario = User.MakeIntern(
            "Juan",
            "Perez",
            "admin@gmail.com",
            Phone.MakePhone("099123456"),
            "PasswordSeguraee3!",
            UserRoles.Administrative
        );
        usuario.Id = 5;

        repoMock
            .Setup(r => r.FindById(5))
            .Returns(usuario);

        var usuarioLogueado = User.MakeIntern(
            "Juan Martin",
            "Perez",
            "admin2@gmail.com",
            Phone.MakePhone("099123456"),
            "PasswordSeguyyyy3!",
            UserRoles.Administrative
        );
        usuarioLogueado.Id = 99;

        repoMock
            .Setup(r => r.FindById(99))
            .Returns(usuarioLogueado);

        var service = new ModifyUserService(repoMock.Object, validadorTelefonoMock.Object);

        var request = new ModificarClaveRequestDTO
        {
            OldPassword = "PasswordSeguraee3!", NewPassword = "ClaveNuevaSegura4!"
        };

        service.UpdateUserPassword(5, 99,request);

        Assert.IsTrue(usuario.VerifyPassword("ClaveNuevaSegura4!"));
    }

    [TestMethod]
    public void UpdateUserPassword_LoggedInUserIsNotAdministrative_ThrowsExceptionWithCorrectMessage()
    {
        var repoMock = new Mock<IRepositoryUser>();
        var phoneValidatorMock = new Mock<IPhoneValidator>();

        var userToModify = User.MakeIntern(
            "Juan",
            "Perez",
            "admin@gmail.com",
            Phone.MakePhone("099123456"),
            "PasswordSeguraee3!",
            UserRoles.Administrative
        );
        userToModify.Id = 5;

        var loggedInUser = User.MakeClient(
            "Pedro",
            "Gomez",
            "cliente@gmail.com",
            Phone.MakePhone("099111222"),
            "PasswordSeguraee3!"
        );
        loggedInUser.Id = 99;

        repoMock
            .Setup(r => r.FindById(5))
            .Returns(userToModify);

        repoMock
            .Setup(r => r.FindById(99))
            .Returns(loggedInUser);

        var service = new ModifyUserService(repoMock.Object, phoneValidatorMock.Object);

        var request = new ModificarClaveRequestDTO
        {
            OldPassword = "PasswordSeguraee3!",
            NewPassword = "ClaveNuevaSegura4!"
        };

        var ex = Assert.ThrowsException<UsuarioExcepcion>(() =>
            service.UpdateUserPassword(5, 99, request)
        );

        Assert.AreEqual("Only an administrative user can perform this action", ex.Message);
    }

    [TestMethod]
public void UpdateUserPassword_WrongCurrentPassword_ThrowsExceptionWithCorrectMessage()
{
    var repoMock = new Mock<IRepositoryUser>();
    var phoneValidatorMock = new Mock<IPhoneValidator>();

    var user = User.MakeIntern(
        "Juan",
        "Perez",
        "admin@gmail.com",
        Phone.MakePhone("099123456"),
        "PasswordSeguraee3!",
        UserRoles.Administrative
    );
    user.Id = 5;

    var loggedInUser = User.MakeIntern(
        "Admin",
        "Root",
        "root@gmail.com",
        Phone.MakePhone("099999999"),
        "PasswordSeguraee3!",
        UserRoles.Administrative
    );
    loggedInUser.Id = 99;

    repoMock.Setup(r => r.FindById(5)).Returns(user);
    repoMock.Setup(r => r.FindById(99)).Returns(loggedInUser);

    var service = new ModifyUserService(repoMock.Object, phoneValidatorMock.Object);

    var request = new ModificarClaveRequestDTO
    {
        OldPassword = "Incorrecta123!",
        NewPassword = "ClaveNuevaSegura4!"
    };

    var ex = Assert.ThrowsException<UsuarioExcepcion>(() =>
        service.UpdateUserPassword(5, 99, request)
    );

    Assert.AreEqual("Current password is incorrect", ex.Message);
}

[TestMethod]
public void UpdateUserPassword_InvalidNewPassword_ThrowsExceptionWithCorrectMessage()
{
    var repoMock = new Mock<IRepositoryUser>();
    var phoneValidatorMock = new Mock<IPhoneValidator>();

    var user = User.MakeIntern(
        "Juan",
        "Perez",
        "admin@gmail.com",
        Phone.MakePhone("099123456"),
        "PasswordSeguraee3!",
        UserRoles.Administrative
    );
    user.Id = 5;

    repoMock
        .Setup(r => r.FindById(5))
        .Returns(user);

    var loggedInUser = User.MakeIntern(
        "Juan",
        "Perez",
        "admin@gmail.com",
        Phone.MakePhone("099123456"),
        "PasswordSeguraee3!",
        UserRoles.Administrative
    );
    loggedInUser.Id = 99;

    repoMock
        .Setup(r => r.FindById(99))
        .Returns(loggedInUser);

    var service = new ModifyUserService(repoMock.Object, phoneValidatorMock.Object);

    var request = new ModificarClaveRequestDTO
    {
        OldPassword = "PasswordSeguraee3!",
        NewPassword = "123"
    };

    var ex = Assert.ThrowsException<UsuarioExcepcion>(() =>
        service.UpdateUserPassword(5, 99, request)
    );

    Assert.AreEqual("Password format is not valid", ex.Message);
}

[TestMethod]
public void UpdateUserPassword_UserDoesNotExist_ThrowsExceptionWithCorrectMessage()
{
    var repoMock = new Mock<IRepositoryUser>();
    var phoneValidatorMock = new Mock<IPhoneValidator>();

    repoMock
        .Setup(r => r.FindById(5))
        .Returns((User?)null);

    var loggedInUser = User.MakeIntern(
        "Juan",
        "Perez",
        "admin@gmail.com",
        Phone.MakePhone("099123456"),
        "PasswordSeguraee3!",
        UserRoles.Administrative
    );
    loggedInUser.Id = 99;

    repoMock
        .Setup(r => r.FindById(99))
        .Returns(loggedInUser);

    var service = new ModifyUserService(repoMock.Object, phoneValidatorMock.Object);

    var request = new ModificarClaveRequestDTO
    {
        OldPassword = "PasswordSeguraee3!",
        NewPassword = "ClaveNuevaSegura4!"
    };

    var ex = Assert.ThrowsException<UsuarioExcepcion>(() =>
        service.UpdateUserPassword(5, 99, request)
    );

    Assert.AreEqual("User does not exist", ex.Message);
}

    [TestMethod]
    public void ModificarClaveUsuario_ClaveActualIncorrecta_NoCambiaLaClave()
    {
        var repoMock = new Mock<IRepositoryUser>();
        var validadorTelefonoMock = new Mock<IPhoneValidator>();

        var usuario = User.MakeIntern(
            "Juan",
            "Perez",
            "admin@gmail.com",
            Phone.MakePhone("099123456"),
            "PasswordSeguraee3!",
            UserRoles.Administrative
        );
        usuario.Id = 5;

        repoMock
            .Setup(r => r.FindById(5))
            .Returns(usuario);

        var usuarioLogueado = User.MakeIntern(
            "Juan",
            "Perez",
            "admin@gmail.com",
            Phone.MakePhone("099123456"),
            "PasswordSeguraee3!",
            UserRoles.Administrative
        );
        usuarioLogueado.Id = 99;

        repoMock
            .Setup(r => r.FindById(99))
            .Returns(usuarioLogueado);

        var service = new ModifyUserService(repoMock.Object, validadorTelefonoMock.Object);
        var request = new ModificarClaveRequestDTO
        {
            OldPassword = "Incorrecta123!",
            NewPassword = "ClaveNuevaSegura4!"
        };

        Assert.ThrowsException<UsuarioExcepcion>(() =>
            service.UpdateUserPassword(5, 99, request)
        );

        Assert.IsTrue(usuario.Password.VerifyHash("PasswordSeguraee3!"));
    }
}
