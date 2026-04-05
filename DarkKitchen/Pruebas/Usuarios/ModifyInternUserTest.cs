using AlmacenamientoAbstracciones;
using Contratos;
using Dominio;
using Dominio.Exceptions;
using LogicaDeNegocio;
using LogicaDeNegocioAbstracciones;
using Moq;

namespace Pruebas.Usuarios;

[TestClass]
public class ModifyInternUserTest
{
    [TestMethod]
    public void UpdateInternalUser_OnlyName_ReturnsUserWithUpdatedName()
    {
        var repoMock = new Mock<IRepositoryUser>();
        var adminValidationMock = new Mock<IAdminValidationService>();

        var existingUser = User.MakeIntern(
            "Juan",
            "Perez",
            "admin@gmail.com",
            Phone.MakePhone("099123456"),
            "PasswordSeguraee3!",
            UserRoles.Administrative);
        existingUser.Id = 5;

        adminValidationMock
            .Setup(v => v.GetExistingUser(5))
            .Returns(existingUser);

        adminValidationMock
            .Setup(v => v.EnsureUserIsAdmin(It.IsAny<int>()));

        adminValidationMock
            .Setup(v => v.EnsureIsNotSelfAction(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()));

        var service = new ModifyUserService(
            repoMock.Object,
            Mock.Of<IPhoneValidator>(),
            adminValidationMock.Object);

        var request = new ModificarUsuarioRequestDTO { Name = "Carlos" };

        var result = service.UpdateInternalUser(5, 99, request);

        Assert.AreEqual(5, result.Id);
        Assert.AreEqual("Carlos", result.Name);
        Assert.AreEqual("Perez", result.LastName);
        Assert.AreEqual("admin@gmail.com", result.Email);
        Assert.AreEqual("Administrative", result.Role);
    }

    [TestMethod]
    public void UpdateInternalUser_WhenUserModifiesItself_ThrowsExceptionWithCorrectMessage()
    {
        var repoMock = new Mock<IRepositoryUser>();
        var adminValidationMock = new Mock<IAdminValidationService>();

        adminValidationMock
            .Setup(v => v.EnsureUserIsAdmin(It.IsAny<int>()));

        adminValidationMock
            .Setup(v => v.EnsureIsNotSelfAction(5, 5, It.IsAny<string>()))
            .Throws(new UserException("A user cannot modify itself"));

        var service = new ModifyUserService(
            repoMock.Object,
            Mock.Of<IPhoneValidator>(),
            adminValidationMock.Object);

        var request = new ModificarUsuarioRequestDTO { Name = "Carlos" };

        var ex = Assert.ThrowsException<UserException>(() =>
            service.UpdateInternalUser(5, 5, request));

        Assert.AreEqual("A user cannot modify itself", ex.Message);
    }

    [TestMethod]
    public void UpdateInternalUser_NameAndLastName_ReturnsUpdatedUser()
    {
        var repoMock = new Mock<IRepositoryUser>();
        var phoneValidatorMock = new Mock<IPhoneValidator>();
        var adminValidationMock = new Mock<IAdminValidationService>();

        var existingUser = User.MakeIntern(
            "Juan",
            "Perez",
            "admin@gmail.com",
            Phone.MakePhone("099123456"),
            "PasswordSeguraee3!",
            UserRoles.Administrative);
        existingUser.Id = 5;

        adminValidationMock
            .Setup(v => v.GetExistingUser(5))
            .Returns(existingUser);

        adminValidationMock
            .Setup(v => v.EnsureUserIsAdmin(It.IsAny<int>()));

        adminValidationMock
            .Setup(v => v.EnsureIsNotSelfAction(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()));

        var service = new ModifyUserService(repoMock.Object, phoneValidatorMock.Object, adminValidationMock.Object);
        var request = new ModificarUsuarioRequestDTO { Name = "Carlos", LastName = "Gomez" };

        var result = service.UpdateInternalUser(5, 99, request);

        Assert.AreEqual(5, result.Id);
        Assert.AreEqual("Carlos", result.Name);
        Assert.AreEqual("Gomez", result.LastName);
        Assert.AreEqual("admin@gmail.com", result.Email);
        Assert.AreEqual("Administrative", result.Role);
    }

