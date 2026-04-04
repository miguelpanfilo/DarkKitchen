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
public class ModifyInternUserTest
{
    [TestMethod]
    public void UpdateInternalUser_OnlyName_ReturnsUserWithUpdatedName()
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

        var loggedInUser = User.MakeIntern(
            "Juan",
            "Perez",
            "admin12@gmail.com",
            Phone.MakePhone("099123456"),
            "PasswordSeguraee3!",
            UserRoles.Administrative
        );
        loggedInUser.Id = 99;

        repoMock
            .Setup(r => r.FindById(99))
            .Returns(loggedInUser);

        var service = new ModifyUserService(repoMock.Object, Mock.Of<IPhoneValidator>());
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

        var service = new ModifyUserService(repoMock.Object, Mock.Of<IPhoneValidator>());
        var request = new ModificarUsuarioRequestDTO { Name = "Carlos" };

        var ex = Assert.ThrowsException<UsuarioExcepcion>(() =>
            service.UpdateInternalUser(5, 5, request)
        );

        Assert.AreEqual("A user cannot modify itself", ex.Message);
    }

    [TestMethod]
    public void UpdateInternalUser_NameAndLastName_ReturnsUpdatedUser()
    {
        var repoMock = new Mock<IRepositoryUser>();
        var phoneValidatorMock = new Mock<IPhoneValidator>();

        var existingUser = User.MakeIntern(
            "Juan",
            "Perez",
            "admin@gmail.com",
            Phone.MakePhone("099123456"),
            "PasswordSeguraee3!",
            UserRoles.Administrative
        );
        existingUser.Id = 5;

        var loggedInUser = User.MakeIntern(
            "Juan",
            "Perez",
            "admin2@gmail.com",
            Phone.MakePhone("099123456"),
            "PasswordSeguraee3!",
            UserRoles.Administrative
        );
        loggedInUser.Id = 99;

        repoMock
            .Setup(r => r.FindById(99))
            .Returns(loggedInUser);

        repoMock
            .Setup(r => r.FindById(5))
            .Returns(existingUser);

        var service = new ModifyUserService(repoMock.Object, phoneValidatorMock.Object);
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

        var loggedInUser = User.MakeIntern(
            "Juan",
            "Perez",
            "admin12@gmail.com",
            Phone.MakePhone("099123456"),
            "PasswordSeguraee3!",
            UserRoles.Administrative
        );
        loggedInUser.Id = 99;

        repoMock
            .Setup(r => r.FindById(99))
            .Returns(loggedInUser);

        var service = new ModifyUserService(repoMock.Object, phoneValidatorMock.Object);
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

        var loggedInUser = User.MakeIntern(
            "Juan",
            "Perez",
            "admin12@gmail.com",
            Phone.MakePhone("099123456"),
            "PasswordSeguraee3!",
            UserRoles.Administrative
        );
        loggedInUser.Id = 99;

        repoMock
            .Setup(r => r.FindById(99))
            .Returns(loggedInUser);

        phoneValidatorMock
            .Setup(v => v.ItsValid("098111222"))
            .Returns(true);

        var service = new ModifyUserService(repoMock.Object, phoneValidatorMock.Object);
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

        var loggedInUser = User.MakeIntern(
            "Admin",
            "Root",
            "root@gmail.com",
            Phone.MakePhone("099999999"),
            "PasswordSeguraee3!",
            UserRoles.Administrative
        );
        loggedInUser.Id = 99;

        repoMock
            .Setup(r => r.FindById(99))
            .Returns(loggedInUser);

        phoneValidatorMock
            .Setup(v => v.ItsValid("123"))
            .Returns(false);

        var service = new ModifyUserService(repoMock.Object, phoneValidatorMock.Object);
        var request = new ModificarUsuarioRequestDTO { Phone = "123" };

        var ex = Assert.ThrowsException<UsuarioExcepcion>(() =>
            service.UpdateInternalUser(5, 99, request)
        );

        Assert.AreEqual("Phone is not valid", ex.Message);
    }

    [TestMethod]
    public void UpdateInternalUser_PhoneInvalidAccordingToValidator_DoesNotModifyPhone()
    {
        var repoMock = new Mock<IRepositoryUser>();
        var phoneValidatorMock = new Mock<IPhoneValidator>();

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

        phoneValidatorMock
            .Setup(v => v.ItsValid("123"))
            .Returns(false);

        var loggedInUser = User.MakeIntern(
            "Admin",
            "Root",
            "root@gmail.com",
            Phone.MakePhone("099999999"),
            "PasswordSeguraee3!",
            UserRoles.Administrative
        );
        loggedInUser.Id = 99;

        repoMock
            .Setup(r => r.FindById(99))
            .Returns(loggedInUser);

        var service = new ModifyUserService(repoMock.Object, phoneValidatorMock.Object);
        var request = new ModificarUsuarioRequestDTO { Phone = "123" };

        Assert.ThrowsException<UsuarioExcepcion>(() =>
            service.UpdateInternalUser(5, 99, request)
        );

        Assert.AreEqual("099123456", existingUser.Phone.Value);
    }

    [TestMethod]
    public void UpdateInternalUser_LastNameGreaterThanTwentyFiveCharacters_ThrowsExceptionWithCorrectMessage()
    {
        var repoMock = new Mock<IRepositoryUser>();
        var phoneValidatorMock = new Mock<IPhoneValidator>();

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

        var loggedInUser = User.MakeIntern(
            "Admin",
            "Root",
            "root@gmail.com",
            Phone.MakePhone("099999999"),
            "PasswordSeguraee3!",
            UserRoles.Administrative
        );
        loggedInUser.Id = 99;

        repoMock
            .Setup(r => r.FindById(99))
            .Returns(loggedInUser);

        var service = new ModifyUserService(repoMock.Object, phoneValidatorMock.Object);
        var request = new ModificarUsuarioRequestDTO { LastName = "ApellidoDemasiadoDemasiadoLargo" };

        var ex = Assert.ThrowsException<UsuarioExcepcion>(() =>
            service.UpdateInternalUser(5, 99, request)
        );

        Assert.AreEqual("Last name must be between 3 and 25 characters", ex.Message);
    }

    [TestMethod]
    public void UpdateInternalUser_Phone_UserDoesNotExist_ThrowsExceptionWithCorrectMessage()
    {
        var repoMock = new Mock<IRepositoryUser>();
        var phoneValidatorMock = new Mock<IPhoneValidator>();

        repoMock
            .Setup(r => r.FindById(5))
            .Returns((User?)null);

        var loggedInUser = User.MakeIntern(
            "Admin",
            "Root",
            "root@gmail.com",
            Phone.MakePhone("099999999"),
            "PasswordSeguraee3!",
            UserRoles.Administrative
        );
        loggedInUser.Id = 99;

        repoMock
            .Setup(r => r.FindById(99))
            .Returns(loggedInUser);

        var service = new ModifyUserService(repoMock.Object, phoneValidatorMock.Object);
        var request = new ModificarUsuarioRequestDTO { Phone = "098111222" };

        var ex = Assert.ThrowsException<UsuarioExcepcion>(() =>
            service.UpdateInternalUser(5, 99, request)
        );

        Assert.AreEqual("User does not exist", ex.Message);
    }

    [TestMethod]
    public void UpdateInternalUser_PhoneDoesNotChange_DoesNotValidatePhone()
    {
        var repoMock = new Mock<IRepositoryUser>();
        var phoneValidatorMock = new Mock<IPhoneValidator>();

        var existingUser = User.MakeIntern(
            "Juan",
            "Perez",
            "admin@gmail.com",
            Phone.MakePhone("099123456"),
            "PasswordSeguraee3!",
            UserRoles.Administrative
        );
        existingUser.Id = 5;

        var loggedInUser = User.MakeIntern(
            "Juan",
            "Perez",
            "admin12@gmail.com",
            Phone.MakePhone("099123456"),
            "PasswordSeguraee3!",
            UserRoles.Administrative
        );
        loggedInUser.Id = 99;

        repoMock
            .Setup(r => r.FindById(99))
            .Returns(loggedInUser);

        repoMock
            .Setup(r => r.FindById(5))
            .Returns(existingUser);

        var service = new ModifyUserService(repoMock.Object, phoneValidatorMock.Object);
        var request = new ModificarUsuarioRequestDTO { Phone = "099123456" };

        service.UpdateInternalUser(5, 99, request);

        phoneValidatorMock.Verify(v => v.ItsValid(It.IsAny<string>()), Times.Never);
    }

    [TestMethod]
    public void UpdateInternalUser_OnlyEmail_ReturnsUserWithUpdatedEmail()
    {
        var repoMock = new Mock<IRepositoryUser>();
        var phoneValidatorMock = new Mock<IPhoneValidator>();

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

        var loggedInUser = User.MakeIntern(
            "Juan",
            "Perez",
            "admin12@gmail.com",
            Phone.MakePhone("099123456"),
            "PasswordSeguraee3!",
            UserRoles.Administrative
        );
        loggedInUser.Id = 99;

        repoMock
            .Setup(r => r.FindById(99))
            .Returns(loggedInUser);

        repoMock
            .Setup(r => r.EmailExist("nuevo@gmail.com"))
            .Returns(false);

        var service = new ModifyUserService(repoMock.Object, phoneValidatorMock.Object);
        var request = new ModificarUsuarioRequestDTO { Email = "nuevo@gmail.com" };

        var result = service.UpdateInternalUser(5, 99, request);

        Assert.AreEqual("nuevo@gmail.com", result.Email);
        Assert.AreEqual("Juan", result.Name);
        Assert.AreEqual("Perez", result.LastName);
        Assert.AreEqual("099123456", result.Phone);
    }

    [TestMethod]
    public void UpdateInternalUser_InvalidEmailFormat_ThrowsExceptionWithCorrectMessage()
    {
        var repoMock = new Mock<IRepositoryUser>();
        var phoneValidatorMock = new Mock<IPhoneValidator>();

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

        repoMock
            .Setup(r => r.EmailExist("correo-invalido"))
            .Returns(false);

        var loggedInUser = User.MakeIntern(
            "Juan",
            "Perez",
            "admin12@gmail.com",
            Phone.MakePhone("099123456"),
            "PasswordSeguraee3!",
            UserRoles.Administrative
        );
        loggedInUser.Id = 99;

        repoMock
            .Setup(r => r.FindById(99))
            .Returns(loggedInUser);

        var service = new ModifyUserService(repoMock.Object, phoneValidatorMock.Object);
        var request = new ModificarUsuarioRequestDTO { Email = "correo-invalido" };

        var ex = Assert.ThrowsException<UsuarioExcepcion>(() =>
            service.UpdateInternalUser(5, 99, request)
        );

        Assert.AreEqual("Email format is not valid", ex.Message);
    }

    [TestMethod]
    public void UpdateInternalUser_EmailAlreadyRegistered_ThrowsExceptionWithCorrectMessage()
    {
        var repoMock = new Mock<IRepositoryUser>();
        var phoneValidatorMock = new Mock<IPhoneValidator>();

        var existingUser = User.MakeIntern(
            "Juan",
            "Perez",
            "admin@gmail.com",
            Phone.MakePhone("099123456"),
            "PasswordSeguraee3!",
            UserRoles.Administrative
        );
        existingUser.Id = 5;

        var loggedInUser = User.MakeIntern(
            "Admin",
            "Root",
            "root@gmail.com",
            Phone.MakePhone("099999999"),
            "PasswordSeguraee3!",
            UserRoles.Administrative
        );
        loggedInUser.Id = 99;

        repoMock
            .Setup(r => r.FindById(99))
            .Returns(loggedInUser);

        repoMock
            .Setup(r => r.FindById(5))
            .Returns(existingUser);

        repoMock
            .Setup(r => r.EmailExist("repetido@gmail.com"))
            .Returns(true);

        var service = new ModifyUserService(repoMock.Object, phoneValidatorMock.Object);
        var request = new ModificarUsuarioRequestDTO { Email = "repetido@gmail.com" };

        var ex = Assert.ThrowsException<UsuarioExcepcion>(() =>
            service.UpdateInternalUser(5, 99, request)
        );

        Assert.AreEqual("Email is already registered", ex.Message);
    }

    [TestMethod]
    public void UpdateInternalUser_EmailAlreadyRegistered_DoesNotModifyEmail()
    {
        var repoMock = new Mock<IRepositoryUser>();
        var phoneValidatorMock = new Mock<IPhoneValidator>();

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

        var loggedInUser = User.MakeIntern(
            "Admin",
            "Perez",
            "admin2@gmail.com",
            Phone.MakePhone("099999999"),
            "PasswordSeguraee3!",
            UserRoles.Administrative
        );
        loggedInUser.Id = 99;

        repoMock
            .Setup(r => r.FindById(99))
            .Returns(loggedInUser);

        repoMock
            .Setup(r => r.EmailExist("repetido@gmail.com"))
            .Returns(true);

        var service = new ModifyUserService(repoMock.Object, phoneValidatorMock.Object);
        var request = new ModificarUsuarioRequestDTO { Email = "repetido@gmail.com" };

        var ex = Assert.ThrowsException<UsuarioExcepcion>(() =>
            service.UpdateInternalUser(5, 99, request)
        );

        Assert.AreEqual("Email is already registered", ex.Message);
    }

    [TestMethod]
    public void UpdateInternalUser_EmailNull_KeepsExistingEmail()
    {
        var repoMock = new Mock<IRepositoryUser>();
        var phoneValidatorMock = new Mock<IPhoneValidator>();

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

        var loggedInUser = User.MakeIntern(
            "Juan",
            "Perez",
            "admin12@gmail.com",
            Phone.MakePhone("099123456"),
            "PasswordSeguraee3!",
            UserRoles.Administrative
        );
        loggedInUser.Id = 99;

        repoMock
            .Setup(r => r.FindById(99))
            .Returns(loggedInUser);

        var service = new ModifyUserService(repoMock.Object, phoneValidatorMock.Object);
        var request = new ModificarUsuarioRequestDTO { Email = null };

        var result = service.UpdateInternalUser(5, 99, request);

        Assert.AreEqual("admin@gmail.com", result.Email);
    }

    [TestMethod]
    public void UpdateInternalUser_EmptyEmail_KeepsExistingEmail()
    {
        var repoMock = new Mock<IRepositoryUser>();
        var phoneValidatorMock = new Mock<IPhoneValidator>();

        var existingUser = User.MakeIntern(
            "Juan",
            "Perez",
            "admin@gmail.com",
            Phone.MakePhone("099123456"),
            "PasswordSeguraee3!",
            UserRoles.Administrative
        );
        existingUser.Id = 5;

        var loggedInUser = User.MakeIntern(
            "Juan",
            "Perez",
            "admin12@gmail.com",
            Phone.MakePhone("099123456"),
            "PasswordSeguraee3!",
            UserRoles.Administrative
        );
        loggedInUser.Id = 99;

        repoMock
            .Setup(r => r.FindById(99))
            .Returns(loggedInUser);

        repoMock
            .Setup(r => r.FindById(5))
            .Returns(existingUser);

        var service = new ModifyUserService(repoMock.Object, phoneValidatorMock.Object);
        var request = new ModificarUsuarioRequestDTO { Email = string.Empty };

        var result = service.UpdateInternalUser(5, 99, request);

        Assert.AreEqual("admin@gmail.com", result.Email);
    }

    [TestMethod]
    public void UpdateInternalUser_EmailSameAsCurrent_DoesNotCallRepository()
    {
        var repoMock = new Mock<IRepositoryUser>();
        var phoneValidatorMock = new Mock<IPhoneValidator>();

        var existingUser = User.MakeIntern(
            "Juan",
            "Perez",
            "admin@gmail.com",
            Phone.MakePhone("099123456"),
            "PasswordSeguraee3!",
            UserRoles.Administrative
        );
        existingUser.Id = 5;

        var loggedInUser = User.MakeIntern(
            "Admin",
            "Root",
            "root@gmail.com",
            Phone.MakePhone("099999999"),
            "PasswordSeguraee3!",
            UserRoles.Administrative
        );
        loggedInUser.Id = 99;

        repoMock
            .Setup(r => r.FindById(5))
            .Returns(existingUser);

        repoMock
            .Setup(r => r.FindById(99))
            .Returns(loggedInUser);

        var service = new ModifyUserService(repoMock.Object, phoneValidatorMock.Object);
        var request = new ModificarUsuarioRequestDTO { Email = "admin@gmail.com" };

        service.UpdateInternalUser(5, 99, request);

        repoMock.Verify(r => r.EmailExist(It.IsAny<string>()), Times.Never);
    }

    [TestMethod]
    public void UpdateInternalUser_LastNameLessThanThreeCharacters_ThrowsExceptionWithCorrectMessage()
    {
        var repoMock = new Mock<IRepositoryUser>();
        var phoneValidatorMock = new Mock<IPhoneValidator>();

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

        phoneValidatorMock
            .Setup(v => v.ItsValid("099123456"))
            .Returns(true);

        var loggedInUser = User.MakeIntern(
            "Admin",
            "Root",
            "root@gmail.com",
            Phone.MakePhone("099999999"),
            "PasswordSeguraee3!",
            UserRoles.Administrative
        );
        loggedInUser.Id = 99;

        repoMock
            .Setup(r => r.FindById(99))
            .Returns(loggedInUser);

        var service = new ModifyUserService(repoMock.Object, phoneValidatorMock.Object);
        var request = new ModificarUsuarioRequestDTO { LastName = "Go" };

        var ex = Assert.ThrowsException<UsuarioExcepcion>(() =>
            service.UpdateInternalUser(5, 99, request)
        );

        Assert.AreEqual("Last name must be between 3 and 25 characters", ex.Message);
    }

    [TestMethod]
    public void UpdateInternalUser_UserDoesNotExist_ThrowsExceptionWithCorrectMessage()
    {
        var repoMock = new Mock<IRepositoryUser>();
        var phoneValidatorMock = new Mock<IPhoneValidator>();

        var service = new ModifyUserService(repoMock.Object, phoneValidatorMock.Object);
        repoMock
            .Setup(r => r.FindById(5))
            .Returns((User?)null);

        var loggedInUser = User.MakeIntern(
            "Admin",
            "Root",
            "root@gmail.com",
            Phone.MakePhone("099999999"),
            "PasswordSeguraee3!",
            UserRoles.Administrative
        );
        loggedInUser.Id = 99;

        repoMock
            .Setup(r => r.FindById(99))
            .Returns(loggedInUser);

        var request = new ModificarUsuarioRequestDTO { Name = "Carlos" };

        var ex = Assert.ThrowsException<UsuarioExcepcion>(() =>
            service.UpdateInternalUser(5, 99, request)
        );

        Assert.AreEqual("User does not exist", ex.Message);
    }

    [TestMethod]
    public void UpdateInternalUser_EmailSameWithDifferentCase_DoesNotCallRepository()
    {
        var repoMock = new Mock<IRepositoryUser>();
        var phoneValidatorMock = new Mock<IPhoneValidator>();

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

        var loggedInUser = User.MakeIntern(
            "Juan",
            "Perez",
            "admin12@gmail.com",
            Phone.MakePhone("099123456"),
            "PasswordSeguraee3!",
            UserRoles.Administrative
        );
        loggedInUser.Id = 99;

        repoMock
            .Setup(r => r.FindById(99))
            .Returns(loggedInUser);

        var service = new ModifyUserService(repoMock.Object, phoneValidatorMock.Object);
        var request = new ModificarUsuarioRequestDTO { Email = "ADMIN@GMAIL.COM" };

        service.UpdateInternalUser(5, 99, request);

        repoMock.Verify(r => r.EmailExist(It.IsAny<string>()), Times.Never);
    }

    [TestMethod]
    public void ModifyName_NameWithNumbers_ThrowsExceptionWithCorrectMessage()
    {
        var user = User.MakeIntern(
            "Juan",
            "Perez",
            "admin@gmail.com",
            Phone.MakePhone("099123456"),
            "PasswordSeguraee3!",
            UserRoles.Administrative
        );

        var ex = Assert.ThrowsException<UsuarioExcepcion>(() =>
            user.ModifyName("Ju4n")
        );

        Assert.AreEqual("Name cannot contain numbers", ex.Message);
    }

    [TestMethod]
    public void UpdateInternalUser_OnlyRole_ReturnsUserWithUpdatedRole()
    {
        var repoMock = new Mock<IRepositoryUser>();
        var phoneValidatorMock = new Mock<IPhoneValidator>();

        var existingUser = User.MakeIntern(
            "Juan",
            "Perez",
            "admin@gmail.com",
            Phone.MakePhone("099123456"),
            "PasswordSeguraee3!",
            UserRoles.Preparer
        );
        existingUser.Id = 5;

        var loggedInUser = User.MakeIntern(
            "Admin",
            "Root",
            "root@gmail.com",
            Phone.MakePhone("099999999"),
            "PasswordSeguraee3!",
            UserRoles.Administrative
        );
        loggedInUser.Id = 99;

        repoMock
            .Setup(r => r.FindById(5))
            .Returns(existingUser);

        repoMock
            .Setup(r => r.FindById(99))
            .Returns(loggedInUser);

        var service = new ModifyUserService(repoMock.Object, phoneValidatorMock.Object);

        var request = new ModificarUsuarioRequestDTO { Role = "Administrative" };

        var result = service.UpdateInternalUser(5, 99, request);

        Assert.AreEqual("Administrative", result.Role);
    }

    [TestMethod]
    public void UpdateInternalUser_RoleNull_KeepsCurrentRole()
    {
        var repoMock = new Mock<IRepositoryUser>();
        var phoneValidatorMock = new Mock<IPhoneValidator>();

        var existingUser = User.MakeIntern(
            "Juan",
            "Perez",
            "admin@gmail.com",
            Phone.MakePhone("099123456"),
            "PasswordSeguraee3!",
            UserRoles.Preparer
        );
        existingUser.Id = 5;

        var loggedInUser = User.MakeIntern(
            "Admin",
            "Root",
            "root@gmail.com",
            Phone.MakePhone("099999999"),
            "PasswordSeguraee3!",
            UserRoles.Administrative
        );
        loggedInUser.Id = 99;

        repoMock.Setup(r => r.FindById(5)).Returns(existingUser);
        repoMock.Setup(r => r.FindById(99)).Returns(loggedInUser);

        var service = new ModifyUserService(repoMock.Object, phoneValidatorMock.Object);

        var request = new ModificarUsuarioRequestDTO { Role = null };

        var result = service.UpdateInternalUser(5, 99, request);

        Assert.AreEqual("Preparer", result.Role);
    }

    [TestMethod]
    public void UpdateInternalUser_InvalidRole_ThrowsExceptionWithCorrectMessage()
    {
        var repoMock = new Mock<IRepositoryUser>();
        var phoneValidatorMock = new Mock<IPhoneValidator>();

        var existingUser = User.MakeIntern(
            "Juan",
            "Perez",
            "admin@gmail.com",
            Phone.MakePhone("099123456"),
            "PasswordSeguraee3!",
            UserRoles.Preparer
        );
        existingUser.Id = 5;

        var loggedInUser = User.MakeIntern(
            "Admin",
            "Root",
            "root@gmail.com",
            Phone.MakePhone("099999999"),
            "PasswordSeguraee3!",
            UserRoles.Administrative
        );
        loggedInUser.Id = 99;

        repoMock.Setup(r => r.FindById(5)).Returns(existingUser);
        repoMock.Setup(r => r.FindById(99)).Returns(loggedInUser);

        var service = new ModifyUserService(repoMock.Object, phoneValidatorMock.Object);

        var request = new ModificarUsuarioRequestDTO { Role = "Anything" };

        var ex = Assert.ThrowsException<UsuarioExcepcion>(() =>
            service.UpdateInternalUser(5, 99, request)
        );

        Assert.AreEqual("Internal role is not valid", ex.Message);
    }

    [TestMethod]
    public void UpdateInternalUser_RoleWithDifferentCase_UpdatesCorrectly()
    {
        var repoMock = new Mock<IRepositoryUser>();
        var phoneValidatorMock = new Mock<IPhoneValidator>();

        var existingUser = User.MakeIntern(
            "Juan",
            "Perez",
            "admin@gmail.com",
            Phone.MakePhone("099123456"),
            "PasswordSeguraee3!",
            UserRoles.Preparer
        );
        existingUser.Id = 5;

        var loggedInUser = User.MakeIntern(
            "Admin",
            "Root",
            "root@gmail.com",
            Phone.MakePhone("099999999"),
            "PasswordSeguraee3!",
            UserRoles.Administrative
        );
        loggedInUser.Id = 99;

        repoMock.Setup(r => r.FindById(5)).Returns(existingUser);
        repoMock.Setup(r => r.FindById(99)).Returns(loggedInUser);

        var service = new ModifyUserService(repoMock.Object, phoneValidatorMock.Object);

        var request = new ModificarUsuarioRequestDTO { Role = "adMinisTrative" };

        var result = service.UpdateInternalUser(5, 99, request);

        Assert.AreEqual("Administrative", result.Role);
    }

    [TestMethod]
    public void UpdateInternalUser_ClientRole_UpdatesCorrectly()
    {
        var repoMock = new Mock<IRepositoryUser>();
        var phoneValidatorMock = new Mock<IPhoneValidator>();

        var existingUser = User.MakeIntern(
            "Juan",
            "Perez",
            "admin@gmail.com",
            Phone.MakePhone("099123456"),
            "PasswordSeguraee3!",
            UserRoles.Preparer
        );
        existingUser.Id = 5;

        var loggedInUser = User.MakeIntern(
            "Admin",
            "Root",
            "root@gmail.com",
            Phone.MakePhone("099999999"),
            "PasswordSeguraee3!",
            UserRoles.Administrative
        );
        loggedInUser.Id = 99;

        repoMock.Setup(r => r.FindById(5)).Returns(existingUser);
        repoMock.Setup(r => r.FindById(99)).Returns(loggedInUser);

        var service = new ModifyUserService(repoMock.Object, phoneValidatorMock.Object);

        var request = new ModificarUsuarioRequestDTO { Role = "Client" };

        var result = service.UpdateInternalUser(5, 99, request);

        Assert.AreEqual("Client", result.Role);
    }
}
