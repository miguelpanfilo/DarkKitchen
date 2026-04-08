using AlmacenamientoAbstracciones;
using Contratos;
using Dominio;
using Dominio.Exceptions;
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
        var repoMock = new Mock<IRepositoryUser>(MockBehavior.Strict);
        var phoneValidatorMock = new Mock<IPhoneValidator>(MockBehavior.Strict);
        var adminValidationMock = new Mock<IAdminValidationService>(MockBehavior.Strict);

        adminValidationMock
            .Setup(v => v.EnsureUserIsAdmin(99))
            .Throws(new UserException("User does not exist"));

        var service = new ModifyUserService(
            repoMock.Object,
            phoneValidatorMock.Object,
            adminValidationMock.Object);

        var request = new ModificarClaveRequestDTO
        {
            OldPassword = "PasswordSeguraee3!",
            NewPassword = "ClaveNuevaSegura4!"
        };

        var ex = Assert.ThrowsException<UserException>(() =>
            service.UpdateUserPassword(5, 99, request));

        Assert.AreEqual("User does not exist", ex.Message);
    }

    [TestMethod]
    public void ModificarClaveUsuario_DatosValidos_CambiaLaClave()
    {
        var repoMock = new Mock<IRepositoryUser>(MockBehavior.Strict);
        var validadorTelefonoMock = new Mock<IPhoneValidator>(MockBehavior.Strict);
        var adminValidationMock = new Mock<IAdminValidationService>(MockBehavior.Strict);

        var usuario = User.MakeIntern(
            "Juan",
            "Perez",
            "admin@gmail.com",
            Phone.MakePhone("099123456"),
            "PasswordSeguraee3!",
            UserRoles.Administrative);
        usuario.Id = 5;

        adminValidationMock
            .Setup(v => v.EnsureUserIsAdmin(It.IsAny<int>()));

        adminValidationMock
            .Setup(v => v.EnsureIsNotSelfAction(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()));

        adminValidationMock
            .Setup(v => v.GetExistingUser(5))
            .Returns(usuario);

        var service = new ModifyUserService(
            repoMock.Object,
            validadorTelefonoMock.Object,
            adminValidationMock.Object);

        var request = new ModificarClaveRequestDTO
        {
            OldPassword = "PasswordSeguraee3!",
            NewPassword = "ClaveNuevaSegura4!"
        };

        service.UpdateUserPassword(5, 99, request);

        Assert.IsTrue(usuario.VerifyPassword("ClaveNuevaSegura4!"));
    }

    [TestMethod]
    public void UpdateUserPassword_LoggedInUserIsNotAdministrative_ThrowsExceptionWithCorrectMessage()
    {
        var repoMock = new Mock<IRepositoryUser>(MockBehavior.Strict);
        var phoneValidatorMock = new Mock<IPhoneValidator>(MockBehavior.Strict);
        var adminValidationMock = new Mock<IAdminValidationService>(MockBehavior.Strict);

        adminValidationMock
            .Setup(v => v.EnsureUserIsAdmin(99))
            .Throws(new UserException("Only an administrative user can perform this action"));

        var service = new ModifyUserService(
            repoMock.Object,
            phoneValidatorMock.Object,
            adminValidationMock.Object);

        var request = new ModificarClaveRequestDTO
        {
            OldPassword = "PasswordSeguraee3!",
            NewPassword = "ClaveNuevaSegura4!"
        };

        var ex = Assert.ThrowsException<UserException>(() =>
            service.UpdateUserPassword(5, 99, request));

        Assert.AreEqual("Only an administrative user can perform this action", ex.Message);
    }

    [TestMethod]
    public void UpdateUserPassword_WrongCurrentPassword_ThrowsExceptionWithCorrectMessage()
    {
        var repoMock = new Mock<IRepositoryUser>(MockBehavior.Strict);
        var phoneValidatorMock = new Mock<IPhoneValidator>(MockBehavior.Strict);
        var adminValidationMock = new Mock<IAdminValidationService>(MockBehavior.Strict);

        var user = User.MakeIntern(
            "Juan",
            "Perez",
            "admin@gmail.com",
            Phone.MakePhone("099123456"),
            "PasswordSeguraee3!",
            UserRoles.Administrative);
        user.Id = 5;

        adminValidationMock
            .Setup(v => v.EnsureUserIsAdmin(It.IsAny<int>()));

        adminValidationMock
            .Setup(v => v.EnsureIsNotSelfAction(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()));

        adminValidationMock
            .Setup(v => v.GetExistingUser(5))
            .Returns(user);

        var service = new ModifyUserService(
            repoMock.Object,
            phoneValidatorMock.Object,
            adminValidationMock.Object);

        var request = new ModificarClaveRequestDTO
        {
            OldPassword = "Incorrecta123!",
            NewPassword = "ClaveNuevaSegura4!"
        };

        var ex = Assert.ThrowsException<UserException>(() =>
            service.UpdateUserPassword(5, 99, request));

        Assert.AreEqual("Current password is incorrect", ex.Message);
    }

    [TestMethod]
    public void UpdateUserPassword_InvalidNewPassword_ThrowsExceptionWithCorrectMessage()
    {
        var repoMock = new Mock<IRepositoryUser>(MockBehavior.Strict);
        var phoneValidatorMock = new Mock<IPhoneValidator>(MockBehavior.Strict);
        var adminValidationMock = new Mock<IAdminValidationService>(MockBehavior.Strict);

        var user = User.MakeIntern(
            "Juan",
            "Perez",
            "admin@gmail.com",
            Phone.MakePhone("099123456"),
            "PasswordSeguraee3!",
            UserRoles.Administrative);
        user.Id = 5;

        adminValidationMock
            .Setup(v => v.EnsureUserIsAdmin(It.IsAny<int>()));

        adminValidationMock
            .Setup(v => v.EnsureIsNotSelfAction(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()));

        adminValidationMock
            .Setup(v => v.GetExistingUser(5))
            .Returns(user);

        var service = new ModifyUserService(
            repoMock.Object,
            phoneValidatorMock.Object,
            adminValidationMock.Object);

        var request = new ModificarClaveRequestDTO
        {
            OldPassword = "PasswordSeguraee3!",
            NewPassword = "123"
        };

        var ex = Assert.ThrowsException<UserException>(() =>
            service.UpdateUserPassword(5, 99, request));

        Assert.AreEqual("Password format is not valid", ex.Message);
    }

    [TestMethod]
    public void UpdateUserPassword_UserDoesNotExist_ThrowsExceptionWithCorrectMessage()
    {
        var repoMock = new Mock<IRepositoryUser>(MockBehavior.Strict);
        var phoneValidatorMock = new Mock<IPhoneValidator>(MockBehavior.Strict);
        var adminValidationMock = new Mock<IAdminValidationService>(MockBehavior.Strict);

        adminValidationMock
            .Setup(v => v.EnsureUserIsAdmin(It.IsAny<int>()));

        adminValidationMock
            .Setup(v => v.EnsureIsNotSelfAction(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()));

        adminValidationMock
            .Setup(v => v.GetExistingUser(5))
            .Throws(new UserException("User does not exist"));

        var service = new ModifyUserService(
            repoMock.Object,
            phoneValidatorMock.Object,
            adminValidationMock.Object);

        var request = new ModificarClaveRequestDTO
        {
            OldPassword = "PasswordSeguraee3!",
            NewPassword = "ClaveNuevaSegura4!"
        };

        var ex = Assert.ThrowsException<UserException>(() =>
            service.UpdateUserPassword(5, 99, request));

        Assert.AreEqual("User does not exist", ex.Message);
    }

    [TestMethod]
    public void ModificarClaveUsuario_ClaveActualIncorrecta_NoCambiaLaClave()
    {
        var repoMock = new Mock<IRepositoryUser>(MockBehavior.Strict);
        var validadorTelefonoMock = new Mock<IPhoneValidator>(MockBehavior.Strict);
        var adminValidationMock = new Mock<IAdminValidationService>(MockBehavior.Strict);

        var usuario = User.MakeIntern(
            "Juan",
            "Perez",
            "admin@gmail.com",
            Phone.MakePhone("099123456"),
            "PasswordSeguraee3!",
            UserRoles.Administrative);
        usuario.Id = 5;

        adminValidationMock
            .Setup(v => v.EnsureUserIsAdmin(It.IsAny<int>()));

        adminValidationMock
            .Setup(v => v.EnsureIsNotSelfAction(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()));

        adminValidationMock
            .Setup(v => v.GetExistingUser(5))
            .Returns(usuario);

        var service = new ModifyUserService(
            repoMock.Object,
            validadorTelefonoMock.Object,
            adminValidationMock.Object);

        var request = new ModificarClaveRequestDTO
        {
            OldPassword = "Incorrecta123!",
            NewPassword = "ClaveNuevaSegura4!"
        };

        Assert.ThrowsException<UserException>(() =>
            service.UpdateUserPassword(5, 99, request));

        Assert.IsTrue(usuario.Password.VerifyHash("PasswordSeguraee3!"));
    }

    [TestMethod]
    public void UpdateUserPassword_WhenUserModifiesItself_ThrowsExceptionWithCorrectMessage()
    {
        var repoMock = new Mock<IRepositoryUser>(MockBehavior.Strict);
        var phoneValidatorMock = new Mock<IPhoneValidator>(MockBehavior.Strict);
        var adminValidationMock = new Mock<IAdminValidationService>(MockBehavior.Strict);

        adminValidationMock
            .Setup(v => v.EnsureUserIsAdmin(It.IsAny<int>()));

        adminValidationMock
            .Setup(v => v.EnsureIsNotSelfAction(5, 5, It.IsAny<string>()))
            .Throws(new UserException("A user cannot modify itself"));

        var service = new ModifyUserService(
            repoMock.Object,
            phoneValidatorMock.Object,
            adminValidationMock.Object);

        var request = new ModificarClaveRequestDTO
        {
            OldPassword = "PasswordSeguraee3!",
            NewPassword = "ClaveNuevaSegura4!"
        };

        var ex = Assert.ThrowsException<UserException>(() =>
            service.UpdateUserPassword(5, 5, request));

        Assert.AreEqual("A user cannot modify itself", ex.Message);
    }

    [TestMethod]
    public void UpdateUserPassword_WhenLoggedUserIsNotAdmin_DoesNotCallGetExistingUser()
    {
        var repoMock = new Mock<IRepositoryUser>(MockBehavior.Strict);
        var phoneValidatorMock = new Mock<IPhoneValidator>(MockBehavior.Strict);
        var adminValidationMock = new Mock<IAdminValidationService>(MockBehavior.Strict);

        adminValidationMock
            .Setup(v => v.EnsureUserIsAdmin(99))
            .Throws(new UserException("Only an administrative user can perform this action"));

        var service = new ModifyUserService(
            repoMock.Object,
            phoneValidatorMock.Object,
            adminValidationMock.Object);

        var request = new ModificarClaveRequestDTO
        {
            OldPassword = "PasswordSeguraee3!",
            NewPassword = "ClaveNuevaSegura4!"
        };

        Assert.ThrowsException<UserException>(() =>
            service.UpdateUserPassword(5, 99, request));

        adminValidationMock.Verify(v => v.GetExistingUser(It.IsAny<int>()), Times.Never);
    }

    [TestMethod]
    public void UpdateUserPassword_WhenUserModifiesItself_DoesNotCallGetExistingUser()
    {
        var repoMock = new Mock<IRepositoryUser>(MockBehavior.Strict);
        var phoneValidatorMock = new Mock<IPhoneValidator>(MockBehavior.Strict);
        var adminValidationMock = new Mock<IAdminValidationService>(MockBehavior.Strict);

        adminValidationMock
            .Setup(v => v.EnsureUserIsAdmin(It.IsAny<int>()));

        adminValidationMock
            .Setup(v => v.EnsureIsNotSelfAction(5, 5, It.IsAny<string>()))
            .Throws(new UserException("A user cannot modify itself"));

        var service = new ModifyUserService(
            repoMock.Object,
            phoneValidatorMock.Object,
            adminValidationMock.Object);

        var request = new ModificarClaveRequestDTO
        {
            OldPassword = "PasswordSeguraee3!",
            NewPassword = "ClaveNuevaSegura4!"
        };

        Assert.ThrowsException<UserException>(() =>
            service.UpdateUserPassword(5, 5, request));

        adminValidationMock.Verify(v => v.GetExistingUser(It.IsAny<int>()), Times.Never);
    }

    [TestMethod]
    public void UpdateUserPassword_CallsAdminValidationMethods()
    {
        var repoMock = new Mock<IRepositoryUser>(MockBehavior.Strict);
        var phoneValidatorMock = new Mock<IPhoneValidator>(MockBehavior.Strict);
        var adminValidationMock = new Mock<IAdminValidationService>(MockBehavior.Strict);

        var usuario = User.MakeIntern(
            "Juan",
            "Perez",
            "admin@gmail.com",
            Phone.MakePhone("099123456"),
            "PasswordSeguraee3!",
            UserRoles.Administrative);
        usuario.Id = 5;

        adminValidationMock
            .Setup(v => v.EnsureUserIsAdmin(99));

        adminValidationMock
            .Setup(v => v.EnsureIsNotSelfAction(5, 99, It.IsAny<string>()));

        adminValidationMock
            .Setup(v => v.GetExistingUser(5))
            .Returns(usuario);

        var service = new ModifyUserService(
            repoMock.Object,
            phoneValidatorMock.Object,
            adminValidationMock.Object);

        var request = new ModificarClaveRequestDTO
        {
            OldPassword = "PasswordSeguraee3!",
            NewPassword = "ClaveNuevaSegura4!"
        };

        service.UpdateUserPassword(5, 99, request);

        adminValidationMock.Verify(v => v.EnsureUserIsAdmin(99), Times.Once);
        adminValidationMock.Verify(v => v.EnsureIsNotSelfAction(5, 99, It.IsAny<string>()), Times.Once);
        adminValidationMock.Verify(v => v.GetExistingUser(5), Times.Once);
    }
}