    [TestMethod]
    public void UpdateInternalUser_OnlyLastName_ReturnsUserWithUpdatedLastName()
    {
        var repoMock = new Mock<IRepositoryUser>();
        var phoneValidatorMock = new Mock<IPhoneValidator>();
        var adminValidationMock = new Mock<IAdminValidationService>();

        var existingUser = User.MakeIntern(
            "Juan",
            "Perez",
            "admin@gmail.com",
            Phone.MakePhone("099123456"),
            "PasswordSeguraee3!",
            UserRoles.Administrative);
        existingUser.Id = 5;

        adminValidationMock
            .Setup(v => v.GetExistingUser(5))
            .Returns(existingUser);

        adminValidationMock
            .Setup(v => v.EnsureUserIsAdmin(It.IsAny<int>()));

        adminValidationMock
            .Setup(v => v.EnsureIsNotSelfAction(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()));

        var service = new ModifyUserService(repoMock.Object, phoneValidatorMock.Object, adminValidationMock.Object);
        var request = new ModificarUsuarioRequestDTO { LastName = "Gomez" };

        var result = service.UpdateInternalUser(5, 99, request);

        Assert.AreEqual("Juan", result.Name);
        Assert.AreEqual("Gomez", result.LastName);
    }

    [TestMethod]
    public void UpdateInternalUser_NoChangesProvided_KeepsExistingData()
    {
        var repoMock = new Mock<IRepositoryUser>();
        var phoneValidatorMock = new Mock<IPhoneValidator>();
        var adminValidationMock = new Mock<IAdminValidationService>();

        var existingUser = User.MakeIntern(
            "Juan",
            "Perez",
            "admin@gmail.com",
            Phone.MakePhone("099123456"),
            "PasswordSeguraee3!",
            UserRoles.Administrative);
        existingUser.Id = 5;

        adminValidationMock
            .Setup(v => v.GetExistingUser(5))
            .Returns(existingUser);

        adminValidationMock
            .Setup(v => v.EnsureUserIsAdmin(It.IsAny<int>()));

        adminValidationMock
            .Setup(v => v.EnsureIsNotSelfAction(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()));

        var service = new ModifyUserService(repoMock.Object, phoneValidatorMock.Object, adminValidationMock.Object);
        var request = new ModificarUsuarioRequestDTO();

        var result = service.UpdateInternalUser(5, 99, request);

        Assert.AreEqual("Juan", result.Name);
        Assert.AreEqual("Perez", result.LastName);
        Assert.AreEqual("099123456", result.Phone);
        Assert.AreEqual("admin@gmail.com", result.Email);
    }

    [TestMethod]
    public void UpdateInternalUser_OnlyPhone_ReturnsUserWithUpdatedPhone()
    {
        var repoMock = new Mock<IRepositoryUser>();
        var phoneValidatorMock = new Mock<IPhoneValidator>();
        var adminValidationMock = new Mock<IAdminValidationService>();

        var existingUser = User.MakeIntern(
            "Juan",
            "Perez",
            "admin@gmail.com",
            Phone.MakePhone("099123456"),
            "PasswordSeguraee3!",
            UserRoles.Administrative);
        existingUser.Id = 5;

        adminValidationMock
            .Setup(v => v.GetExistingUser(5))
            .Returns(existingUser);

        adminValidationMock
            .Setup(v => v.EnsureUserIsAdmin(It.IsAny<int>()));

        adminValidationMock
            .Setup(v => v.EnsureIsNotSelfAction(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()));

        phoneValidatorMock
            .Setup(v => v.ItsValid("098111222"))
            .Returns(true);

        var service = new ModifyUserService(repoMock.Object, phoneValidatorMock.Object, adminValidationMock.Object);
        var request = new ModificarUsuarioRequestDTO { Phone = "098111222" };

        var result = service.UpdateInternalUser(5, 99, request);

        Assert.AreEqual("098111222", result.Phone);
        Assert.AreEqual("Juan", result.Name);
        Assert.AreEqual("Perez", result.LastName);
        Assert.AreEqual("admin@gmail.com", result.Email);
    }

    [TestMethod]
    public void UpdateInternalUser_InvalidPhone_ThrowsExceptionWithCorrectMessage()
    {
        var repoMock = new Mock<IRepositoryUser>();
        var phoneValidatorMock = new Mock<IPhoneValidator>();
        var adminValidationMock = new Mock<IAdminValidationService>();

        var existingUser = User.MakeIntern(
            "Juan",
            "Perez",
            "admin@gmail.com",
            Phone.MakePhone("099123456"),
            "PasswordSeguraee3!",
            UserRoles.Administrative);
        existingUser.Id = 5;

        adminValidationMock
            .Setup(v => v.GetExistingUser(5))
            .Returns(existingUser);

        adminValidationMock
            .Setup(v => v.EnsureUserIsAdmin(It.IsAny<int>()));

        adminValidationMock
            .Setup(v => v.EnsureIsNotSelfAction(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()));

        phoneValidatorMock
            .Setup(v => v.ItsValid("123"))
            .Returns(false);

        var service = new ModifyUserService(repoMock.Object, phoneValidatorMock.Object, adminValidationMock.Object);
        var request = new ModificarUsuarioRequestDTO { Phone = "123" };

        var ex = Assert.ThrowsException<UserException>(() =>
            service.UpdateInternalUser(5, 99, request));

        Assert.AreEqual("Phone is not valid", ex.Message);
    }

    [TestMethod]
    public void UpdateInternalUser_PhoneInvalidAccordingToValidator_DoesNotModifyPhone()
    {
        var repoMock = new Mock<IRepositoryUser>();
        var phoneValidatorMock = new Mock<IPhoneValidator>();
        var adminValidationMock = new Mock<IAdminValidationService>();

        var existingUser = User.MakeIntern(
            "Juan",
            "Perez",
            "admin@gmail.com",
            Phone.MakePhone("099123456"),
            "PasswordSeguraee3!",
            UserRoles.Administrative);
        existingUser.Id = 5;

        adminValidationMock
            .Setup(v => v.GetExistingUser(5))
            .Returns(existingUser);

        adminValidationMock
            .Setup(v => v.EnsureUserIsAdmin(It.IsAny<int>()));

        adminValidationMock
            .Setup(v => v.EnsureIsNotSelfAction(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()));

        phoneValidatorMock
            .Setup(v => v.ItsValid("123"))
            .Returns(false);

        var service = new ModifyUserService(repoMock.Object, phoneValidatorMock.Object, adminValidationMock.Object);
        var request = new ModificarUsuarioRequestDTO { Phone = "123" };

        Assert.ThrowsException<UserException>(() =>
            service.UpdateInternalUser(5, 99, request));

        Assert.AreEqual("099123456", existingUser.Phone.Value);
    }

    [TestMethod]
    public void UpdateInternalUser_LastNameGreaterThanTwentyFiveCharacters_ThrowsExceptionWithCorrectMessage()
    {
        var repoMock = new Mock<IRepositoryUser>();
        var phoneValidatorMock = new Mock<IPhoneValidator>();
        var adminValidationMock = new Mock<IAdminValidationService>();

        var existingUser = User.MakeIntern(
            "Juan",
            "Perez",
            "admin@gmail.com",
            Phone.MakePhone("099123456"),
            "PasswordSeguraee3!",
            UserRoles.Administrative);
        existingUser.Id = 5;

        adminValidationMock
            .Setup(v => v.GetExistingUser(5))
            .Returns(existingUser);

        adminValidationMock
            .Setup(v => v.EnsureUserIsAdmin(It.IsAny<int>()));

        adminValidationMock
            .Setup(v => v.EnsureIsNotSelfAction(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()));

        var service = new ModifyUserService(repoMock.Object, phoneValidatorMock.Object, adminValidationMock.Object);
        var request = new ModificarUsuarioRequestDTO { LastName = "ApellidoDemasiadoDemasiadoLargo" };

        var ex = Assert.ThrowsException<UserException>(() =>
            service.UpdateInternalUser(5, 99, request));

        Assert.AreEqual("Last name must be between 3 and 25 characters", ex.Message);
    }

    [TestMethod]
    public void UpdateInternalUser_Phone_UserDoesNotExist_ThrowsExceptionWithCorrectMessage()
    {
        var repoMock = new Mock<IRepositoryUser>();
        var phoneValidatorMock = new Mock<IPhoneValidator>();
        var adminValidationMock = new Mock<IAdminValidationService>();

        adminValidationMock
            .Setup(v => v.GetExistingUser(5))
            .Throws(new UserException("User does not exist"));

        adminValidationMock
            .Setup(v => v.EnsureUserIsAdmin(It.IsAny<int>()));

        adminValidationMock
            .Setup(v => v.EnsureIsNotSelfAction(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()));

        var service = new ModifyUserService(repoMock.Object, phoneValidatorMock.Object, adminValidationMock.Object);
        var request = new ModificarUsuarioRequestDTO { Phone = "098111222" };

        var ex = Assert.ThrowsException<UserException>(() =>
            service.UpdateInternalUser(5, 99, request));

        Assert.AreEqual("User does not exist", ex.Message);
    }

    [TestMethod]
    public void UpdateInternalUser_PhoneDoesNotChange_DoesNotValidatePhone()
    {
        var repoMock = new Mock<IRepositoryUser>();
        var phoneValidatorMock = new Mock<IPhoneValidator>();
        var adminValidationMock = new Mock<IAdminValidationService>();

        var existingUser = User.MakeIntern(
            "Juan",
            "Perez",
            "admin@gmail.com",
            Phone.MakePhone("099123456"),
            "PasswordSeguraee3!",
            UserRoles.Administrative);
        existingUser.Id = 5;

        adminValidationMock
            .Setup(v => v.GetExistingUser(5))
            .Returns(existingUser);

        adminValidationMock
            .Setup(v => v.EnsureUserIsAdmin(It.IsAny<int>()));

        adminValidationMock
            .Setup(v => v.EnsureIsNotSelfAction(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()));

        var service = new ModifyUserService(repoMock.Object, phoneValidatorMock.Object, adminValidationMock.Object);
        var request = new ModificarUsuarioRequestDTO { Phone = "099123456" };

        service.UpdateInternalUser(5, 99, request);

        phoneValidatorMock.Verify(v => v.ItsValid(It.IsAny<string>()), Times.Never);
    }

    [TestMethod]
    public void UpdateInternalUser_OnlyEmail_ReturnsUserWithUpdatedEmail()
    {
        var repoMock = new Mock<IRepositoryUser>();
        var phoneValidatorMock = new Mock<IPhoneValidator>();
        var adminValidationMock = new Mock<IAdminValidationService>();

        var existingUser = User.MakeIntern(
            "Juan",
            "Perez",
            "admin@gmail.com",
            Phone.MakePhone("099123456"),
            "PasswordSeguraee3!",
            UserRoles.Administrative);
        existingUser.Id = 5;

        adminValidationMock
            .Setup(v => v.GetExistingUser(5))
            .Returns(existingUser);

        adminValidationMock
            .Setup(v => v.EnsureUserIsAdmin(It.IsAny<int>()));

        adminValidationMock
            .Setup(v => v.EnsureIsNotSelfAction(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()));

        repoMock
            .Setup(r => r.EmailExist("nuevo@gmail.com"))
            .Returns(false);

        var service = new ModifyUserService(repoMock.Object, phoneValidatorMock.Object, adminValidationMock.Object);
        var request = new ModificarUsuarioRequestDTO { Email = "nuevo@gmail.com" };

        var result = service.UpdateInternalUser(5, 99, request);

        Assert.AreEqual("nuevo@gmail.com", result.Email);
        Assert.AreEqual("Juan", result.Name);
        Assert.AreEqual("Perez", result.LastName);
        Assert.AreEqual("099123456", result.Phone);
    }

    [TestMethod]
    public void UpdateInternalUser_WhenLoggedUserIsNotAdmin_DoesNotCallGetExistingUser()
    {
        var repoMock = new Mock<IRepositoryUser>();
        var phoneValidatorMock = new Mock<IPhoneValidator>();
        var adminValidationMock = new Mock<IAdminValidationService>();

        adminValidationMock
            .Setup(v => v.EnsureUserIsAdmin(99))
            .Throws(new UserException("Only an administrative user can perform this action"));

        var service = new ModifyUserService(
            repoMock.Object,
            phoneValidatorMock.Object,
            adminValidationMock.Object);

        var request = new ModificarUsuarioRequestDTO();

        Assert.ThrowsException<UserException>(() =>
            service.UpdateInternalUser(5, 99, request));

        adminValidationMock.Verify(v => v.GetExistingUser(It.IsAny<int>()), Times.Never);
    }

    [TestMethod]
    public void UpdateInternalUser_InvalidEmailFormat_ThrowsException()
    {
        var repoMock = new Mock<IRepositoryUser>();
        var phoneValidatorMock = new Mock<IPhoneValidator>();
        var adminValidationMock = new Mock<IAdminValidationService>();

        var user = User.MakeIntern(
            "Juan",
            "Perez",
            "admin@gmail.com",
            Phone.MakePhone("099123456"),
            "PasswordSeguraee3!",
            UserRoles.Administrative);
        user.Id = 5;

        adminValidationMock.Setup(v => v.GetExistingUser(5)).Returns(user);
        adminValidationMock.Setup(v => v.EnsureUserIsAdmin(It.IsAny<int>()));
        adminValidationMock.Setup(v => v.EnsureIsNotSelfAction(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()));

        repoMock.Setup(r => r.EmailExist("correo-invalido")).Returns(false);

        var service = new ModifyUserService(repoMock.Object, phoneValidatorMock.Object, adminValidationMock.Object);

        var request = new ModificarUsuarioRequestDTO { Email = "correo-invalido" };

        var ex = Assert.ThrowsException<UserException>(() =>
            service.UpdateInternalUser(5, 99, request));

        Assert.AreEqual("Email format is not valid", ex.Message);
    }

    [TestMethod]
    public void UpdateInternalUser_EmailAlreadyExists_ThrowsException()
    {
        var repoMock = new Mock<IRepositoryUser>();
        var phoneValidatorMock = new Mock<IPhoneValidator>();
        var adminValidationMock = new Mock<IAdminValidationService>();

        var user = User.MakeIntern(
            "Juan",
            "Perez",
            "admin@gmail.com",
            Phone.MakePhone("099123456"),
            "PasswordSeguraee3!",
            UserRoles.Administrative);
        user.Id = 5;

        adminValidationMock.Setup(v => v.GetExistingUser(5)).Returns(user);
        adminValidationMock.Setup(v => v.EnsureUserIsAdmin(It.IsAny<int>()));
        adminValidationMock.Setup(v => v.EnsureIsNotSelfAction(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()));

        repoMock.Setup(r => r.EmailExist("repetido@gmail.com")).Returns(true);

        var service = new ModifyUserService(repoMock.Object, phoneValidatorMock.Object, adminValidationMock.Object);

        var request = new ModificarUsuarioRequestDTO { Email = "repetido@gmail.com" };

        var ex = Assert.ThrowsException<UserException>(() =>
            service.UpdateInternalUser(5, 99, request));

        Assert.AreEqual("Email is already registered", ex.Message);
    }

    [TestMethod]
    public void UpdateInternalUser_EmailSame_DoesNotCallRepository()
    {
        var repoMock = new Mock<IRepositoryUser>();
        var phoneValidatorMock = new Mock<IPhoneValidator>();
        var adminValidationMock = new Mock<IAdminValidationService>();

        var user = User.MakeIntern(
            "Juan",
            "Perez",
            "admin@gmail.com",
            Phone.MakePhone("099123456"),
            "PasswordSeguraee3!",
            UserRoles.Administrative);
        user.Id = 5;

        adminValidationMock.Setup(v => v.GetExistingUser(5)).Returns(user);
        adminValidationMock.Setup(v => v.EnsureUserIsAdmin(It.IsAny<int>()));
        adminValidationMock.Setup(v => v.EnsureIsNotSelfAction(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()));

        var service = new ModifyUserService(repoMock.Object, phoneValidatorMock.Object, adminValidationMock.Object);

        var request = new ModificarUsuarioRequestDTO { Email = "admin@gmail.com" };

        service.UpdateInternalUser(5, 99, request);

        repoMock.Verify(r => r.EmailExist(It.IsAny<string>()), Times.Never);
    }

    [TestMethod]
    public void UpdateInternalUser_InvalidRole_ThrowsException()
    {
        var repoMock = new Mock<IRepositoryUser>();
        var phoneValidatorMock = new Mock<IPhoneValidator>();
        var adminValidationMock = new Mock<IAdminValidationService>();

        var user = User.MakeIntern(
            "Juan",
            "Perez",
            "admin@gmail.com",
            Phone.MakePhone("099123456"),
            "PasswordSeguraee3!",
            UserRoles.Preparer);
        user.Id = 5;

        adminValidationMock.Setup(v => v.GetExistingUser(5)).Returns(user);
        adminValidationMock.Setup(v => v.EnsureUserIsAdmin(It.IsAny<int>()));
        adminValidationMock.Setup(v => v.EnsureIsNotSelfAction(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()));

        var service = new ModifyUserService(repoMock.Object, phoneValidatorMock.Object, adminValidationMock.Object);

        var request = new ModificarUsuarioRequestDTO { Role = "InvalidRole" };

        var ex = Assert.ThrowsException<UserException>(() =>
            service.UpdateInternalUser(5, 99, request));

        Assert.AreEqual("Internal role is not valid", ex.Message);
    }

    [TestMethod]
    public void UpdateInternalUser_RoleDifferentCase_UpdatesCorrectly()
    {
        var repoMock = new Mock<IRepositoryUser>();
        var phoneValidatorMock = new Mock<IPhoneValidator>();
        var adminValidationMock = new Mock<IAdminValidationService>();

        var user = User.MakeIntern(
            "Juan",
            "Perez",
            "admin@gmail.com",
            Phone.MakePhone("099123456"),
            "PasswordSeguraee3!",
            UserRoles.Preparer);
        user.Id = 5;

        adminValidationMock.Setup(v => v.GetExistingUser(5)).Returns(user);
        adminValidationMock.Setup(v => v.EnsureUserIsAdmin(It.IsAny<int>()));
        adminValidationMock.Setup(v => v.EnsureIsNotSelfAction(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()));

        var service = new ModifyUserService(repoMock.Object, phoneValidatorMock.Object, adminValidationMock.Object);

        var request = new ModificarUsuarioRequestDTO { Role = "adMinisTrative" };

        var result = service.UpdateInternalUser(5, 99, request);

        Assert.AreEqual("Administrative", result.Role);
    }

    [TestMethod]
    public void UpdateInternalUser_CallsAdminValidationMethods()
    {
        var repoMock = new Mock<IRepositoryUser>();
        var phoneValidatorMock = new Mock<IPhoneValidator>();
        var adminValidationMock = new Mock<IAdminValidationService>();

        var user = User.MakeIntern(
            "Juan",
            "Perez",
            "admin@gmail.com",
            Phone.MakePhone("099123456"),
            "PasswordSeguraee3!",
            UserRoles.Administrative);
        user.Id = 5;

        adminValidationMock.Setup(v => v.GetExistingUser(5)).Returns(user);

        var service = new ModifyUserService(
            repoMock.Object,
            phoneValidatorMock.Object,
            adminValidationMock.Object);

        service.UpdateInternalUser(5, 99, new ModificarUsuarioRequestDTO());

        adminValidationMock.Verify(v => v.EnsureUserIsAdmin(99), Times.Once);
        adminValidationMock.Verify(v => v.EnsureIsNotSelfAction(5, 99, It.IsAny<string>()), Times.Once);
        adminValidationMock.Verify(v => v.GetExistingUser(5), Times.Once);
    }

    [TestMethod]
    public void UpdateInternalUser_WhenLoggedUserIsNotAdmin_ThrowsException()
    {
        var repoMock = new Mock<IRepositoryUser>();
        var phoneValidatorMock = new Mock<IPhoneValidator>();
        var adminValidationMock = new Mock<IAdminValidationService>();

        adminValidationMock
            .Setup(v => v.EnsureUserIsAdmin(99))
            .Throws(new UserException("Only an administrative user can perform this action"));

        var service = new ModifyUserService(
            repoMock.Object,
            phoneValidatorMock.Object,
            adminValidationMock.Object);

        var request = new ModificarUsuarioRequestDTO { Name = "Carlos" };

        var ex = Assert.ThrowsException<UserException>(() =>
            service.UpdateInternalUser(5, 99, request));

        Assert.AreEqual("Only an administrative user can perform this action", ex.Message);
    }

    [TestMethod]
    public void UpdateInternalUser_WhenUserModifiesItself_DoesNotCallGetExistingUser()
    {
        var repoMock = new Mock<IRepositoryUser>();
        var phoneValidatorMock = new Mock<IPhoneValidator>();
        var adminValidationMock = new Mock<IAdminValidationService>();

        adminValidationMock
            .Setup(v => v.EnsureUserIsAdmin(It.IsAny<int>()));

        adminValidationMock
            .Setup(v => v.EnsureIsNotSelfAction(5, 5, It.IsAny<string>()))
            .Throws(new UserException("A user cannot modify itself"));

        var service = new ModifyUserService(
            repoMock.Object,
            phoneValidatorMock.Object,
            adminValidationMock.Object);

        var request = new ModificarUsuarioRequestDTO { Name = "Carlos" };

        Assert.ThrowsException<UserException>(() =>
            service.UpdateInternalUser(5, 5, request));

        adminValidationMock.Verify(v => v.GetExistingUser(It.IsAny<int>()), Times.Never);
    }
}
